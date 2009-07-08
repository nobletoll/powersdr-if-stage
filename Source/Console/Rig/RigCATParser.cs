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

		public RigCATParser(Console c, RigHW hw)
			: base(c)
		{
			this.rigCmdList = new RigCATCommands(c,hw,this);

			// W1CEG: TS-940S and K3 has spaces in the IF answer.
			this.sfxpattern = new Regex("^[Vv0-9 +-]*$");
		}

		#endregion Constructor

		#region Answer

		// Overloaded Get method accepts either byte or string
		public new string Answer(byte[] pCmdString)
		{
			string rtncmd = this.Get(AE.GetString(pCmdString));
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
				case "FA":
					rtncmd = this.rigCmdList.FA(suffix);
					break;
				case "FB":
					rtncmd = this.rigCmdList.FB(suffix);
					break;
				case "FI":
					rtncmd = this.rigCmdList.FI(suffix);
					break;
				case "FT":
					rtncmd = this.rigCmdList.FT(suffix);
					break;
				case "IF":
					rtncmd = this.rigCmdList.IF(suffix);
					break;
			}

			return rtncmd;	// Read successfully executed
		}

		#endregion Answer
	}
}
