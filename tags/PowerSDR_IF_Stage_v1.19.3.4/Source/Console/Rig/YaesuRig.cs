//=============================================================================
// YaesuRig.cs
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
	public class YaesuRig : KenwoodRig
	{
		public enum Mode
		{
			LSB = 1,
			USB = 2,
			CWU = 3,
			FM = 4,
			AM = 5,
			FSKL = 6,
			CWL = 7,
			PKTL = 8,
			FSKU = 9,
			PKTFM = 10,
			FMN = 11,
			PKTU = 12
		}

	
		public YaesuRig(RigHW hw, Console console)
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

		public override bool ritAppliedInIFCATCommand()
		{
			return false;
		}

		public override int getModeFromDSPMode(DSPMode dspMode)
		{
			switch (dspMode)
			{
				case DSPMode.LSB:
					return (int)Mode.LSB;
				case DSPMode.USB:
					return (int)Mode.USB;
				case DSPMode.CWL:
					return (int)Mode.CWL;
				case DSPMode.CWU:
					return (int)Mode.CWU;
				case DSPMode.FMN:
					return (int)Mode.FM;
				case DSPMode.AM:
					return (int)Mode.AM;
				case DSPMode.SAM:
					return (int)Mode.AM;
				case DSPMode.DIGU:
					return (int)Mode.FSKU;
				case DSPMode.DIGL:
					return (int)Mode.FSKL;
				default:
					return (int)Mode.LSB;
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
					this.console.RX1DSPMode = DSPMode.CWU;
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
					this.console.RX1DSPMode = DSPMode.CWL;
					break;
				case "9":
					this.console.RX1DSPMode = DSPMode.DIGU;
					break;
				default:
					break;
			}
		}

		public override bool useRITForVFOB()
		{
			return false;
		}

		public override bool hasCWL()
		{
			return true;
		}

		public override bool hasCWU()
		{
			return true;
		}

		public override bool hasFSKL()
		{
			return true;
		}

		public override bool hasFSKU()
		{
			return true;
		}

		#endregion Defaults & Supported Functions


		#region Get CAT Commands

		public override void getTX()
		{
			this.doRigCATCommand("TX;");
		}

		public override void getTXVFO()
		{
			this.doRigCATCommand("FT;");
		}

		#endregion Get CAT Commands


		#region Set CAT Commands

		public override void setVFOAFreq(double freq)
		{
			if (!this.active)
				return;

			string frequency =
				freq.ToString("f6").Replace(Rig.separator, "").PadLeft(8, '0');

			// Only do this if our Frequency State has changed.
			// :TODO: Do we need to pay attention to the VFO state?
			if (frequency == this.VFOAFrequency)
				return;

			this.enqueueRigCATCommand("FA" + frequency + ';');

			// Set our Frequency State so we don't do this again.
			this.VFOAFrequency = frequency;
		}

		public override void setVFOAFreq(string freq)
		{
			if (!this.active)
				return;

			freq = freq.Substring(3);

			// Only do this if our Frequency State has changed.
			if (freq == this.VFOAFrequency)
				return;

			this.doRigCATCommand("FA" + freq + ';', false, false);

			// Set our Frequency State so we don't do this again.
			this.VFOAFrequency = freq;
		}

		public override void setVFOBFreq(double freq)
		{
			if (!this.active)
				return;

			string frequency =
				freq.ToString("f6").Replace(Rig.separator, "").PadLeft(8, '0');

			// Only do this if our Frequency State has changed.
			// :TODO: Do we need to pay attention to the VFO state?
			if (frequency == this.VFOBFrequency)
				return;

			this.enqueueRigCATCommand("FB" + frequency + ';');

			// Set our Frequency State so we don't do this again.
			this.VFOBFrequency = frequency;
		}

		public override void setVFOBFreq(string freq)
		{
			if (!this.active)
				return;

			freq = freq.Substring(3);

			// Only do this if our Frequency State has changed.
			if (freq == this.VFOBFrequency)
				return;

			this.doRigCATCommand("FB" + freq + ';', false, false);

			// Set our Frequency State so we don't do this again.
			this.VFOBFrequency = freq;
		}

		public override void setSplit(bool splitOn)
		{
			if (!this.active || this.Split == splitOn)
				return;

			this.doRigCATCommand("FT" + ((splitOn) ? '1' : '0') + ';',true,false);
			this.Split = splitOn;
		}

		public override void setMode(DSPMode mode)
		{
			int setMode = this.getModeFromDSPMode(mode);

			if (!this.active || this.VFOAMode == setMode)
				return;

			this.VFOAMode = setMode;
			this.doRigCATCommand("MD0" + setMode + ';', true, false);
		}

        public override void setVFOA()
        {
            // :TODO:
        }

        public override void setRIT(int ritOffset)
        {
            if (!this.RITOffsetInitialized || ritOffset == this.RITOffset)
                return;

            int deltaOffset = ritOffset - this.RITOffset;

            if (deltaOffset > 0)
            {
                this.doRigCATCommand("RU" + deltaOffset.ToString().PadLeft(4, '0') + ';', true, false);
            }
            else if (deltaOffset < 0)
            {
                deltaOffset = deltaOffset * -1;  // Its negative, make it positive. 
                this.doRigCATCommand("RD" + deltaOffset.ToString().PadLeft(4, '0') + ';', true, false);
            }

            this.RITOffset = ritOffset;
        }

		#endregion Set CAT Commands
	}
}