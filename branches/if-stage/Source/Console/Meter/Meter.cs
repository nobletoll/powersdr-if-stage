//=============================================================================
// Meter.cs
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

using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Timers;

using SDRSerialSupportII;


namespace PowerSDR
{
	public abstract class Meter
	{
		protected MeterHW hw;
		protected Console console;

		protected SDRSerialPort SIO;

		protected bool connected = false;

		// Serial Read Handler
		private byte[] commBuffer = null;


		protected Meter(MeterHW hw, Console console)
		{
			this.hw = hw;
			this.console = console;
		}


		#region Initialization

		public virtual void connect()
		{
			lock (this)
			{
				if (this.connected)
					return;

				if (this.SIO == null)
				{
					this.SIO = new SDRSerialPort(this.hw.COMPort);
					this.SIO.setRTS(false);

					// Event handler for Serial RX Events
					this.SIO.serial_rx_event +=
						new SerialRXEventHandler(this.SerialRXEventHandler);

					this.SIO.setCommParms(this.hw.COMBaudRate,this.hw.COMParity,
						this.hw.COMDataBits,this.hw.COMStopBits);

					MeterHW.dbgWriteLine("Meter.connect(), Opening COM" +
						this.hw.COMPort + "...");

					try
					{
						if (this.SIO.Create() == 0)
							MeterHW.dbgWriteLine("Meter.connect(), Opened COM" +
								this.hw.COMPort + ".");
						else
							throw new Exception();
					}
					catch (Exception ex)
					{
						// Event handler for Serial RX Events
						this.SIO.serial_rx_event -=
							new SerialRXEventHandler(this.SerialRXEventHandler);

						this.SIO = null;
						return;
					}
				}
				else
				{
					this.SIO.registerEventHandlers();

					// Event handler for Serial RX Events
					this.SIO.serial_rx_event +=
						new SerialRXEventHandler(this.SerialRXEventHandler);
				}

				this.connected = true;
			}
		}

		public virtual void disconnect()
		{
			lock (this)
			{
				if (!this.connected)
					return;

				this.connected = false;

				if (this.SIO != null && this.SIO.PortIsOpen)
				{
					MeterHW.dbgWriteLine("Meter.disconnect(), Closing COM" +
						this.hw.COMPort + "...");

					this.SIO.deregisterEventHandlers();
					this.SIO.serial_rx_event -=
						new SerialRXEventHandler(this.SerialRXEventHandler);

					this.SIO.Destroy();
					this.SIO = null;

					MeterHW.dbgWriteLine("Meter.disconnect(), Closed COM" +
						this.hw.COMPort + ".");
				}
			}
		}

		#endregion Initialization


		#region Meter Status

		protected bool mox = false;

		private double fwdPower = 0;
		public double FwdPower
		{
			get { return this.fwdPower; }
			set { this.fwdPower = value; }
		}

		private double revPower = 0;
		public double RevPower
		{
			get { return this.revPower; }
			set { this.revPower = value; }
		}

		private double vswr = 0;
		public double VSWR
		{
			get { return this.vswr; }
			set { this.vswr = value; }
		}

		#endregion Meter Status


		#region Defaults & Supported Functions

		public abstract int defaultBaudRate();

		#endregion Defaults & Supported Functions


		#region Get Commands

		#endregion Get Commands


		#region Set Commands

		public abstract void startDataReporting();
		public abstract void stopDataReporting();

		#endregion Set Commands


		#region Answer Commands

		public abstract void handleMeterAnswer(byte[] answer);

		#endregion Answer Commands

	
		#region Event Handlers

		public void SerialRXEventHandler(object source, SerialRXEvent e)
		{
			// Resize the commBuffer as needed...
			int commBufferLen = 0;
			if (this.commBuffer != null)
			{
				commBufferLen = this.commBuffer.Length;
				Array.Resize<byte>(ref this.commBuffer,commBufferLen + e.buffer.Length);
			}
			else
				this.commBuffer = new byte[e.buffer.Length];

			// Append the incoming buffer into commBuffer
			Array.Copy(e.buffer,0,this.commBuffer,commBufferLen,e.buffer.Length);

			// Find each <CR>
			int iCR;
			int iStart = 0;
			bool leftovers = false;
			for (iCR = 0; iCR < e.buffer.Length; iCR++)
			{
				leftovers = true;

				if (e.buffer[iCR] == '\r')
				{
					byte[] answer = new byte[iCR - iStart];
					Array.Copy(e.buffer,iStart,answer,0,iCR - iStart);

					MeterHW.dbgWriteLine("<-- " + Meter.PrintBuffer(answer));

					this.handleMeterAnswer(answer);

					iStart = iCR + 1;
					leftovers = false;
				}
			}

			// Save the left over data for next read...
			if (leftovers)
			{
				this.commBuffer = new byte[iCR - iStart];
				Array.Copy(e.buffer,iStart,this.commBuffer,0,iCR - iStart);
			}
		}

		public static string PrintBuffer(byte[] buffer)
		{
			string s = "";

			foreach (byte b in buffer)
			{
				if (b >= 33 && b <= 126)
					s += (char) b + " ";
				else
					s += Convert.ToString(b,16).PadLeft(2,'0') + ' ';
			}

			return s;
		}

		#endregion Event Handlers
	}
}
