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
        private RigControl rigControl = new RigControl("Ham Radio Deluxe");


        public HRDRig(RigHW hw, Console console)
            : base(hw, console)
        {
        }


        public override void disconnect()
        {
        }

        public override int defaultBaudRate()
        {

            // TODO
            return 0;
        }

        public override bool needsPollVFOB()
        {
            // TODO
            return false;
        }

        public override bool supportsIFFreq()
        {
            return false;
        }

        public override int getModeFromDSPMode(PowerSDR.DSPMode dspMode)
        {
            // TODO
            return 0;
        }

        public override void getRigInformation()
        {

        }

        public override void getVFOAFreq()
        {

        }

        public override void getVFOBFreq()
        {

        }


        public override void getIFFreq()
        {

        }

        public override void setVFOAFreq(double freq)
        {

        }

        public override void setVFOBFreq(double freq)
        {

        }
        public override void setVFOAFreq(string freq)
        {

        }

        public override void setVFOBFreq(string freq)
        {

        }

        public override void setVFOA()
        {

        }

        public override void setVFOB()
        {

        }

        public override void setMode(PowerSDR.DSPMode mode)
        {

        }

        public override void setSplit(bool split)
        {

        }

        public override void clearRIT()
        {

        }

        public override void connect()
        {

            if (!connected)
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
                IntPtr service = DDEML.DdeCreateStringHandle(pidInst, Marshal.StringToBSTR(rigControl.getService()), DDEML.CP_WINUNICODE);
                IntPtr topic = DDEML.DdeCreateStringHandle(pidInst, Marshal.StringToBSTR(rigControl.getTopic()), DDEML.CP_WINUNICODE);

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

                   
                    MessageBox.Show("Unable to connect to " + rigControl.getProgramName() + ". Please make sure that " + rigControl.getProgramName() + " is running and properly controlling your radio. DDE Connection Error: " + result, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
 



                }
                else
                {
                    connected = true;
                }



                // Setup hot advise loop on the items of interest - frequency, mode, and TX status
                Int32 pwdResult = 0;
                IntPtr topic2 = DDEML.DdeCreateStringHandle(pidInst, Marshal.StringToBSTR(rigControl.getHertz()), DDEML.CP_WINUNICODE);
                IntPtr topic3 = DDEML.DdeCreateStringHandle(pidInst, Marshal.StringToBSTR(rigControl.getMode()), DDEML.CP_WINUNICODE);
                IntPtr topic4 = DDEML.DdeCreateStringHandle(pidInst, Marshal.StringToBSTR(rigControl.getTX()), DDEML.CP_WINUNICODE);
                IntPtr topic5 = DDEML.DdeCreateStringHandle(pidInst, Marshal.StringToBSTR(rigControl.getRadio()), DDEML.CP_WINUNICODE);

                IntPtr Data2 = DDEML.DdeClientTransaction(null, 0, hConv, topic2, DDEML.CF_TEXT, DDEML.XTYP_ADVSTART, 1000, ref pwdResult);
                IntPtr Data3 = DDEML.DdeClientTransaction(null, 0, hConv, topic3, DDEML.CF_TEXT, DDEML.XTYP_ADVSTART, 1000, ref pwdResult);
                IntPtr Data4 = DDEML.DdeClientTransaction(null, 0, hConv, topic4, DDEML.CF_TEXT, DDEML.XTYP_ADVSTART, 1000, ref pwdResult);

                // Request radio HRD is connected to and set the window title with it
                IntPtr Data5 = DDEML.DdeClientTransaction(null, 0, hConv, topic5, DDEML.CF_TEXT, DDEML.XTYP_REQUEST, 1000, ref pwdResult);
                Int32 len31 = DDEML.DdeGetData(Data5, null, 0, 0) + 1;
                byte[] data1 = new byte[len31];
                DDEML.DdeGetData(Data5, data1, len31, 0);
                String dataString1 = System.Text.Encoding.Default.GetString(data1);
               

                // Grab frequency and mode from rig and sync PowerSDR
                // The external radio is the master and powersdr is the the slave
                // TODO: Turn code to poll DDE connection on a topic into a method

                // Frequency 
                topic5 = DDEML.DdeCreateStringHandle(pidInst, Marshal.StringToBSTR(rigControl.getHertz()), DDEML.CP_WINUNICODE);
                Data5 = DDEML.DdeClientTransaction(null, 0, hConv, topic5, DDEML.CF_TEXT, DDEML.XTYP_REQUEST, 1000, ref pwdResult);
                len31 = DDEML.DdeGetData(Data5, null, 0, 0) + 1;
                data1 = new byte[len31];
                DDEML.DdeGetData(Data5, data1, len31, 0);
                dataString1 = System.Text.Encoding.Default.GetString(data1);
                processFrequency(dataString1);

                // Mode 
                topic5 = DDEML.DdeCreateStringHandle(pidInst, Marshal.StringToBSTR(rigControl.getMode()), DDEML.CP_WINUNICODE);
                Data5 = DDEML.DdeClientTransaction(null, 0, hConv, topic5, DDEML.CF_TEXT, DDEML.XTYP_REQUEST, 1000, ref pwdResult);
                len31 = DDEML.DdeGetData(Data5, null, 0, 0) + 1;
                data1 = new byte[len31];
                DDEML.DdeGetData(Data5, data1, len31, 0);
                dataString1 = System.Text.Encoding.Default.GetString(data1);
                processMode(dataString1);


                // Hertz_B  (VFO) Setup hot advise 
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
            this.rigPollingLockoutTimer.Interval = this.rigControl.getTimerValue(); 





        }


        protected string AddLeadingZeros(int n)
        {
            string num = n.ToString();

            while (num.Length < 11)
                num = num.Insert(0, "0");

            return num;
        }


        private void processFrequency(String frequency)
        {

            if (this.console.SetupForm.RttyOffsetEnabledA &&
                (this.console.RX1DSPMode == DSPMode.DIGU ||
                this.console.RX1DSPMode == DSPMode.DIGL))
            {
                int f = int.Parse(frequency);

                if (this.console.RX1DSPMode == DSPMode.DIGU)
                    f = f - Convert.ToInt32(console.SetupForm.RttyOffsetHigh);
                else if (console.RX1DSPMode == DSPMode.DIGL)
                    f = f + Convert.ToInt32(console.SetupForm.RttyOffsetLow);

                frequency = this.AddLeadingZeros(f);
            }


            double freq = double.Parse(frequency.Insert(5, separator));
            this.console.txtVFOAFreq.Text = freq.ToString("f6");
            this.console.txtVFOAFreq_LostFocus(this, new RigCATEventArgs());
           






            // TODO
       


            /*
            // If less than 1Mhz, fill in zeros
            String frequency2 = frequency.Trim('\0').PadLeft(7, '0');
            String freqMhz = frequency2.Substring(0, frequency2.Length - 6) + "." + frequency2.Substring(frequency2.Length - 6, 6);

            // If the PowerSDR VFOA doesn't equal the VFO of the Rig, update it
            if (!txtVFOAFreq.Text.Equals(freqMhz))
            {
                txtVFOAFreq.Text = freqMhz;
                // EventArgs signal this change comes from an external source
                txtVFOAFreq_LostFocus(this, new HRDEventArgs(HRDEventArgs.Source.External));
            }
             * 
             * */
        }

        private void processMode(String mode)
        {


            // TODO

            /*

            // Don't force a mode state change unless we need to
            if ((mode.StartsWith("LSB")) && (!radModeLSB.Checked)) radModeLSB.Checked = true;
            if ((mode.StartsWith("USB")) && (!radModeUSB.Checked)) radModeUSB.Checked = true;
            if ((mode.StartsWith("AM")) && (!radModeAM.Checked)) radModeAM.Checked = true;
            if ((mode.StartsWith("FM")) && (!radModeFMN.Checked)) radModeFMN.Checked = true;

            // Modes with IF Frequency not in the setup menu
            // LP Bridge will support these
            // TODO: Add IF Frequencies for these modes in setup menu
            // TODO: Mode names are not standard between radios. 

            if ((this.DIGMode.Equals("DIG-USB") && ((mode.StartsWith("FSK\0") || mode.StartsWith("DATA\0")) && (!radModeDIGU.Checked)))) radModeDIGU.Checked = true;
            if ((this.DIGMode.Equals("DIG-LSB") && ((mode.StartsWith("FSK\0") || mode.StartsWith("DATA\0")) && (!radModeDIGL.Checked)))) radModeDIGL.Checked = true;
            // Does the opposite mode of FSK

            if ((mode.StartsWith("FSK-R")) || (mode.StartsWith("DATA-R")))
            {
                if ((this.DIGMode.Equals("DIG-USB") && (!radModeDIGL.Checked))) radModeDIGL.Checked = true;
                if ((this.DIGMode.Equals("DIG-LSB") && (!radModeDIGU.Checked))) radModeDIGU.Checked = true;
            }

            // CW
            if ((this.CWMode.Equals("CW-USB") && mode.StartsWith("CW\0") && (!radModeCWU.Checked))) radModeCWU.Checked = true;
            if ((this.CWMode.Equals("CW-LSB") && mode.StartsWith("CW\0") && (!radModeCWL.Checked))) radModeCWL.Checked = true;
            // Does the opposite mode of CW
            if ((mode.StartsWith("CW-R")))
            {   // Do reverse of default mode
                if ((this.CWMode.Equals("CW-LSB") && (!radModeCWU.Checked))) radModeCWU.Checked = true;
                if ((this.CWMode.Equals("CW-USB") && (!radModeCWL.Checked))) radModeCWL.Checked = true;
            }

              */

        }



        public IntPtr HRDDdeCallback(Int32 uType, Int32 uFmt, IntPtr hconv, IntPtr hsz1, IntPtr hsz2, IntPtr hdata, IntPtr dwData1, IntPtr dwData2)
        {
            // CalibrateFreq(9.958015F);
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
                    if (item.ToString().Equals(rigControl.getTX()) && dataString.StartsWith("On"))
                    {
                        // TX ON - MUTE
                        // Save mute state - on TX off revert to this state
                        //this.chkMUT.CheckedChanged += new System.EventHandler(this.chkMUT_CheckedChanged);
                        // TODO muteCache = chkMUT.Checked;
                        // If the monitor isn't checked then mute 
                      //TODO  if (!chkMON.Checked)
                        {
                      //TODO      chkMUT.Checked = true;
                        }
                    }
                    else if (item.ToString().Equals(rigControl.getTX()))
                    {
                        // TX_OFF - UNMUTE - Unless mute was already on
                        // Revert to previous state
                       // TODO  chkMUT.Checked = muteCache;
                    }

                    ///////////////////////
                    // HRD_MODE Handling //
                    ///////////////////////

                    if ((item.ToString().Equals(rigControl.getMode())) && !(this.rigPollingLockout))
                    {

                        // System.Console.WriteLine("RECEIVED:  Mode: " + dataString);
                        processMode(dataString);

                    }
                    else if ((item.ToString().Equals(rigControl.getMode())))
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



                    if (item.ToString().Equals(rigControl.getHertz()) && !(this.rigPollingLockout))
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

    }
}
