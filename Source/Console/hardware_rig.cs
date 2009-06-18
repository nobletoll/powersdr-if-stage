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

using System.IO.Ports;
using SDRSerialSupportII;
using System.Diagnostics;


namespace PowerSDR
{
	public class RigHW : AbstractHW
	{
		#region Variable Declarations

		private Console console;
		private Rig rig;

		public enum Mode
		{
			LSB = 1,
			USB = 2,
			CW = 3,
			FM = 4,
			AM = 5,
			FSK = 6,
		}

		public enum RigTypeEnum
		{
			Kenwood,
			K3
		}

		#endregion Variable Declarations


		#region Constructor

		public RigHW(Console console)
		{
			this.console = console;
		}

		#endregion Constructor


		#region SetupIF Settings

		private string rigType;
		public string RigType
		{
			get { return this.rigType; }
			set
			{
				this.rigType = value;

				// :NOTE: We can only reassign the Rig if the Power is off.
				// :TODO: See if we can force a power off to do this.
				if (!this.console.PowerOn)
				{
					switch (RigHW.getRigType(this.rigType))
					{
						case RigTypeEnum.Kenwood:
							this.rig = new KenwoodRig(this,this.console);
							break;
						case RigTypeEnum.K3:
							this.rig = new K3Rig(this,this.console);
							break;
					}
				}
			}
		}

		private int com_port;
		public int COMPort
		{
			get { return com_port; }
			set { com_port = value; }
		}

		private Parity com_parity;
		public Parity COMParity
		{
			set { com_parity = value; }
			get { return com_parity; }
		}

		private StopBits com_stop_bits;
		public StopBits COMStopBits
		{
			set { com_stop_bits = value; }
			get { return com_stop_bits; }
		}
		private SDRSerialPort.DataBits com_data_bits;
		public SDRSerialPort.DataBits COMDataBits
		{
			set { com_data_bits = value; }
			get { return com_data_bits; }
		}

		private int com_baud_rate;
		public int COMBaudRate
		{
			set { com_baud_rate = value; }
			get { return com_baud_rate; }
		}


		private int rigPollingInterval = 200;
		public int RigPollingInterval
		{
			get { return this.rigPollingInterval; }
			set { this.rigPollingInterval = value; }
		}

		private int rigTuningPollingInterval = 50;
		public int RigTuningPollingInterval
		{
			get { return this.rigTuningPollingInterval; }
			set { this.rigTuningPollingInterval = value; }
		}

		private int rigPollingLockoutTime = 2000;
		public int RigPollingLockoutTime
		{
			get { return this.rigPollingLockoutTime; }
			set { this.rigPollingLockoutTime = value; }
		}


		public bool rigPollVFOB = false;
		public bool RigPollVFOB
		{
			get { return this.rigPollVFOB; }
			set { this.rigPollVFOB = value; }
		}

		public bool rigPollIFFreq = false;
		public bool RigPollIFFreq
		{
			get { return this.rigPollIFFreq; }
			set { this.rigPollIFFreq = value; }
		}

		#endregion SetupIF Settings


		#region Public Functions

		[Conditional("DEBUG")]
		public static void dbgWriteLine(string s)
		{
			System.Console.Error.WriteLine(s);
		}

		public override void Init()
		{
		}

		public override void StandBy()
		{
			if (this.rig != null)
				this.rig.disableSerialConnection();
		}

		public override void PowerOn()
		{
			if (this.rig != null)
				this.rig.enableSerialConnection();
		}

		public void setVFOAFreq(double freq)
		{
			if (this.rig != null)
				this.rig.setVFOAFreq(freq);
		}

		public void setVFOBFreq(double freq)
		{
			if (this.rig != null)
				this.rig.setVFOBFreq(freq);
		}

		public void setMode(Mode mode)
		{
			if (this.rig != null)
				this.rig.setMode((int) mode);
		}

		public void setSplit(bool splitOn)
		{
			if (this.rig != null)
				this.rig.setSplit(splitOn);
		}


		public int defaultBaudRate()
		{
			if (this.rig != null)
				return this.rig.defaultBaudRate();

			return 4800;
		}

		public bool needsPollVFOB()
		{
			if (this.rig != null)
				return this.rig.needsPollVFOB();

			return false;
		}

		public bool supportsIFFreq()
		{
			if (this.rig != null)
				return this.rig.supportsIFFreq();

			return false;
		}


		public static int getModeFromDSPMode(DSPMode dspMode)
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

		#endregion Public Functions


		#region Private Functions

		private static RigTypeEnum getRigType(string rigType)
		{
			switch (rigType)
			{
				case "Kenwood TS-940S":
					return RigTypeEnum.Kenwood;
				case "Elecraft K3":
					return RigTypeEnum.K3;
				default:
					return RigTypeEnum.Kenwood;
			}
		}

		#endregion Private Functions


		#region Overrided Public Functions

		public override void ignorePTT(bool v) { }

		public override byte StatusPort() { return 0; }

		public override void Impulse() { }

		public override byte PA_GetADC(int chan) { return 0; }

		public override bool PA_ATUTune(ATUTuneMode mode) { return false; }

		public override void SetDDSDAC(int level) { }

		#endregion Overrided Public Functions


		#region Overrided Configurations

		public override ushort LPTAddr
		{
			get { return 0; }
			set { }
		}

		public override bool XVTRPresent
		{
			set { }
		}

		public override bool PAPresent
		{
			set { }
		}

		public override bool ATUPresent
		{
			set { }
		}

		public override bool USBPresent
		{
			set { }
		}

		public override bool OzyControl
		{
			set { }
		}

		public override int PLLMult
		{
			set { }
		}

		#endregion Overrided Configurations


		#region Control

		// gets or sets the BPF Relay using an integer index
		public override BPFBand BPFRelay
		{
			set { }
		}

		// true means TX mode
		public override bool TransmitRelay
		{
			set { }
		}

		// true means the Mute Relay is engaged (muted)
		public override bool MuteRelay
		{
			set { }
		}

		// gets or sets the X2 pins 1-7
		public override byte X2
		{
			get { return 0; }
			set { }
		}

		// true means 0dB (40dB for old configs)
		public override bool GainRelay
		{
			set { }
		}

		// RFE only properties
		// returns an integer index into the LPF switches
		public override RFELPFBand RFE_LPF
		{
			set { }
		}

		public override bool RFE_TR
		{
			set { }
		}

		// true means the RF path is active to the XVTR
		public override bool XVTR_RF
		{
			get { return false; }
			set { }
		}

		// true means the TR relay on the xvtr is active
		public override bool XVTR_TR
		{
			set { }
		}

		// true means the 10dB attenuator is switched inline
		public override bool Attn
		{
			set { }
		}

		public override bool ImpulseEnable
		{
			set { }
		}

		public override bool PABias
		{
			set { }
		}

		public override PAFBand PA_LPF
		{
			set { }
		}

		public override bool PA_ADC_CLK
		{
			set { }
		}

		public override bool PA_ADC_DI
		{
			set { }
		}

		public override bool PA_ADC_CS_NOT
		{
			set { }
		}

		public override bool PA_TR_Relay
		{
			set { }
		}

		public override bool ATU_DI
		{
			set { }
		}

		public override long DDSTuningWord
		{
			get { return 0; }
			set { }
		}

		public override bool UpdateHardware
		{
			get { return false; }
			set { }
		}

		#endregion Control


		#region Test Functions

		public override void TestPIO1() { }

		public override void TestPIO2(bool evens) { }

		public override void TestPIO3() { }

		public override void TestRFEIC11() { }

		#endregion Test Functions
	}
}
