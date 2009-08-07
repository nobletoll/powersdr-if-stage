//=============================================================================
// PowerMasterMeter.cs
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
	public class PowerMasterMeter : Meter
	{
		private PowerMasterMeterParser parser;

		private ASCIIEncoding AE = new ASCIIEncoding();


		public PowerMasterMeter(MeterHW hw, Console console) : base(hw,console)
		{
			this.parser = new PowerMasterMeterParser(this.console,this);
		}

		
		#region Initialization

		public override void disconnect()
		{
			// :Issue 58: Tell the PowerMaster to stop sending real-time data
			//            the Rig is in TX when PowerSDR goes to Standby.
			this.stopDataReporting();

			base.disconnect();
		}

		#endregion Initialization


		#region Defaults & Supported Functions

		public override int defaultBaudRate()
		{
			return 38400;
		}

		#endregion Defaults & Supported Functions


		#region Get Commands

		#endregion Get Commands


		#region Set Commands

		public override void startDataReporting()
		{
			int rate;

			if (this.mox)
				return;

			if (this.hw.MeterTimingInterval < 70)
				rate = 1;
			else if (this.hw.MeterTimingInterval < 140)
				rate = 2;
			else if (this.hw.MeterTimingInterval < 280)
				rate = 3;
			else
				rate = 4;

			this.doCommand("D" + rate);
			this.mox = true;
		}

		public override void stopDataReporting()
		{
			if (!this.mox)
				return;

			this.doCommand("D0");
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
			PowerMasterCRC crc = new PowerMasterCRC();

			foreach (char b in command)
				crc.calculateCRC((byte) b);

			byte[] bCmd = this.AE.GetBytes(command);
			byte[] bData = new byte[command.Length + 5];

			// Format:  STX Command ETX CRC1 CRC2 CR
			bData[0] = PowerMasterCRC.STX;
			Array.Copy(bCmd,0,bData,1,bCmd.Length);
			bData[bCmd.Length + 1] = PowerMasterCRC.ETX;
			bData[bCmd.Length + 2] = PowerMasterCRC.MakeASCIIHexFromBinary((byte) (crc.CRC >> 4));
			bData[bCmd.Length + 3] = PowerMasterCRC.MakeASCIIHexFromBinary((byte) (crc.CRC & 0x0F));
			bData[bCmd.Length + 4] = (byte) '\r';

			MeterHW.dbgWriteLine("--> " + Meter.PrintBuffer(bData));

			PowerMasterCRC.CheckCRC(bData);

			this.SIO.put(bData,(uint) bData.Length);
		}

		#endregion Commands
	}
}
