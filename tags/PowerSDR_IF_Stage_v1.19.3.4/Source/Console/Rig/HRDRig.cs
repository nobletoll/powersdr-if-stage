//=================================================================
// HRDRig.cs
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
// Author: S.McClements - WU2X, 6/20/2009
//
// This class communicates with HRD over DDE. This allows any rig
// HRD supports to connect to PowerSDR/IF. 
//
//=================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PowerSDR
{
    class HRDRig : Rig
    {

        private IntPtr hConv;								// HRD DDE Conversation Handle.
        private Int32 pidInst;								// DDE 
        private DDEML.DdeCallback dcb;                      // DDE callback method for HRD updates
        private DDEApplication ddeApplication = new DDEApplication("Ham Radio Deluxe");
        private string rigName;
        private bool muteCache;				                // Mute button state cache
        private bool warningOpen;
        private bool lockoutConnectionAttempt;
        private System.Timers.Timer lockoutConnectTimer = new System.Timers.Timer(); // Timer for update lockout



        public HRDRig(RigHW hw, Console console)
            : base(hw, console)
        {

            // Setup connect lockout timer
            lockoutConnectTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.LockoutConnectTimeExpiredEvent); //WU2X
            lockoutConnectTimer.Interval = 5000;  //  Lock for reconnect to rig control program 
            lockoutConnectTimer.AutoReset = false;
            lockoutConnectTimer.Enabled = true;    
        }


        public override void disconnect()
        {
            // DDE Disconnect doesn't work. Works ok without disconnect. 
        }

        public string getRigName()
        {
            return this.rigName;
        }

        public override void setVFOAFreq(double f)
        {
            // Check to see that we are connected to HRD
            if (!active)
            {
                this.connect();
            }

            Int32 pwdResult = 0;

            String frequency = f.ToString("f6");
            String freq = frequency.Replace(".", ""); // Remove the period from the string

            byte[] data = Encoding.ASCII.GetBytes("freq " + freq + "\x00");
            // Send frequency update to HRD over DDE Execute
            DDEML.DdeClientTransaction(data, (uint)data.Length, hConv, IntPtr.Zero, 0, DDEML.XTYP_EXECUTE, 60000, ref pwdResult);
            // Int32 result =  DDEML.DdeGetLastError(pidInst);

            // Note - I don't know if there is a reliable way to tell if a DDE connection to a server was 
            // broken. The result return code seems to be 16390 -  DMLERR_INVALIDPARAMETER  
            // when the connection is broken - however, 16390 show up randomly when tuning around
            // and things are working normally.  

            //System.Console.WriteLine("Result of DDE Freq:" + result); 
            //System.Console.WriteLine("DDE Result: " + pwdResult);

            // Start or Restart lockout timer to ignore external VFO events
            // for a second or two...

            this.rigPollingLockout = true;
            this.rigPollingLockoutTimer.Stop();
            this.rigPollingLockoutTimer.Start();

        }

        public override void setMode(PowerSDR.DSPMode m)
        {

            // Check to see that we are connected to HRD
            if (!active)
            {
                connect();
            }

            String mode = null;

            //TODO:

            // For now, just map the PowerSDR modes to some common HRD strings.
            // Modes like DIGL map to different string names depending on what radio
            // you are connected to in HRD. We can fix this if someone asks. 

            if (m == PowerSDR.DSPMode.LSB) mode = "LSB";
            if (m == PowerSDR.DSPMode.USB) mode = "USB";
            if (m == PowerSDR.DSPMode.CWU) mode = "CW";
            if (m == PowerSDR.DSPMode.CWL) mode = "CW";
            if (m == PowerSDR.DSPMode.FMN) mode = "FM";
            if (m == PowerSDR.DSPMode.AM) mode = "AM";
            if (m == PowerSDR.DSPMode.DIGU) mode = "FSK";
            if (m == PowerSDR.DSPMode.DIGL) mode = "FSK";

            // We just take the mode in the string format - No Validation 
            byte[] data = Encoding.ASCII.GetBytes("mode " + mode + "\x00");
            Int32 pwdResult = 0;

            DDEML.DdeClientTransaction(data, (uint)data.Length, hConv, IntPtr.Zero, 0, DDEML.XTYP_EXECUTE, 60000, ref pwdResult);

            // Start or Restart lockout timer to ignore external VFO events
            // for a second or two...

            this.rigPollingLockout = true;
            this.rigPollingLockoutTimer.Stop();
            this.rigPollingLockoutTimer.Start();
        }

        public override void connect()
        {
            if ((!active) && (this.lockoutConnectionAttempt == false))
            {
                Int32 ulRef = 0;

                // This instance is global as it needs to hang around for HRD to call
                Boolean status1;
                if (pidInst != 0)
                {
                    status1 = DDEML.DdeUninitialize(pidInst);
                    pidInst = 0;
                }

                dcb = new DDEML.DdeCallback(HRDDdeCallback);
                Int32 status2 = DDEML.DdeInitialize(ref pidInst, dcb, 0, ulRef);
                // Allocate String Handles
                IntPtr service = DDEML.DdeCreateStringHandle(pidInst, Marshal.StringToBSTR(ddeApplication.getService()), DDEML.CP_WINUNICODE);
                IntPtr topic = DDEML.DdeCreateStringHandle(pidInst, Marshal.StringToBSTR(ddeApplication.getTopic()), DDEML.CP_WINUNICODE);

                hConv = DDEML.DdeConnect(pidInst,
                    service,
                    topic,
                    (IntPtr)null);

                Int32 result = DDEML.DdeGetLastError(pidInst);
                // Free String Handles
                DDEML.DdeFreeStringHandle(pidInst, service);
                DDEML.DdeFreeStringHandle(pidInst, topic);

                // Check to see if our DDE Connection to HRD was successful
                if (result != 0)
                {

                    if (!this.warningOpen)
                    {
                        this.warningOpen = true;
                        MessageBox.Show("Unable to connect to " + ddeApplication.getProgramName() + ". Please make sure that " + ddeApplication.getProgramName() + " is running and properly controlling your radio. DDE Connection Error: " + result, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.warningOpen = false;
                        // Will try to connect again next time we need to send something to HRD...
                        this.lockoutConnectionAttempt = true;
                        // Start or Restart lockout to not try to reconnect for 5 seconds. 
                        this.lockoutConnectTimer.Stop();
                        this.lockoutConnectTimer.Start();
                      
                    }
                    return;
                }
                else
                {
                    active = true;
                }

                // Setup hot advise loop on the items of interest - frequency, mode, and TX status
                Int32 pwdResult = 0;
                IntPtr topic2 = DDEML.DdeCreateStringHandle(pidInst, Marshal.StringToBSTR(ddeApplication.getHertz()), DDEML.CP_WINUNICODE);
                IntPtr topic3 = DDEML.DdeCreateStringHandle(pidInst, Marshal.StringToBSTR(ddeApplication.getMode()), DDEML.CP_WINUNICODE);
                IntPtr topic4 = DDEML.DdeCreateStringHandle(pidInst, Marshal.StringToBSTR(ddeApplication.getTX()), DDEML.CP_WINUNICODE);
                IntPtr topic5 = DDEML.DdeCreateStringHandle(pidInst, Marshal.StringToBSTR(ddeApplication.getRadio()), DDEML.CP_WINUNICODE);

                IntPtr Data2 = DDEML.DdeClientTransaction(null, 0, hConv, topic2, DDEML.CF_TEXT, DDEML.XTYP_ADVSTART, 1000, ref pwdResult);
                IntPtr Data3 = DDEML.DdeClientTransaction(null, 0, hConv, topic3, DDEML.CF_TEXT, DDEML.XTYP_ADVSTART, 1000, ref pwdResult);
                IntPtr Data4 = DDEML.DdeClientTransaction(null, 0, hConv, topic4, DDEML.CF_TEXT, DDEML.XTYP_ADVSTART, 1000, ref pwdResult);

                // Request radio HRD is connected to and set the window title with it
                IntPtr Data5 = DDEML.DdeClientTransaction(null, 0, hConv, topic5, DDEML.CF_TEXT, DDEML.XTYP_REQUEST, 1000, ref pwdResult);
                Int32 len31 = DDEML.DdeGetData(Data5, null, 0, 0) + 1;
                byte[] data1 = new byte[len31];
                DDEML.DdeGetData(Data5, data1, len31, 0);
                String dataString1 = System.Text.Encoding.Default.GetString(data1);
                this.rigName = dataString1.Trim('\0');


                // Grab frequency and mode from rig and sync PowerSDR
                // The external radio is the master and powersdr is the the slave
                // TODO: Turn code to poll DDE connection on a topic into a method

                // Frequency 
                topic5 = DDEML.DdeCreateStringHandle(pidInst, Marshal.StringToBSTR(ddeApplication.getHertz()), DDEML.CP_WINUNICODE);
                Data5 = DDEML.DdeClientTransaction(null, 0, hConv, topic5, DDEML.CF_TEXT, DDEML.XTYP_REQUEST, 1000, ref pwdResult);
                len31 = DDEML.DdeGetData(Data5, null, 0, 0) + 1;
                data1 = new byte[len31];
                DDEML.DdeGetData(Data5, data1, len31, 0);
                dataString1 = System.Text.Encoding.Default.GetString(data1);
                processFrequency(dataString1);

                // Mode 
                topic5 = DDEML.DdeCreateStringHandle(pidInst, Marshal.StringToBSTR(ddeApplication.getMode()), DDEML.CP_WINUNICODE);
                Data5 = DDEML.DdeClientTransaction(null, 0, hConv, topic5, DDEML.CF_TEXT, DDEML.XTYP_REQUEST, 1000, ref pwdResult);
                len31 = DDEML.DdeGetData(Data5, null, 0, 0) + 1;
                data1 = new byte[len31];
                DDEML.DdeGetData(Data5, data1, len31, 0);
                dataString1 = System.Text.Encoding.Default.GetString(data1);
                processMode(dataString1);


                // Hertz_B  (VFO) Setup hot advise 
                // HRD Currently has a problem returning Hertz_A and Hertz_B - intermitantly sends 000000000. Really screws things. 

                /*
                topic5 = DDEML.DdeCreateStringHandle(pidInst, Marshal.StringToBSTR(rigControl.getHertzB()), DDEML.CP_WINUNICODE);
                Data5 =   DDEML.DdeClientTransaction(null, 0, hConv, topic5, DDEML.CF_TEXT, DDEML.XTYP_REQUEST, 1000, ref pwdResult);
                len31 = DDEML.DdeGetData(Data5, null, 0, 0) + 1;
                data1 = new byte[len31];		
                DDEML.DdeGetData(Data5, data1, len31, 0);
                dataString1 = System.Text.Encoding.Default.GetString(data1);
                Data5 =   DDEML.DdeClientTransaction(null, 0, hConv, topic5, DDEML.CF_TEXT, DDEML.XTYP_ADVSTART, 1000, ref pwdResult);
                */

                // Free String Handles
                DDEML.DdeFreeStringHandle(pidInst, topic2);
                DDEML.DdeFreeStringHandle(pidInst, topic3);
                DDEML.DdeFreeStringHandle(pidInst, topic4);
                DDEML.DdeFreeStringHandle(pidInst, topic5);
            }

            // Setup lockout timer interval based on Rig Control program
            this.rigPollingLockoutTimer.Interval = this.ddeApplication.getTimerValue();

            this.console.updateConsoleTitle();
        }

        private void processFrequency(String frequency)
        {

            String frequency2 = frequency.Trim('\0').PadLeft(7, '0');
            String freqMhz = frequency2.Substring(0, frequency2.Length - 6) + separator + frequency2.Substring(frequency2.Length - 6, 6);

            if (this.console.SetupForm.RttyOffsetEnabledA &&
                (this.console.RX1DSPMode == DSPMode.DIGU ||
                this.console.RX1DSPMode == DSPMode.DIGL))
            {
                int f = int.Parse(freqMhz);

                if (this.console.RX1DSPMode == DSPMode.DIGU)
                    f = f - Convert.ToInt32(console.SetupForm.RttyOffsetHigh);
                else if (console.RX1DSPMode == DSPMode.DIGL)
                    f = f + Convert.ToInt32(console.SetupForm.RttyOffsetLow);

            }


            double freq = double.Parse(freqMhz);
            this.console.txtVFOAFreq.Text = freq.ToString("f6");
            this.console.txtVFOAFreq_LostFocus(this, new RigCATEventArgs());

        }

        private void processMode(String mode)
        {

            // TODO:
            // Some modes sent depend on the radio HRD is connected to, i.e. FSK. Fix if someone needs it. 

            if ((mode.StartsWith("LSB")) && (this.console.RX1DSPMode != DSPMode.LSB)) this.console.RX1DSPMode = DSPMode.LSB;
            if ((mode.StartsWith("USB")) && (this.console.RX1DSPMode != DSPMode.USB)) this.console.RX1DSPMode = DSPMode.USB;
            if ((mode.StartsWith("CW")) && (this.console.RX1DSPMode != DSPMode.CWU)) this.console.RX1DSPMode = DSPMode.CWU;
            if ((mode.StartsWith("FM")) && (this.console.RX1DSPMode != DSPMode.FMN)) this.console.RX1DSPMode = DSPMode.FMN;
            if ((mode.StartsWith("AM")) && (this.console.RX1DSPMode != DSPMode.LSB)) this.console.RX1DSPMode = DSPMode.AM;
            if ((mode.StartsWith("FSK")) && (this.console.RX1DSPMode != DSPMode.DIGU)) this.console.RX1DSPMode = DSPMode.DIGU;

        }

        public IntPtr HRDDdeCallback(Int32 uType, Int32 uFmt, IntPtr hconv, IntPtr hsz1, IntPtr hsz2, IntPtr hdata, IntPtr dwData1, IntPtr dwData2)
        {

            switch (uType)
            {
                case DDEML.XTYP_ADVDATA:  // Advise Data

                    Int32 len = DDEML.DdeQueryString(pidInst, hsz1, null, 100, DDEML.CP_WINANSI) + 1;
                    Int32 len2 = DDEML.DdeQueryString(pidInst, hsz2, null, 100, DDEML.CP_WINANSI) + 1;
                    Int32 len3 = DDEML.DdeGetData(hdata, null, 0, 0) + 1;

                    byte[] data = new byte[len3];

                    StringBuilder topic = new StringBuilder(len);
                    StringBuilder item = new StringBuilder(len2);

                    DDEML.DdeQueryString(pidInst, hsz1, topic, len, DDEML.CP_WINANSI);
                    DDEML.DdeQueryString(pidInst, hsz2, item, len2, DDEML.CP_WINANSI);
                    DDEML.DdeGetData(hdata, data, len3, 0);

                    String dataString = System.Text.Encoding.Default.GetString(data);

                    /////////////////////
                    // HRD_TX Handling //
                    /////////////////////	
                    if (item.ToString().Equals(ddeApplication.getTX()) && dataString.StartsWith("On"))
                    {
                        // TX ON - MUTE
                        // Save mute state - on TX off revert to this state

                        this.muteCache = this.console.MUT;
                        // If the monitor isn't checked then mute 
                        if (!this.console.MON)
                        {
                            this.console.MUT = true;
                        }
                    }
                    else if (item.ToString().Equals(ddeApplication.getTX()))
                    {
                        // TX_OFF - UNMUTE - Unless mute was already on
                        // Revert to previous state
                        this.console.MUT = this.muteCache;
                    }

                    ///////////////////////
                    // HRD_MODE Handling //
                    ///////////////////////

                    if ((item.ToString().Equals(ddeApplication.getMode())) && !(this.rigPollingLockout))
                    {

                        // System.Console.WriteLine("RECEIVED:  Mode: " + dataString);
                        processMode(dataString);

                    }
                    else if ((item.ToString().Equals(ddeApplication.getMode())))
                    {

                        // System.Console.WriteLine("RECEIVED:  Mode: " + dataString + " : IGNORING - IN LOCKOUT");

                    }

                    ////////////////////////
                    // HRD_HERTZ Handling //
                    ////////////////////////

                    // We ignore these messages if the PowerSDR software VFO has been
                    // tuned within the last second.
                    // HRD generates HRD_HERTZ in response to us sending updates
                    // but its not 1:1 and the frequencies sent back are not necessarily
                    // the ones we sent. So we just lockout getting updates from HRD
                    // for a second or two. 
                    // TODO: When lockout timer expires poll the radio for freq and mode



                    if (item.ToString().Equals(ddeApplication.getHertz()) && !(this.rigPollingLockout))
                    {
                        // System.Console.WriteLine("RECEIVED:  frequency: " + dataString);
                        processFrequency(dataString);

                    }

                    //////////////////////////
                    // HRD_HERTZ_B Handling //
                    //////////////////////////
                    /*
                    if (item.ToString().Equals(rigControl.getHertzB()))
                    {
                        System.Console.WriteLine("RECEIVED:  HERTZ_B frequency: " + dataString);
                        processFrequencyB(dataString);

                    } 
                    else if (item.ToString().Equals(rigControl.getHertzB())) 
                    {
                        System.Console.WriteLine("RECEIVED:  ERTZ_B frequency: " + dataString + " : IGNORING - IN LOCKOUT");
                    }
                    */

                    return new IntPtr(DDEML.DDE_FACK); // - Means OK I processed it...

            }

            // Ignore any other transaction type
            return IntPtr.Zero;

        }

        // Methods not implementable when using HRD
        public override void setVFOBFreq(double freq) { }
        public override void setVFOAFreq(string freq) { }
        public override void setVFOBFreq(string freq) { }
        public override void setVFOA() { }
        public override void setVFOB() { }
        public override void setSplit(bool split) { }
        public override void clearRIT() { }
        public override void setRIT(bool rit) { }
        public override void setRIT(int ritOffset) { }
        public override void getVFOAFreq() { }
        public override void getVFOBFreq() { }
        public override void getIFFreq() { }
        public override void getRigInformation() { }


        public override int defaultBaudRate()
        {
            return 0;
        }

        public override int getModeFromDSPMode(PowerSDR.DSPMode dspMode)
        {
            return 0;
        }

		public override double minFreq()
		{
			return 1.0;
		}

		public override double maxFreq()
		{
			return 30.0;
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

		private void LockoutConnectTimeExpiredEvent(object source,
			System.Timers.ElapsedEventArgs e)
        {
            this.lockoutConnectionAttempt = false;
        }
    }
}
