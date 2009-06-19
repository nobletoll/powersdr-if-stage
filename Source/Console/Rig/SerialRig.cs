//=============================================================================
// Rig.cs
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

using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

using SDRSerialSupportII;


namespace PowerSDR
{
	public abstract class SerialRig : Rig
	{
		private RigCATParser rigParser;

		private RigSerialPoller rigSerialPoller;

		protected SDRSerialPort SIO;

		// Rig Commands
		private bool runRigCommands = true;
		private Thread rigCommandThread;
		private EventWaitHandle rigCommandWaitHandle = new AutoResetEvent(false);
		private Queue rigCommandQueue = Queue.Synchronized(new Queue());


		protected SerialRig(RigHW hw, Console console) : base(hw,console)
		{
			this.rigSerialPoller = new RigSerialPoller(this.console,this.hw,this);
			this.rigParser = new RigCATParser(this.console,this);
		}

		#region Rig States

		private int vfo = 99;
		public int VFO
		{
			get { return this.vfo; }
			set { this.vfo = value; }
		}

		private int vfoaMode = 0;
		public int VFOAMode
		{
			get { return this.vfoaMode; }
			set { this.vfoaMode = value; }
		}

		private int vfobMode = -1;
		public int VFOBMode
		{
			get { return this.vfobMode; }
			set { this.vfobMode = value; }
		}

		private string vfoaFrequency = "";
		public string VFOAFrequency
		{
			get { return this.vfoaFrequency; }
			set { this.vfoaFrequency = value; }
		}

		private string vfobFrequency = "";
		public string VFOBFrequency
		{
			get { return this.vfobFrequency; }
			set { this.vfobFrequency = value; }
		}

		private bool frequencyChanged = false;
		public bool FrequencyChanged
		{
			get { return this.frequencyChanged; }
			set { this.frequencyChanged = value; }
		}

		private int ritOffset = 0;
		public int RITOffset
		{
			get { return this.ritOffset; }
			set { this.ritOffset = value; }
		}

		private bool split = false;
		public bool Split
		{
			get { return this.split; }
			set { this.split = value; }
		}

		#endregion Rig States


		#region Initialization

		public override void connect()
		{
			{
				if (this.connected)
					return;

				this.initRigStates();

				if (this.SIO == null)
				{
					this.SIO = new SDRSerialPort(this.hw.COMPort);

					// Event handler for Serial RX Events
					this.SIO.serial_rx_event +=
						new SerialRXEventHandler(this.rigSerialPoller.SerialRXEventHandler);

					this.SIO.setCommParms(this.hw.COMBaudRate,this.hw.COMParity,
						this.hw.COMDataBits,this.hw.COMStopBits);

					RigHW.dbgWriteLine("Rig.enableSerialConnection(), Opening COM" +
						this.hw.COMPort + "...");

					try
					{
						if (this.SIO.Create() == 0)
							RigHW.dbgWriteLine("Rig.enableSerialConnection(), Opened COM" +
								this.hw.COMPort + ".");
						else
							throw new Exception();
					}
					catch (Exception ex)
					{
						// Event handler for Serial RX Events
						this.SIO.serial_rx_event -=
							new SerialRXEventHandler(this.rigSerialPoller.SerialRXEventHandler);

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
						new SerialRXEventHandler(this.rigSerialPoller.SerialRXEventHandler);
				}

				this.rigSerialPoller.enable();

				this.connected = true;
			}
		}

		public override void disconnect()
		{
			lock (this)
			{
				if (!this.connected)
					return;

				this.connected = false;

				RigHW.dbgWriteLine("Rig.disableSerialConnection(), Shutting down Rig Command Thread.");
				this.runRigCommands = false;
				this.rigCommandWaitHandle.Set();

				this.rigSerialPoller.disable();

				RigHW.dbgWriteLine("Rig.disableSerialConnection(), Waiting for Rig Command Thread to finish...");
				if (this.rigCommandThread != null)
				{
					this.rigCommandThread.Join();
					this.rigCommandThread = null;
				}

				if (this.SIO != null && this.SIO.PortIsOpen)
				{
					RigHW.dbgWriteLine("Rig.disableSerialConnection(), Closing COM" +
						this.hw.COMPort + "...");

					this.SIO.deregisterEventHandlers();
					this.SIO.serial_rx_event -=
						new SerialRXEventHandler(this.rigSerialPoller.SerialRXEventHandler);

					// W1CEG: This hangs...I don't know why, but there's a lot
					//        of discussion on this on the Internet.
					this.SIO.Destroy();
					this.SIO = null;

					RigHW.dbgWriteLine("Rig.disableSerialConnection(), Closed COM" +
						this.hw.COMPort + ".");
				}
			}
		}

		private void initRigStates()
		{
			this.vfoaMode = this.getModeFromDSPMode(this.console.RX1DSPMode);
			this.split = this.console.VFOSplit;
		}

		public void startCommandThread()
		{
			if (this.rigCommandThread != null)
				return;

			this.runRigCommands = true;
			this.rigCommandThread = new Thread(new ThreadStart(this.RigCommandThread));
			this.rigCommandThread.Name = "Rig CAT Command Thread";
			this.rigCommandThread.Start();
		}

		#endregion Initialization


		#region Defaults & Supported Functions

		public abstract override int defaultBaudRate();
		public abstract override bool needsPollVFOB();
		public abstract override bool supportsIFFreq();
		public abstract override int getModeFromDSPMode(DSPMode dspMode);

		#endregion Defaults & Supported Functions


		#region Get CAT Commands

		public abstract override void getRigInformation();
		public abstract override void getVFOAFreq();
		public abstract override void getVFOBFreq();
		public abstract override void getIFFreq();

		#endregion Get CAT Commands

		#region Set CAT Commands

		public abstract override void setVFOAFreq(double freq);
		public abstract override void setVFOBFreq(double freq);
		public abstract override void setVFOAFreq(string freq);
		public abstract override void setVFOBFreq(string freq);
		public abstract override void setVFOA();
		public abstract override void setVFOB();
		public abstract override void setMode(DSPMode mode);
		public abstract override void setSplit(bool splitOn);
		public abstract override void clearRIT();
		
		#endregion Set CAT Commands

		#region Answer CAT Commands

		public void handleRigAnswer(string s)
		{
			this.rigParser.Answer(s);
		}

		#endregion Answer CAT Commands


		#region Rig Commands

		protected void enqueueRigCATCommand(string command)
		{
			this.rigCommandQueue.Enqueue(command);
			this.rigCommandWaitHandle.Set();
		}

		protected void doRigCATCommand(string command)
		{
			this.doRigCATCommand(command,false,true);
		}

		protected void doRigCATCommand(string command, bool bPollingLockout,
			bool bCheckRigPollingLockout)
		{
			this.doRigCATCommand(command,bPollingLockout,
				this.hw.RigPollingLockoutTime,bCheckRigPollingLockout);
		}

		protected void doRigCATCommand(string command, bool bPollingLockout,
			int pollingLockoutTime, bool bCheckRigPollingLockout)
		{
			if (!this.connected ||
				(bCheckRigPollingLockout && this.rigPollingLockout))
				return;

			byte[] cmd = this.AE.GetBytes(command);

			RigHW.dbgWriteLine("==> " + command);
			this.SIO.put(cmd,(uint) cmd.Length);

			// Start or Restart lockout timer to ignore incoming Rig CAT Answers.
			if (bPollingLockout && pollingLockoutTime > 0)
			{
				this.rigPollingLockoutTimer.Interval = this.hw.RigPollingLockoutTime;
				this.rigPollingLockout = true;
				this.rigPollingLockoutTimer.Stop();
				this.rigPollingLockoutTimer.Start();
			}
		}

		#endregion Rig Commands


		#region Threads

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
			RigHW.dbgWriteLine("Rig.RigCommandThread(), Start.");

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

			RigHW.dbgWriteLine("Rig.RigCommandThread(), End.");
		}

		#endregion Threads


		#region Event Handlers

		private void RigPollingLockoutTimerExpiredEvent(object source,
			System.Timers.ElapsedEventArgs e)
		{
			this.rigPollingLockout = false;
		}

		#endregion Event Handlers
	}
}
