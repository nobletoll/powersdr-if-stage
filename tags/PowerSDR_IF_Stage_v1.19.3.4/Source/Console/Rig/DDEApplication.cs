//=================================================================
// RigControl.cs
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
// Author: S.McClements - WU2X, 5/31/2008
//
//=================================================================


using System;

namespace PowerSDR
{
    public class DDEApplication
    {

        // TODO: Actually make this class read from a config file, so that new programs can be added
        // without a recompile. Also, note, this would need to populate the Rig Control program list
        // in the setup menu. 

        public enum Program { HAM_RADIO_DELUXE = 1, LP_BRIDGE };
        private int program = 0;
        private string[] service = new string[2] { "HRD_RADIO_000", "LP_BRIDGE" };
        private string[] topic = new string[2] { "HRD_CAT", "LPB_CAT" };
        private string[] mode = new string[2] { "HRD_MODE", "LPB_MODE" };
        private string[] tx = new string[2] { "HRD_TX", "LPB_TX" };
        private string[] radio = new string[2] { "HRD_RADIO", "LPB_RADIO" };
        private string[] hertz = new string[2] { "HRD_HERTZ", "LPB_HERTZ" };
        private string[] offset = new string[2] { "", "LPB_OFFSET" };
        private string[] timer = new string[2] { "2000", "500" };  // Lockout timer values - zero is not allowed. 
        private string[] hertz_b = new string[2] { "HRD_HERTZ_B", "LPB_HERTZ_B" };

        private string programName = "";

        public DDEApplication(string pgm)
        {
            if (pgm.Equals("Ham Radio Deluxe"))
            {
                this.program = 0;
                this.programName = "Ham Radio Deluxe";
            }
            if (pgm.Equals("LP Bridge"))
            {
                this.program = 1;
                this.programName = "LP Bridge";
            }
        }

        public void setProgram(string pgm)
        {
            if (pgm.Equals("Ham Radio Deluxe"))
            {
                this.program = 0;
                this.programName = "Ham Radio Deluxe";
            }
            if (pgm.Equals("LP Bridge"))
            {
                this.program = 1;
                this.programName = "LP Bridge";
            }
        }

        public string getService()
        {
            return this.service[program];
        }

        public string getTopic()
        {
            return this.topic[program];
        }

        public string getTX()
        {
            return this.tx[program];
        }

        public string getMode()
        {
            return this.mode[program];
        }

        public string getHertz()
        {
            return this.hertz[program];
        }

        public string getHertzB()
        {
            return this.hertz_b[program];
        }

        public string getRadio()
        {
            return this.radio[program];

        }

        public string getOffset()
        {
            return this.offset[program];

        }

        public string getProgramName()
        {
            return this.programName;
        }

        public double getTimerValue()
        {
            return System.Double.Parse(this.timer[program]); ;
        }
    }
}