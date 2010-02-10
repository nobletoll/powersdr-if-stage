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

		public ASCIIEncoding AE = new ASCIIEncoding();
		protected SDRSerialPort SIO;

		protected bool active = false;
		private bool connected = false;

		// Serial Read Handler
		protected byte[] commBuffer = null;


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
				if (this.active || this.connected)
					return;

				if (this.SIO == null)
				{
					this.SIO = new SDRSerialPort(this.hw.COMPort);

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

				this.active = true;
				this.connected = true;
			}
		}

		public virtual void disconnect()
		{
			lock (this)
			{
				if (!this.active || !this.connected)
					return;

				this.active = false;

				if (this.SIO != null && this.SIO.PortIsOpen)
				{
					MeterHW.dbgWriteLine("Meter.disconnect(), Deregistering COM" +
						this.hw.COMPort + " Handlers...");

					this.SIO.deregisterEventHandlers();
					this.SIO.serial_rx_event -=
						new SerialRXEventHandler(this.SerialRXEventHandler);

					/* :Issue 67: When the app is closing, we don't want to
					 *            bother closing the serial ports.
					 *                    
					 *            This will reduce the likelyhood of a hang
					 *            on Serial close.  Let the OS clean up the
					 *            open handles.
					 *            
					 *            If we do have to close the serial port, do
					 *            it in a separate thread so we don't block
					 *            the main UI thread.
					 */
					if (!this.console.ConsoleClosing)
						new Thread(new ThreadStart(this.destroySIO)).Start();
				}
			}
		}

		private void destroySIO()
		{
			MeterHW.dbgWriteLine("Meter.destroySIO(), Closing COM" +
				this.hw.COMPort + "...");

			this.SIO.Destroy();
			this.SIO = null;
			this.connected = false;

			MeterHW.dbgWriteLine("Meter.destroySIO(), Closed COM" +
				this.hw.COMPort + ".");
		}

		#endregion Initialization


		#region Meter Status

		protected bool mox = false;
		public bool MOX
		{
			get { return this.mox;  }
		}

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

		public abstract void getMeterInformation();

		#endregion Get Commands


		#region Set Commands

		public abstract void startDataReporting();
		public abstract void stopDataReporting();

		#endregion Set Commands


		#region Answer Commands

		public abstract void handleMeterAnswer(byte[] answer);

		#endregion Answer Commands

	
		#region Event Handlers

		public abstract void SerialRXEventHandler(object source,SerialRXEvent e);

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
