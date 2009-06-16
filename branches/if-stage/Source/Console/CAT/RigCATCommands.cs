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

#define RIT_FOR_VFOB

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
			bool rit = (s[21] == '1');
			char vfo = s[28];
			int mode = s[27] - '0';

			// Frequency
			// :NOTE: Store Frequency Status for RigSerialPoller Performance.
			// :TODO: Deal with vfo = 2 or 3
			string frequency = s.Substring(0,11);

			// :NOTE: When RIT is on, IF returns Current VFO Frequency + RIT.
			//        We need to revert the RIT Offset from the Frequency.
			if (rit)
			{
				int ritOffset = int.Parse(s[16] + s.Substring(17,4));
				frequency = (int.Parse(frequency) - ritOffset).ToString().PadLeft(11,'0');
			}

			if (vfo == '0')
			{
				this.rigParser.VFO = 0;
				this.changeVFOA(frequency);

				// Mode
				if (this.rigParser.VFOAMode != mode)
				{
					this.rigParser.VFOAMode = mode;
					this.sdrParser.Get("MD" + s[27] + ';');
				}
			}
			else if (vfo == '1')
			{
				// Force the Rig on VFO-A to conform to the way PowerSDR handles RX1
				this.rigSerialPoller.doRigCATCommand("FN0;",false,false);
				this.rigParser.VFO = 1;
				this.changeVFOB(frequency);
				this.rigParser.VFO = 0;

				// Mode
				this.rigParser.VFOBMode = mode;
			}

			// RIT Frequency - Control VFO-B when RIT and XIT are off
#if RIT_FOR_VFOB
			if (!rit && s[22] == '0')
			{
				int ritOffset = int.Parse(s[16] + s.Substring(17,4));
				int offsetDiff = ritOffset - this.rigParser.RITOffset;
				this.rigParser.RITOffset = ritOffset;

				if (offsetDiff != 0)
				{
					frequency = (int.Parse(this.rigParser.VFOBFrequency) +
						offsetDiff).ToString().PadLeft(11,'0');

					this.rigSerialPoller.doRigCATCommand("FB" + frequency + ';',
						false,false);
					this.changeVFOB(frequency);
				}

				// Reset RIT when it gets close to 9.990 max.
				if (Math.Abs(ritOffset) > 8000)
				{
					this.rigSerialPoller.doRigCATCommand("RC;",false,false);
					this.rigParser.RITOffset = 0;
				}
			}
#endif

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

			// Split
			bool split = (s[30] == '1');
			if (this.rigParser.Split != split)
			{
				this.rigParser.Split = split;
				this.sdrParser.Get("ZZSP" + s[30] + ';');
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

