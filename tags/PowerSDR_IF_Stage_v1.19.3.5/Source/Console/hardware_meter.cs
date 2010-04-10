//=============================================================================
// hardware_meter.cs
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
#define DEBUG_METER

using System.IO.Ports;
using SDRSerialSupportII;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;


namespace PowerSDR
{
	public class MeterHW
	{
		#region Variable Declarations

		private Console console;
		private Meter meter;

		public enum MeterTypeEnum
		{
			PowerMaster,
			Alpha8100,
		}

		#endregion Variable Declarations


		#region Constructor

		public MeterHW(Console console)
		{
			this.console = console;
		}

		#endregion Constructor


		#region SetupIF Settings

		public bool UseMeter
		{
			get { return (this.meter != null); }
			set
			{
				if (value)
					this.createMeter();
				else
					this.destroyMeter();
			}
		}

		private string meterType = "";
		public string MeterType
		{
			get { return this.meterType; }
			set { this.meterType = value; }
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

		private int meterTimingInterval = 100;
		public int MeterTimingInterval
		{
			get { return this.meterTimingInterval; }
			set { this.meterTimingInterval = value; }
		}

		#endregion SetupIF Settings


		#region Public Functions

		public void logGeneral(string msg)
		{
			if (this.console.SetupIFForm != null)
				this.console.SetupIFForm.logGeneral(msg);
		}

		public void logIncomingCAT(string msg)
		{
			if (this.console.SetupIFForm != null)
				this.console.SetupIFForm.logIncomingCAT(msg);
		}

		public void logOutgoingCAT(string msg)
		{
			if (this.console.SetupIFForm != null)
				this.console.SetupIFForm.logOutgoingCAT(msg);
		}

		public void StandBy()
		{
			if (this.meter != null)
				this.meter.disconnect();
		}

		public void PowerOn()
		{
			if (this.meter != null)
				this.meter.connect();
		}

		public void MOX(bool on)
		{
			if (this.meter == null)
				return;

			if (on)
				this.meter.startDataReporting();
			else
				this.meter.stopDataReporting();
		}

		public double FwdPower
		{
			get { return (this.meter != null) ? this.meter.FwdPower : 0.0; }
		}

		public double RevPower
		{
			get { return (this.meter != null) ? this.meter.RevPower : 0.0; }
		}

		public double VSWR
		{
			get { return (this.meter != null) ? this.meter.VSWR : 0.0; }
		}

		public int defaultBaudRate()
		{
			if (this.meter != null)
				return this.meter.defaultBaudRate();

			return 38400;
		}

		#endregion Public Functions


		#region Paint Meter

		public int PaintMeter(Graphics g, int meterWidth, int meterHeight, double power)
		{
			int scale = 100;

			// :NOTE: PaintMeter will work with scales evenly divisible by 4

			if (power <= 6)
				scale = 4;
			else if (power <= 60)
				scale = 40;
			else if (power <= 150)
				scale = 100;
			else if (power <= 300)
				scale = 200;
			else if (power <= 600)
				scale = 400;
			else if (power <= 1500)
				scale = 1200;
			else if (power <= 2000)
				scale = 1600;
			else
				scale = 2400;

			return this.PaintMeter(g,meterWidth,meterHeight,power,scale);
		}

		public int PaintMeter(Graphics g, int meterWidth, int meterHeight,
			double power, int scale)
		{
			SolidBrush low_brush = new SolidBrush(this.console.EdgeLowColor);
			SolidBrush high_brush = new SolidBrush(this.console.EdgeHighColor);
			Font f = new Font("Arial",7.0f,FontStyle.Bold);

			// Horizontal Line (with red end)
			g.FillRectangle(low_brush,0,meterHeight - 4,(int) (meterWidth * 0.75),2);
			g.FillRectangle(high_brush,(int) (meterWidth * 0.75),
				meterHeight - 4,(int) (meterWidth * 0.25) - 10,2);

			double step = scale / 4;
			double spacing = (meterWidth * 0.75 - 2.0) / 4.0;
			string[] list = { step.ToString(), (step * 2).ToString(),
				(step * 3).ToString(), scale.ToString() };

			// White Vertical Lines
			for (int i = 1; i < 5; i++)
			{
				// Short White Vertical
				g.FillRectangle(low_brush,(int) (i * spacing - spacing * 0.5),
					meterHeight - 4 - 3,1,3);

				// Tall White Vertical
				g.FillRectangle(low_brush,(int) (i * spacing),meterHeight - 4 - 6,2,6);

				string label = list[i - 1];
				SizeF size = g.MeasureString("0",f,1,StringFormat.GenericTypographic);
				double string_width = size.Width - 2.0;
				double string_height = size.Height - 2.0;

				// Label
				g.TextRenderingHint = TextRenderingHint.SystemDefault;
				g.DrawString(label,f,low_brush,
					(int) (i * spacing - string_width * label.Length + (int) (i / 3) + (int) (i / 4)),
					(int) (meterHeight - 4 - 8 - string_height));
			}


			// Red Vertical Lines
			spacing = (meterWidth * 0.25 - 2.0 - 10.0) / 1.0;
			for (int i = 1; i < 2; i++)
			{
				// Short Red Vertical
				g.FillRectangle(high_brush,(int) ((double) meterWidth * 0.75 + i * spacing - spacing * 0.5),
					meterHeight - 4 - 3,1,3);

				// Tall Red Vertical
				g.FillRectangle(high_brush,(int) ((double) meterWidth * 0.75 + i * spacing),
					meterHeight - 4 - 6,2,6);

				SizeF size = g.MeasureString("0",f,3,StringFormat.GenericTypographic);
				double string_width = size.Width - 2.0;
				double string_height = size.Height - 2.0;

				g.TextRenderingHint = TextRenderingHint.SystemDefault;
				g.DrawString((scale+step).ToString() + '+',f,high_brush,
					(int) (meterWidth * 0.75 + i * spacing - (int) 3.5 * string_width),
					(int) (meterHeight - 4 - 8 - string_height));
			}


			// Determine X Position for Needle
			int pixel_x;
			if (power <= scale)  // Low (White) Area
			{
				spacing = (meterWidth * 0.75 - 2.0) / 4.0;

				if (power <= step)
					pixel_x = (int) (power / step * (int) spacing);
				else if (power <= (step * 2))
					pixel_x = (int) (spacing + (power - step) / step * spacing);
				else if (power <= (step * 3))
					pixel_x = (int) (2 * spacing + (power - (step * 2)) / step * spacing);
				else
					pixel_x = (int) (3 * spacing + (power - (step * 3)) / step * spacing);
			}
			else  // High (Red) Area
			{
				spacing = (meterWidth * 0.25 - 2.0 - 10.0) / 1.0;

				if (power <= scale + step)
					pixel_x = (int) (meterWidth * 0.75 + (power - scale) / step * spacing);
				else
					pixel_x = (int) (meterWidth * 0.75 + spacing + (power - scale + step) / step * spacing);
			}

			return pixel_x;
		}

		#endregion Paint Meter


		#region Private Functions

		private void createMeter()
		{
			// :NOTE: We can only reassign the Meter if the Power is off.
			// :TODO: See if we can force a power off to do this.
			if (!this.console.PowerOn)
			{
				switch (MeterHW.getMeterType(this.meterType))
				{
					case MeterTypeEnum.PowerMaster:
						this.meter = new PowerMasterMeter(this,this.console);
						break;
					default:
						this.meter = null;
						break;
				}
			}
		}

		private void destroyMeter()
		{
			// :NOTE: We can only reassign the Meter if the Power is off.
			// :TODO: See if we can force a power off to do this.
			if (!this.console.PowerOn)
				this.meter = null;
		}

		private static MeterTypeEnum getMeterType(string meterType)
		{
			switch (meterType)
			{
				case "Array Solutions PowerMaster":
					return MeterTypeEnum.PowerMaster;
				case "Alpha 8100":
					return MeterTypeEnum.Alpha8100;
				default:
					return MeterTypeEnum.PowerMaster;
			}
		}

		#endregion Private Functions
	}
}
