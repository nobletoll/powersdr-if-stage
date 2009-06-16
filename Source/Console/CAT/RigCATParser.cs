//=============================================================================
// RigCATParser.cs
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

using System.Text.RegularExpressions;

namespace PowerSDR
{
	public class RigCATParser : CATParser
	{
		#region Variables

		private RigCATCommands rigCmdList;

		#endregion Variables


		#region Constructor

		public RigCATParser(Console c,RigSerialPoller rigSerialPoller)
			: base(c)
		{
			this.rigCmdList = new RigCATCommands(c,rigSerialPoller,this);

			// W1CEG: TS-940S has spaces in the IF answer.
			this.sfxpattern = new Regex("^[0-9+-]*[Vv0-9 ]*$");
		}


		public void initStates()
		{
			this.mode = RigHW.getModeFromDSPMode(this.console.RX1DSPMode);
			this.split = this.console.VFOSplit;
		}

		#endregion Constructor


		#region Accessors

		private int vfo = 99;
		public int VFO
		{
			get { return this.vfo; }
			set { this.vfo = value; }
		}

		private int mode = 0;
		public int Mode
		{
			get { return this.mode; }
			set { this.mode = value; }
		}

		private string vfoaFrequency = "";
		public string VFOAFrequency
		{
			get { return this.vfoaFrequency; }
			set { this.vfoaFrequency = value; }
		}

		private string vfobFrequency = "";
		public string VFOBFrequency
		{
			get { return this.vfobFrequency; }
			set { this.vfobFrequency = value; }
		}

		private bool frequencyChanged = false;
		public bool FrequencyChanged
		{
			get { return this.frequencyChanged; }
			set { this.frequencyChanged = value; }
		}

		private int ritOffset = 0;
		public int RITOffset
		{
			get { return this.ritOffset; }
			set { this.ritOffset = value; }
		}

		private bool split = false;
		public bool Split
		{
			get { return this.split; }
			set { this.split = value; }
		}

		#endregion Accessors


		#region Answer

		// Overloaded Get method accepts either byte or string
		public new string Answer(byte[] pCmdString)
		{
			string rtncmd = Get(AE.GetString(pCmdString));
			return rtncmd;
		}

		public new string Answer(string pCmdString)
		{
			string rtncmd = null;
			this.current_cat = pCmdString;

			// Abort if the overall string length is less than 3 (aa;)
			if (current_cat.Length < 3)
				return Error1;

			if (!this.CheckFormat())
				return Error1;

			switch (prefix)
			{
				case "AC":
					break;
				case "AG":
					break;
				case "AI":
					break;
				case "AL":
					break;
				case "AM":
					break;
				case "AN":
					break;
				case "AR":
					break;
				case "AS":
					break;
				case "BC":
					break;
				case "BD":
					break;
				case "BP":
					break;
				case "BU":
					break;
				case "BY":
					break;
				case "CA":
					break;
				case "CG":
					break;
				case "CH":
					break;
				case "CI":
					break;
				case "CM":
					break;
				case "CN":
					break;
				case "CT":
					break;
				case "DC":
					break;
				case "DN":
					break;
				case "DQ":
					break;
				case "EX":
					break;
				case "FA":
					rtncmd = this.rigCmdList.FA(suffix);
					break;
				case "FB":
					rtncmd = this.rigCmdList.FB(suffix);
					break;
				case "FC":
					break;
				case "FD":
					break;
				case "FR":
					break;
				case "FS":
					break;
				case "FT":
					rtncmd = this.rigCmdList.FT(suffix);
					break;
				case "FW":
					break;
				case "GT":
					break;
				case "ID":
					break;
				case "IF":
					rtncmd = this.rigCmdList.IF(suffix);
					break;
				case "IS":
					break;
				case "KS":
					break;
				case "KY":
					break;
				case "LK":
					break;
				case "LM":
					break;
				case "LT":
					break;
				case "MC":
					break;
				case "MD":
					break;
				case "MF":
					break;
				case "MG":
					break;
				case "ML":
					break;
				case "MO":
					break;
				case "MR":
					break;
				case "MU":
					break;
				case "MW":
					break;
				case "NB":
					break;
				case "NL":
					break;
				case "NR":
					break;
				case "NT":
					break;
				case "OF":
					break;
				case "OI":
					break;
				case "OS":
					break;
				case "PA":
					break;
				case "PB":
					break;
				case "PC":
					break;
				case "PI":
					break;
				case "PK":
					break;
				case "PL":
					break;
				case "PM":
					break;
				case "PR":
					break;
				case "PS":
					break;
				case "QC":
					break;
				case "QI":
					break;
				case "QR":
					break;
				case "RA":
					break;
				case "RC":
					break;
				case "RD":
					break;
				case "RG":
					break;
				case "RL":
					break;
				case "RM":
					break;
				case "RT":
					break;
				case "RU":
					break;
				case "RX":
					break;
				case "SA":
					break;
				case "SB":
					break;
				case "SC":
					break;
				case "SD":
					break;
				case "SH":
					break;
				case "SI":
					break;
				case "SL":
					break;
				case "SM":
					break;
				case "SQ":
					break;
				case "SR":
					break;
				case "SS":
					break;
				case "ST":
					break;
				case "SU":
					break;
				case "SV":
					break;
				case "TC":
					break;
				case "TD":
					break;
				case "TI":
					break;
				case "TN":
					break;
				case "TO":
					break;
				case "TS":
					break;
				case "TX":
					break;
				case "TY":
					break;
				case "UL":
					break;
				case "UP":
					break;
				case "VD":
					break;
				case "VG":
					break;
				case "VR":
					break;
				case "VX":
					break;
				case "XT":
					break;
			}

			return rtncmd;	// Read successfully executed
		}

		#endregion Answer
	}
}
