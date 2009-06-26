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

		public override int getModeFromDSPMode(DSPMode dspMode)
		{
			switch (dspMode)
			{
				case DSPMode.LSB:
					return (int) Mode.LSB;
				case DSPMode.USB:
					return (int) Mode.USB;
				case DSPMode.CWL:
					return (int) Mode.CWU;
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
			if (!this.connected || this.Split == splitOn)
				return;

			// :TODO: Figure out how to sync up VFO-A/B Modes on K3
#if false
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
#endif

			this.enqueueRigCATCommand("FT" + ((splitOn) ? '1' : '0') + ';');
			this.Split = splitOn;
		}

		#endregion Set CAT Commands
	}
}
