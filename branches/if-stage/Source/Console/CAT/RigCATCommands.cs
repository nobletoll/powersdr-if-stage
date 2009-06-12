//=============================================================================
// RigCATCommands.cs
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
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;


namespace PowerSDR
{
	public class RigCATEventArgs : EventArgs
	{
		public RigCATEventArgs() { }
	}

	public class RigCATCommands : CATCommands
	{
		#region Variable Definitions

		private RigSerialPoller rigSerialPoller;
		private RigCATParser rigParser;
		private CATParser sdrParser;

		private bool transmitting = false;
		private bool transmittingWithMute = false;

		#endregion Variable Definitions


		#region Constructors

		public RigCATCommands()
		{
		}

		public RigCATCommands(Console console, RigSerialPoller rigSerialPoller,
			RigCATParser parser) : base(console,parser)
		{
			this.rigSerialPoller = rigSerialPoller;
			this.rigParser = parser;
			this.sdrParser = parser;
		}

		#endregion Constructors


		// Sets or reads the frequency of VFO A
		public string FA(string s)
		{
			this.changeVFOA(s);
			return null;
		}

		// Sets or reads the frequency of VFO B
		public string FB(string s)
		{
			this.changeVFOB(s);
			return null;
		}

		// Reads the transceiver status
		// needs work in the split area
		public string IF(string s)
		{
			char vfo = s[28];

			// Set TX VFO
			// :TODO: Decide on how to handle VFO Switching
//			this.sdrParser.Get("FT" + vfo + ';');

			// Frequency
			// :NOTE: Store Frequency Status for RigSerialPoller Performance.
			// :TODO: Deal with vfo = 2 or 3
			string frequency = s.Substring(0,11);
			if (vfo == '0')
			{
				this.rigParser.VFO = 0;
				this.changeVFOA(frequency);
			}
			else if (vfo == '1')
			{
				this.rigParser.VFO = 1;
				this.changeVFOB(frequency);
			}

			// RIT Frequency - Control VFO-B
			int rit = int.Parse(s[16] + s.Substring(17,4));
			int offset = rit - this.rigParser.RITOffset;
			this.rigParser.RITOffset = rit;
			
			if (offset != 0) {
//				string vfob = this.console.VFOBFreq.ToString("f6").Replace(separator,"").PadLeft(11,'0');
//				double newFreq = ((double) (int.Parse(this.rigParser.VFOBFrequency) + offset)) / 1000000;
				string newFreq =
					(int.Parse(this.rigParser.VFOBFrequency) + offset).ToString().PadLeft(11,'0');

//				this.rigSerialPoller.updateVFOBFrequency(newFreq);
				this.rigSerialPoller.doRigCATCommand("FB" + newFreq + ';',false,false);
				this.changeVFOB(newFreq);
			}

			
			// :TODO: RIT
			// :TODO: XIT

			// :TODO: Memory Bank?

			// Mode
			int mode = s[27] - '0';
			if (this.rigParser.Mode != mode)
			{
				this.rigParser.Mode = mode;
				this.sdrParser.Get("MD" + s[27] + ';');
			}

			// Split
			// :TODO: Decide on how to deal with this since SPLIT is only on
			//        when TX is set to VFO-B in PowerSDR.
			this.sdrParser.Get("ZZSP" + s[30] + ';');

			// RX/TX
			// This crazy logic sets MUTE on when TXing (when Monitor is turned off)
			if (!this.console.MON)
			{
				if (!this.transmitting && s[26] == '1')
				{
					this.transmitting = true;

					if (this.console.MUT)
						this.transmittingWithMute = true;
					else
						this.console.MUT = true;
				}
				else if (this.transmitting && s[26] == '0')
				{
					this.transmitting = false;

					if (!this.transmittingWithMute)
						this.console.MUT = false;
				}
			}

			return null;
		}


		private void changeVFOA(string s)
		{
			if (this.console.SetupForm.RttyOffsetEnabledA &&
				(this.console.RX1DSPMode == DSPMode.DIGU ||
				this.console.RX1DSPMode == DSPMode.DIGL))
			{
				int f = int.Parse(s);
		
				if (this.console.RX1DSPMode == DSPMode.DIGU)
					f = f - Convert.ToInt32(console.SetupForm.RttyOffsetHigh);
				else if (console.RX1DSPMode == DSPMode.DIGL)
					f = f + Convert.ToInt32(console.SetupForm.RttyOffsetLow);
				
				s = this.AddLeadingZeros(f);
			}

			if (this.rigParser.VFO != 0 || s != this.rigParser.VFOAFrequency)
			{
				double freq = double.Parse(s.Insert(5,separator));
				this.console.txtVFOAFreq.Text = freq.ToString("f6");
				this.console.txtVFOAFreq_LostFocus(this,new RigCATEventArgs());
			}

			// Store Frequency Status for RigSerialPoller Performance.
			if (this.rigParser.VFO == 0)
				this.rigParser.FrequencyChanged = (this.rigParser.VFOAFrequency != s);

			this.rigParser.VFOAFrequency = s;
		}

		private void changeVFOB(string s)
		{
			if (this.console.SetupForm.RttyOffsetEnabledB &&
				(this.console.RX1DSPMode == DSPMode.DIGU ||
				this.console.RX1DSPMode == DSPMode.DIGL))
			{
				int f = int.Parse(s);

				if (this.console.RX1DSPMode == DSPMode.DIGU)
					f = f - Convert.ToInt32(console.SetupForm.RttyOffsetHigh);
				else if (console.RX1DSPMode == DSPMode.DIGL)
					f = f + Convert.ToInt32(console.SetupForm.RttyOffsetLow);

				s = this.AddLeadingZeros(f);
			}

			if (this.rigParser.VFO != 1 || s != this.rigParser.VFOBFrequency)
			{
				double freq = double.Parse(s.Insert(5,separator));
				this.console.txtVFOBFreq.Text = freq.ToString("f6");
				this.console.txtVFOBFreq_LostFocus(this,new RigCATEventArgs());
			}

			// Store Frequency Status for RigSerialPoller Performance.
			if (this.rigParser.VFO == 1)
				this.rigParser.FrequencyChanged = (this.rigParser.VFOBFrequency != s);

			this.rigParser.VFOBFrequency = s;
		}
	}
}
