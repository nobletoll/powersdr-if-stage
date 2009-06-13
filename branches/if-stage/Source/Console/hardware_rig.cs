//=============================================================================
// hardware_rig.cs
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

namespace PowerSDR
{
	public class RigHW : AbstractHW
	{
		#region Variable Declarations

		private Console console;
		private RigSerialPoller rigSerialPoller;

		public enum Mode
		{
			LSB = 1,
			USB = 2,
			CW = 3,
			FM = 4,
			AM = 5,
			FSK = 6,
		}

		#endregion Variable Declarations


		#region Constructor

		public RigHW(Console console)
		{
			this.console = console;
		}

		#endregion Constructor

	
		#region Configurations

		public override ushort LPTAddr
		{
			get { return 0; }
			set {}
		}

		public override bool XVTRPresent
		{
			set {}
		}

		public override bool PAPresent
		{
			set {}
		}

		public override bool ATUPresent
		{
			set {}
		}

		public override bool USBPresent
		{
			set {}
		}

		public override bool OzyControl
		{
			set {}
		}

		public override int PLLMult
		{
			set {}
		}

		#endregion Configurations


		#region Control

		// gets or sets the BPF Relay using an integer index
		public override BPFBand BPFRelay
		{
			set {}
		}

		// true means TX mode
		public override bool TransmitRelay
		{
			set {}
		}

		// true means the Mute Relay is engaged (muted)
		public override bool MuteRelay
		{
			set {}
		}

		// gets or sets the X2 pins 1-7
		public override byte X2
		{
			get { return 0; }
			set {}
		}

		// true means 0dB (40dB for old configs)
		public override bool GainRelay
		{
			set {}
		}

		// RFE only properties
		// returns an integer index into the LPF switches
		public override RFELPFBand RFE_LPF
		{
			set {}
		}

		public override bool RFE_TR
		{
			set {}
		}

		// true means the RF path is active to the XVTR
		public override bool XVTR_RF
		{
			get { return false; }
			set {}
		}

		// true means the TR relay on the xvtr is active
		public override bool XVTR_TR
		{
			set {}
		}

		// true means the 10dB attenuator is switched inline
		public override bool Attn
		{
			set {}
		}

		public override bool ImpulseEnable
		{
			set {}
		}

		public override bool PABias
		{
			set {}
		}

		public override PAFBand PA_LPF
		{
			set {}
		}

		public override bool PA_ADC_CLK
		{
			set {}
		}

		public override bool PA_ADC_DI
		{
			set {}
		}

		public override bool PA_ADC_CS_NOT
		{
			set {}
		}

		public override bool PA_TR_Relay
		{
			set {}
		}

		public override bool ATU_DI
		{
			set {}
		}

		public override long DDSTuningWord
		{
			get { return 0; }
			set { }
		}

		public override bool UpdateHardware
		{
			get { return false; }
			set {}
		}

		#endregion Control


		#region Public Functions

		public override void Init()
		{
			this.rigSerialPoller = new RigSerialPoller(this.console);
		}

		public override void ignorePTT(bool v) { }

		public override void StandBy()
		{
			this.rigSerialPoller.disableCAT();
		}

		public override void PowerOn()
		{
			this.rigSerialPoller.enableCAT();
		}

		public override byte StatusPort() { return 0; }

		public override void Impulse() { }

		public override byte PA_GetADC(int chan) { return 0; }

		public override bool PA_ATUTune(ATUTuneMode mode) { return false; }

		public override void SetDDSDAC(int level) { }

		public void updateVFOAFrequency(double freq)
		{
			this.rigSerialPoller.updateVFOAFrequency(freq);
		}

		public void updateVFOBFrequency(double freq)
		{
			this.rigSerialPoller.updateVFOBFrequency(freq);
		}

		public void setMode(Mode mode)
		{
			this.rigSerialPoller.setMode((int) mode);
		}

        public void setSplit(bool splitOn)
        {
            this.rigSerialPoller.setSplit(splitOn);
        }

        #endregion Public Functions


		#region Test Functions

		public override void TestPIO1() { }

		public override void TestPIO2(bool evens) { }

		public override void TestPIO3() { }

		public override void TestRFEIC11() { }

		#endregion Test Functions
	}
}
