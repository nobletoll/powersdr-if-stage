//=============================================================================
// Alpha8100MeterParser.cs
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
using System.Text;
using System.Text.RegularExpressions;

namespace PowerSDR
{
	public class Alpha8100MeterParser
	{
		#region Variables

		private Console console;
		private Alpha8100Meter meter;

		private ASCIIEncoding AE = new ASCIIEncoding();

		#endregion Variables


		#region Constructor

		public Alpha8100MeterParser(Console console, Alpha8100Meter meter)
		{
			this.console = console;
			this.meter = meter;
		}

		#endregion Constructor


		#region Answer

		public void Answer(byte[] answer)
		{
			/* Format:
			 * 
			 * MagicNumber,FwdPower,RevPower,InputRF (10th Watt),PlateVoltage,AmpGain (10th unit),V on Grid 1 (- 10th V),GridCurrent (10th mA),Band,Mode (0:off 1:warmup 2:standby 3:on 4:keyed up),FaultCode,Fletcher checksum
			 * $APA01,01536,00002,0595,2654,0867,260,0519,008,3,4,00,0*8DA3
			 */
			string data = this.AE.GetString(answer);

			// Check magic numbers...
			if (answer.Length < 7 || !data.Substring(0,7).Equals("$APA01,"))
				return;

			int fwdPowerIdx = 7;
			int fwdPowerEnd = data.IndexOf(',',fwdPowerIdx);
			int fwdPowerLen = fwdPowerEnd - fwdPowerIdx;
			int revPowerIdx = fwdPowerEnd + 1;
			int revPowerLen = data.IndexOf(',',revPowerIdx) - revPowerIdx;

			int fwdPower = int.Parse(data.Substring(fwdPowerIdx,fwdPowerLen));
			int revPower = int.Parse(data.Substring(revPowerIdx,revPowerLen));
			this.meter.FwdPower = fwdPower;
			this.meter.RevPower = revPower;

			// :TODO: Is it save/accurate to use this SWR function??
			this.meter.VSWR = this.console.SWR(fwdPower,revPower);

			MeterHW.dbgWriteLine("Forward: " + this.meter.FwdPower + "W  Reflected: " + this.meter.RevPower + "W  VSWR: " + this.meter.VSWR);
		}

		#endregion Answer
	}
}
