//=============================================================================
// KenwoodRig.cs
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
	public class KenwoodRig : Rig
	{
		public enum Mode
		{
			LSB = 1,
			USB = 2,
			CW = 3,
			FM = 4,
			AM = 5,
			FSK = 6,
		}


		public KenwoodRig(RigHW hw,Console console)
			: base(hw,console)
		{
		}


		#region Defaults & Supported Functions

		public override int defaultBaudRate()
		{
			return 4800;
		}

		public override bool needsPollVFOB()
		{
			return false;
		}

		public override bool supportsIFFreq()
		{
			return false;
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
				case DSPMode.CWU:
					return (int) Mode.CW;
				case DSPMode.FMN:
					return (int) Mode.FM;
				case DSPMode.AM:
					return (int) Mode.AM;
				case DSPMode.DIGU:
				case DSPMode.DIGL:
					return (int) Mode.FSK;
				default:
					return (int) Mode.LSB;
			}
		}

		#endregion Defaults & Supported Functions


		#region Get CAT Commands

		public override void getRigInformation()
		{
			this.doRigCATCommand("IF;");
		}

		public override void getVFOAFreq()
		{
			this.doRigCATCommand("FA;");
		}

		public override void getVFOBFreq()
		{
			this.doRigCATCommand("FB;");
		}

		public override void getIFFreq()
		{
		}

		#endregion Get CAT Commands

		#region Set CAT Commands

		public override void setVFOAFreq(double freq)
		{
			if (!this.enabled)
				return;

			string frequency =
				freq.ToString("f6").Replace(Rig.separator,"").PadLeft(11,'0');

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
			if (!this.enabled)
				return;

			// Only do this if our Frequency State has changed.
			if (freq == this.VFOAFrequency)
				return;

			this.doRigCATCommand("FA" + freq + ';',false,false);

			// Set our Frequency State so we don't do this again.
			this.VFOAFrequency = freq;
		}

		public override void setVFOBFreq(double freq)
		{
			if (!this.enabled)
				return;

			string frequency =
				freq.ToString("f6").Replace(Rig.separator,"").PadLeft(11,'0');

			// Only do this if our Frequency State has changed.
			// :TODO: Do we need to pay attention to the VFO state?
			if (frequency == this.VFOBFrequency)
				return;

			this.enqueueRigCATCommand("FB" + freq + ';');

			// Set our Frequency State so we don't do this again.
			this.VFOBFrequency = frequency;
		}

		public override void setVFOBFreq(string freq)
		{
			if (!this.enabled)
				return;

			// Only do this if our Frequency State has changed.
			if (freq == this.VFOBFrequency)
				return;

			this.doRigCATCommand("FB" + freq + ';',false,false);

			// Set our Frequency State so we don't do this again.
			this.VFOBFrequency = freq;
		}

		public override void setVFOA()
		{
			this.doRigCATCommand("FN0;",false,false);
		}

		public override void setVFOB()
		{
			this.doRigCATCommand("FN1;",false,false);
		}

		public override void setMode(DSPMode mode)
		{
			int setMode = this.getModeFromDSPMode(mode);

			if (!this.enabled || this.VFOAMode == setMode)
				return;

			this.VFOAMode = setMode;
			this.doRigCATCommand("MD" + setMode + ';',true,false);
		}

		public override void setSplit(bool splitOn)
		{
			if (!this.enabled || this.Split == splitOn)
				return;

			if (splitOn)
			{
				if (this.VFOAMode != this.VFOBMode)
				{
					// Jump to VFO-B and change the mode to sync with VFO-A...
					this.doRigCATCommand("FN1;",true,false);
					Thread.Sleep(this.hw.RigTuningPollingInterval);
					this.doRigCATCommand("MD" + this.VFOAMode + ';',true,false);
					Thread.Sleep(this.hw.RigTuningPollingInterval);
					this.doRigCATCommand("FN0;",true,false);
					this.VFOBMode = this.VFOAMode;
				}
			}

			this.enqueueRigCATCommand("SP" + ((splitOn) ? '1' : '0') + ';');
			this.Split = splitOn;
		}

		public override void clearRIT()
		{
			this.doRigCATCommand("RC;",false,false);
		}

		#endregion Set CAT Commands
	}
}
