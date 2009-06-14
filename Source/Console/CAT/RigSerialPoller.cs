//=============================================================================
// RigSerialPoller.cs
//=============================================================================
// Author: Chad Gatesman (W1CEG)
//=============================================================================
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//=============================================================================

//#define USE_COMMAND_MUTEX

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO.Ports;

using SDRSerialSupportII;


namespace PowerSDR
{
	public class RigSerialPoller
	{
		#region Variables

		// Configureable Variables
		public int pollingInterval = 200;
		public int pollingLockoutTime = 2000;
		public int commandLockoutTime = 2000;

		
		private Console console;
		private RigHW hw;
		private SDRSerialSupportII.SDRSerialPort SIO;
		private RigCATParser rigParser;
		private CATParser sdrParser;

		private ASCIIEncoding AE = new ASCIIEncoding();
		private static string separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
		private bool enabled = false;
		private string commBuffer = "";

		private bool rigPollingLockout = false;
		private System.Timers.Timer rigPollingLockoutTimer = new System.Timers.Timer();
		private bool rigCommandLockout = false;
		private System.Timers.Timer rigCommandLockoutTimer = new System.Timers.Timer();

#if USE_COMMAND_MUTEX
		private Mutex commandMutex = new Mutex();
#endif

		private Thread pollingThread;
		private bool keepPolling = true;
		private bool vfoAInitialized = false;
		private bool vfoBInitialized = false;

		private bool runRigCommands = true;
		private Thread rigCommandThread;
		private EventWaitHandle rigCommandWaitHandle = new AutoResetEvent(false);
		private Queue rigCommandQueue = Queue.Synchronized(new Queue());

		#endregion Variables


		#region Constructor

		public RigSerialPoller(Console console, RigHW hw)
		{
			this.console = console;
			this.hw = hw;

			this.rigParser = new RigCATParser(console,this);
			this.sdrParser = new CATParser(console);

			// Initialize Rig Answer Lockout Timer
			this.rigPollingLockoutTimer.Elapsed +=
				new System.Timers.ElapsedEventHandler(this.RigAnswerLockoutTimerExpiredEvent);
			this.rigPollingLockoutTimer.Interval = pollingLockoutTime;
			this.rigPollingLockoutTimer.AutoReset = false;
			this.rigPollingLockoutTimer.Enabled = false;

			// Initialize Rig Answer Lockout Timer
			this.rigCommandLockoutTimer.Elapsed +=
				new System.Timers.ElapsedEventHandler(this.RigCommandLockoutTimerExpiredEvent);
			this.rigCommandLockoutTimer.Interval = commandLockoutTime;
			this.rigCommandLockoutTimer.AutoReset = false;
			this.rigCommandLockoutTimer.Enabled = false;
		}

		~RigSerialPoller()
		{
			this.disableCAT();
		}

		#endregion Constructor


		#region Methods

		[Conditional("DEBUG")]
		private static void dbgWriteLine(string s)
		{
			System.Console.Error.WriteLine(s);
		}

		public void enableCAT()
		{
			lock (this)
			{
				if (this.enabled)
					return;

				if (this.SIO == null)
				{
					this.SIO = new SDRSerialPort(this.hw.COMPort);

					// Event handler for Serial RX Events
					this.SIO.serial_rx_event +=
						new SDRSerialSupportII.SerialRXEventHandler(SerialRXEventHandler);

					this.SIO.setCommParms(this.hw.COMBaudRate,this.hw.COMParity,
						this.hw.COMDataBits,this.hw.COMStopBits);

					dbgWriteLine("RigSerialPoller.enableCAT(), Opening COM" +
						this.hw.COMPort + "...");

					try
					{
						if (this.SIO.Create() == 0)
							dbgWriteLine("RigSerialPoller.enableCAT(), Opened COM" +
								this.hw.COMPort + ".");
						else
							throw new Exception();
					}
					catch (Exception ex)
					{
						// Event handler for Serial RX Events
						this.SIO.serial_rx_event -=
							new SDRSerialSupportII.SerialRXEventHandler(SerialRXEventHandler);
						
						this.SIO = null;

						MessageBox.Show("Could not initialize Rig CAT Control." +
							((ex.Message != null) ? "\nException was:\n\n " + ex.Message : ""),
							"Error Initializing Rig CAT Control",
							MessageBoxButtons.OK,MessageBoxIcon.Error);
						return;
					}
				}
				else
				{
					this.SIO.registerEventHandlers();

					// Event handler for Serial RX Events
					this.SIO.serial_rx_event +=
						new SDRSerialSupportII.SerialRXEventHandler(SerialRXEventHandler);
				}

				this.vfoAInitialized = false;
				this.vfoBInitialized = false;

				this.keepPolling = true;
				this.pollingThread = new Thread(new ThreadStart(this.poll));
				this.pollingThread.Name = "Rig Polling Thread";
				this.pollingThread.Start();

				this.enabled = true;
			}
		}

		public void disableCAT()
		{
			lock (this)
			{
				if (!this.enabled)
					return;

				this.enabled = false;

				dbgWriteLine("RigSerialPoller.disableCAT(), Shutting down Rig Polling Thread.");
				this.keepPolling = false;

				dbgWriteLine("RigSerialPoller.disableCAT(), Shutting down Rig Command Thread.");
				this.runRigCommands = false;
				this.rigCommandWaitHandle.Set();

				dbgWriteLine("RigSerialPoller.disableCAT(), Waiting for Rig Polling Thread to finish...");
				if (this.pollingThread != null)
				{
					this.pollingThread.Join();
					this.pollingThread = null;
				}

				dbgWriteLine("RigSerialPoller.disableCAT(), Waiting for Rig Command Thread to finish...");
				if (this.rigCommandThread != null)
				{
					this.rigCommandThread.Join();
					this.rigCommandThread = null;
				}

				if (this.SIO != null && this.SIO.PortIsOpen)
				{
					dbgWriteLine("RigSerialPoller.disableCAT(), Closing COM" +
						this.hw.COMPort + "...");

					// W1CEG: This hangs...I don't know why, but there's a lot
					//        of discussion on this on the Internet.
//					this.SIO.Destroy();

					this.SIO.deregisterEventHandlers();
					this.SIO.serial_rx_event -=
						new SDRSerialSupportII.SerialRXEventHandler(SerialRXEventHandler);

					dbgWriteLine("RigSerialPoller.disableCAT(), Closed COM" +
						this.hw.COMPort + ".");
				}
			}
		}

		#endregion Methods


		#region CAT Polling

		private void poll()
		{
			dbgWriteLine("RigSerialPoller.poll(), Start.");

			while (this.keepPolling)
			{
				bool bInitialized = (this.vfoAInitialized && this.vfoBInitialized);
				bool vfoAInit = this.vfoAInitialized;
				bool vfoBInit = this.vfoBInitialized;

				if (!bInitialized)
				{
					if (!this.vfoAInitialized)
					{
						Thread.Sleep(50);
						this.doRigCATCommand("FA;");
					}

					if (!this.vfoBInitialized)
					{
						Thread.Sleep(50);
						this.doRigCATCommand("FB;");
					}
				}
				else if (this.rigCommandThread == null)
				{
					// Start up the Command Thread after VFO-A and VFO-B are initialized.
					this.runRigCommands = true;
					this.rigCommandThread = new Thread(new ThreadStart(this.RigCommandThread));
					this.rigCommandThread.Name = "Rig CAT Command Thread";
					this.rigCommandThread.Start();
				}

				Thread.Sleep(50);

				if (!this.rigPollingLockout)
				{
					// If the operator is tuning the VFO Knob, we'll focus on just
					// that for maximum performance!
					if (this.rigParser.FrequencyChanged)
					{
						if (this.rigParser.VFO == 0)
							this.doRigCATCommand("FA;");
						else if (this.rigParser.VFO == 1)
							this.doRigCATCommand("FB;");
						else
						{
							Thread.Sleep(this.pollingInterval - 50);
							this.doRigCATCommand("IF;");
						}
					}
					else
					{
						Thread.Sleep(this.pollingInterval - 50);
						this.doRigCATCommand("IF;");
					}
				}
			}

			dbgWriteLine("RigSerialPoller.poll(), End.");
		}

		/**
		 * We use a separate thread to asynchronously handle UI invoked Rig CAT Commands.
		 * 
		 * This is needed so that we we don't get in a deadlock:
		 * 
		 * Serial Read Thread: Change Frequency (wait on Main Thread)
		 * Main Thread: User clicked to change frequency -> Waiting on Serial Read
		 * 
		 */
		private void RigCommandThread()
		{
			dbgWriteLine("RigSerialPoller.RigCommandThread(), Start.");

			this.rigCommandQueue.Clear();

			while (this.runRigCommands)
			{
				string command = null;

				if (this.rigCommandQueue.Count > 0)
					command = (string) this.rigCommandQueue.Dequeue();

				if (command != null)
					this.doRigCATCommand(command,true,false);
				else
					this.rigCommandWaitHandle.WaitOne();  // No more commands - wait for a signal
			}

			dbgWriteLine("RigSerialPoller.RigCommandThread(), End.");
		}

		#endregion CAT Polling


		#region Outgoing CAT Commands

		public void updateVFOAFrequency(double freq)
		{
			if (!this.enabled)
				return;

			string frequency =
				freq.ToString("f6").Replace(separator,"").PadLeft(11,'0');

			// Only do this if our Frequency State has changed.
			// :TODO: Do we need to pay attention to the VFO state?
			if (frequency == this.rigParser.VFOAFrequency)
				return;

			this.enqueueRigCATCommand("FA" + frequency + ';');

			// Set our Frequency State so we don't do this again.
			this.rigParser.VFOAFrequency = frequency;
		}

		public void updateVFOBFrequency(double freq)
		{
			if (!this.enabled)
				return;

			string frequency =
				freq.ToString("f6").Replace(separator,"").PadLeft(11,'0');

			// Only do this if our Frequency State has changed.
			// :TODO: Do we need to pay attention to the VFO state?
			if (frequency == this.rigParser.VFOBFrequency)
				return;

			this.enqueueRigCATCommand("FB" + frequency + ';');

			// Set our Frequency State so we don't do this again.
			this.rigParser.VFOBFrequency = frequency;
		}

		public void setMode(int mode)
		{
			if (!this.enabled)
				return;

			// Mode changing appears to be a bit intensive on the Rig.  We need
			// to lock out our polling for a bit to allow the Rig to safely
			// change modes.
			this.rigCommandLockout = true;
			this.rigCommandLockoutTimer.Stop();
			this.rigCommandLockoutTimer.Start();

			this.rigParser.Mode = mode;
			this.doRigCATCommand("MD" + mode + ';',true,false);
		}

        public void setSplit(bool splitOn)
        {
            if (!this.enabled)
                return;

            this.enqueueRigCATCommand("SP" + ((splitOn) ? '1' : '0') + ';');
        }


		private void doRigCATCommand(string command)
		{
			this.doRigCATCommand(command,false,true);
		}

		public void doRigCATCommand(string command,bool bPollingLockout,
			bool bCheckCommandLockout)
		{
			this.doRigCATCommand(command,bPollingLockout,
				this.pollingLockoutTime,bCheckCommandLockout);
		}

		public void doRigCATCommand(string command, bool bPollingLockout, int pollingLockoutTime,
			bool bCheckCommandLockout)
		{
			if (!this.enabled || (bCheckCommandLockout && this.rigCommandLockout))
				return;

			byte[] cmd = this.AE.GetBytes(command);

#if USE_COMMAND_MUTEX
			this.commandMutex.WaitOne();
#endif

			dbgWriteLine("==> " + command);
			this.SIO.put(cmd,(uint) cmd.Length);

			// Start or Restart lockout timer to ignore incoming Rig CAT Answers.
			if (bPollingLockout)
			{
				this.rigPollingLockoutTimer.Interval = pollingLockoutTime;
				this.rigPollingLockout = true;
				this.rigPollingLockoutTimer.Stop();
				this.rigPollingLockoutTimer.Start();
			}

#if USE_COMMAND_MUTEX
			this.commandMutex.ReleaseMutex();
#endif
		}

		private void enqueueRigCATCommand(string command)
		{
			this.rigCommandQueue.Enqueue(command);
			this.rigCommandWaitHandle.Set();
		}

		#endregion Outgoing CAT Commands


		#region Events

		private void RigAnswerLockoutTimerExpiredEvent(object source,
			System.Timers.ElapsedEventArgs e)
		{
			this.rigPollingLockout = false;
		}

		private void RigCommandLockoutTimerExpiredEvent(object source,
			System.Timers.ElapsedEventArgs e)
		{
			this.rigCommandLockout = false;
		}

		private void SerialRXEventHandler(object source,SDRSerialSupportII.SerialRXEvent e)
		{
			this.commBuffer += this.AE.GetString(e.buffer,0,e.buffer.Length);

			try
			{
				// Accept any string ending in ;
				Regex rex = new Regex(".*?;");

				// Loop thru the buffer and find matches
				for (Match m = rex.Match(this.commBuffer); m.Success; m = m.NextMatch())
				{
					if (!this.vfoAInitialized && m.Value.StartsWith("FA"))
						this.vfoAInitialized = true;
					if (!this.vfoBInitialized && m.Value.StartsWith("FB"))
						this.vfoBInitialized = true;

#if USE_COMMAND_MUTEX
					this.commandMutex.WaitOne();
#endif

					// Don't process the Rig's Answer if we're in a lockout state.
					if (this.enabled && !this.rigPollingLockout)
					{
						dbgWriteLine("<== " + m.Value);

						// Send the match to the Rig Parser
						this.rigParser.Answer(m.Value);
					}
					else
						dbgWriteLine("<=| " + m.Value);

#if USE_COMMAND_MUTEX
					this.commandMutex.ReleaseMutex();
#endif

					// Remove the match from the buffer
					this.commBuffer = this.commBuffer.Replace(m.Value,"");
				}
			}
			catch (Exception ex)
			{
//				Debug.WriteLine("RX Event:  " + ex.Message);
//				Debug.WriteLine("RX Event:  " + ex.StackTrace);
			}
		}

		#endregion Events
	}
}

