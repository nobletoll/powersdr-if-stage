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
	public abstract class Rig
	{
		protected RigHW hw;
		protected Console console;

		public bool rigPollingLockout = false;
		protected System.Timers.Timer rigPollingLockoutTimer = new System.Timers.Timer();

		protected bool connected = false;

		protected ASCIIEncoding AE = new ASCIIEncoding();
		protected static string separator =
			CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;


		protected Rig(RigHW hw, Console console)
		{
			this.hw = hw;
			this.console = console;

			// Initialize Rig Polling Lockout Timer
			this.rigPollingLockoutTimer.Elapsed +=
				new ElapsedEventHandler(this.RigPollingLockoutTimerExpiredEvent);
			this.rigPollingLockoutTimer.Interval = this.hw.RigPollingLockoutTime;
			this.rigPollingLockoutTimer.AutoReset = false;
			this.rigPollingLockoutTimer.Enabled = false;
		}


		#region Initialization

		public abstract void connect();
		public abstract void disconnect();

		#endregion Initialization


		#region Defaults & Supported Functions

		public abstract int defaultBaudRate();
		public abstract bool needsPollVFOB();
		public abstract bool supportsIFFreq();
		public abstract int getModeFromDSPMode(DSPMode dspMode);

		#endregion Defaults & Supported Functions


		#region Get CAT Commands

		public abstract void getRigInformation();
		public abstract void getVFOAFreq();
		public abstract void getVFOBFreq();
		public abstract void getIFFreq();

		#endregion Get CAT Commands

		#region Set CAT Commands

		public abstract void setVFOAFreq(double freq);
		public abstract void setVFOBFreq(double freq);
		public abstract void setVFOAFreq(string freq);
		public abstract void setVFOBFreq(string freq);
		public abstract void setVFOA();
		public abstract void setVFOB();
		public abstract void setMode(DSPMode mode);
		public abstract void setSplit(bool splitOn);
		public abstract void clearRIT();
		
		#endregion Set CAT Commands


		#region Event Handlers

		private void RigPollingLockoutTimerExpiredEvent(object source,
			System.Timers.ElapsedEventArgs e)
		{
			this.rigPollingLockout = false;
		}

		#endregion Event Handlers
	}
}
