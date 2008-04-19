//=================================================================
// HRDEventArgs.cs
//=================================================================
//
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
//
// ================================================================
//
// Author: S.McClements - WU2X, 7/16/2007
//
// This class is used to give context to the VFO tuning event,
// i.e. is it an internal sourced event (PowerSDR is tuned) 
// or external sourced event (Rig VFO is tuned)
//
//=================================================================

using System;

namespace PowerSDR
{
	public class HRDEventArgs : EventArgs
	{
		public enum Source {Internal=1, External};
        public Source source;

		public HRDEventArgs(Source src)
		{
	      this.source = src;
		}
	}
}
