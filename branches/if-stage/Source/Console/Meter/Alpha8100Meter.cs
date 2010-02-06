//=============================================================================
// Alpha8100Meter.cs
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

using SDRSerialSupportII;


namespace PowerSDR
{
	public class Alpha8100Meter : Meter
	{
		private Alpha8100MeterParser parser;

		private ASCIIEncoding AE = new ASCIIEncoding();


		public Alpha8100Meter(MeterHW hw, Console console) : base(hw,console)
		{
			this.parser = new Alpha8100MeterParser(this.console,this);
		}

		
		#region Initialization

		public override void disconnect()
		{
			base.disconnect();
		}

		#endregion Initialization


		#region Defaults & Supported Functions

		public override int defaultBaudRate()
		{
			return 115200;
		}

		#endregion Defaults & Supported Functions


		#region Get Commands

		#endregion Get Commands


		#region Set Commands

		public override void startDataReporting()
		{
			this.mox = true;
		}

		public override void stopDataReporting()
		{
			this.mox = false;
		}

		#endregion Set Commands


		#region Answer Commands

		public override void handleMeterAnswer(byte[] answer)
		{
			this.parser.Answer(answer);
		}

		#endregion Answer Commands


		#region Commands

		private void doCommand(string command)
		{
			byte[] bCmd = this.AE.GetBytes(command);

			MeterHW.dbgWriteLine("--> " + command);

			this.SIO.put(bCmd,(uint) bCmd.Length);
		}

		#endregion Commands
	}
}
