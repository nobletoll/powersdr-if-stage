//=============================================================================
// K3Rig.cs
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

using System.Threading;


namespace PowerSDR
{
	public class K3Rig : KenwoodRig
	{
		public enum Mode
		{
			LSB = 1,
			USB = 2,
			CWL = 3,
			FM = 4,
			AM = 5,
			DIGL = 6,
			CWU = 7,
			DIGU = 9,
		}


		public K3Rig(RigHW hw,Console console)
			: base(hw,console)
		{
		}


		#region Defaults & Supported Functions

		public override int defaultBaudRate()
		{
			return 38400;
		}

		public override bool needsPollVFOB()
		{
			return true;
		}

		public override bool supportsIFFreq()
		{
			return true;
		}

		public override bool needsLOFreq()
		{
			return true;
		}

		public override int iqSwapFreq()
		{
			return 48;
		}

		public override double minFreq()
		{
			return 0.5;
		}

		public override double maxFreq()
		{
			return 54.0;
		}

		public override int getModeFromDSPMode(DSPMode dspMode)
		{
			switch (dspMode)
			{
				case DSPMode.LSB:
					return (int) Mode.LSB;
				case DSPMode.USB:
					return (int) Mode.USB;
				case DSPMode.CWL:
					return (int) Mode.CWL;
				case DSPMode.CWU:
					return (int) Mode.CWU;
				case DSPMode.FMN:
					return (int) Mode.FM;
				case DSPMode.AM:
					return (int) Mode.AM;
				case DSPMode.DIGU:
					return (int) Mode.DIGU;
				case DSPMode.DIGL:
					return (int) Mode.DIGL;
				default:
					return (int) Mode.LSB;
			}
		}

		public override void setConsoleModeFromString(string mode)
		{
			switch (mode)
			{
				case "1":								
					this.console.RX1DSPMode = DSPMode.LSB;
					break;
				case "2":
					this.console.RX1DSPMode = DSPMode.USB;
					break;
				case "3":
					this.console.RX1DSPMode = DSPMode.CWL;
					break;
				case "4":
					this.console.RX1DSPMode = DSPMode.FMN;
					break;
				case "5":
					this.console.RX1DSPMode = DSPMode.AM;
					break;
				case "6":
					this.console.RX1DSPMode = DSPMode.DIGL;
					break;
				case "7":
					this.console.RX1DSPMode = DSPMode.CWU;
					break;
				case "9":
					this.console.RX1DSPMode = DSPMode.DIGU;
					break;
				default:
					break;
			}
		}

		public override bool ritAppliedInIFCATCommand()
		{
			return false;
		}

		#endregion Defaults & Supported Functions


		#region Get CAT Commands

		public override void getIFFreq()
		{
			this.doRigCATCommand("FI;");
		}

		#endregion Get CAT Commands

		#region Set CAT Commands

		public override void setVFOA()
		{
			// :TODO:
		}

		public override void setVFOB()
		{
			// :TODO:
		}

		public override void setSplit(bool splitOn)
		{
			if (!this.active || this.Split == splitOn)
				return;

			this.doRigCATCommand("FT" + ((splitOn) ? '1' : '0') + ';',true,false);
			this.Split = splitOn;
		}

		#endregion Set CAT Commands
	}
}