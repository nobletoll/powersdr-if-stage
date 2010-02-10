//=============================================================================
// PowerMasterMeterParser.cs
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
	public class PowerMasterMeterParser
	{
		#region Variables

		private Console console;
		private PowerMasterMeter meter;

		#endregion Variables


		#region Constructor

		public PowerMasterMeterParser(Console console, PowerMasterMeter meter)
		{
			this.console = console;
			this.meter = meter;
		}

		#endregion Constructor


		#region Answer

		// Overloaded Get method accepts either byte or string
		public void Answer(byte[] answer)
		{
			if (!PowerMasterCRC.CheckCRC(answer))
				return;

			// Remove STX, Command, ETX, and CRC from answer.
			// Format:  STX Command Data ETX CRC1 CRC2
			string data = this.meter.AE.GetString(answer,2,answer.Length - 5);

			switch ((char) answer[1])
			{
				case 'D':
					this.DataAnswer(data);
					break;
			}
		}

		#endregion Answer


		#region Answer Commands

		/**
		 * ,forward power,reflected power,VSWR,status1;status2;status3;status4;status5
		 *
		 * :NOTE: 'D' command has been stripped off at this point.
		 * 
		 * Powers are ints from 0 to 20200. VSWR is float from 1.00 to 99.99 or  0.00 in idle mode.
		 *     Status1 - VSWR Alarm
		 *     Status2 - Low Power Alarm.
		 *     Status3 – High Power Alarm
		 *     Status4 – Red LED On
		 *     Status5 – Yellow LED On
		 */
		private void DataAnswer(string data)
		{
			// +----------------------------------+
			// | 00000000001111111111222222222233 |
			// | 01234567890123456789012345678901 |
			// |----------------------------------|
			// | ,    0.0,    0.0, 0.00,0;0;0;0;0 |
			// +----------------------------------+

			this.meter.FwdPower = double.Parse(data.Substring(1,7).TrimStart(' '));
			this.meter.RevPower = double.Parse(data.Substring(9,7).TrimStart(' '));
			this.meter.VSWR = double.Parse(data.Substring(17,5).TrimStart(' '));

			MeterHW.dbgWriteLine("Forward: " + this.meter.FwdPower + "W  Reflected: " + this.meter.RevPower + "W  VSWR: " + this.meter.VSWR);
		}

		#endregion Answer Commands
	}
}
