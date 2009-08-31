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
	public class Kenwood950Rig : KenwoodRig
	{
		public Kenwood950Rig(RigHW hw, Console console)
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
			return true;
		}

		public override bool ritAppliedInIFCATCommand()
		{
			return false;
		}

		#endregion Defaults & Supported Functions


		#region Get CAT Commands

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
