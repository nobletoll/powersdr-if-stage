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
		private bool connected = false;

		// Rig Commands
		private bool runRigCommands = true;
		private Thread rigCommandThread;
		private string pendingRigCommand = null;
		private AutoResetEvent pendingRigCommandWaitHandle = new AutoResetEvent(false);
		private EventWaitHandle noPendingCommandWaitHandle =
			new EventWaitHandle(true,EventResetMode.ManualReset);
		private object pendingRigCommandSyncObject = new object();


		protected SerialRig(RigHW hw, Console console) : base(hw,console)
		{
			this.rigSerialPoller = new RigSerialPoller(this.console,this.hw,this);
			this.rigParser = new RigCATParser(this.console,this.hw,this);
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

		private bool vfoaFrequencyChanged = false;
		public bool VFOAFrequencyChanged
		{
			get { return this.vfoaFrequencyChanged; }
			set { this.vfoaFrequencyChanged = value; }
		}

		private bool vfobFrequencyChanged = false;
		public bool VFOBFrequencyChanged
		{
			get { return this.vfobFrequencyChanged; }
			set { this.vfobFrequencyChanged = value; }
		}

		private int ritOffset = 0;
		public int RITOffset
		{
			get { return this.ritOffset; }
			set { this.ritOffset = value; }
		}

		private bool ritOffsetInitialized = false;
		public bool RITOffsetInitialized
		{
			get { return this.ritOffsetInitialized; }
			set { this.ritOffsetInitialized = value; }
		}

		private bool split = false;
		public bool Split
		{
			get { return this.split; }
			set { this.split = value; }
		}

		private bool txWithMute = false;
		public bool TXWithMute
		{
			get { return this.txWithMute; }
			set { this.txWithMute = value; }
		}

		#endregion Rig States


		#region Initialization

		public override void connect()
		{
			lock (this)
			{
				if (this.active || this.connected)
					return;

				this.initRigStates();

				if (this.SIO == null)
				{
					this.SIO = new SDRSerialPort(this.hw.COMPort);
					this.setRTS();

					// Event handler for Serial RX Events
					this.SIO.serial_rx_event +=
						new SerialRXEventHandler(this.rigSerialPoller.SerialRXEventHandler);

					this.SIO.setCommParms(this.hw.COMBaudRate,this.hw.COMParity,
						this.hw.COMDataBits,this.hw.COMStopBits);

					this.hw.logGeneral("SerialRig.connect(), Opening COM" +
						this.hw.COMPort + "...");

					try
					{
						if (this.SIO.Create() == 0)
							this.hw.logGeneral("SerialRig.connect(), Opened COM" +
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

				this.active = true;
				this.connected = true;
			}
		}

		public override void disconnect()
		{
			lock (this)
			{
				if (!this.active || !this.connected)
					return;

				this.active = false;

				this.hw.logGeneral("SerialRig.disconnect(), Shutting down Rig Command Thread.");
				this.runRigCommands = false;
				this.pendingRigCommandWaitHandle.Set();

				this.rigSerialPoller.disable();

				this.hw.logGeneral("SerialRig.disconnect(), Waiting for Rig Command Thread to finish...");
				if (this.rigCommandThread != null)
				{
					this.rigCommandThread.Join();
					this.rigCommandThread = null;
				}

				if (this.SIO != null && this.SIO.PortIsOpen)
				{
					this.hw.logGeneral("SerialRig.disconnect(), Deregistering COM" +
						this.hw.COMPort + " Handlers...");

					this.SIO.deregisterEventHandlers();
					this.SIO.serial_rx_event -=
						new SerialRXEventHandler(this.rigSerialPoller.SerialRXEventHandler);

					/* :Issue 67: When the app is closing, we don't want to
					 *            bother closing the serial ports.
					 *                    
					 *            This will reduce the likelyhood of a hang
					 *            on Serial close.  Let the OS clean up the
					 *            open handles.
					 *            
					 *            If we do have to close the serial port, do
					 *            it in a separate thread so we don't block
					 *            the main UI thread.
					 */
					if (!this.console.ConsoleClosing)
						new Thread(new ThreadStart(this.destroySIO)).Start();
				}
			}
		}

		private void destroySIO()
		{
			this.hw.logGeneral("SerialRig.destroySIO(), Closing COM" +
				this.hw.COMPort + "...");

			this.SIO.Destroy();
			this.SIO = null;
			this.connected = false;

			this.hw.logGeneral("SerialRig.destroySIO(), Closed COM" +
				this.hw.COMPort + ".");
		}

		private void initRigStates()
		{
			this.vfoaMode = this.getModeFromDSPMode(this.console.RX1DSPMode);
			this.split = this.console.VFOSplit;
			this.ritOffsetInitialized = false;
			this.txWithMute = false;
		}

		protected virtual void setRTS()
		{
			this.SIO.setRTS(true);
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
		public abstract void setConsoleModeFromString(string mode);
		public abstract bool ritAppliedInIFCATCommand();
		public abstract bool useRITForVFOB();

		#endregion Defaults & Supported Functions


		#region Get CAT Commands

		public abstract override void getRigInformation();
		public abstract override void getVFOAFreq();
		public abstract override void getVFOBFreq();
		public abstract override void getIFFreq();
		public abstract void getTX();
		public abstract void getTXVFO();

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
			bool waitForPendingRigCommand = false;

			lock (this.pendingRigCommandSyncObject)
			{
				/* :Issue 57: If the command is different than the pending
				 *            command, we need to make sure it happens.
				 *            
				 *            For example, a A <> B swap needs to do FA and FB
				 *            commands right after one another.
				 */
				waitForPendingRigCommand = (this.pendingRigCommand != null &&
					(command[0] != this.pendingRigCommand[0] ||
					command[1] != this.pendingRigCommand[1]));
			}

			if (waitForPendingRigCommand)
				this.noPendingCommandWaitHandle.WaitOne();

			lock (this.pendingRigCommandSyncObject)
			{
				if (this.rigCommandThread != null)
				{
					this.pendingRigCommand = command;
					this.noPendingCommandWaitHandle.Reset();
					this.pendingRigCommandWaitHandle.Set();
				}
			}
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
			if (!this.active ||
				(bCheckRigPollingLockout && this.rigPollingLockout))
				return;

			this.rigSerialPoller.checkConnectionEstablished();

			byte[] cmd = this.AE.GetBytes(command);

			this.hw.logOutgoingCAT("-> " + command);
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
			this.hw.logGeneral("SerialRig.RigCommandThread(), Start.");

			lock (this.pendingRigCommandSyncObject)
			{
				this.pendingRigCommand = null;
			}

			while (this.runRigCommands)
			{
				this.pendingRigCommandWaitHandle.WaitOne();

				if (!this.runRigCommands)
					break;

				lock (this.pendingRigCommandSyncObject)
				{
					if (this.pendingRigCommand != null)
					{
						this.doRigCATCommand(this.pendingRigCommand,true,false);
						this.pendingRigCommand = null;
						this.noPendingCommandWaitHandle.Set();
					}
				}

				Thread.Sleep(this.hw.RigTuningCATInterval);
			}

			this.hw.logGeneral("SerialRig.RigCommandThread(), End.");
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
