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

		private RigHW hw;
		private SerialRig rig;
		private RigCATParser rigParser;
		private CATParser sdrParser;

		private bool transmittingWithMute = false;

		#endregion Variable Definitions


		#region Constructors

		public RigCATCommands(Console console, RigHW hw, RigCATParser parser) :
			base(console,parser)
		{
			this.hw = hw;
			this.rig = (SerialRig) hw.Rig;
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

		public string FI(string s)
		{
			// :NOTE: K3 Center Frequency is 8.210 MHz
			int freq = 8210000 + int.Parse(s);
			freq = this.hw.LOCenterFreq - freq;
			double ifFreq = ((double) freq) / 1000000 + this.console.globalIFOffset;

			switch ((K3Rig.Mode) this.rig.VFOAMode)
			{
				case K3Rig.Mode.LSB:
					ifFreq += this.console.IFLSB;
					break;
				case K3Rig.Mode.USB:
					ifFreq += this.console.IFUSB;
					break;
				case K3Rig.Mode.CWL:
					ifFreq += this.console.IFCW;
					break;
				case K3Rig.Mode.CWU:
					ifFreq += this.console.IFCW;
					break;
				case K3Rig.Mode.FM:
					ifFreq += this.console.IFFM;
					break;
				case K3Rig.Mode.AM:
					ifFreq += this.console.IFAM;
					break;
				case K3Rig.Mode.DIGU:
					ifFreq += this.console.IFFSK;
					break;
				case K3Rig.Mode.DIGL:
					ifFreq += this.console.IFFSK;
					break;
			}

			if (this.console.IFFreq != ifFreq)
			{
				RigHW.dbgWriteLine("FI: " + ifFreq);
				this.console.IFFreq = ifFreq;
			}

			return null;
		}

		// Reads the transceiver status
		// needs work in the split area
		public string IF(string s)
		{
			bool rit = (s[21] == '1');
			int ritOffset = int.Parse(s.Substring(16,5));
			char vfo = s[28];
			int mode = s[27] - '0';

			// Frequency
			// :NOTE: Store Frequency Status for RigSerialPoller Performance.
			// :TODO: Deal with vfo = 2 or 3
			string frequency = s.Substring(0,11);

			// :NOTE: When RIT is on, some rigts return Current VFO Frequency + RIT.
			//        We need to revert the RIT Offset from the Frequency.
			if (this.rig.ritAppliedInIFCATCommand() && rit)
				frequency = (int.Parse(frequency) - ritOffset).ToString().PadLeft(11,'0');

			if (vfo == '0')
			{
				this.rig.VFO = 0;
				this.changeVFOA(frequency);

				// Mode
				if (this.rig.VFOAMode != mode)
				{
					this.rig.VFOAMode = mode;
					this.rig.setConsoleModeFromString(s[27].ToString());
				}
			}
			else if (vfo == '1')
			{
				// Force the Rig on VFO-A to conform to the way PowerSDR handles RX1
				this.rig.setVFOA();
				this.rig.VFO = 1;
				this.changeVFOB(frequency);
				this.rig.VFO = 0;

				// Mode
				this.rig.VFOBMode = mode;
			}


			// RIT
			if (this.console.RITOn != rit)
				this.console.RITOn = rit;

			if (this.rig.RITOffset != ritOffset)
				this.sdrParser.Get("ZZRF" + s.Substring(16,5) + ';');

			// Control VFO-B when RIT and XIT are off
#if RIT_FOR_VFOB
			if (!rit && s[22] == '0')
			{
				int offsetDiff = ritOffset - this.rig.RITOffset;
				this.rig.RITOffset = ritOffset;

				if (offsetDiff != 0)
				{
					frequency = (int.Parse(this.rig.VFOBFrequency) +
						offsetDiff).ToString().PadLeft(11,'0');

					this.rig.setVFOBFreq(frequency);
					this.changeVFOB(frequency);
				}

				// Reset RIT when it gets close to 9.990 max.
				if (Math.Abs(ritOffset) > 8000)
				{
					this.rig.clearRIT();
					this.rig.RITOffset = 0;
				}
			}
			else
				this.rig.RITOffset = ritOffset;
#else
			this.rig.RITOffset = ritOffset;
#endif

			this.rig.RITOffsetInitialized = true;


			// RX/TX
			bool tx = (s[26] == '1');

			// This crazy logic sets MUTE on when TXing (when Monitor is turned off)
			if (!this.console.MON)
			{
				if (!this.console.MOX && tx)
				{
					if (this.console.MUT)
						this.transmittingWithMute = true;
					else
						this.console.MUT = true;
				}
				else if (this.console.MOX && !tx)
				{
					if (!this.transmittingWithMute)
						this.console.MUT = false;
				}
			}

			// If RX/TX Status is changing, set the console's MOX
			if (!this.console.MOX && tx || this.console.MOX && !tx)
				this.console.MOX = tx;


			// Split
			bool split = (s[30] == '1');
			if (this.rig.Split != split)
			{
				this.rig.Split = split;
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

			if (this.rig.VFO != 0 || s != this.rig.VFOAFrequency)
			{
				double freq = double.Parse(s.Insert(5,separator));
				this.console.txtVFOAFreq.Text = freq.ToString("f6");
				this.console.txtVFOAFreq_LostFocus(this,new RigCATEventArgs());
			}

			// Store Frequency Status for RigSerialPoller Performance.
			if (this.rig.VFO == 0)
				this.rig.FrequencyChanged = (this.rig.VFOAFrequency != s);

			this.rig.VFOAFrequency = s;
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

			if (this.rig.VFO != 1 || s != this.rig.VFOBFrequency)
			{
				double freq = double.Parse(s.Insert(5,separator));
				this.console.txtVFOBFreq.Text = freq.ToString("f6");
				this.console.txtVFOBFreq_LostFocus(this,new RigCATEventArgs());
			}

			// Store Frequency Status for RigSerialPoller Performance.
			if (this.rig.VFO == 1)
				this.rig.FrequencyChanged = (this.rig.VFOBFrequency != s);

			this.rig.VFOBFrequency = s;
		}
	}
}

