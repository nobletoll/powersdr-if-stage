//=============================================================================
// hardware.cs
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
	public abstract class AbstractHW
	{
		#region Configurations

		public abstract ushort LPTAddr
		{
			get;
			set;
		}

		public abstract bool XVTRPresent
		{
			set;
		}

		public abstract bool PAPresent
		{
			set;
		}

		public abstract bool ATUPresent
		{
			set;
		}

		public abstract bool USBPresent 
		{
			set;
		}

		public abstract bool OzyControl 
		{			
			set;
		}

		public abstract int PLLMult
		{
			set;
		}

		#endregion Configurations


		#region Control

		// gets or sets the BPF Relay using an integer index
		public abstract BPFBand BPFRelay
		{
			set;
		}
		
		// true means TX mode
		public abstract bool TransmitRelay
		{
			set;
		}

		// true means the Mute Relay is engaged (muted)
		public abstract bool MuteRelay
		{
			set;
		}

		// gets or sets the X2 pins 1-7
		public abstract byte X2
		{
			get;
			set;
		}

		// true means 0dB (40dB for old configs)
		public abstract bool GainRelay
		{
			set;
		}

		// RFE only properties
		// returns an integer index into the LPF switches
		public abstract RFELPFBand RFE_LPF
		{
			set;
		}

		public abstract bool RFE_TR
		{
			set;
		}

		// true means the RF path is active to the XVTR
		public abstract bool XVTR_RF
		{
			get;
			set;
		}

		// true means the TR relay on the xvtr is active
		public abstract bool XVTR_TR
		{
			set;
		}

		// true means the 10dB attenuator is switched inline
		public abstract bool Attn
		{
			set;
		}

		public abstract bool ImpulseEnable
		{
			set;
		}

		public abstract bool PABias
		{
			set;
		}

		public abstract PAFBand PA_LPF
		{
			set;
		}

		public abstract bool PA_ADC_CLK
		{
			set;
		}

		public abstract bool PA_ADC_DI
		{
			set;
		}

		public abstract bool PA_ADC_CS_NOT
		{
			set;
		}

		public abstract bool PA_TR_Relay
		{
			set;
		}

		public abstract bool ATU_DI
		{
			set;
		}

		public abstract long DDSTuningWord
		{
			get;
			set;
		}

		public abstract bool UpdateHardware
		{
			get;
			set;
		}

		#endregion Control


		#region Public Functions

		public abstract void Init();

		public abstract void StandBy();

		public abstract void PowerOn();

		public abstract byte StatusPort();

		public abstract void Impulse();
	
		public abstract byte PA_GetADC(int chan);

		public abstract bool PA_ATUTune(ATUTuneMode mode);

		public abstract void SetDDSDAC(int level);

		#endregion Public Functions


		#region Test Functions

		public abstract void TestPIO1();

		public abstract void TestPIO2(bool evens);

		public abstract void TestPIO3();

		public abstract void TestRFEIC11();

		#endregion Test Functions
	}
}
