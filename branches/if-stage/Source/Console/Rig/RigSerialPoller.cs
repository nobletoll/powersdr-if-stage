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

using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;
using System.Text;

using SDRSerialSupportII;
using System;
using System.Windows.Forms;


namespace PowerSDR
{
	public class RigSerialPoller
	{
		private Console console;
		private RigHW hw;
		private SerialRig rig;

		private bool enabled = false;
		private ASCIIEncoding AE = new ASCIIEncoding();

		private Thread pollingThread;
		private bool keepPolling = true;
		private bool initializing = false;
		private bool pollError = false;
		private bool vfoAInitialized = false;
		private bool vfoBInitialized = false;
		private EventWaitHandle communicationEstablishedWaitHandle =
			new EventWaitHandle(true,EventResetMode.ManualReset);

		// Serial Read Handler
		private string commBuffer = "";


		#region Constructor

		public RigSerialPoller(Console console, RigHW hw, SerialRig rig)
		{
			this.console = console;
			this.hw = hw;
			this.rig = rig;
		}

		#endregion Constructor


		#region Initialization

		public void enable()
		{
			lock (this)
			{
				if (this.enabled)
					return;

				this.initializing = true;
				this.pollError = false;
				this.vfoAInitialized = false;
				this.vfoBInitialized = false;
				this.communicationEstablishedWaitHandle.Reset();

				this.keepPolling = true;
				this.pollingThread = new Thread(new ThreadStart(this.poll));
				this.pollingThread.Name = "Rig Polling Thread";
				this.pollingThread.Start();

				this.enabled = true;
			}
		}

		public void disable()
		{
			lock (this)
			{
				if (!this.enabled)
					return;

				this.enabled = false;

				this.hw.logGeneral("RigSerialPoller.disable(), Shutting down Rig Polling Thread.");
				this.keepPolling = false;

				this.hw.logGeneral("RigSerialPoller.disable(), Waiting for Rig Polling Thread to finish...");
				if (this.pollingThread != null)
				{
					this.pollingThread.Join();
					this.pollingThread = null;
				}
			}
		}

		public void checkConnectionEstablished()
		{
			if (!this.initializing &&
				!this.communicationEstablishedWaitHandle.WaitOne(1))
				MessageBox.Show("Unable to connect to the " + this.hw.getRigName() + " on COM" +
					this.hw.COMPort + ".  Please make sure that it is connected and turned on.","Error",
					MessageBoxButtons.OK,MessageBoxIcon.Error);
		}

		#endregion Initialization


		#region CAT Polling

		private void poll()
		{
			int lastVFOBPoll = System.Environment.TickCount;

			this.hw.logGeneral("RigSerialPoller.poll(), Start.");

			while (this.keepPolling)
			{
				bool sleep = false;
				int sleepTime = 0;

				if (!this.vfoAInitialized || !this.vfoBInitialized)
				{
					if (!this.vfoAInitialized)
					{
						Thread.Sleep(this.hw.RigTuningPollingInterval);
						sleepTime += this.hw.RigTuningPollingInterval;
						this.initializing = true;
						this.rig.getVFOAFreq();
						this.initializing = false;
						if (!this.communicationEstablishedWaitHandle.WaitOne(5000))
						{
							if (!this.pollError)
							{
								this.pollError = true;
								MessageBox.Show("Unable to connect to the " + this.hw.getRigName() + " on COM" +
									this.hw.COMPort + ".  Please make sure that it is connected and turned on.","Error",
									MessageBoxButtons.OK,MessageBoxIcon.Error);
							}

							continue;
						}
					}

					if (!this.vfoBInitialized)
					{
						Thread.Sleep(this.hw.RigTuningPollingInterval);
						sleepTime += this.hw.RigTuningPollingInterval;
						this.rig.getVFOBFreq();
					}
				}
				else
				{
					// Start up the Command Thread after VFO-A and VFO-B are initialized.
					this.rig.startCommandThread();
				}

				Thread.Sleep(this.hw.RigTuningPollingInterval);
				sleepTime += this.hw.RigTuningPollingInterval;

				if (!this.rig.rigPollingLockout)
				{
					// If the operator is tuning the VFO Knob, we'll focus on just
					// that for maximum performance!
					if (this.rig.VFOAFrequencyChanged || this.rig.VFOBFrequencyChanged)
					{
						if (this.rig.VFOAFrequencyChanged)
							this.rig.getVFOAFreq();
						if (this.rig.VFOBFrequencyChanged)
							this.rig.getVFOBFreq();

						continue;
					}

					if (this.hw.RigPollVFOB)
					{
						/* Special Case for TS-950.  It seems to be sensitive
						 * to excessive VFO-B polling, so we are going to keep
						 * it to a minimum.
						 * 
						 * :NOTE: On the TS-950, you can only change VFO-B
						 *        when SPLIT is on (assuming we enforce RX only
						 *        on VFO-A.
						 */
						if (this.rig is Kenwood950Rig)
						{
							if (this.rig.Split)
							{
								int tick = System.Environment.TickCount;

								if ((tick - lastVFOBPoll) > (this.hw.RigPollingInterval * 5))
								{
									this.rig.getVFOBFreq();
									lastVFOBPoll = tick;
									sleep = true;
								}
							}
						}
						else
						{
							this.rig.getVFOBFreq();
							sleep = true;
						}
					}


					// :TODO: Query from RigHW...
					// :TODO: Do this for K3Rig (and other supporting rigs) as well...
					if (this.rig is YaesuRig)
					{
						if (sleep)
						{
							Thread.Sleep(10);
							sleepTime += 10;
						}

						this.rig.getTX();
						Thread.Sleep(10);
						sleepTime += 10;
						this.rig.getTXVFO();
						sleep = true;
					}

					if (this.hw.RigPollFilterWidth)
					{
						if (sleep)
						{
							Thread.Sleep(10);
							sleepTime += 0;
						}

						this.rig.getRX1FilterWidth();
						sleep = true;
					}

					Thread.Sleep(Math.Max(this.hw.RigTuningPollingInterval,
						this.hw.RigPollingInterval - sleepTime));

					this.rig.getRigInformation();
				}

				// :NOTE: We want to still poll for the IFFreq during poll lockout.
				if (this.hw.RigPollIFFreq)
					this.rig.getIFFreq();
			}

			this.hw.logGeneral("RigSerialPoller.poll(), End.");
		}

		#endregion CAT Polling


		#region Event Handlers

		public void SerialRXEventHandler(object source, SerialRXEvent e)
		{
			this.commBuffer += this.AE.GetString(e.buffer,0,e.buffer.Length);

			try
			{
#if (DEBUG)
				string cmd = this.console.txtRigAnsInjection.Text;
				if (source == this.console && e.buffer.Length == 0 &&
					this.console.txtRigAnsInjection.Text.Length > 0)
				{
					this.console.txtRigAnsInjection.Text = "";

					if (!cmd.EndsWith(";"))
						cmd += ";";

					this.hw.logIncomingCAT("<- " + cmd);

					// Send the match to the Rig Parser
					this.rig.handleRigAnswer(cmd);
				}
#endif

				// Accept any string ending in ;
				Regex rex = new Regex(".*?;");

				// Loop thru the buffer and find matches
				for (Match m = rex.Match(this.commBuffer); m.Success; m = m.NextMatch())
				{
					if (!this.vfoAInitialized && m.Value.StartsWith("FA"))
						this.vfoAInitialized = true;
					if (!this.vfoBInitialized && m.Value.StartsWith("FB"))
						this.vfoBInitialized = true;

					try
					{
						// Don't process the Rig's Answer if we're in a lockout state.
						// :NOTE: Allow FI during Polling Lockout
						if ((this.enabled && !this.rig.rigPollingLockout) || m.Value.StartsWith("FI"))
						{
							this.communicationEstablishedWaitHandle.Set();
							this.hw.logIncomingCAT("<- " + m.Value);

							// Send the match to the Rig Parser
							this.rig.handleRigAnswer(m.Value);
						}
						else
							this.hw.logIncomingCAT("|<- " + m.Value);
					}
					catch (Exception ex)
					{
						this.hw.logGeneral("SerialRXEventHandler Exception: " + ex.Message);
						this.hw.logGeneral(ex.StackTrace);
					}

					// Remove the match from the buffer
					this.commBuffer = this.commBuffer.Replace(m.Value,"");
				}
			}
			catch (Exception ex)
			{
				this.hw.logGeneral("SerialRXEventHandler Exception: " + ex.Message);
				this.hw.logGeneral(ex.StackTrace);
			}
		}

		#endregion Event Handlers
	}
}

