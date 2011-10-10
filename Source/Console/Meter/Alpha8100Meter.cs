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



namespace PowerSDR
{
	public class Alpha8100Meter : Meter
	{
		private MeterSerialPoller meterSerialPoller;
		private Alpha8100MeterParser parser;


		public Alpha8100Meter(MeterHW hw, Console console) : base(hw,console)
		{
			this.meterSerialPoller = new MeterSerialPoller(this.console,this.hw,this);
			this.parser = new Alpha8100MeterParser(console,hw,this);
		}

		
		#region Initialization

		public override void connect()
		{
			base.connect();

			this.meterSerialPoller.enable();
		}

		public override void disconnect()
		{
			this.meterSerialPoller.disable();

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

		public override void getMeterInformation()
		{
			this.doCommand("v");
		}

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
			byte[] bCmd = new byte[command.Length + 1];

			Array.Copy(this.AE.GetBytes(command),0,bCmd,0,command.Length);
			bCmd[command.Length] = (byte) '\r';

			this.hw.logOutgoingCAT("-> " + command);

			this.SIO.put(bCmd,(uint) bCmd.Length);
		}

		#endregion Commands

	
		#region Event Handlers

		public override void SerialRXEventHandler(object source, SerialRXEvent e)
		{
			byte[] answer = new byte[60];

			// Resize the commBuffer as needed...
			int commBufferLen = 0;
			if (this.commBuffer != null)
			{
				commBufferLen = this.commBuffer.Length;
				Array.Resize<byte>(ref this.commBuffer,commBufferLen + e.buffer.Length);
			}
			else
				this.commBuffer = new byte[e.buffer.Length];

			// Append the incoming buffer into commBuffer
			Array.Copy(e.buffer,0,this.commBuffer,commBufferLen,e.buffer.Length);

			// Search for first $
			int iStart = 0;
			for (iStart = 0; iStart < this.commBuffer.Length; iStart++)
				if (this.commBuffer[iStart] == '$')
					break;

			if (iStart == this.commBuffer.Length) {
				this.commBuffer = null;
				return;
			}

			if (this.commBuffer.Length - iStart < 60)
				return;


			Array.Copy(this.commBuffer,iStart,answer,0,60);
			this.hw.logIncomingCAT("<- " + Meter.PrintBuffer(answer));
			this.handleMeterAnswer(answer);


			// Save the left over data for next read...
			if (this.commBuffer.Length - iStart > 60)
			{
				byte[] buf = new byte[this.commBuffer.Length - iStart];
				Array.Copy(this.commBuffer,0,buf,0,this.commBuffer.Length - iStart);
				this.commBuffer = buf;
			}
			else
				this.commBuffer = null;
		}

		#endregion Event Handlers
	}
}
