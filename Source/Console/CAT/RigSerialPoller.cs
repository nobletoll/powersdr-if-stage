#define DBG_PRINT

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
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

		private int rigCOMPort = 1;

		private Thread pollingThread;
		private bool keepPolling = true;

		// States for Polling Performance
		private bool vfoAInitialized = false;
		private bool vfoBInitialized = false;

		private Console console;
		private SDRSerialSupportII.SDRSerialPort SIO;
		private RigCATParser rigParser;
		private CATParser sdrParser;

		private ASCIIEncoding AE = new ASCIIEncoding();
		private static string separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
		private bool enabled = false;
		private string commBuffer = "";

		private EventWaitHandle wh = new AutoResetEvent(false);
		private bool rigAnswerLockout = false;
		private System.Timers.Timer rigAnswerLockoutTimer = new System.Timers.Timer();

		#endregion Variables


		#region Constructor

		public RigSerialPoller(Console console)
		{
			this.console = console;

			this.rigParser = new RigCATParser(console);
			this.sdrParser = new CATParser(console);

			// Initialize Rig Answer Lockout Timer
			this.rigAnswerLockoutTimer.Elapsed +=
				new System.Timers.ElapsedEventHandler(this.RigLockoutTimerExpiredEvent);
			this.rigAnswerLockoutTimer.Interval = 1000;
			this.rigAnswerLockoutTimer.AutoReset = false;
			this.rigAnswerLockoutTimer.Enabled = false;          
		}

		~RigSerialPoller()
		{
			this.disableCAT();
		}

		#endregion Constructor


		#region Methods

		private static void dbgWriteLine(string s)
		{
#if (DBG_PRINT)
			System.Console.Out.WriteLine(s);
#endif
		}

		public void enableCAT()
		{
			lock (this)
			{
				if (this.enabled)
					return;

				if (this.SIO == null)
				{
					this.SIO = new SDRSerialPort(this.rigCOMPort);

					// Event handler for Serial RX Events
					this.SIO.serial_rx_event +=
						new SDRSerialSupportII.SerialRXEventHandler(SerialRXEventHandler);

					this.SIO.setCommParms(4800,Parity.None,SDRSerialPort.DataBits.EIGHT,StopBits.One);

					dbgWriteLine("RigSerialPoller.enableCAT(), Opening COM" +
						this.rigCOMPort + "...");

					if (this.SIO.Create() == 0)
						dbgWriteLine("RigSerialPoller.enableCAT(), Opened COM" +
							this.rigCOMPort + ".");
					else
						dbgWriteLine("RigSerialPoller.enableCAT(), Failed to open COM" +
							this.rigCOMPort + ".");
				}
				else
				{
					this.SIO.registerEventHandlers();

					// Event handler for Serial RX Events
					this.SIO.serial_rx_event +=
						new SDRSerialSupportII.SerialRXEventHandler(SerialRXEventHandler);
				}

				this.keepPolling = true;
				this.pollingThread = new Thread(new ThreadStart(this.poll));
				this.pollingThread.Name = "Kenwood COM Port CAT Polling Thread";
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
				dbgWriteLine("RigSerialPoller.disableCAT(), Waiting for Polling Thread to finish...");

				this.keepPolling = false;
				this.pollingThread.Join();

				if (this.SIO != null && this.SIO.PortIsOpen)
				{
					dbgWriteLine("RigSerialPoller.disableCAT(), Closing COM" +
						this.rigCOMPort + "...");

					// W1CEG: This hangs...I don't know why, but there's a lot
					//        of discussion on this on the Internet.
//					this.SIO.Destroy();

					this.SIO.deregisterEventHandlers();
					this.SIO.serial_rx_event -=
						new SDRSerialSupportII.SerialRXEventHandler(SerialRXEventHandler);

					dbgWriteLine("RigSerialPoller.disableCAT(), Closed COM" +
						this.rigCOMPort + ".");
				}
			}
		}

		#endregion Methods


		#region CAT Polling

		private void poll()
		{
			byte[] cmdIF = this.AE.GetBytes("IF;");
			byte[] cmdFA = this.AE.GetBytes("FA;");
			byte[] cmdFB = this.AE.GetBytes("FB;");

			dbgWriteLine("RigSerialPoller.pollingThread(), Start.");

			while (this.keepPolling)
			{
				if (!this.vfoAInitialized)
				{
					Thread.Sleep(50);
					this.SIO.put(cmdFA,(uint) cmdFA.Length);
				}

				if (!this.vfoBInitialized)
				{
					Thread.Sleep(50);
					this.SIO.put(cmdFB,(uint) cmdFB.Length);
				}

				Thread.Sleep(50);

				// If the operator is tuning the VFO Knob, we'll focus on just
				// that for maximum performance!
				if (this.rigParser.FrequencyChanged)
				{
					if (this.rigParser.VFO == 0)
						this.SIO.put(cmdFA,(uint) cmdFA.Length);
					else if (this.rigParser.VFO == 1)
						this.SIO.put(cmdFB,(uint) cmdFB.Length);
					else
						this.SIO.put(cmdIF,(uint) cmdIF.Length);
				}
				else
				{
					// Sleep an additional 50ms if we're in an "idle" state.
					Thread.Sleep(50);
					this.SIO.put(cmdIF,(uint) cmdIF.Length);
				}
			}

			dbgWriteLine("RigSerialPoller.pollingThread(), End.");
		}

		#endregion CAT Polling


		#region Outgoing CAT Commands

		public void updateVFOAFrequency(double freq)
		{
			string frequency =
				freq.ToString("f6").Replace(separator,"").PadLeft(11,'0');

			// Only do this if our Frequency State has changed.
			// :TODO: Do we need to pay attention to the VFO state?
			if (frequency == this.rigParser.Frequency)
				return;

			this.doRigCATCommand("FA" + frequency + ';');

			// Set our Frequency State so we don't do this again.
			this.rigParser.Frequency = frequency;
		}

		public void updateVFOBFrequency(double freq)
		{
			string frequency =
				freq.ToString("f6").Replace(separator,"").PadLeft(11,'0');

			// Only do this if our Frequency State has changed.
			// :TODO: Do we need to pay attention to the VFO state?
			if (frequency == this.rigParser.Frequency)
				return;

			this.doRigCATCommand("FB" + frequency + ';');

			// Set our Frequency State so we don't do this again.
			this.rigParser.Frequency = frequency;
		}

		private void doRigCATCommand(string command)
		{
			if (!enabled)
				return;

			byte[] bCommand = this.AE.GetBytes(command);

dbgWriteLine("CAT Command To Rig: " + command);

			this.SIO.put(bCommand,(uint) bCommand.Length);

			// Start or Restart lockout timer to ignore incoming Rig CAT Answers.
			this.rigAnswerLockout = true;
			this.rigAnswerLockoutTimer.Stop();
			this.rigAnswerLockoutTimer.Start();
		}

		#endregion Outgoing CAT Commands


		#region Events

		private void RigLockoutTimerExpiredEvent(object source, System.Timers.ElapsedEventArgs e)
		{
			this.rigAnswerLockout = false;
		}

		private void SerialRXEventHandler(object source, SDRSerialSupportII.SerialRXEvent e)
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

					// Don't process the Rig's Answer if we're in a lockout state.
					if (!this.rigAnswerLockout)
					{
// dbgWriteLine(m.Value);
						// Send the match to the Rig Parser
						this.rigParser.Answer(m.Value);
					}

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

