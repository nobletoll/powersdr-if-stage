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

//		private Console console;
		private RigCATParser rigParser;
		private CATParser sdrParser;

		private bool transmitting = false;
		private bool transmittingWithMute = false;

		#endregion Variable Definitions


		#region Constructors

		public RigCATCommands()
		{
		}

		public RigCATCommands(Console console, RigCATParser parser)
		{
			this.console = console;
			this.rigParser = parser;
			this.sdrParser = parser;
		}

		#endregion Constructors


		// Commands getting this far have been checked for a valid prefix, a correct suffix length,
		// and a terminator.

		#region Standard CAT Methods A-F

		// Sets or reads the Audio Gain control
		public string AG(string s)
		{
			return null;
		}

		public string AI(string s)
		{
			return null;
		}

		// Moves one band down from the currently selected band
		public string BD()
		{
			return null;
		}

		// Moves one band up from the currently selected band
		// write only
		public string BU()
		{
			return null;
		}

		//Moves the VFO A frequency by the step size set on the console
		public string DN()
		{
			return null;
		}

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

		// Sets VFO A to control rx
		// this is a dummy command to keep other software happy
		// since the SDR-1000 always uses VFO A for rx
		public string FR(string s)
		{
			return null;
		}

		// Sets or reads VFO B to control tx
		// another "happiness" command
		public string FT(string s)
		{
			return null;
		}

		// Sets or reads the DSP filter width
		//OBSOLETE
		public string FW(string s)
		{
			return null;
		}

		#endregion Standard CAT Methods A-F

		#region Standard CAT Methods G-M

		// Sets or reads the AGC constant
		// this is a wrapper that calls ZZGT
		public string GT(string s)
		{
			return null;
		}

		// Reads the transceiver ID number
		// this needs changing when 3rd party folks on line.
		public string ID()
		{
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

			// :TODO: RIT Frequency
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

		//Sets or reads the CWX CW speed
		public string KS(string s)
		{
			return null;
		}

		//Sends text data to CWX for conversion to Morse
		public string KY(string s)
		{
			return null;
		}


		// Sets or reads the transceiver mode
		public string MD(string s)
		{
			return null;
		}

		// Sets or reads the Mic Gain thumbwheel
		public string MG(string s)
		{
			return null;
		}

		// Sets or reads the Monitor status
		public string MO(string s)
		{
			return null;
		}

		#endregion Standard CAT Methods G-M

		#region Standard CAT Methods N-Q

		// Sets or reads the Noise Blanker 1 status
		public string NB(string s)
		{
			return null;
		}

		// Sets or reads the Automatic Notch Filter status
		public string NT(string s)
		{
			return null;
		}

		// Sets or reads the PA output thumbwheel
		public string PC(string s)
		{
			return null;
		}

		// Sets or reads the console power on/off status
		public string PS(string s)
		{
			return null;
		}

		// Sets the Quick Memory with the current contents of VFO A
		public string QI()
		{
			return null;
		}

		#endregion Standard CAT Methods N-Q

		#region Standard CAT Methods R-Z

		// Clears the RIT value
		// write only
		public string RC()
		{
			return null;
		}

		//Decrements RIT
		public string RD(string s)
		{
			return null;
		}

		// Sets or reads the RIT status (on/off)
		public string RT(string s)
		{
			return null;
		}

		//Increments RIT
		public string RU(string s)
		{
			return null;
		}

		// Sets or reads the transceiver receive mode status
		// write only but spec shows an answer parameter for a read???
		public string RX(string s)
		{
			return null;
		}

		// Sets or reads the variable DSP filter high side
		public string SH(string s)
		{
			return null;
		}

		// Sets or reads the variable DSP filter low side
		public string SL(string s)
		{
			return null;
		}

		// Reads the S Meter value
		public string SM(string s)
		{
			return null;
		}

		// Sets or reads the Squelch value
		public string SQ(string s)
		{
			return null;
		}

		// Sets the transmitter on, write only
		// will eventually need eiter Commander change or ZZ code
		// since it is not CAT compliant as it is
		public string TX(string s)
		{
			return null;
		}

		//Moves the VFO A frequency up by the step size set on the console
		public string UP()
		{
			return null;
		}


		// Sets or reads the transceiver XIT status (on/off)
		public string XT(string s)
		{
			return null;
		}

		#endregion Standard CAT Methods R-Z

		#region Extended CAT Methods ZZA-ZZF

		// Sets or reads the SDR-1000 Audio Gain control
		public string ZZAG(string s)
		{
			return null;
		}

		public string ZZAI(string s)
		{
			return null;
		}

		//Sets or reads the AGC RF gain
		public string ZZAR(string s)
		{
			return null;
		}

		//Moves the bandswitch down one band
		public string ZZBD()
		{
			return null;
		}

		// Sets the Band Group (HF/VHF)
		public string ZZBG(string s)
		{
			return null;
		}

		// Sets or reads the BIN button status
		public string ZZBI(string s)
		{
			return null;
		}

		//Sets or reads the BCI Rejection button status
		public string ZZBR(string s)
		{
			return null;
		}


		//Sets or reads the current band setting
		public string ZZBS(string s)
		{
			return null;
		}

		//Moves the bandswitch up one band
		public string ZZBU()
		{
			return null;
		}

		// Sets or reads the CW Break In Enabled checkbox
		public string ZZCB(string s)
		{
			return null;
		}


		// Sets or reads the CW Break In Delay
		public string ZZCD(string s)
		{
			return null;
		}

		// Sets or reads the Show CW Frequency checkbox
		public string ZZCF(string s)
		{
			return null;
		}

		// Sets or reads the CW Iambic checkbox
		public string ZZCI(string s)
		{
			return null;
		}

		// Sets or reads the CW Pitch thumbwheel
		public string ZZCL(string s)
		{
			return null;
		}

		// Sets or reads the CW Monitor Disable button status
		public string ZZCM(string s)
		{
			return null;
		}

		// Sets or reads the compander button status
		public string ZZCP(string s)
		{
			return null;
		}

		// Sets or reads the CW Speed thumbwheel
		public string ZZCS(string s)
		{
			return null;
		}

		//Reads or sets the compander threshold
		public string ZZCT(string s)
		{
			return null;
		}

		// Reads the CPU Usage
		public string ZZCU()
		{
			return null;
		}

		// Sets or reads the Display Average status
		public string ZZDA(string s)
		{
			return null;
		}

		// Sets or reads the current display mode
		public string ZZDM(string s)
		{
			return null;
		}

		/// <summary>
		/// Sets or reads the DX button status
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public string ZZDX(string s)
		{
			return null;
		}

		/// <summary>
		/// Reads or sets the RX equalizer.
		/// The CAT suffix string is 36 characters constant.
		/// Each value in the string occupies exactly three characters
		/// starting with the number of bands (003 or 010) followed by
		/// the preamp setting (-12 to 015) followed by 3 or 10 three digit
		/// EQ thumbwheel positions.  If the number of bands is 3, the
		/// last seven positions (21 characters) are all set to zero.
		/// Example:  10 band ZZEA010-09009005000-04-07-09-05000005009;
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public string ZZEA(string s)
		{
			return null;
		}

		//Sets or reads the TX EQ settings
		public string ZZEB(string s)
		{
			return null;
		}

		//Sets or reads the RXEQ button statusl
		public string ZZER(string s)
		{
			return null;
		}

		//Sets or reads the TXEQ button status
		public string ZZET(string s)
		{
			return null;
		}


		//Sets or reads VFO A frequency
		public string ZZFA(string s)
		{
			return null;
		}

		//Sets or reads VFO B frequency
		public string ZZFB(string s)
		{
			return null;
		}


		//Sets or reads the current filter index number
		public string ZZFI(string s)
		{
			return null;
		}


		/// <summary>
		/// Reads or sets the DSP Filter Low value
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public string ZZFL(string s)
		{
			return null;
		}

		/// <summary>
		/// Reads or sets the DSP Filter High value
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public string ZZFH(string s)
		{
			return null;
		}

		public string ZZFM()
		{
			return null;
		}

		//Reads FlexWire single byte value commands
		public string ZZFV(string s)
		{
			return null;
		}

		//Reds FlexWire double byte value commands
		public string ZZFW(String s)
		{
			return null;
		}

		//Sends FlexWire single byte value commands
		public string ZZFX(string s)
		{
			return null;
		}

		//Sends FlexWire double byte value commands
		public string ZZFY(String s)
		{
			return null;
		}

		#endregion Extended CAT Methods ZZA-ZZF

		#region Extended CAT Methods ZZG-ZZM


		// Sets or reads the noise gate enable button status
		public string ZZGE(string s)
		{
			return null;
		}

		//Sets or reads the noise gate level control
		public string ZZGL(string s)
		{
			return null;
		}

		// Sets or reads the AGC constant
		public string ZZGT(string s)
		{
			return null;
		}

		// Sets or reads the Audio Buffer Size
		public string ZZHA(string s)
		{
			return null;
		}

		//Sets or reads the DSP Phone RX Buffer Size
		public string ZZHR(string s)
		{
			return null;
		}

		//Sets or reads the DSP Phone TX Buffer Size
		public string ZZHT(string s)
		{
			return null;
		}

		//Sets or reads the DSP CW RX Buffer Size
		public string ZZHU(string s)
		{
			return null;
		}

		//Sets or reads the DSP CW TX Buffer Size
		public string ZZHV(string s)
		{
			return null;
		}

		//Sets or reads the DSP Digital RX Buffer Size
		public string ZZHW(string s)
		{
			return null;
		}

		//Sets or reads the DSP Digital TX Buffer Size
		public string ZZHX(string s)
		{
			return null;
		}

		// Sets the CAT Rig Type to SDR-1000
		//Modified 10/12/08 BT changed "SDR-1000" to "PowerSDR"
		public string ZZID()
		{
			return null;
		}

		// Reads the SDR-1000 transceiver status
		public string ZZIF(string s)
		{
			return null;
		}

		// Sets or reads the IF width
		public string ZZIS(string s)
		{
			return null;
		}

		//Sets or reads the IF Shift
		public string ZZIT(string s)
		{
			return null;
		}

		// Resets the Filter Shift to zero.  Write only
		public string ZZIU()
		{
			return null;
		}

		//Sets or reads the CWX CW speed
		public string ZZKS(string s)
		{
			return null;
		}

		//Sends text to CWX for conversion to Morse
		public string ZZKY(string s)
		{
			return null;
		}

		// Sets or reads the MUT button on/off status
		public string ZZMA(string s)
		{
			return null;
		}

		// Sets or reads the SDR-1000 DSP mode
		public string ZZMD(string s)
		{
			return null;
		}

		//Sets or reads the Mic gain control
		public string ZZMG(string s)
		{
			return null;
		}

		//Reads the DSP filter presets for filter index (s)
		//Returns 180 character length word for 12 filters x 15 characters each.
		//Format is name high low: ZZMN 5.0k 5150 -160...
		public string ZZMN(string s)
		{
			return null;
		}

		//Sets or reads the Monitor (MON) button status
		public string ZZMO(string s)
		{
			return null;
		}

		// Sets or reads the RX meter mode
		public string ZZMR(string s)
		{
			return null;
		}

		//Sets or reads the MultiRX Swap checkbox
		public string ZZMS(string s)
		{
			return null;
		}

		// Sets or reads the TX meter mode
		public string ZZMT(string s)
		{
			return null;
		}

		//Sets or reads the MultiRX button status
		public string ZZMU(string s)
		{
			return null;
		}


		#endregion Extended CAT Methods ZZG-ZZM

		#region Extended CAT Methods ZZN-ZZQ

		//Sets or reads Noise Blanker 2 status
		public string ZZNA(string s)
		{
			return null;
		}

		// Sets or reads the Noise Blanker 2 status
		public string ZZNB(string s)
		{
			return null;
		}

		// Sets or reads the Noise Blanker 1 threshold
		public string ZZNL(string s)
		{
			return null;
		}

		// Sets or reads the Noise Blanker 2 threshold
		public string ZZNM(string s)
		{
			return null;
		}


		// Sets or reads the Noise Reduction status
		//		public string ZZNR()
		//		{
		//			int nr = console.CATNR;
		//			return nr.ToString();
		//		}

		public string ZZNR(string s)
		{
			return null;
		}

		//Sets or reads the ANF button status
		public string ZZNT(string s)
		{
			return null;
		}

		//Sets or reads the RX1 antenna
		public string ZZOA(string s)
		{
			return null;
		}

		//Sets or reads the RX2 antenna (if RX2 installed)
		public string ZZOB(string s)
		{
			return null;
		}

		//Sets or reads the TX antenna
		public string ZZOC(string s)
		{
			return null;
		}

		//Sets or reads the current Antenna Mode
		public string ZZOD(string s)
		{
			return null;
		}

		//Sets or reads the RX1 External Antenna checkbox
		public string ZZOE(string s)
		{
			return null;
		}

		//Sets or reads the TX relay RCA jack
		public string ZZOF(string s)
		{
			return null;
		}


		//Sets or reads the TX Relay Delay enables
		public string ZZOG(string s)
		{
			return null;
		}

		//Sets or reads the TX Relay Delays
		public string ZZOH(string s)
		{
			return null;
		}

		public string ZZOJ(string s)
		{
			return null;
		}

		// Sets or reads the Preamp thumbwheel
		public string ZZPA(string s)
		{
			return null;
		}

		//Sets or reads the Drive level
		public string ZZPC(string s)
		{
			return null;
		}

		//Centers the Display Pan scroll
		public string ZZPD()
		{
			return null;
		}

		//		//Sets or reads the Speech Compressor button status
		//		public string ZZPK(string s)
		//		{
		//			if(s.Length == parser.nSet)
		//			{
		//				if(s == "0")
		//					console.COMP = false;
		//				else if(s == "1")
		//					console.COMP = true;
		//				return "";
		//			}
		//			else if(s.Length == parser.nGet)
		//			{
		//				bool comp = console.COMP;
		//				if(comp)
		//					return "1";
		//				else
		//					return "0";
		//			}
		//			else
		//			{
		//				return "";
		//			}
		//		}
		//
		//		// Sets or reads the Speech Compressor threshold
		//		public string ZZPL(string s)
		//		{
		//			if(s.Length == parser.nSet)
		//			{
		//				console.SetupForm.CATCompThreshold = Convert.ToInt32(s);
		//				return "";
		//			}
		//			else if(s.Length == parser.nGet)
		//			{
		//				return AddLeadingZeros(console.SetupForm.CATCompThreshold);
		//			}
		//			else
		//			{
		//				return parser.Error1;
		//			}
		//
		//		}

		// Sets or reads the Speech Compressor threshold
		//		public string ZZPL(string s)
		//		{
		//			if(s.Length == parser.nSet)
		//			{
		//				console.SetupForm.CATCompThreshold = Convert.ToInt32(s);
		//				return "";
		//			}
		//			else if(s.Length == parser.nGet)
		//			{
		//				return AddLeadingZeros(console.SetupForm.CATCompThreshold);
		//			}
		//			else
		//			{
		//				return parser.Error1;
		//			}
		//
		//		}

		//Sets or reads the Display Peak button status
		public string ZZPO(string s)
		{
			return null;
		}

		//Sets or reads the Power button status
		public string ZZPS(string s)
		{
			return null;
		}

		//Sets the Display Zoom buttons
		public string ZZPZ(string s)
		{
			return null;
		}

		// Reads the Quick Memory Save value
		public string ZZQM()
		{
			return null;
		}

		// Recalls Memory Quick Save
		public string ZZQR()
		{
			return null;
		}

		//Saves Quick Memory value
		public string ZZQS()
		{
			return null;
		}


		#endregion Extended CAT Methods ZZN-ZZQ

		#region Extended CAT Methods ZZR-ZZZ

		// Sets or reads the RTTY Offset Enable VFO A checkbox
		public string ZZRA(string s)
		{
			return null;
		}

		// Sets or reads the RTTY Offset Enable VFO B checkbox
		public string ZZRB(string s)
		{
			return null;
		}

		//Clears the RIT frequency
		public string ZZRC()
		{
			return null;
		}

		//Decrements RIT
		public string ZZRD(string s)
		{
			return null;
		}

		// Sets or reads the RIT frequency value
		public string ZZRF(string s)
		{
			return null;
		}


		//Sets or reads the RTTY DIGH offset frequency ud counter
		public string ZZRH(string s)
		{
			return null;
		}

		//Sets or reads the RTTY DIGL offset frequency ud counter
		public string ZZRL(string s)
		{
			return null;
		}

		// Reads the Console RX meter
		public string ZZRM(string s)
		{
			return null;
		}

		//Sets or reads the RX2 button status
		public string ZZRS(string s)
		{
			return null;
		}


		//Sets or reads the RIT button status
		public string ZZRT(string s)
		{
			return null;
		}

		//Increments RIT
		public string ZZRU(string s)
		{
			return null;
		}

		//Moves VFO A down one Tune Step
		public string ZZSA()
		{
			return null;
		}

		//Moves VFO A up one Tune Step
		public string ZZSB()
		{
			return null;
		}

		//Moves the mouse wheel tuning step down
		public string ZZSD()
		{
			return null;
		}

		// ZZSFccccwwww  Set Filter, cccc=center freq www=width both in hz 
		public string ZZSF(string s)
		{
			return null;
		}

		// Reads the S Meter value
		public string ZZSM(string s)
		{
			return null;
		}

		// Sets or reads the VFO Split status
		public string ZZSP(string s)
		{
			return null;
		}

		// Sets or reads the Squelch on/off status
		public string ZZSO(string s)
		{
			return null;
		}

		// Sets or reads the SDR-1000 Squelch contorl
		public string ZZSQ(string s)
		{
			return null;
		}

		//Reads or sets the Spur Reduction button status
		public string ZZSR(string s)
		{
			return null;
		}

		// Reads the current console step size (read-only property)
		public string ZZST()
		{
			return null;
		}

		// Moves the mouse wheel step tune up
		public string ZZSU()
		{
			return null;
		}

		// Sets or reads the Show TX Filter checkbox
		public string ZZTF(string s)
		{
			return null;
		}


		// Sets or reads the TX filter high setting
		public string ZZTH(string s)
		{
			return null;
		}

		//Inhibits power output when using external antennas, tuners, etc.
		public string ZZTI(string s)
		{
			return null;
		}

		// Sets or reads the TX filter low setting
		public string ZZTL(string s)
		{
			return null;
		}

		//Sets or reads the Tune Power level
		public string ZZTO(string s)
		{
			return null;
		}


		//Sets or reads the TX Profile
		public string ZZTP(string s)
		{
			return null;
		}

		// Reads the Flex 5000 temperature sensor
		public string ZZTS()
		{
			return null;
		}

		// Sets or reads the TUN button on/off status
		public string ZZTU(string s)
		{
			return null;
		}

		//Sets or reads the MOX button status
		public string ZZTX(string s)
		{
			return null;
		}

		//Reads the XVTR Band Names
		public string ZZUA()
		{
			return null;
		}

		// Reads or sets the VAC Enable checkbox (Setup Form)
		public string ZZVA(string s)
		{
			return null;
		}


		/// <summary>
		/// Sets or reads the VAC RX Gain 
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public string ZZVB(string s)
		{
			return null;
		}

		/// <summary>
		/// Sets or reads the VAC TX Gain
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public string ZZVC(string s)
		{
			return null;
		}

		/// <summary>
		/// Sets or reads the VAC Sample Rate
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public string ZZVD(string s)
		{
			return null;
		}

		//Reads or sets the VOX Enable button status
		public string ZZVE(string s)
		{
			return null;
		}


		/// <summary>
		/// Sets or reads the VAC Stereo checkbox
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public string ZZVF(string s)
		{
			return null;
		}

		//Reads or set the VOX Gain control
		public string ZZVG(string s)
		{
			return null;
		}

		// Reads or sets the I/Q to VAC checkbox on the setup form
		public string ZZVH(string s)
		{
			return null;
		}

		// Reads or sets the VFO Lock button status
		public string ZZVL(string s)
		{
			return null;
		}

		// Returns the version number of the PowerSDR program
		public string ZZVN()
		{
			return null;
		}

		// Sets the VFO swap status
		// write only
		public string ZZVS(string s)
		{
			return null;
		}

		//Sets or reads the F5K Mixer Mic Gain
		public string ZZWA(string s)
		{
			return null;
		}

		//Sets or reads the F5K Line In RCA level
		public string ZZWB(string s)
		{
			return null;
		}

		//Sets or reads the F5K Line In Phono level
		public string ZZWC(string s)
		{
			return null;
		}

		//Sets or reads the F5K Mixer Line In DB9 level
		public string ZZWD(string s)
		{
			return null;
		}


		// Sets or reads the F5K Mixer Mic Selected Checkbox
		public string ZZWE(string s)
		{
			return null;
		}

		// Sets or reads the F5K Mixer Line In RCA Checkbox
		public string ZZWF(string s)
		{
			return null;
		}

		// Sets or reads the F5K Mixer Line In Phono Checkbox
		public string ZZWG(string s)
		{
			return null;
		}

		// Sets or reads the F5K Mixer Line In DB9 Checkbox
		public string ZZWH(string s)
		{
			return null;
		}


		// Sets or reads the F5K Mixer Mute All Checkbox
		public string ZZWJ(string s)
		{
			return null;
		}

		//Sets or reads the F5K Mixer Internal Speaker level
		public string ZZWK(string s)
		{
			return null;
		}

		//Sets or reads the F5K Mixer External Speaker level
		public string ZZWL(string s)
		{
			return null;
		}

		//Sets or reads the F5K Mixer Headphone level
		public string ZZWM(string s)
		{
			return null;
		}

		//Sets or reads the F5K Mixer Line Out RCA level
		public string ZZWN(string s)
		{
			return null;
		}

		// Sets or reads the F5K Mixer Internal Speaker Selected Checkbox
		public string ZZWO(string s)
		{
			return null;
		}

		// Sets or reads the F5K Mixer External Speaker Selected Checkbox
		public string ZZWP(string s)
		{
			return null;
		}

		// Sets or reads the F5K Mixer Headphone Selected Checkbox
		public string ZZWQ(string s)
		{
			return null;
		}

		// Sets or reads the F5K Mixer Line Out RCA Selected Checkbox
		public string ZZWR(string s)
		{
			return null;
		}

		// Sets or reads the F5K Mixer Output Mute All Checkbox
		public string ZZWS(string s)
		{
			return null;
		}

		// Clears the XIT frequency
		// write only
		public string ZZXC()
		{
			return null;
		}

		// Sets or reads the XIT frequency value
		public string ZZXF(string s)
		{
			return null;
		}

		//Sets or reads the XIT button status
		public string ZZXS(string s)
		{
			return null;
		}

		//Sets or reads the X2TR button status
		public string ZZXT(string s)
		{
			return null;
		}

		public string ZZZB()
		{
			return null;
		}

		public string ZZZZ()
		{
			return null;
		}

		#endregion Extended CAT Methods ZZR-ZZZ


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

			if (this.rigParser.VFO != 0 || s != this.rigParser.Frequency)
			{
				double freq = double.Parse(s.Insert(5,separator));
				this.console.txtVFOAFreq.Text = freq.ToString("f6");
				this.console.txtVFOAFreq_LostFocus(this,new RigCATEventArgs());
			}

			// Store Frequency Status for RigSerialPoller Performance.
			if (this.rigParser.VFO == 0)
			{
				this.rigParser.FrequencyChanged = (this.rigParser.Frequency != s);
				this.rigParser.Frequency = s;
			}
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

			if (this.rigParser.VFO != 1 || s != this.rigParser.Frequency)
			{
				double freq = double.Parse(s.Insert(5,separator));
				this.console.txtVFOBFreq.Text = freq.ToString("f6");
				this.console.txtVFOBFreq_LostFocus(this,new RigCATEventArgs());
			}

			// Store Frequency Status for RigSerialPoller Performance.
			if (this.rigParser.VFO == 1)
			{
				this.rigParser.FrequencyChanged = (this.rigParser.Frequency != s);
				this.rigParser.Frequency = s;
			}
		}
	}
}

