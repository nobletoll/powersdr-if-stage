//=============================================================================
// MeterSerialPoller.cs
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

using System;
using System.Diagnostics;


namespace PowerSDR
{
	public class MeterSerialPoller
	{
		private Console console;
		private MeterHW hw;
		private Meter meter;

		private bool enabled = false;

		private Thread pollingThread;
		private bool keepPolling = true;

		// Serial Read Handler
		private byte[] commBuffer = null;


		#region Constructor

		public MeterSerialPoller(Console console, MeterHW hw, Meter meter)
		{
			this.console = console;
			this.hw = hw;
			this.meter = meter;
		}

		~MeterSerialPoller()
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

				this.keepPolling = true;
				this.pollingThread = new Thread(new ThreadStart(this.poll));
				this.pollingThread.Name = "Meter Polling Thread";
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

				this.hw.logGeneral("MeterSerialPoller.disable(), Shutting down Meter Polling Thread.");
				this.keepPolling = false;

				this.hw.logGeneral("MeterSerialPoller.disable(), Waiting for Meter Polling Thread to finish...");
				if (this.pollingThread != null)
				{
					this.pollingThread.Join();
					this.pollingThread = null;
				}
			}
		}

		#endregion Initialization


		#region Polling

		private void poll()
		{
			this.hw.logGeneral("MeterSerialPoller.poll(), Start.");

			while (this.keepPolling)
			{
				Thread.Sleep(this.hw.MeterTimingInterval);

				// :NOTE: Only poll when in TX
				if (this.meter.MOX)
					this.meter.getMeterInformation();
			}

			this.hw.logGeneral("MeterSerialPoller.poll(), End.");
		}

		#endregion CAT Polling
	}
}

