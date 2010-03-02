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

		private RigHW hw;
		private SerialRig rig;
		private RigCATParser rigParser;
		private CATParser sdrParser;

		private bool dspStatesInitialized = false;
		private int nr = 0;
		private int anf = 0;
		private bool nb = false;
		private bool nb2 = false;
		private int bin = 0;


		#endregion Variable Definitions


		#region Constructors

		public RigCATCommands(Console console, RigHW hw, SerialRig rig, RigCATParser parser) :
			base(console,parser)
		{
			this.hw = hw;
			this.rig = rig;
			this.rigParser = parser;
			this.sdrParser = parser;
		}

		#endregion Constructors


		public string BW(string s)
		{
			int width = int.Parse(s) * 10;

//			this.hw.logGeneral("BW: " + width + "Hz  RX1: " + this.rig.RX1FilterWidth + "Hz");

			// :NOTE: VAR2 Overrides Syncing
			if (width == this.rig.RX1FilterWidth || this.console.RX1Filter == Filter.VAR2)
				return null;

			// Set the cache value, first, so that setting the UI filter
			// doesn't trigger changing it on the rig.
			this.rig.RX1FilterWidth = width;

			// Find if there is a filter preset at this width.
			FilterPreset preset = this.console.rx1_filters[(int) this.console.RX1DSPMode];
			Filter filter = this.findRX1Filter(preset,width);

//			this.hw.logGeneral("BW Filter: " + filter);

			if (filter != Filter.NONE)
			{
				switch (filter)
				{
					case Filter.F1:
						this.console.radFilter1.Checked = true;
						return null;
					case Filter.F2:
						this.console.radFilter2.Checked = true;
						return null;
					case Filter.F3:
						this.console.radFilter3.Checked = true;
						return null;
					case Filter.F4:
						this.console.radFilter4.Checked = true;
						return null;
					case Filter.F5:
						this.console.radFilter5.Checked = true;
						return null;
					case Filter.F6:
						this.console.radFilter6.Checked = true;
						return null;
					case Filter.F7:
						this.console.radFilter7.Checked = true;
						return null;
					case Filter.F8:
						this.console.radFilter8.Checked = true;
						return null;
					case Filter.F9:
						this.console.radFilter9.Checked = true;
						return null;
					case Filter.F10:
						this.console.radFilter10.Checked = true;
						return null;
					case Filter.VAR1:
						this.console.radFilterVar1.Checked = true;
						return null;
					case Filter.VAR2:
						break;
				}
			}


			// Use VAR1 to set specific filter width...
			int low = 0, high = 0;
			switch (this.console.RX1DSPMode)
			{
				case DSPMode.LSB:
				case DSPMode.DIGL:
					low = -width - this.console.DefaultLowCut;
					high = -this.console.DefaultLowCut;
					break;

				case DSPMode.USB:
				case DSPMode.DIGU:
					low = this.console.DefaultLowCut;
					high = width + this.console.DefaultLowCut;
					break;

				case DSPMode.CWL:
					low = -this.console.CWPitch - width / 2;
					high = -this.console.CWPitch + width / 2;
					break;

				case DSPMode.CWU:
					low = this.console.CWPitch - width / 2;
					high = this.console.CWPitch + width / 2;
					break;

				case DSPMode.AM:
				case DSPMode.SAM:
				case DSPMode.FMN:
				case DSPMode.DSB:
					low = -width / 2;
					high = width / 2;
					break;
			}

//			this.hw.logGeneral("BW Low/High: " + low + "/" + high);

			this.console.radFilterVar1.Checked = true;
			this.console.UpdateRX1Filters(low,high);
			return null;
		}

		// Sets or reads the frequency of VFO A
		public string FA(string s)
		{
			if (this.rig is YaesuRig)
				s = s.PadLeft(11,'0');

			this.changeVFOA(s);
			return null;
		}

		// Sets or reads the frequency of VFO B
		public string FB(string s)
		{
			if (this.rig is YaesuRig)
				s = s.PadLeft(11, '0');

			this.changeVFOB(s);
			return null;
		}

		public string FI(string s)
		{
			// :NOTE: K3 Center Frequency is 8.210 MHz
			int fi = int.Parse(s);

			if (this.console.SWAPIQ && this.console.VFOAFreq >= this.console.SwapIQFreq)
				fi = -fi;

			int freq = 8210000 + fi;
			freq = this.hw.LOCenterFreq - freq;
			double ifFreq = ((double) freq) / 1000000 + this.console.globalIFOffset;
			double ifMode = 0;

			switch ((K3Rig.Mode) this.rig.VFOAMode)
			{
				case K3Rig.Mode.LSB:
					ifMode = this.console.IFLSB;
					break;
				case K3Rig.Mode.USB:
					ifMode = this.console.IFUSB;
					break;
				case K3Rig.Mode.CWL:
					ifMode = this.console.IFCWL + (double) this.console.CWPitch * 0.0000010;
					break;
				case K3Rig.Mode.CWU:
					ifMode = this.console.IFCWU - (double) this.console.CWPitch * 0.0000010;
					break;
				case K3Rig.Mode.FM:
					ifMode = this.console.IFFM;
					break;
				case K3Rig.Mode.AM:
					ifMode = this.console.IFAM;
					break;
				case K3Rig.Mode.DIGU:
					ifMode = this.console.IFFSKU;
					break;
				case K3Rig.Mode.DIGL:
					ifMode = this.console.IFFSKL;
					break;
			}

			ifFreq += ifMode;

			if (this.console.IFFreq != ifFreq)
			{
				this.hw.logGeneral("FI: " +
					this.hw.LOCenterFreq + " - " + "(8210000 + " + fi + ") + " +
					this.console.globalIFOffset + " + " + ifMode + " = " + ifFreq);
				this.console.IFFreq = ifFreq;
			}

			return null;
		}

		public string FT(string s)
		{
			bool split = (s[0] == '1');
			
			if (this.rig.Split != split)
			{
				this.rig.Split = split;
				this.sdrParser.Get("ZZSP" + s + ';');
			}

			return null;
		}

		// Reads the transceiver status
		// needs work in the split area
		public string IF(string s)
		{
			if (this.rig is YaesuRig)
			{
				bool rit = (s[16] == '1');
				int ritOffset = int.Parse(s.Substring(11, 5));
				char vfo = s[19];
				int mode = s[18] - '0';

				// Frequency
				// :NOTE: Store Frequency Status for RigSerialPoller Performance.
				string frequency = s.Substring(3, 8).PadLeft(11, '0');

				// :NOTE: When RIT is on, some rigs return Current VFO Frequency + RIT.
				//        We need to revert the RIT Offset from the Frequency.
				if (this.rig.ritAppliedInIFCATCommand() && rit)
					frequency = (int.Parse(frequency) - ritOffset).ToString().PadLeft(11, '0');

				if (vfo == '0')
				{
					this.rig.VFO = 0;
					this.changeVFOA(frequency);

					// Mode
					if (this.rig.VFOAMode != mode)
					{
						this.rig.VFOAMode = mode;
						this.rig.setConsoleModeFromString(s[18].ToString());
					}
				}

				// RIT
				if (this.console.RITOn != rit)
					this.console.RITOn = rit;

				bool changeRIT = false;
				if (this.rig.RITOffset != ritOffset)
					changeRIT = true;

				this.rig.RITOffset = ritOffset;

				if (changeRIT)
					this.sdrParser.Get("ZZRF" + s.Substring(11, 5 ).PadRight(5, '0') + ';');

				this.rig.RITOffsetInitialized = true;
			}
			else
			{
				bool rit = (s[21] == '1');
				int ritOffset = int.Parse(s.Substring(16, 5));
				char vfo = s[28];
				int mode = s[27] - '0';

				// Frequency
				// :NOTE: Store Frequency Status for RigSerialPoller Performance.
				// :TODO: Deal with vfo = 2 or 3
				string frequency = s.Substring(0, 11);

				// :NOTE: When RIT is on, some rigts return Current VFO Frequency + RIT.
				//        We need to revert the RIT Offset from the Frequency.
				if (this.rig.ritAppliedInIFCATCommand() && rit)
					frequency = (int.Parse(frequency) - ritOffset).ToString().PadLeft(11, '0');

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

				bool changeRIT = false;
				if (this.rig.RITOffset != ritOffset)
					changeRIT = true;

				// Control VFO-B when RIT and XIT are off
				if (this.rig.useRITForVFOB())
				{
					if (!rit && s[22] == '0')
					{
						int offsetDiff = ritOffset - this.rig.RITOffset;
						this.rig.RITOffset = ritOffset;

						if (offsetDiff != 0)
						{
							frequency = (int.Parse(this.rig.VFOBFrequency) +
								offsetDiff).ToString().PadLeft(11, '0');

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
				}
				else
					this.rig.RITOffset = ritOffset;

				if (changeRIT)
					this.sdrParser.Get("ZZRF" + s.Substring(16, 5) + ';');

				this.rig.RITOffsetInitialized = true;


				// RX/TX
				this.setTX(s[26] == '1');


				// Split
				bool split = (s[30] == '1');
				if (this.rig.Split != split)
				{
					this.rig.Split = split;
					this.sdrParser.Get("ZZSP" + s[30] + ';');
				}
			}

			return null;
		}

		public string MD(string s)
		{
			int mode = ((this.rig is YaesuRig) ? s[1] : s[0]) - '0';

			if (this.rig.VFOAMode != mode)
			{
				this.rig.VFOAMode = mode;
				this.rig.setConsoleModeFromString(s);
			}

			return null;
		}

		public string TQ(string s)
		{
			this.setTX(s[0] == '1');

			return null;
		}

		public string TX(string s)
		{
			this.setTX(s[0] != '0');

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
			this.rig.VFOAFrequencyChanged = (this.rig.VFOAFrequency != s);

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
			this.rig.VFOBFrequencyChanged = (this.rig.VFOBFrequency != s);

			this.rig.VFOBFrequency = s;
		}

		private void setTX(bool tx)
		{
			if (tx)
			{
				// If we're coming out of TX, store the state of all DSP Buttons...
				if (!this.console.MOX)
				{
					this.dspStatesInitialized = true;
					this.nr = this.console.CATNR;
					this.anf = this.console.CATANF;
					this.nb = this.console.NB;
					this.nb2 = this.console.NB2;
					this.bin = this.console.CATBIN;
				}

				// Clear all DSP Buttons when TX...
				if (this.dspStatesInitialized)
				{
					if (this.console.CATNR != 0)
						this.console.CATNR = 0;
					if (this.console.CATANF != 0)
						this.console.CATANF = 0;
					if (this.console.NB)
						this.console.NB = false;
					if (this.console.NB2)
						this.console.NB2 = false;
					if (this.console.CATBIN != 0)
						this.console.CATBIN = 0;
				}
			}
			else if (this.dspStatesInitialized && this.console.MOX)
			{
				// If we're coming out of TX, reset all DSP Buttons...
				if (this.console.CATNR != this.nr)
					this.console.CATNR = this.nr;
				if (this.console.CATANF != this.anf)
					this.console.CATANF = this.anf;
				if (this.console.NB != this.nb)
					this.console.NB = this.nb;
				if (this.console.NB2 != this.nb2)
					this.console.NB2 = this.nb2;
				if (this.console.CATBIN != this.bin)
					this.console.CATBIN = this.bin;
			}

			// This crazy logic sets MUTE on when TXing (when Monitor is turned off)
			if (!this.console.MON)
			{
				if (!this.console.MOX && tx)
				{
					if (this.console.MUT)
						this.rig.TXWithMute = true;
					else
						this.console.MUT = true;
				}
				else if (this.console.MOX && !tx)
				{
					if (!this.rig.TXWithMute)
						this.console.MUT = false;
				}
			}

			// If RX/TX Status is changing, set the console's MOX
			if (!this.console.MOX && tx || this.console.MOX && !tx)
				this.console.MOX = tx;
		}

		private Filter findRX1Filter(FilterPreset preset, int width)
		{
			for (Filter filter = Filter.F1; filter != Filter.LAST; filter++)
				if (Math.Abs(preset.GetHigh(filter) - preset.GetLow(filter)) == width)
					return filter;

			return Filter.NONE;
		}
	}
}

