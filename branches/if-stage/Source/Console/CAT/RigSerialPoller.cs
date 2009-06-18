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


namespace PowerSDR
{
	public class RigSerialPoller
	{
		private Console console;
		private RigHW hw;
		private Rig rig;

		private bool enabled = false;
		private ASCIIEncoding AE = new ASCIIEncoding();

		private Thread pollingThread;
		private bool keepPolling = true;
		private bool vfoAInitialized = false;
		private bool vfoBInitialized = false;

		// Serial Read Handler
		private string commBuffer = "";


		#region Constructor

		public RigSerialPoller(Console console, RigHW hw, Rig rig)
		{
			this.console = console;
			this.hw = hw;
			this.rig = rig;
		}

		~RigSerialPoller()
		{
			this.disable();
		}

		#endregion Constructor


		#region Initialization

		public void enable()
		{
			lock (this)
			{
				if (this.enabled)
					return;

				this.vfoAInitialized = false;
				this.vfoBInitialized = false;

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

				RigHW.dbgWriteLine("RigSerialPoller.disable(), Shutting down Rig Polling Thread.");
				this.keepPolling = false;

				RigHW.dbgWriteLine("RigSerialPoller.disable(), Waiting for Rig Polling Thread to finish...");
				if (this.pollingThread != null)
				{
					this.pollingThread.Join();
					this.pollingThread = null;
				}
			}
		}

		#endregion Initialization


		#region CAT Polling

		private void poll()
		{
			RigHW.dbgWriteLine("RigSerialPoller.poll(), Start.");

			while (this.keepPolling)
			{
				if (!this.vfoAInitialized || !this.vfoBInitialized)
				{
					if (!this.vfoAInitialized)
					{
						Thread.Sleep(this.hw.RigTuningPollingInterval);
						this.rig.getVFOAFreq();
					}

					if (!this.vfoBInitialized)
					{
						Thread.Sleep(this.hw.RigTuningPollingInterval);
						this.rig.getVFOBFreq();
					}
				}
				else
				{
					// Start up the Command Thread after VFO-A and VFO-B are initialized.
					this.rig.startCommandThread();
				}

				Thread.Sleep(this.hw.RigTuningPollingInterval);

				if (!this.rig.rigPollingLockout)
				{
					// If the operator is tuning the VFO Knob, we'll focus on just
					// that for maximum performance!
					if (this.rig.FrequencyChanged)
					{
						if (this.rig.VFO == 0)
							this.rig.getVFOAFreq();
						else if (this.rig.VFO == 1)
							this.rig.getVFOBFreq();

						continue;
					}

					if (this.hw.RigPollVFOB)
						this.rig.getVFOBFreq();

					if (this.hw.RigPollingInterval > this.hw.RigTuningPollingInterval)
						Thread.Sleep(this.hw.RigPollingInterval - this.hw.RigTuningPollingInterval);

					this.rig.getRigInformation();
				}
			}

			RigHW.dbgWriteLine("RigSerialPoller.poll(), End.");
		}

		#endregion CAT Polling


		#region Event Handlers

		public void SerialRXEventHandler(object source, SerialRXEvent e)
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
					if (this.enabled && !this.rig.rigPollingLockout)
					{
						RigHW.dbgWriteLine("<== " + m.Value);

						// Send the match to the Rig Parser
						this.rig.handleRigAnswer(m.Value);
					}
					else
						RigHW.dbgWriteLine("<=| " + m.Value);

					// Remove the match from the buffer
					this.commBuffer = this.commBuffer.Replace(m.Value,"");
				}
			}
			catch (Exception ex)
			{
				RigHW.dbgWriteLine("SerialRXEventHandler Exception: " + ex.Message);
				RigHW.dbgWriteLine(ex.StackTrace);
			}
		}

		#endregion Event Handlers
	}
}

