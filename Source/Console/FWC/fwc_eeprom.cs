//=================================================================
// fwc_eeprom.cs
//=================================================================
// PowerSDR is a C# implementation of a Software Defined Radio.
// Copyright (C) 2004-2011  FlexRadio Systems
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
// You may contact us via email at: gpl@flexradio.com.
// Paper mail may be sent to: 
//    FlexRadio Systems
//    4616 W. Howard Lane  Suite 1-150
//    Austin, TX 78728
//    USA
//=================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PowerSDR
{
	public class FWCEEPROM
	{
		#region Misc Routines

		public static void Init()
		{
            int val = 0;
			if(model == 99) // keep from reading the model more than once
                val = FWC.GetModel(out model);
            if (val == -1) return;

            FWC.GetSerialNum(out serial_number); Thread.Sleep(10);// get serial number
            FWC.GetTRXOK(out trx_ok); Thread.Sleep(10);
            if (trx_ok) FWC.GetTRXSN(out trx_serial); Thread.Sleep(10);
            if (trx_ok) FWC.GetTRXRev(out trx_rev); Thread.Sleep(10);

            FWC.GetPAOK(out pa_ok); Thread.Sleep(10);
            if (pa_ok) FWC.GetPASN(out pa_serial); Thread.Sleep(10);
            if (pa_ok) FWC.GetPARev(out pa_rev); Thread.Sleep(10);
            
			switch(model)
			{
				case 0:
				case 1:
				case 2:
                    FWC.GetRFIOOK(out rfio_ok); Thread.Sleep(10);
                    if (rfio_ok) FWC.GetRFIOSN(out rfio_serial); Thread.Sleep(10);
                    if (rfio_ok) FWC.GetRFIORev(out rfio_rev); Thread.Sleep(10);
                    FWC.GetRX2OK(out rx2_ok); Thread.Sleep(10);
                    if (rx2_ok) FWC.GetRX2SN(out rx2_serial); Thread.Sleep(10);
                    if (rx2_ok) FWC.GetRX2Rev(out rx2_rev); Thread.Sleep(10);
                    FWC.GetATUOK(out atu_ok); Thread.Sleep(10);
                    if (atu_ok) FWC.GetATURev(out atu_rev); Thread.Sleep(10);
                    if (atu_ok) FWC.GetATUSN(out atu_serial); Thread.Sleep(10);
                    if (atu_ok) FWCATU.FlushBuffer(false);
                    FWC.GetVUOK(out vu_ok); Thread.Sleep(10);
                    if (vu_ok) FWC.GetVUSN(out vu_serial); Thread.Sleep(10);
                    if (vu_ok) FWC.GetVURev(out vu_rev); Thread.Sleep(10);
					break;
				case 3:
					atu_ok = true;
					break;
			}
			
			uint data; // get last cal date/time
			FWC.ReadTRXEEPROMUint(0x40, out data);
			last_cal_date_time = (long)data<<32;
			FWC.ReadTRXEEPROMUint(0x44, out data);
			last_cal_date_time += data;

			byte temp;
			FWC.ReadTRXEEPROMByte(0x3F, out temp);
			trx_checksum_present = (temp == 0xC5);

			FWC.ReadTRXEEPROMByte(0x1BE, out temp);
			rx1_image_ver = temp;

			if(rx2_ok)
			{
				FWC.ReadRX2EEPROMUint(0x40, out data);
				last_rx2_cal_date_time = (long)data<<32;
				FWC.ReadRX2EEPROMUint(0x44, out data);
				last_rx2_cal_date_time += data;

				FWC.ReadRX2EEPROMByte(0x3F, out temp);
				rx2_checksum_present = (temp == 0xC5);

				FWC.ReadRX2EEPROMByte(0x1BE, out temp);
				rx2_image_ver = temp;
			}

            //FWC.ReadTRXEEPROMUint(0x10, out data);
            FWC.GetATURev(out atu_rev);
            byte rev = ((byte)(ATURev >> 8));
            if (!(rev >= 1 && rev <= 0xFE))  //if old model
            {
                FWC.old_atu = true;     //old_firmware implies old revision
            }

            if (rev >= 1 && rev <= 0xFE)  //if new model
            {
                FWC.old_atu = false;
            }

			FWC.GetRegion(out region);
            if (region >= FRSRegion.LAST)
                region = FRSRegion.US;
		}

		public static bool NeedDump()
		{
			if(File.Exists(Application.StartupPath+"\\nobackup")) return false;  // for production
			uint data;
			if(model == 99) // keep from reading the model more than once
				FWC.GetModel(out model);
			StringBuilder s = new StringBuilder("");
			switch(model)
			{
				case 0: // 5000A
				case 1: // 5000C
					s.Append("F5K_");
					break;
				case 3: // 3000
					s.Append("F3K_");
					break;
			}
			FWC.ReadTRXEEPROMUint(0x18, out data);
			s.Append(((byte)(data)).ToString("00"));
			s.Append(((byte)(data>>8)).ToString("00"));
			s.Append("-"+((ushort)(data>>16)).ToString("0000"));
			if(File.Exists(app_data_path+"Backup\\"+s+" backup.csv"))
				return false;
			return true;
		}

		private static Progress progress;
		public static void StartDump()
		{
			progress = new Progress("Backing Up EEPROM");
            progress.PercentDigits = 0;
			Thread t = new Thread(new ThreadStart(Dump));
			t.Name = "EEPROM Dump Thread";
			t.IsBackground = true;
			t.Priority = ThreadPriority.Normal;
			t.Start();
			progress.Show();
			while(t.IsAlive)
			{
				Thread.Sleep(50);
				Application.DoEvents();
			}
		}

		private static void Dump()
		{
			if(File.Exists(Application.StartupPath+"\\nobackup")) return;  // for production
			uint data, count=0;
			if(model == 99) // keep from reading the model more than once
				FWC.GetModel(out model);
			StringBuilder s = new StringBuilder("");
			switch(model)
			{
				case 0:
				case 1:
					s.Append("F5K_");
					break;
				case 3:
					s.Append("F3K_");
					break;
			}
			FWC.ReadTRXEEPROMUint(0x18, out data);
			s.Append(((byte)(data)).ToString("00"));
			s.Append(((byte)(data>>8)).ToString("00"));
			s.Append("-"+((ushort)(data>>16)).ToString("0000"));
            if(!Directory.Exists(app_data_path+"\\Backup\\"))
                Directory.CreateDirectory(app_data_path + "\\Backup\\");
            if (File.Exists(app_data_path + "\\Backup\\" + s + " backup.csv"))
			{
				progress.Hide();
				return;
			}
			StreamWriter writer = new StreamWriter(app_data_path+"\\Backup\\"+s+" backup.csv");

			s = new StringBuilder(",");
			for(int i=0; i<16; i++)
				s.Append(i.ToString("X")+",");
			writer.WriteLine(s);
			for(int i=0; i<0xC1; i++)
			{
				s = new StringBuilder(i.ToString("X")+",");
				for(int j=0; j<4; j++)
				{
					FWC.ReadTRXEEPROMUint((uint)(i*16+j*4), out data);
					//Thread.Sleep(40);
					for(int k=0; k<4; k++)
						s.Append(((byte)(data>>(k*8))).ToString("X")+",");
					progress.SetPercent(count++/772.0f);
				}
				writer.WriteLine(s);
			}

			writer.Close();
			progress.Close();
		}

		public static string SerialToString(uint serial)
		{
			string s = "";
			s += ((byte)(serial)).ToString("00");
			s += ((byte)(serial>>8)).ToString("00")+"-";
			s += ((ushort)(serial>>16)).ToString("0000");
			return s;
		}

		private static void WriteCalDateTime()
		{
			long l = DateTime.Now.ToFileTimeUtc();
			FWC.WriteTRXEEPROMUint(0x40, (uint)(l>>32));
			FWC.WriteTRXEEPROMUint(0x44, (uint)l);
			last_cal_date_time = l;
		}

		private static void WriteRX2CalDateTime()
		{
			long l = DateTime.Now.ToFileTimeUtc();
			FWC.WriteRX2EEPROMUint(0x40, (uint)(l>>32));
			FWC.WriteRX2EEPROMUint(0x44, (uint)l);
			last_rx2_cal_date_time = l;
		}

		public static bool CheckAll()
		{
			bool b = true;
			b = CheckRXLevel(); if(!b) return b;
			b = CheckRXImage(); if(!b) return b;
			b = CheckTXImage(); if(!b) return b;
			b = CheckTXCarrier(); if(!b) return b;
			b = CheckPABias(); if(!b) return b;
			b = CheckPABridge(); if(!b) return b;
			b = CheckPAPower(); if(!b) return b;
			b = CheckPASWR(); if(!b) return b;

			if(rx2_ok)
			{
				b = CheckRX2Level(); if(!b) return b;
				b = CheckRX2Image(); if(!b) return b;
			}
			return b;
		}

        public static void TestVitaFreq()
        {
            Random r = new Random();

            for (int i = 0; i < 1e6; i++)
            {
                double whole = r.Next(8789999);
                double frac = 0.0;// r.NextDouble();

                double d = whole + frac;

                ulong v = ToVitaFreq(d);

                double test = FromVitaFreq(v);

                Debug.Assert((ulong)Math.Floor(d*1e6) == (ulong)Math.Floor(test*1e6));
            }
        }

        public static ulong ToVitaFreq(double freq_mhz)
        {
            return (ulong)(freq_mhz * 1e6 * Math.Pow(2, 20));
        }

        public static double FromVitaFreq(ulong vita)
        {
            return vita * Math.Pow(2, -20) * 1e-6;
        }

        private static bool CheckedWriteUint(uint addr, uint data)
        {
            uint temp = 0;
            int error_count = 0;
            do
            {
                FWC.WriteTRXEEPROMUint(addr, data);
                FWC.ReadTRXEEPROMUint(addr, out temp);

                if (temp != data)
                {
                    if (error_count++ > NUM_WRITES_TO_TRY)
                        return false;
                }
            } while (temp != data);

            return true;
        }

        private static bool CheckedWriteUshort(uint addr, ushort data)
        {
            ushort temp = 0;
            int error_count = 0;
            do
            {
                FWC.WriteTRXEEPROMUshort(addr, data);
                FWC.ReadTRXEEPROMUshort(addr, out temp);

                if (temp != data)
                {
                    if (error_count++ > NUM_WRITES_TO_TRY)
                        return false;
                }
            } while (temp != data);

            return true;
        }

        private static bool CheckedWriteByte(uint addr, byte data)
        {
            byte temp = 0;
            int error_count = 0;
            do
            {
                FWC.WriteTRXEEPROMByte(addr, data);
                FWC.ReadTRXEEPROMByte(addr, out temp);

                if (temp != data)
                {
                    if (error_count++ > NUM_WRITES_TO_TRY)
                        return false;
                }
            } while (temp != data);

            return true;
        }

		#endregion

		#region Properties

        private const uint END_OF_TABLE_FLAG = 0xDEADBEEF;

        private static string app_data_path = "";
        public static string AppDataPath
        {
            set { app_data_path = value; }
        }

		private static uint model = 99;
		public static uint Model
		{
			get { return model; }
		}

		private static FRSRegion region = FRSRegion.US;
		public static FRSRegion Region
		{ 
			get { return region; }
		}

		private static long last_cal_date_time = 0;
		public static long LastCalDateTime
		{
			get { return last_cal_date_time; }
			set { last_cal_date_time = value; }
		}

		private static long last_rx2_cal_date_time = 0;
		public static long LastRX2CalDateTime
		{
			get { return last_rx2_cal_date_time; }
			set { last_rx2_cal_date_time = value; }
		}

		private static uint serial_number;
		public static uint SerialNumber
		{
			get { return serial_number; }
			//set { serial_number = value; }
		}

		private static bool trx_ok;
		public static bool TRXOK
		{
			get { return trx_ok; }
		}

		private static uint trx_serial;
		public static uint TRXSerial
		{
			get { return trx_serial; }
			
		}

		private static uint trx_rev;
		public static uint TRXRev
		{
			get { return trx_rev; }
		}

        private static uint atu_serial;
        public static uint ATUSerial
        {
            get { return atu_serial; }
        }

        private static uint atu_rev;
        public static uint ATURev
        {
            get { return atu_rev; }
        }

		private static bool pa_ok;
		public static bool PAOK
		{
			get { return pa_ok; }
		}

		private static uint pa_serial;
		public static uint PASerial
		{
			get { return pa_serial; }
			//set { pa_serial = value; }
		}

		private static uint pa_rev;
		public static uint PARev
		{
			get { return pa_rev; }
			//set { pa_rev = value; }
		}

		private static bool rfio_ok;
		public static bool RFIOOK
		{
			get { return rfio_ok; }
		}

		private static uint rfio_serial;
		public static uint RFIOSerial
		{
			get { return rfio_serial; }
		}

		private static uint rfio_rev;
		public static uint RFIORev
		{
			get { return rfio_rev; }
		}

		private static bool atu_ok;
		public static bool ATUOK
		{
			get { return atu_ok; }
		}

		private static bool rx2_ok;
		public static bool RX2OK
		{
			get { return rx2_ok; }
		}

		private static uint rx2_serial;
		public static uint RX2Serial
		{
			get { return rx2_serial; }
		}

		private static uint rx2_rev;
		public static uint RX2Rev
		{
			get { return rx2_rev; }
		}

        private static bool vu_ok;
        public static bool VUOK
        {
            get { return vu_ok; }
        }

        private static uint vu_serial;
        public static uint VUSerial
        {
            get { return vu_serial; }
        }

        private static uint vu_rev;
        public static uint VURev
        {
            get { return vu_rev; }
        }

		private static bool trx_checksum_present;
		public static bool TRXChecksumPresent
		{
			get { return trx_checksum_present; }
			set
			{
				if(value) FWC.WriteTRXEEPROMByte(0x3F, 0xC5);
				else FWC.WriteTRXEEPROMByte(0x3F, 0xFF);
				trx_checksum_present = value;
			}
		}

		private static bool rx2_checksum_present;
		public static bool RX2ChecksumPresent
		{
			get { return rx2_checksum_present; }
			set
			{
				if(value) FWC.WriteRX2EEPROMByte(0x3F, 0xC5);
				else FWC.WriteRX2EEPROMByte(0x3F, 0xFF);
				rx2_checksum_present = value; 
			}
		}

		private static int rx1_image_ver = 0;
		public static int RX1ImageVer
		{
			get { return rx1_image_ver; }
		}

		private static int rx2_image_ver = 0;
		public static int RX2ImageVer
		{
			get { return rx2_image_ver; }
		}

		#endregion

		#region RX1

		#region RX Level

		private const int NUM_WRITES_TO_TRY = 5;
		public static bool CheckRXLevel()
		{
			Random rand = new Random();
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };

			float[][] rx_level_table = new float[(int)Band.LAST][];
			for(int i=0; i<(int)Band.LAST; i++)
				rx_level_table[i] = new float[3];

			for(int i=0; i<bands.Length; i++)
				for(int j=0; j<3; j++)
					rx_level_table[(int)bands[i]][j] = (float)rand.NextDouble();

			byte temp;
			WriteRXLevel(rx_level_table, out temp);

			float[][] rx_level_check = new float[(int)Band.LAST][];
			for(int i=0; i<(int)Band.LAST; i++)
				rx_level_check[i] = new float[3];
			
			ReadRXLevel(rx_level_check);

			for(int i=0; i<bands.Length; i++)
			{
				for(int j=0; j<3; j++)
				{
					if(rx_level_table[(int)bands[i]][j] != rx_level_check[(int)bands[i]][j])
						return false;
				}
			}
			return true;
		}

		public static void WriteRXLevel(float[][] table, out byte checksum)
		{
			WriteCalDateTime();			
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };

			uint addr = 0x48;
			ushort offset = 0x0100, temp;
			int error_count = 0;
			do
			{
				FWC.WriteTRXEEPROMUshort(addr, offset);
				//Thread.Sleep(10);
				FWC.ReadTRXEEPROMUshort(addr, out temp);
				//Thread.Sleep(40);

				if(temp != offset)
				{
					if(error_count++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing RX Level pointer to EEPROM.\n"+
							"Tried to write "+offset.ToString()+", but read back "+temp.ToString());
						checksum = 0xFF;
						return;
					}
				}
			} while(temp != offset);

			for(int i=0; i<bands.Length; i++)
			{
				for(int j=0; j<3; j++)
				{
					error_count = 0;
					float test = -99.99f;
					do
					{
						FWC.WriteTRXEEPROMFloat((uint)(offset+i*12+j*4), table[(int)bands[i]][j]);
						//Thread.Sleep(10);
						
						FWC.ReadTRXEEPROMFloat((uint)(offset+i*12+j*4), out test);
						//Thread.Sleep(40);
                        
						if(test != table[(int)bands[i]][j])
						{
							if(error_count++ > NUM_WRITES_TO_TRY)
							{
								MessageBox.Show("Error writing RX Level value to EEPROM.\n"+
									"Tried to write "+table[(int)bands[i]][j].ToString("f4")+", but read back "+test.ToString("f4"));
								checksum = 0xFF;
								return;
							}
						}
					} while(test != table[(int)bands[i]][j]);
				}
			}

			// calculate and write checksum
			byte sum = Checksum.Calc(table);
			WriteRXLevelChecksum(sum);
			checksum = sum;
		}

		public static void WriteRXLevelChecksum(byte sum)
		{
			byte read, errors=0;
			do
			{				
				FWC.WriteTRXEEPROMByte(0x18F, sum);
				//Thread.Sleep(10);

				FWC.ReadTRXEEPROMByte(0x18F, out read);
				if(read != sum)
				{
					if(errors++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing RX Level checksum to EEPROM.\n"+
							"Tried to write "+sum+", but read back "+read);
						return;
					}
				}
			} while(read != sum);
		}

		public static void ReadRXLevel(float[][] table)
		{
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };

			ushort offset;
			FWC.ReadTRXEEPROMUshort(0x48, out offset);
			//Thread.Sleep(40);
			if(offset == 0 || offset == 0xFFFF) return;

			uint data;
			FWC.ReadTRXEEPROMUint(offset, out data);
			//Thread.Sleep(40);
			if(data == 0xFFFFFFFF) return;

			for(int i=0; i<bands.Length; i++)
			{
				for(int j=0; j<3; j++)
				{
					float f;
					FWC.ReadTRXEEPROMFloat((uint)(offset+i*12+j*4), out f);
					//Thread.Sleep(40);
					table[(int)bands[i]][j] = (float)Math.Round(f, 3);
				}
			}
		}

		public static byte ReadRXLevelChecksum()
		{
			byte read;
			FWC.ReadTRXEEPROMByte(0x18F, out read);
			return read;
		}

		#endregion

		#region RX Image

		public static bool CheckRXImage()
		{
			Random rand = new Random();
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };

			float[] gain_table = new float[(int)Band.LAST];
			float[] phase_table = new float[(int)Band.LAST];

			for(int i=0; i<bands.Length; i++)
			{
				gain_table[(int)bands[i]] = (float)rand.NextDouble();
				phase_table[(int)bands[i]] = (float)rand.NextDouble();
			}

			byte gain_sum = Checksum.CalcHF(gain_table);
			byte phase_sum = Checksum.CalcHF(phase_table);

			byte temp;
			WriteRXImage(gain_table, phase_table, out temp, out temp);

			float[] gain_check = new float[(int)Band.LAST];
			float[] phase_check = new float[(int)Band.LAST];

			ReadRXImage(gain_check, phase_check);

			byte gain_sum_check = Checksum.CalcHF(gain_check);
			byte phase_sum_check = Checksum.CalcHF(phase_check);

			if((gain_sum_check != gain_sum) ||
				(phase_sum_check != phase_sum))
				return false;

			for(int i=0; i<bands.Length; i++)
			{
				if(gain_table[(int)bands[i]] != gain_check[(int)bands[i]] ||
					phase_table[(int)bands[i]] != phase_check[(int)bands[i]])
					return false;
			}
			return true;
		}

		public static void WriteRXImage(float[] gain_table, float[] phase_table, out byte gain_sum, out byte phase_sum)
		{
			WriteCalDateTime();
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };
			
			uint addr = 0x4A;
			ushort gain_offset = 0x0190;
			ushort temp;
			int error_count = 0;			
			do
			{
				FWC.WriteTRXEEPROMUshort(addr, gain_offset);
				//Thread.Sleep(10);
				FWC.ReadTRXEEPROMUshort(addr, out temp);
				//Thread.Sleep(40);

				if(temp != gain_offset)
				{
					if(error_count++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing RX Image Gain pointer to EEPROM.\n"+
							"Tried to write "+gain_offset.ToString()+", but read back "+temp.ToString());
						gain_sum = phase_sum = 0xFF;
						return;
					}
				}
			} while(temp != gain_offset);
			
			FWC.WriteTRXEEPROMByte(0x1BE, 5);
			rx1_image_ver = 5;

			addr = 0x4C;
			ushort phase_offset = 0x01C0;
			error_count = 0;
			do
			{
				FWC.WriteTRXEEPROMUshort(addr, phase_offset);
				//Thread.Sleep(10);
				FWC.ReadTRXEEPROMUshort(addr, out temp);
				//Thread.Sleep(40);

				if(temp != phase_offset)
				{
					if(error_count++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing RX Image Phase pointer to EEPROM.\n"+
							"Tried to write "+phase_offset.ToString()+", but read back "+temp.ToString());
						gain_sum = phase_sum = 0xFF;
						return;
					}
				}
			} while(temp != phase_offset);

			for(uint i=0; i<bands.Length; i++)
			{
				if(gain_table[(int)bands[i]] > 500.0f || gain_table[(int)bands[i]] < -500.0f)
				{
					/*MessageBox.Show("Error writing RX Image Gain value to EEPROM.\n"+
						bands[i].ToString()+" - Value out of range [-500.0, 500.0] ("+gain_table[(int)bands[i]].ToString("f4")+").\n"+
						"Recalibrate RX Image on this band.");*/
					TextWriter writer = new StreamWriter(app_data_path+"\\eeprom_error.log", true);
					writer.WriteLine(DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString()+" "+
						"Error writing RX Image Gain value to EEPROM -- "+
						bands[i].ToString()+" - Value out of range [-500.0, 500.0] ("+gain_table[(int)bands[i]].ToString("f4")+").");
					writer.Close();
					gain_table[(int)bands[i]] = 0.0f;
				}
				error_count = 0;
				float test = -999.99f;
				do
				{
					FWC.WriteTRXEEPROMFloat((uint)(gain_offset+i*4), gain_table[(int)bands[i]]);
					//Thread.Sleep(10);

					FWC.ReadTRXEEPROMFloat((uint)(gain_offset+i*4), out test);
					//Thread.Sleep(40);

					if(test != gain_table[(int)bands[i]])
					{
						if(error_count++ > NUM_WRITES_TO_TRY)
						{
							MessageBox.Show("Error writing RX Image value to EEPROM.\n"+
								"Tried to write "+gain_table[(int)bands[i]].ToString("f4")+", but read back "+test.ToString("f4"));
							gain_sum = phase_sum = 0xFF;
							return;
						}
					}
				} while(test != gain_table[(int)bands[i]]);

				if(phase_table[(int)bands[i]] > 400.0f || phase_table[(int)bands[i]] < -400.0f)
				{
					/*MessageBox.Show("Error writing RX Image Phase value to EEPROM.\n"+
						bands[i].ToString()+" - Value out of range [-400.0, 400.0] ("+gain_table[(int)bands[i]].ToString("f4")+").\n"+
						"Recalibrate RX Image on this band.");*/
					TextWriter writer = new StreamWriter(app_data_path+"\\eeprom_error.log", true);
					writer.WriteLine(DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString()+" "+
						"Error writing RX Image Phase value to EEPROM -- "+
						bands[i].ToString()+" - Value out of range [-400.0, 400.0] ("+gain_table[(int)bands[i]].ToString("f4")+").");
					writer.Close();
					gain_table[(int)bands[i]] = 0.0f;
				}
				error_count = 0;
				test = -999.99f;
				do
				{
					FWC.WriteTRXEEPROMFloat((uint)(phase_offset+i*4), phase_table[(int)bands[i]]);
					//Thread.Sleep(10);

					FWC.ReadTRXEEPROMFloat((uint)(phase_offset+i*4), out test);
					//Thread.Sleep(40);

					if(test != phase_table[(int)bands[i]])
					{
						if(error_count++ > NUM_WRITES_TO_TRY)
						{
							MessageBox.Show("Error writing RX Image value to EEPROM.\n"+
								"Tried to write "+gain_table[(int)bands[i]].ToString("f4")+", but read back "+test.ToString("f4"));
							gain_sum = phase_sum = 0xFF;
							return;
						}
					}
				} while(test != phase_table[(int)bands[i]]);
			}

			// calculate and write checksums
			byte sum = Checksum.CalcHF(gain_table);
			WriteRXImageGainChecksum(sum);
			gain_sum = sum;

			sum = Checksum.CalcHF(phase_table);
			WriteRXImagePhaseChecksum(sum);
			phase_sum = sum;
		}

		public static void WriteRXImageGainChecksum(byte sum)
		{
			byte read, errors=0;
			do
			{				
				FWC.WriteTRXEEPROMByte(0x1BF, sum);
				//Thread.Sleep(10);

				FWC.ReadTRXEEPROMByte(0x1BF, out read);
				if(read != sum)
				{
					if(errors++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing RX Image Gain checksum to EEPROM.\n"+
							"Tried to write "+sum+", but read back "+read);
						return;
					}
				}
			} while(read != sum);
		}

		public static void WriteRXImagePhaseChecksum(byte sum)
		{
			byte read, errors=0;
			do
			{				
				FWC.WriteTRXEEPROMByte(0x1EF, sum);
				//Thread.Sleep(10);

				FWC.ReadTRXEEPROMByte(0x1EF, out read);
				if(read != sum)
				{
					if(errors++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing RX Image Phase checksum to EEPROM.\n"+
							"Tried to write "+sum+", but read back "+read);
						return;
					}
				}
			} while(read != sum);
		}

		public static void ReadRXImage(float[] gain_table, float[] phase_table)
		{
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };

			ushort gain_offset;
			FWC.ReadTRXEEPROMUshort(0x4A, out gain_offset);
			//Thread.Sleep(40);
			if(gain_offset == 0 || gain_offset == 0xFFFF) return;

			ushort phase_offset;
			FWC.ReadTRXEEPROMUshort(0x4C, out phase_offset);
			//Thread.Sleep(40);
			if(phase_offset == 0 || phase_offset == 0xFFFF) return;

			uint data;
			FWC.ReadTRXEEPROMUint(gain_offset, out data);
			//Thread.Sleep(40);
			if(data == 0xFFFFFFFF) return;

			for(uint i=0; i<bands.Length; i++)
			{
				float f;
				FWC.ReadTRXEEPROMFloat((uint)(gain_offset+i*4), out f);
				//Thread.Sleep(40);
				if(f > 500.0f || f < -500.0f)
				{
					MessageBox.Show("Bad data detected in EEPROM.\n"+
						"RX Image Gain ("+bands[i].ToString()+" = "+f.ToString("f2")+")");
					f = 0.0f;
				}
				gain_table[(int)bands[i]] = f;
				FWC.ReadTRXEEPROMFloat((uint)(phase_offset+i*4), out f);
				//Thread.Sleep(40);
				if(f > 400.0f || f < -400.0f)
				{
					MessageBox.Show("Bad data detected in EEPROM.\n"+
						"RX Image Phase ("+bands[i].ToString()+" = "+f.ToString("f2")+")");
					f = 0.0f;
				}
				phase_table[(int)bands[i]] = f;
			}
		}

		public static byte ReadRXImageGainChecksum()
		{
			byte read;
			FWC.ReadTRXEEPROMByte(0x1BF, out read);
			return read;
		}

		public static byte ReadRXImagePhaseChecksum()
		{
			byte read;
			FWC.ReadTRXEEPROMByte(0x1EF, out read);
			return read;
		}

		#endregion

		#endregion

		#region TX

		#region TX Image

		public static bool CheckTXImage()
		{
			Random rand = new Random();
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };

			float[] gain_table = new float[(int)Band.LAST];
			float[] phase_table = new float[(int)Band.LAST];

			for(int i=0; i<bands.Length; i++)
			{
				gain_table[(int)bands[i]] = (float)rand.NextDouble();
				phase_table[(int)bands[i]] = (float)rand.NextDouble();
			}

			byte temp;
			WriteTXImage(gain_table, phase_table, out temp, out temp);

			float[] gain_check = new float[(int)Band.LAST];
			float[] phase_check = new float[(int)Band.LAST];

			ReadTXImage(gain_check, phase_check);

			for(int i=0; i<bands.Length; i++)
			{
				if(gain_table[(int)bands[i]] != gain_check[(int)bands[i]] ||
					phase_table[(int)bands[i]] != phase_check[(int)bands[i]])
					return false;
			}
			return true;
		}

		public static void WriteTXImage(float[] gain_table, float[] phase_table, out byte gain_sum, out byte phase_sum)
		{
			WriteCalDateTime();
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };
			
			uint addr = 0x4E;
			ushort gain_offset = 0x01F0;
			ushort temp;
			int error_count = 0;			
			do
			{
				FWC.WriteTRXEEPROMUshort(addr, gain_offset);
				//Thread.Sleep(10);
				FWC.ReadTRXEEPROMUshort(addr, out temp);
				//Thread.Sleep(40);

				if(temp != gain_offset)
				{
					if(error_count++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing TX Image Gain pointer to EEPROM.\n"+
							"Tried to write "+gain_offset.ToString()+", but read back "+temp.ToString());
						gain_sum = phase_sum = 0xFF;
						return;
					}
				}
			} while(temp != gain_offset);

			addr = 0x50;
			ushort phase_offset = 0x0220;
			error_count = 0;
			do
			{
				FWC.WriteTRXEEPROMUshort(addr, phase_offset);
				//Thread.Sleep(10);
				FWC.ReadTRXEEPROMUshort(addr, out temp);
				//Thread.Sleep(40);

				if(temp != phase_offset)
				{
					if(error_count++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing TX Image Phase pointer to EEPROM.\n"+
							"Tried to write "+phase_offset.ToString()+", but read back "+temp.ToString());
						gain_sum = phase_sum = 0xFF;
						return;
					}
				}
			} while(temp != phase_offset);

			for(uint i=0; i<bands.Length; i++)
			{
				if(gain_table[(int)bands[i]] > 500.0f || gain_table[(int)bands[i]] < -500.0f)
				{
					/*MessageBox.Show("Error writing TX Image Gain value to EEPROM.\n"+
						bands[i].ToString()+" - Value out of range [-500.0, 500.0] ("+gain_table[(int)bands[i]].ToString("f4")+").\n"+
						"Recalibrate TX Image on this band.");*/
					TextWriter writer = new StreamWriter(app_data_path+"\\eeprom_error.log", true);
					writer.WriteLine(DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString()+" "+
						"Error writing TX Image Gain value to EEPROM -- "+
						bands[i].ToString()+" - Value out of range [-500.0, 500.0] ("+gain_table[(int)bands[i]].ToString("f4")+").");
					writer.Close();
					gain_table[(int)bands[i]] = 0.0f;
				}
				error_count = 0;
				float test = -999.99f;
				do
				{
					FWC.WriteTRXEEPROMFloat((uint)(gain_offset+i*4), gain_table[(int)bands[i]]);
					//Thread.Sleep(10);

					FWC.ReadTRXEEPROMFloat((uint)(gain_offset+i*4), out test);
					//Thread.Sleep(40);

					if(test != gain_table[(int)bands[i]])
					{
						if(error_count++ > NUM_WRITES_TO_TRY)
						{
							MessageBox.Show("Error writing TX Image value to EEPROM.\n"+
								"Tried to write "+gain_table[(int)bands[i]].ToString("f4")+", but read back "+test.ToString("f4"));
							gain_sum = phase_sum = 0xFF;
							return;
						}
					}
				} while(test != gain_table[(int)bands[i]]);

				if(phase_table[(int)bands[i]] > 400.0f || phase_table[(int)bands[i]] < -400.0f)
				{
					MessageBox.Show("Error writing TX Image Phase value to EEPROM.\n"+
						bands[i].ToString()+" - Value out of range [-400.0, 400.0] ("+gain_table[(int)bands[i]].ToString("f4")+").\n"+
						"Recalibrate TX Image on this band.");
					gain_table[(int)bands[i]] = 0.0f;
				}
				error_count = 0;
				test = -999.99f;
				do
				{
					FWC.WriteTRXEEPROMFloat((uint)(phase_offset+i*4), phase_table[(int)bands[i]]);
					//Thread.Sleep(10);

					FWC.ReadTRXEEPROMFloat((uint)(phase_offset+i*4), out test);
					//Thread.Sleep(40);

					if(test != phase_table[(int)bands[i]])
					{
						if(error_count++ > NUM_WRITES_TO_TRY)
						{
							MessageBox.Show("Error writing TX Image value to EEPROM.\n"+
								"Tried to write "+phase_table[(int)bands[i]].ToString("f4")+", but read back "+test.ToString("f4"));
							gain_sum = phase_sum = 0xFF;
							return;
						}
					}
				} while(test != phase_table[(int)bands[i]]);
			}

			// calculate and write checksums
			byte sum = Checksum.CalcHF(gain_table);
			WriteTXImageGainChecksum(sum);
			gain_sum = sum;

			sum = Checksum.CalcHF(phase_table);
			WriteTXImagePhaseChecksum(sum);
			phase_sum = sum;
		}

		public static void WriteTXImageGainChecksum(byte sum)
		{
			byte read, errors=0;
			do
			{				
				FWC.WriteTRXEEPROMByte(0x21F, sum);
				//Thread.Sleep(10);

				FWC.ReadTRXEEPROMByte(0x21F, out read);
				if(read != sum)
				{
					if(errors++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing TX Image Gain checksum to EEPROM.\n"+
							"Tried to write "+sum+", but read back "+read);
						return;
					}
				}
			} while(read != sum);
		}

		public static void WriteTXImagePhaseChecksum(byte sum)
		{
			byte read, errors=0;
			do
			{				
				FWC.WriteTRXEEPROMByte(0x24F, sum);
				//Thread.Sleep(10);

				FWC.ReadTRXEEPROMByte(0x24F, out read);
				if(read != sum)
				{
					if(errors++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing TX Image Phase checksum to EEPROM.\n"+
							"Tried to write "+sum+", but read back "+read);
						return;
					}
				}
			} while(read != sum);
		}

		public static void ReadTXImage(float[] gain_table, float[] phase_table)
		{
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };

			ushort gain_offset;
			FWC.ReadTRXEEPROMUshort(0x4E, out gain_offset);
			//Thread.Sleep(40);
			if(gain_offset == 0 || gain_offset == 0xFFFF) return;

			ushort phase_offset;
			FWC.ReadTRXEEPROMUshort(0x50, out phase_offset);
			if(phase_offset == 0 || phase_offset == 0xFFFF) return;

			uint data;
			FWC.ReadTRXEEPROMUint(gain_offset, out data);
			//Thread.Sleep(40);
			if(data == 0xFFFFFFFF) return;

			for(uint i=0; i<bands.Length; i++)
			{
				float f;
				FWC.ReadTRXEEPROMFloat((uint)(gain_offset+i*4), out f);
				//Thread.Sleep(40);
				if(f > 500.0f || f < -500.0f)
				{
					/*MessageBox.Show("Bad data detected in EEPROM.\n"+
						"TX Image Gain ("+bands[i].ToString()+" = "+f.ToString("f2")+")");*/
					TextWriter writer = new StreamWriter(app_data_path+"\\eeprom_error.log", true);
					writer.WriteLine(DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString()+" "+
						"Bad data detected in EEPROM -- "+
						"TX Image Gain ("+bands[i].ToString()+" = "+f.ToString("f2")+")");
					writer.Close();
					f = 0.0f;
				}
				gain_table[(int)bands[i]] = f;
				FWC.ReadTRXEEPROMFloat((uint)(phase_offset+i*4), out f);
				//Thread.Sleep(40);
				if(f > 400.0f || f < -400.0f)
				{
					/*MessageBox.Show("Bad data detected in EEPROM.\n"+
						"TX Image Phase ("+bands[i].ToString()+" = "+f.ToString("f2")+")");*/
					TextWriter writer = new StreamWriter(app_data_path+"\\eeprom_error.log", true);
					writer.WriteLine(DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString()+" "+
						"Bad data detected in EEPROM -- "+
						"TX Image Phase ("+bands[i].ToString()+" = "+f.ToString("f2")+")");
					writer.Close();
					f = 0.0f;
				}
				phase_table[(int)bands[i]] = f;
			}
		}

		public static byte ReadTXImageGainChecksum()
		{
			byte read;
			FWC.ReadTRXEEPROMByte(0x21F, out read);
			return read;
		}

		public static byte ReadTXImagePhaseChecksum()
		{
			byte read;
			FWC.ReadTRXEEPROMByte(0x24F, out read);
			return read;
		}

		#endregion

		#region TX Carrier

		public static bool CheckTXCarrier()
		{
			Random rand = new Random();
            double[] band_freqs = { 1.85f, 3.75f, 5.3665f, 7.15f, 10.125f, 14.175f, 18.1f, 21.300f, 24.9f, 28.4f, 50.11f };

			SortedDictionary<double, uint> tx_carrier_cal = new SortedDictionary<double, uint>();

            for (int i = 0; i < band_freqs.Length; i++)
                tx_carrier_cal[Math.Round(band_freqs[i], 3)] = (uint)rand.Next();

			byte temp;
            WriteTXCarrier(tx_carrier_cal, out temp);

			SortedDictionary<double, uint> tx_carrier_check = new SortedDictionary<double, uint>();

			ReadTXCarrier(tx_carrier_check);

			for(int i=0; i<band_freqs.Length; i++)
			{
			    if(tx_carrier_cal[Math.Round(band_freqs[i], 3)] != tx_carrier_check[Math.Round(band_freqs[i], 3)])
					return false;
			}
			return true;
		}

		public static void WriteTXCarrier(SortedDictionary<double, uint> table, out byte checksum)
		{
			WriteCalDateTime();
			
			ushort ptr_addr = 0x62;
			ushort table_addr = 0x0760;

            if (!CheckedWriteUshort(ptr_addr, table_addr))
                goto error;

            ushort offset = 0;
            foreach (KeyValuePair<double, uint> pair in table)
            {
                ulong freq = ToVitaFreq(Math.Round(pair.Key, 3));
                if (!CheckedWriteUint((uint)(table_addr + offset), (uint)(freq >> 32)))
                    goto error;
                offset += 4;

                if (!CheckedWriteUint((uint)(table_addr + offset), (uint)freq))
                    goto error;
                offset += 4;

                if (!CheckedWriteUint((uint)(table_addr + offset), pair.Value))
                    goto error;
                offset += 4;
            }

            // write end of table flag here
            if (!CheckedWriteUint((uint)(table_addr + offset), END_OF_TABLE_FLAG))
                goto error;

			// calculate and write checksum
			byte sum = Checksum.Calc(table);
			WriteTXCarrierChecksum(sum);
			checksum = sum;
            return;

        error:
            TextWriter writer = new StreamWriter(app_data_path + "\\eeprom_error.log", true);
            writer.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " " +
                "Error writing TX Carrier data to EEPROM");
            writer.Close();
            MessageBox.Show("Error writing TX Carrier data to EEPROM");
            checksum = 0xFF;
		}

		public static void WriteTXCarrierChecksum(byte sum)
		{
			if(!CheckedWriteByte(0x27F, sum))
				MessageBox.Show("Error writing TX Carrier checksum to EEPROM");
		}

        private static uint SwapBytes(uint x)
        {
            return (x & 0xff) << 24
                | (x & 0xff00) << 8
                | (x & 0xff0000) >> 8
                | (x & 0xff000000) >> 24;
        }

		public static void ReadTXCarrier(SortedDictionary<double, uint> table)
		{
			ushort table_addr;
            FWC.ReadTRXEEPROMUshort(0x62, out table_addr);
            
            ushort offset = 0;
            uint data;

            if(table_addr == 0xFFFF)
            {
                table_addr = 0x250;

			    
                FWC.ReadTRXEEPROMUint(table_addr, out data);
			    if(data == 0xFFFFFFFF) return;

                // convert old style data
                float[] band_freqs = { 1.85f, 3.75f, 5.3665f, 7.15f, 10.125f, 14.175f, 18.1f, 21.300f, 24.9f, 28.4f, 50.11f };

                for (int i = 0; i < band_freqs.Length; i++)
                {
                    FWC.ReadTRXEEPROMUint((uint)(table_addr + offset), out data);
                    offset += 4;

                    data = SwapBytes(data);

                    table[Math.Round(band_freqs[i], 3)] = data;
                }

                byte checksum = 0;
                WriteTXCarrier(table, out checksum);
                return;
            }

            int count = 0;
			while(true) // do this until we find end of table flag (or an error)
			{
				FWC.ReadTRXEEPROMUint((uint)(table_addr + offset), out data);
                offset += 4;
                if (data == END_OF_TABLE_FLAG)
                    break;

                // if it's not the end of the table, data is the MSB of the Vita Freq
                uint vita_freq_msb = data;

                // read in LSBs to data2
                uint vita_freq_lsb;
                FWC.ReadTRXEEPROMUint((uint)(table_addr + offset), out vita_freq_lsb);
                offset += 4;

                // read in value to val
                uint val;
                FWC.ReadTRXEEPROMUint((uint)(table_addr + offset), out val);
                offset += 4;

                double key = Math.Round(FromVitaFreq(((ulong)vita_freq_msb << 32) + vita_freq_lsb), 3);
                if (key > 0.0 && key < 451.0)
                {
                    table[key] = val;
                }
                if (++count >= 100) break; // stop if no deadbeef is found
			}
		}

		public static byte ReadTXCarrierChecksum()
		{
			byte read;
			FWC.ReadTRXEEPROMByte(0x27F, out read);
			return read;
		}

		#endregion

		#endregion

		#region PA

		#region PA Bias

		public static bool CheckPABias()
		{
			Random rand = new Random();

			int[][] pa_bias_table = new int[4][];
			for(int i=0; i<4; i++)
				pa_bias_table[i] = new int[8];

			for(int i=0; i<4; i++)
				for(int j=0; j<8; j++)
					pa_bias_table[i][j] = rand.Next(255);

			byte temp;
			WritePABias(pa_bias_table, out temp);

			int[][] pa_bias_check = new int[4][];
			for(int i=0; i<4; i++)
				pa_bias_check[i] = new int[8];

			ReadPABias(pa_bias_check);

			for(int i=0; i<4; i++)
			{
				for(int j=0; j<8; j++)
				{
					if(pa_bias_table[i][j] != pa_bias_check[i][j])
						return false;
				}
			}
			return true;
		}

		public static void WritePABias(int[][] table, out byte checksum)
		{
			WriteCalDateTime();
			
			uint addr = 0x54;
			ushort offset = 0x02C0;
			ushort temp2;
			int error_count = 0;
			do
			{
				FWC.WriteTRXEEPROMUshort(addr, offset);
				//Thread.Sleep(10);
				FWC.ReadTRXEEPROMUshort(addr, out temp2);
				//Thread.Sleep(40);

				if(temp2 != offset)
				{
					if(error_count++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing PA Bias pointer to EEPROM.\n"+
							"Tried to write "+offset.ToString()+", but read back "+temp2.ToString());
						checksum = 0xFF;
						return;
					}
				}
			} while(temp2 != offset);

			for(int i=0; i<4; i++)
			{
				uint data = 0;
				for(int j=0; j<4; j++)
					data += (uint)table[i][j]<<(j*8);

				uint temp = 0;
				error_count = 0;
				do
				{
					FWC.WriteTRXEEPROMUint((uint)(offset+i*8), data);
					//Thread.Sleep(10);

					FWC.ReadTRXEEPROMUint((uint)(offset+i*8), out temp);
					//Thread.Sleep(40);

					if(temp != data)
					{
						if(error_count++ > NUM_WRITES_TO_TRY)
						{
							MessageBox.Show("Error writing PA Bias value to EEPROM.\n"+
								"Tried to write "+data+", but read back "+temp);
							checksum = 0xFF;
							return;
						}
					}
				} while(temp != data);

				data = 0;
				for(int j=4; j<8; j++)
					data += (uint)table[i][j]<<((j-4)*8);

				error_count = 0;
				temp = 0;
				do
				{
					FWC.WriteTRXEEPROMUint((uint)(offset+i*8+4), data);
					//Thread.Sleep(10);

					FWC.ReadTRXEEPROMUint((uint)(offset+i*8+4), out temp);
					//Thread.Sleep(40);

					if(temp != data)
					{
						if(error_count++ > NUM_WRITES_TO_TRY)
						{
							MessageBox.Show("Error writing PA Bias value to EEPROM.\n"+
								"Tried to write "+data+", but read back "+temp);
							checksum = 0xFF;
							return;
						}
					}
				} while(temp != data);
			}

			// calculate and write checksum
			byte sum = Checksum.Calc(table, false);
			WritePABiasChecksum(sum);
			checksum = sum;
		}

		public static void WritePABiasChecksum(byte sum)
		{
			byte read, errors=0;
			do
			{				
				FWC.WriteTRXEEPROMByte(0x27E, sum);
				//Thread.Sleep(10);

				FWC.ReadTRXEEPROMByte(0x27E, out read);
				if(read != sum)
				{
					if(errors++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing PA Bias checksum to EEPROM.\n"+
							"Tried to write "+sum+", but read back "+read);
						return;
					}
				}
			} while(read != sum);
		}

		public static void ReadPABias(int[][] table)
		{
			ushort offset;
			FWC.ReadTRXEEPROMUshort(0x54, out offset);
			//Thread.Sleep(40);
			if(offset == 0 || offset == 0xFFFF) return;

			uint data;
			FWC.ReadTRXEEPROMUint(offset, out data);
			//Thread.Sleep(40);
			if(data == 0xFFFFFFFF) return;

			for(int i=0; i<4; i++)
			{
				FWC.ReadTRXEEPROMUint((uint)(offset+i*8), out data);
				//Thread.Sleep(40);
				for(int j=0; j<4; j++)
					table[i][j] = (byte)(data>>(j*8));

				FWC.ReadTRXEEPROMUint((uint)(offset+i*8+4), out data);
				//Thread.Sleep(40);
				for(int j=4; j<8; j++)
					table[i][j] = (byte)(data>>((j-4)*8));
			}
		}

		public static byte ReadPABiasChecksum()
		{
			byte read;
			FWC.ReadTRXEEPROMByte(0x27E, out read);
			return read;
		}

		#endregion

		#region PA Bridge

		public static bool CheckPABridge()
		{
			Random rand = new Random();
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };

			float[][] pa_bridge_table = new float[(int)Band.LAST][];
			for(int i=0; i<(int)Band.LAST; i++)
				pa_bridge_table[i] = new float[6];

			for(int i=0; i<bands.Length; i++)
				for(int j=0; j<4; j++)
					pa_bridge_table[(int)bands[i]][j] = (float)rand.NextDouble();

			byte temp;
			WritePABridge(pa_bridge_table, out temp);

			float[][] pa_bridge_check = new float[(int)Band.LAST][];
			for(int i=0; i<(int)Band.LAST; i++)
				pa_bridge_check[i] = new float[6];

			ReadPABridge(pa_bridge_check);

			for(int i=0; i<bands.Length; i++)
			{
				for(int j=0; j<6; j++)
				{
					if(pa_bridge_table[(int)bands[i]][j] != pa_bridge_check[(int)bands[i]][j])
						return false;
				}
			}
			return true;
		}

		public static void WritePABridge(float[][] table, out byte checksum)
		{
			WriteCalDateTime();			
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };

			uint addr = 0x56;
			ushort offset = 0x02E0;
			ushort temp2;
			int error_count = 0;
			do
			{
				FWC.WriteTRXEEPROMUshort(addr, offset);
				//Thread.Sleep(10);
				FWC.ReadTRXEEPROMUshort(addr, out temp2);
				//Thread.Sleep(40);

				if(temp2 != offset)
				{
					if(error_count++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing PA Bridge pointer to EEPROM.\n"+
							"Tried to write "+offset.ToString()+", but read back "+temp2.ToString());
						checksum = 0xFF;
						return;
					}
				}
			} while(temp2 != offset);

			for(int i=0; i<bands.Length; i++)
			{
				for(int j=0; j<6; j++)
				{
					error_count = 0;
					float temp = 0.0f;
					if(table[(int)bands[i]][j] > 2.3f || table[(int)bands[i]][j] < 0.01f)
					{
						/*MessageBox.Show("Error writing PA Bridge value to EEPROM.\n"+
							"Value out of range 0.01 to 2.3 ("+table[(int)bands[i]][j].ToString("f4")+").");*/
						TextWriter writer = new StreamWriter(app_data_path+"\\eeprom_error.log", true);
						writer.WriteLine(DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString()+" "+
							"Error writing PA Bridge value to EEPROM -- "+
							"Value out of range 0.01 to 2.3 ("+table[(int)bands[i]][j].ToString("f4")+").");
						writer.Close();
						table[(int)bands[i]][j] = 0.01f;
					}
					do
					{
						FWC.WriteTRXEEPROMFloat((uint)(offset+i*24+j*4), table[(int)bands[i]][j]);
						//Thread.Sleep(10);

						FWC.ReadTRXEEPROMFloat((uint)(offset+i*24+j*4), out temp);
						//Thread.Sleep(40);

						if(temp != table[(int)bands[i]][j])
						{
							if(error_count++ > NUM_WRITES_TO_TRY)
							{
								MessageBox.Show("Error writing PA Bridge value to EEPROM.\n"+
									"Tried to write "+table[(int)bands[i]][j].ToString("f4")+", but read back "+temp.ToString("f4"));
								checksum = 0xFF;
								return;
							}
						}
					} while(temp != table[(int)bands[i]][j]);
				}
			}

			// calculate and write checksum
			byte sum = Checksum.Calc(table);
			WritePABridgeChecksum(sum);
			checksum = sum;
		}

		public static void WritePABridgeChecksum(byte sum)
		{
			byte read, errors=0;
			do
			{				
				FWC.WriteTRXEEPROMByte(0x3EF, sum);
				//Thread.Sleep(10);

				FWC.ReadTRXEEPROMByte(0x3EF, out read);
				if(read != sum)
				{
					if(errors++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing PA Bridge checksum to EEPROM.\n"+
							"Tried to write "+sum+", but read back "+read);
						return;
					}
				}
			} while(read != sum);
		}

		public static void ReadPABridge(float[][] table)
		{
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };

			ushort offset;
			FWC.ReadTRXEEPROMUshort(0x56, out offset);
			//Thread.Sleep(40);
			if(offset == 0 || offset == 0xFFFF) return;

			uint data;
			FWC.ReadTRXEEPROMUint(offset, out data);
			//Thread.Sleep(40);
			if(data == 0xFFFFFFFF) return;

			for(int i=0; i<bands.Length; i++)
			{
				float f;
				for(int j=0; j<6; j++)
				{
					FWC.ReadTRXEEPROMFloat((uint)(offset+i*24+j*4), out f);
					//Thread.Sleep(40);
					if(f > 2.3f || f < 0.01)
					{
						/*MessageBox.Show("Bad data detected in EEPROM.\n"+
							"PA Bridge("+bands[i].ToString()+", "+j+") = "+f.ToString("f4"));*/
						TextWriter writer = new StreamWriter(app_data_path+"\\eeprom_error.log", true);
						writer.WriteLine(DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString()+" "+
							"Bad data detected in EEPROM -- "+
							"PA Bridge("+bands[i].ToString()+", "+j+") = "+f.ToString("f4"));
						writer.Close();
						table[(int)bands[i]][j] = 0.01f;
					}
					else table[(int)bands[i]][j] = (float)Math.Round(f, 4);
				}
			}
		}

		public static byte ReadPABridgeChecksum()
		{
			byte read;
			FWC.ReadTRXEEPROMByte(0x3EF, out read);
			return read;
		}

		#endregion PA Bridge

		#region PA Power

		public static bool CheckPAPower()
		{
			Random rand = new Random();
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };

			float[][] pa_power_table = new float[(int)Band.LAST][];
			for(int i=0; i<(int)Band.LAST; i++)
				pa_power_table[i] = new float[13];

			for(int i=0; i<bands.Length; i++)
				for(int j=0; j<13; j++)
					pa_power_table[(int)bands[i]][j] = (float)rand.NextDouble();

			byte temp;
			WritePAPower(pa_power_table, out temp);

			float[][] pa_power_check = new float[(int)Band.LAST][];
			for(int i=0; i<(int)Band.LAST; i++)
				pa_power_check[i] = new float[13];

			ReadPAPower(pa_power_check);

			for(int i=0; i<bands.Length; i++)
			{
				for(int j=0; j<13; j++)
				{
					if(pa_power_table[(int)bands[i]][j] != pa_power_check[(int)bands[i]][j])
						return false;
				}
			}
			return true;
		}

		public static void WritePAPower(float[][] table, out byte checksum)
		{
			WriteCalDateTime();
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };
			
			uint addr = 0x58;
			ushort offset = 0x03F0;
			ushort temp2;
			int error_count = 0;
			do
			{
				FWC.WriteTRXEEPROMUshort(addr, offset);
				//Thread.Sleep(10);
				FWC.ReadTRXEEPROMUshort(addr, out temp2);
				//Thread.Sleep(40);

				if(temp2 != offset)
				{
					if(error_count++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing PA Power pointer to EEPROM.\n"+
							"Tried to write "+offset.ToString()+", but read back "+temp2.ToString());
						checksum = 0xFF;
						return;
					}
				}
			} while(temp2 != offset);

			for(int i=0; i<bands.Length; i++)
			{
				for(int j=0; j<13; j++)
				{
					if(table[(int)bands[i]][j] > 1.0f || table[(int)bands[i]][j] < 0.0f)
					{
						/*MessageBox.Show("Error writing PA Power value to EEPROM.\n"+
							"Value out of range 0.0 to 1.0 ("+table[(int)bands[i]][j].ToString("f4")+").");*/
						TextWriter writer = new StreamWriter(app_data_path+"\\eeprom_error.log", true);
						writer.WriteLine(DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString()+" "+
							"Error writing PA Power value to EEPROM -- "+
							"Value out of range 0.0 to 1.0 ("+table[(int)bands[i]][j].ToString("f4")+").");
						writer.Close();
						table[(int)bands[i]][j] = 0.0f;
					}
					error_count = 0;
					float temp = 0.0f;
					do
					{
						FWC.WriteTRXEEPROMFloat((uint)(offset+i*52+j*4), table[(int)bands[i]][j]);
						//Thread.Sleep(10);

						FWC.ReadTRXEEPROMFloat((uint)(offset+i*52+j*4), out temp);
						//Thread.Sleep(40);

						if(temp != table[(int)bands[i]][j])
						{
							if(error_count++ > NUM_WRITES_TO_TRY)
							{
								MessageBox.Show("Error writing PA Power value to EEPROM.\n"+
									"Tried to write "+table[(int)bands[i]][j].ToString("f4")+", but read back "+temp.ToString("f4"));
								checksum = 0xFF;
								return;
							}
						}
					} while(temp != table[(int)bands[i]][j]);
				}
			}

			// calculate and write checksum
			byte sum = Checksum.Calc(table);
			WritePAPowerChecksum(sum);
			checksum = sum;
		}

		public static void WritePAPowerChecksum(byte sum)
		{
			byte read, errors=0;
			do
			{				
				FWC.WriteTRXEEPROMByte(0x62F, sum);
				//Thread.Sleep(10);

				FWC.ReadTRXEEPROMByte(0x62F, out read);
				if(read != sum)
				{
					if(errors++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing PA Power checksum to EEPROM.\n"+
							"Tried to write "+sum+", but read back "+read);
						return;
					}
				}
			} while(read != sum);
		}

		public static void ReadPAPower(float[][] table)
		{
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };

			ushort offset;
			FWC.ReadTRXEEPROMUshort(0x58, out offset);
			//Thread.Sleep(40);
			if(offset == 0 || offset == 0xFFFF) return;

			uint data;
			FWC.ReadTRXEEPROMUint(offset, out data);
			//Thread.Sleep(40);
			if(data == 0xFFFFFFFF) return;

			for(int i=0; i<bands.Length; i++)
			{
				for(int j=0; j<13; j++)
				{
					float f;
					FWC.ReadTRXEEPROMFloat((uint)(offset+i*52+j*4), out f);
					//Thread.Sleep(40);
					if(f > 1.0f || f < 0.0f)
					{
						/*MessageBox.Show("Bad data detected in EEPROM.\n"+
							"PA Power ("+bands[i].ToString()+", "+j+") = "+f.ToString("f4"));*/
						TextWriter writer = new StreamWriter(app_data_path+"\\eeprom_error.log", true);
						writer.WriteLine(DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString()+" "+
							"Bad data detected in EEPROM -- "+
							"PA Power ("+bands[i].ToString()+", "+j+") = "+f.ToString("f4"));
						writer.Close();
						table[(int)bands[i]][j] = 0.0f;
					}
					else table[(int)bands[i]][j] = (float)Math.Round(f, 4);
				}
			}
		}

		public static byte ReadPAPowerChecksum()
		{
			byte read;
			FWC.ReadTRXEEPROMByte(0x62F, out read);
			return read;
		}

		#endregion

		#region PA SWR

		public static bool CheckPASWR()
		{
			Random rand = new Random();
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };

			float[] pa_swr_table = new float[(int)Band.LAST];

			for(int i=0; i<bands.Length; i++)
				pa_swr_table[(int)bands[i]] = (float)rand.NextDouble();

			byte temp;
			WritePASWR(pa_swr_table, out temp);

			float[] pa_swr_check = new float[(int)Band.LAST];
			
			ReadPASWR(pa_swr_check);

			for(int i=0; i<bands.Length; i++)
			{
				if(pa_swr_table[(int)bands[i]] != pa_swr_check[(int)bands[i]])
					return false;
			}
			return true;
		}

		public static void WritePASWR(float[] table, out byte checksum)
		{
			WriteCalDateTime();
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };
			
			uint addr = 0x5A;
			ushort offset = 0x0630;
			ushort temp2;
			int error_count = 0;
			do
			{
				FWC.WriteTRXEEPROMUshort(addr, offset);
				//Thread.Sleep(10);
				FWC.ReadTRXEEPROMUshort(addr, out temp2);
				//Thread.Sleep(40);

				if(temp2 != offset)
				{
					if(error_count++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing PA SWR pointer to EEPROM.\n"+
							"Tried to write "+offset.ToString()+", but read back "+temp2.ToString());
						checksum = 0xFF;
						return;
					}
				}
			} while(temp2 != offset);

			for(int i=0; i<bands.Length; i++)
			{
				error_count = 0;
				float temp = 0.0f;
				if(table[(int)bands[i]] > 5.0f || table[(int)bands[i]] < 0.1f)
				{
					MessageBox.Show("Error writing SWR value to EEPROM.\n"+
						"Value out of range 0.1 to 5.0 ("+table[(int)bands[i]].ToString("f4")+").");
					table[(int)bands[i]] = 1.0f;
				}
				do
				{
					FWC.WriteTRXEEPROMFloat((uint)(offset+i*4), (float)table[(int)bands[i]]);
					//Thread.Sleep(10);

					FWC.ReadTRXEEPROMFloat((uint)(offset+i*4), out temp);
					//Thread.Sleep(40);

					if(temp != table[(int)bands[i]])
					{
						if(error_count++ > NUM_WRITES_TO_TRY)
						{
							MessageBox.Show("Error writing PA SWR value to EEPROM.\n"+
								"Tried to write "+table[(int)bands[i]].ToString("f4")+", but read back "+temp.ToString("f4"));
							checksum = 0xFF;
							return;
						}
					}
				} while(temp != table[(int)bands[i]]);
			}

			// calculate and write checksum
			byte sum = Checksum.CalcHF(table);
			WritePASWRChecksum(sum);
			checksum = sum;
		}

		public static void WritePASWRChecksum(byte sum)
		{
			byte read, errors=0;
			do
			{				
				FWC.WriteTRXEEPROMByte(0x65F, sum);
				//Thread.Sleep(10);

				FWC.ReadTRXEEPROMByte(0x65F, out read);
				if(read != sum)
				{
					if(errors++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing PA SWR checksum to EEPROM.\n"+
							"Tried to write "+sum+", but read back "+read);
						return;
					}
				}
			} while(read != sum);
		}

		public static void ReadPASWR(float[] table)
		{
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };

			ushort offset;
			FWC.ReadTRXEEPROMUshort(0x5A, out offset);
			//Thread.Sleep(40);
			if(offset == 0 || offset == 0xFFFF) return;

			uint data;
			FWC.ReadTRXEEPROMUint(offset, out data);
			//Thread.Sleep(40);
			if(data == 0xFFFFFFFF) return;

			for(int i=0; i<bands.Length; i++)
			{
				float f;
				FWC.ReadTRXEEPROMFloat((uint)(offset+i*4), out f);
				//Thread.Sleep(40);
				if(f > 5.0 || f < 0.1)
				{
					/*MessageBox.Show("Bad data detected in EEPROM.\n"+
						"SWR ("+bands[i].ToString()+") = "+f.ToString("f4"));*/
					TextWriter writer = new StreamWriter(app_data_path+"\\eeprom_error.log", true);
					writer.WriteLine(DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString()+" "+
						"Bad data detected in EEPROM -- "+
						"SWR ("+bands[i].ToString()+") = "+f.ToString("f4"));
					writer.Close();
					table[(int)bands[i]] = 1.0f;
				}
				else table[(int)bands[i]] = (float)Math.Round(f, 4);
			}
		}

		public static byte ReadPASWRChecksum()
		{
			byte read;
			FWC.ReadTRXEEPROMByte(0x65F, out read);
			return read;
		}

		#endregion

		#endregion

		#region RX2

		#region RX2 Level

		public static bool CheckRX2Level()
		{
			Random rand = new Random();
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };

			float[][] rx_level_table = new float[(int)Band.LAST][];
			for(int i=0; i<(int)Band.LAST; i++)
				rx_level_table[i] = new float[3];

			for(int i=0; i<bands.Length; i++)
				for(int j=0; j<3; j++)
					rx_level_table[(int)bands[i]][j] = (float)rand.NextDouble();

			byte temp;
			WriteRX2Level(rx_level_table, out temp);

			float[][] rx_level_check = new float[(int)Band.LAST][];
			for(int i=0; i<(int)Band.LAST; i++)
				rx_level_check[i] = new float[3];
			
			ReadRX2Level(rx_level_check);

			for(int i=0; i<bands.Length; i++)
			{
				for(int j=0; j<3; j++)
				{
					if(rx_level_table[(int)bands[i]][j] != rx_level_check[(int)bands[i]][j])
						return false;
				}
			}
			return true;
		}

		public static void WriteRX2Level(float[][] table, out byte checksum)
		{
			WriteRX2CalDateTime();			
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };

			uint addr = 0x48;
			ushort offset = 0x0100, temp;
			int error_count = 0;
			do
			{
				FWC.WriteRX2EEPROMUshort(addr, offset);
				//Thread.Sleep(10);
				FWC.ReadRX2EEPROMUshort(addr, out temp);
				//Thread.Sleep(40);

				if(temp != offset)
				{
					if(error_count++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing RX2 Level pointer to EEPROM.\n"+
							"Tried to write "+offset.ToString()+", but read back "+temp.ToString());
						checksum = 0xFF;
						return;
					}
				}
			} while(temp != offset);

			for(int i=0; i<bands.Length; i++)
			{
				for(int j=0; j<3; j++)
				{
					error_count = 0;
					float test = -99.99f;
					do
					{
						FWC.WriteRX2EEPROMFloat((uint)(offset+i*12+j*4), table[(int)bands[i]][j]);
						//Thread.Sleep(10);
						
						FWC.ReadRX2EEPROMFloat((uint)(offset+i*12+j*4), out test);
						//Thread.Sleep(40);
                        
						if(test != table[(int)bands[i]][j])
						{
							if(error_count++ > 5)
							{
								MessageBox.Show("Error writing RX2 Level value to EEPROM.\n"+
									"Tried to write "+table[(int)bands[i]][j].ToString("f4")+", but read back "+test.ToString("f4"));
								checksum = 0xFF;
								return;
							}
						}
					} while(test != table[(int)bands[i]][j]);
				}
			}

			// calculate and write checksum
			byte sum = Checksum.Calc(table);
			WriteRX2LevelChecksum(sum);
			checksum = sum;
		}

		public static void WriteRX2LevelChecksum(byte sum)
		{
			byte read, errors=0;
			do
			{				
				FWC.WriteRX2EEPROMByte(0x18F, sum);
				//Thread.Sleep(10);

				FWC.ReadRX2EEPROMByte(0x18F, out read);
				if(read != sum)
				{
					if(errors++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing RX2 Level checksum to EEPROM.\n"+
							"Tried to write "+sum+", but read back "+read);
						return;
					}
				}
			} while(read != sum);
		}

		public static void ReadRX2Level(float[][] table)
		{
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };

			ushort offset;
			FWC.ReadRX2EEPROMUshort(0x48, out offset);
			//Thread.Sleep(40);
			if(offset == 0 || offset == 0xFFFF) return;

			uint data;
			FWC.ReadRX2EEPROMUint(offset, out data);
			//Thread.Sleep(40);
			if(data == 0xFFFFFFFF) return;

			for(int i=0; i<bands.Length; i++)
			{
				for(int j=0; j<3; j++)
				{
					float f;
					FWC.ReadRX2EEPROMFloat((uint)(offset+i*12+j*4), out f);
					//Thread.Sleep(40);
					table[(int)bands[i]][j] = (float)Math.Round(f, 3);
				}
			}
		}

		public static byte ReadRX2LevelChecksum()
		{
			byte read;
			FWC.ReadRX2EEPROMByte(0x18F, out read);
			return read;
		}

		#endregion

		#region RX2 Image

		public static bool CheckRX2Image()
		{
			Random rand = new Random();
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };

			float[] gain_table = new float[(int)Band.LAST];
			float[] phase_table = new float[(int)Band.LAST];

			for(int i=0; i<bands.Length; i++)
			{
				gain_table[(int)bands[i]] = (float)rand.NextDouble();
				phase_table[(int)bands[i]] = (float)rand.NextDouble();
			}

			byte temp;
			WriteRX2Image(gain_table, phase_table, out temp, out temp);

			float[] gain_check = new float[(int)Band.LAST];
			float[] phase_check = new float[(int)Band.LAST];

			ReadRX2Image(gain_check, phase_check);

			for(int i=0; i<bands.Length; i++)
			{
				if(gain_table[(int)bands[i]] != gain_check[(int)bands[i]] ||
					phase_table[(int)bands[i]] != phase_check[(int)bands[i]])
					return false;
			}
			return true;
		}

		public static int GetRX2ImageVersion()
		{
			byte b;
			FWC.ReadRX2EEPROMByte(0x1BE, out b);
			return b;
		}

		public static void WriteRX2Image(float[] gain_table, float[] phase_table, out byte gain_sum, out byte phase_sum)
		{
			WriteRX2CalDateTime();
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };
			
			uint addr = 0x4A;
			ushort gain_offset = 0x0190;
			ushort temp;
			int error_count = 0;			
			do
			{
				FWC.WriteRX2EEPROMUshort(addr, gain_offset);
				//Thread.Sleep(10);
				FWC.ReadRX2EEPROMUshort(addr, out temp);
				//Thread.Sleep(40);

				if(temp != gain_offset)
				{
					if(error_count++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing RX2 Image Gain pointer to EEPROM.\n"+
							"Tried to write "+gain_offset.ToString()+", but read back "+temp.ToString());
						gain_sum = phase_sum = 0xFF;
						return;
					}
				}
			} while(temp != gain_offset);

			FWC.WriteRX2EEPROMByte(0x1BE, 5);
			rx2_image_ver = 5;

			addr = 0x4C;
			ushort phase_offset = 0x01C0;
			error_count = 0;
			do
			{
				FWC.WriteRX2EEPROMUshort(addr, phase_offset);
				//Thread.Sleep(10);
				FWC.ReadRX2EEPROMUshort(addr, out temp);
				//Thread.Sleep(40);

				if(temp != phase_offset)
				{
					if(error_count++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing RX2 Image Phase pointer to EEPROM.\n"+
							"Tried to write "+phase_offset.ToString()+", but read back "+temp.ToString());
						gain_sum = phase_sum = 0xFF;
						return;
					}
				}
			} while(temp != phase_offset);

			for(uint i=0; i<bands.Length; i++)
			{
				if(gain_table[(int)bands[i]] > 500.0f || gain_table[(int)bands[i]] < -500.0f)
				{
					/*MessageBox.Show("Error writing RX2 Image Gain value to EEPROM.\n"+
						bands[i].ToString()+" - Value out of range [-500.0, 500.0] ("+gain_table[(int)bands[i]].ToString("f4")+").");*/
					TextWriter writer = new StreamWriter(app_data_path+"\\eeprom_error.log", true);
					writer.WriteLine(DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString()+" "+
						"Error writing RX2 Image Gain value to EEPROM -- "+
						bands[i].ToString()+" - Value out of range [-500.0, 500.0] ("+gain_table[(int)bands[i]].ToString("f4")+").");
					writer.Close();
					gain_table[(int)bands[i]] = 0.0f;
				}
				error_count = 0;
				float test = -999.99f;
				do
				{
					FWC.WriteRX2EEPROMFloat((uint)(gain_offset+i*4), gain_table[(int)bands[i]]);
					//Thread.Sleep(10);

					FWC.ReadRX2EEPROMFloat((uint)(gain_offset+i*4), out test);
					//Thread.Sleep(40);

					if(test != gain_table[(int)bands[i]])
					{
						if(error_count++ > 5)
						{
							MessageBox.Show("Error writing RX2 Image value to EEPROM.\n"+
								"Tried to write "+gain_table[(int)bands[i]].ToString("f4")+", but read back "+test.ToString("f4"));
							gain_sum = phase_sum = 0xFF;
							return;
						}
					}
				} while(test != gain_table[(int)bands[i]]);

				if(phase_table[(int)bands[i]] > 400.0f || phase_table[(int)bands[i]] < -400.0f)
				{
					/*MessageBox.Show("Error writing RX2 Image Phase value to EEPROM.\n"+
						bands[i].ToString()+" - Value out of range [-400.0, 400.0] ("+gain_table[(int)bands[i]].ToString("f4")+").");*/
					TextWriter writer = new StreamWriter(app_data_path+"\\eeprom_error.log", true);
					writer.WriteLine(DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString()+" "+
						"Error writing RX2 Image Phase value to EEPROM -- "+
						bands[i].ToString()+" - Value out of range [-400.0, 400.0] ("+gain_table[(int)bands[i]].ToString("f4")+").");
					writer.Close();
					gain_table[(int)bands[i]] = 0.0f;
				}
				error_count = 0;
				test = -999.99f;
				do
				{
					FWC.WriteRX2EEPROMFloat((uint)(phase_offset+i*4), phase_table[(int)bands[i]]);
					//Thread.Sleep(10);

					FWC.ReadRX2EEPROMFloat((uint)(phase_offset+i*4), out test);
					//Thread.Sleep(40);

					if(test != phase_table[(int)bands[i]])
					{
						if(error_count++ > 5)
						{
							MessageBox.Show("Error writing RX2 Image value to EEPROM.\n"+
								"Tried to write "+gain_table[(int)bands[i]].ToString("f4")+", but read back "+test.ToString("f4"));
							gain_sum = phase_sum = 0xFF;
							return;
						}
					}
				} while(test != phase_table[(int)bands[i]]);
			}

			// calculate and write checksums
			byte sum = Checksum.CalcHF(gain_table);
			WriteRX2ImageGainChecksum(sum);
			gain_sum = sum;

			sum = Checksum.CalcHF(phase_table);
			WriteRX2ImagePhaseChecksum(sum);
			phase_sum = sum;
		}

		public static void WriteRX2ImageGainChecksum(byte sum)
		{
			byte read, errors=0;
			do
			{				
				FWC.WriteRX2EEPROMByte(0x1BF, sum);
				//Thread.Sleep(10);

				FWC.ReadRX2EEPROMByte(0x1BF, out read);
				if(read != sum)
				{
					if(errors++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing RX2 Image Gain checksum to EEPROM.\n"+
							"Tried to write "+sum+", but read back "+read);
						return;
					}
				}
			} while(read != sum);
		}

		public static void WriteRX2ImagePhaseChecksum(byte sum)
		{
			byte read, errors=0;
			do
			{				
				FWC.WriteRX2EEPROMByte(0x1EF, sum);
				//Thread.Sleep(10);

				FWC.ReadRX2EEPROMByte(0x1EF, out read);
				if(read != sum)
				{
					if(errors++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing RX2 Image Phase checksum to EEPROM.\n"+
							"Tried to write "+sum+", but read back "+read);
						return;
					}
				}
			} while(read != sum);
		}

		public static void ReadRX2Image(float[] gain_table, float[] phase_table)
		{
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };

			ushort gain_offset;
			FWC.ReadRX2EEPROMUshort(0x4A, out gain_offset);
			//Thread.Sleep(40);
			if(gain_offset == 0 || gain_offset == 0xFFFF) return;

			ushort phase_offset;
			FWC.ReadRX2EEPROMUshort(0x4C, out phase_offset);
			//Thread.Sleep(40);
			if(phase_offset == 0 || phase_offset == 0xFFFF) return;

			uint data;
			FWC.ReadRX2EEPROMUint(gain_offset, out data);
			//Thread.Sleep(40);
			if(data == 0xFFFFFFFF) return;

			for(uint i=0; i<bands.Length; i++)
			{
				float f;
				FWC.ReadRX2EEPROMFloat((uint)(gain_offset+i*4), out f);
				//Thread.Sleep(40);
				if(f > 500.0f || f < -500.0f)
				{
					/*MessageBox.Show("Bad data detected in EEPROM.\n"+
						"RX2 Image Gain ("+bands[i].ToString()+" = "+f.ToString("f2")+")");*/
					TextWriter writer = new StreamWriter(app_data_path+"\\eeprom_error.log", true);
					writer.WriteLine(DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString()+" "+
						"Bad data detected in EEPROM -- "+
						"RX2 Image Gain ("+bands[i].ToString()+" = "+f.ToString("f2")+")");
					writer.Close();
					f = 0.0f;
				}
				gain_table[(int)bands[i]] = f;
				FWC.ReadRX2EEPROMFloat((uint)(phase_offset+i*4), out f);
				//Thread.Sleep(40);
				if(f > 400.0f || f < -400.0f)
				{
					/*MessageBox.Show("Bad data detected in EEPROM.\n"+
						"RX2 Image Phase ("+bands[i].ToString()+" = "+f.ToString("f2")+")");*/
					TextWriter writer = new StreamWriter(app_data_path+"\\eeprom_error.log", true);
					writer.WriteLine(DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString()+" "+
						"Bad data detected in EEPROM -- "+
						"RX2 Image Phase ("+bands[i].ToString()+" = "+f.ToString("f2")+")");
					writer.Close();
					f = 0.0f;
				}
				phase_table[(int)bands[i]] = f;
			}
		}

		public static byte ReadRX2ImageGainChecksum()
		{
			byte read;
			FWC.ReadRX2EEPROMByte(0x1BF, out read);
			return read;
		}

		public static byte ReadRX2ImagePhaseChecksum()
		{
			byte read;
			FWC.ReadRX2EEPROMByte(0x1EF, out read);
			return read;
		}

		#endregion

		#endregion

		#region ATU

		public static bool CheckATUSWR()
		{
			Random rand = new Random();
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };

			float[] atu_swr_table = new float[(int)Band.LAST];

			for(int i=0; i<bands.Length; i++)
				atu_swr_table[(int)bands[i]] = (float)rand.NextDouble();

			byte temp;
			WriteATUSWR(atu_swr_table, out temp);

			float[] atu_swr_check = new float[(int)Band.LAST];
			
			ReadATUSWR(atu_swr_check);

			for(int i=0; i<bands.Length; i++)
			{
				if(atu_swr_table[(int)bands[i]] != atu_swr_check[(int)bands[i]])
					return false;
			}
			return true;
		}

		public static void WriteATUSWR(float[] table, out byte checksum)
		{
			WriteCalDateTime();
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };
			
			uint addr = 0x5C;
			ushort offset = 0x0660;
			ushort temp2;
			int error_count = 0;
			do
			{
				FWC.WriteTRXEEPROMUshort(addr, offset);
				//Thread.Sleep(10);
				FWC.ReadTRXEEPROMUshort(addr, out temp2);
				//Thread.Sleep(40);

				if(temp2 != offset)
				{
					if(error_count++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing ATU SWR pointer to EEPROM.\n"+
							"Tried to write "+offset.ToString()+", but read back "+temp2.ToString());
						checksum = 0xFF;
						return;
					}
				}
			} while(temp2 != offset);

			for(int i=0; i<bands.Length; i++)
			{
				error_count = 0;
				float temp = 0.0f;
				if(table[(int)bands[i]] > 5.0f || table[(int)bands[i]] < 0.1f)
				{
					MessageBox.Show("Error writing ATU SWR value to EEPROM.\n"+
						"Value out of range 0.1 to 5.0 ("+table[(int)bands[i]].ToString("f4")+").");
					table[(int)bands[i]] = 1.0f;
				}
				do
				{
					FWC.WriteTRXEEPROMFloat((uint)(offset+i*4), (float)table[(int)bands[i]]);
					//Thread.Sleep(10);

					FWC.ReadTRXEEPROMFloat((uint)(offset+i*4), out temp);
					//Thread.Sleep(40);

					if(temp != table[(int)bands[i]])
					{
						if(error_count++ > NUM_WRITES_TO_TRY)
						{
							MessageBox.Show("Error writing PA SWR value to EEPROM.\n"+
								"Tried to write "+table[(int)bands[i]].ToString("f4")+", but read back "+temp.ToString("f4"));
							checksum = 0xFF;
							return;
						}
					}
				} while(temp != table[(int)bands[i]]);
			}

			// calculate and write checksum
			byte sum = Checksum.CalcHF(table);
			WriteATUSWRChecksum(sum);
			checksum = sum;
		}

		public static void WriteATUSWRChecksum(byte sum)
		{
			byte read, errors=0;
			do
			{				
				FWC.WriteTRXEEPROMByte(0x68F, sum);
				//Thread.Sleep(10);

				FWC.ReadTRXEEPROMByte(0x68F, out read);
				if(read != sum)
				{
					if(errors++ > NUM_WRITES_TO_TRY)
					{
						MessageBox.Show("Error writing ATU SWR checksum to EEPROM.\n"+
							"Tried to write "+sum+", but read back "+read);
						return;
					}
				}
			} while(read != sum);
		}

		public static void ReadATUSWR(float[] table)
		{
			Band[] bands = { Band.B160M, Band.B80M, Band.B60M, Band.B40M, Band.B30M, Band.B20M,
							   Band.B17M, Band.B15M, Band.B12M, Band.B10M, Band.B6M };

			ushort offset;
			FWC.ReadTRXEEPROMUshort(0x5C, out offset);
			//Thread.Sleep(40);
			if(offset == 0 || offset == 0xFFFF) return;

			uint data;
			FWC.ReadTRXEEPROMUint(offset, out data);
			//Thread.Sleep(40);
			if(data == 0xFFFFFFFF) return;

			for(int i=0; i<bands.Length; i++)
			{
				float f;
				FWC.ReadTRXEEPROMFloat((uint)(offset+i*4), out f);
				//Thread.Sleep(40);
				if(f > 5.0 || f < 0.1)
				{
					/*MessageBox.Show("Bad data detected in EEPROM.\n"+
						"SWR ("+bands[i].ToString()+") = "+f.ToString("f4"));*/
					TextWriter writer = new StreamWriter(app_data_path+"\\eeprom_error.log", true);
					writer.WriteLine(DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString()+" "+
						"Bad data detected in EEPROM -- "+
						"ATU SWR ("+bands[i].ToString()+") = "+f.ToString("f4"));
					writer.Close();
					table[(int)bands[i]] = 1.0f;
				}
				else table[(int)bands[i]] = (float)Math.Round(f, 4);
			}
		}

		public static byte ReadATUSWRChecksum()
		{
			byte read;
			FWC.ReadTRXEEPROMByte(0x68F, out read);
			return read;
		}


		#endregion

        #region VU

        #region VU Level

        public static bool CheckVULevel()
        {
            Random rand = new Random();

            float[] v_level_table = new float[2];
            float[] u_level_table = new float[2];
            for (int i = 0; i < 2; i++)
            {
                v_level_table[i] = (float)Math.Round(rand.NextDouble(), 1);
                u_level_table[i] = (float)Math.Round(rand.NextDouble(), 1);
            }

            byte temp;
            WriteVULevel(v_level_table, u_level_table, out temp);

            float[] v_level_check = new float[2];
            float[] u_level_check = new float[2];

            ReadVULevel(v_level_check, u_level_check);

            for (int i = 0; i < 2; i++)
            {
                if (v_level_table[i] != v_level_check[i] ||
                    u_level_table[i] != u_level_check[i])
                    return false;
            }
            return true;
        }

        public static void WriteVULevel(float[] vtable, float[] utable, out byte checksum)
        {
            WriteCalDateTime();

            uint addr = 0x5E;
            ushort offset = 0x690, temp;
            int error_count = 0;
            do
            {
                FWC.WriteTRXEEPROMUshort(addr, offset);
                //Thread.Sleep(10);
                FWC.ReadTRXEEPROMUshort(addr, out temp);
                //Thread.Sleep(40);

                if (temp != offset)
                {
                    if (error_count++ > NUM_WRITES_TO_TRY)
                    {
                        MessageBox.Show("Error writing VU Level pointer to EEPROM.\n" +
                            "Tried to write " + offset.ToString() + ", but read back " + temp.ToString());
                        checksum = 0xFF;
                        return;
                    }
                }
            } while (temp != offset);

            for (int i = 0; i < 2; i++)
            {
                error_count = 0;
                float test = -99.99f;
                float val = (float)Math.Round(vtable[i], 1);

                do
                {                    
                    FWC.WriteTRXEEPROMFloat((uint)(offset + i * 4), val);
                    //Thread.Sleep(10);

                    FWC.ReadTRXEEPROMFloat((uint)(offset + i * 4), out test);
                    //Thread.Sleep(40);

                    if (test != val)
                    {
                        if (error_count++ > NUM_WRITES_TO_TRY)
                        {
                            MessageBox.Show("Error writing VU Level value to EEPROM.\n" +
                                "Tried to write " + val.ToString("f4") + ", but read back " + test.ToString("f4"));
                            checksum = 0xFF;
                            return;
                        }
                    }
                } while (test != val);
            }

            for (int i = 0; i < 2; i++)
            {
                error_count = 0;
                float test = -99.99f;
                float val = (float)Math.Round(utable[i], 1);

                do
                {                    
                    FWC.WriteTRXEEPROMFloat((uint)(offset + 8 + i * 4), val);
                    //Thread.Sleep(10);

                    FWC.ReadTRXEEPROMFloat((uint)(offset + 8 + i * 4), out test);
                    //Thread.Sleep(40);

                    if (test != val)
                    {
                        if (error_count++ > NUM_WRITES_TO_TRY)
                        {
                            MessageBox.Show("Error writing VU Level value to EEPROM.\n" +
                                "Tried to write " + val.ToString("f4") + ", but read back " + test.ToString("f4"));
                            checksum = 0xFF;
                            return;
                        }
                    }
                } while (test != val);
            }

            // calculate and write checksum
            byte sum = Checksum.Calc(vtable, utable);
            WriteVULevelChecksum(sum);
            checksum = sum;
        }

        public static void WriteVULevelChecksum(byte sum)
        {
            byte read, errors = 0;
            do
            {
                FWC.WriteTRXEEPROMByte(0x68E, sum);
                //Thread.Sleep(10);

                FWC.ReadTRXEEPROMByte(0x68E, out read);
                if (read != sum)
                {
                    if (errors++ > NUM_WRITES_TO_TRY)
                    {
                        MessageBox.Show("Error writing VU Level checksum to EEPROM.\n" +
                            "Tried to write " + sum + ", but read back " + read);
                        return;
                    }
                }
            } while (read != sum);
        }

        public static void ReadVULevel(float[] vtable, float[] utable)
        {
            ushort offset;
            FWC.ReadTRXEEPROMUshort(0x5E, out offset);
            //Thread.Sleep(40);
            if (offset == 0 || offset == 0xFFFF) return;

            uint data;
            FWC.ReadTRXEEPROMUint(offset, out data);
            //Thread.Sleep(40);
            if (data == 0xFFFFFFFF) return;

            for (int i = 0; i < 2; i++)
            {
                float f;
                FWC.ReadTRXEEPROMFloat((uint)(offset + i * 4), out f);
                //Thread.Sleep(40);
                vtable[i] = (float)Math.Round(f, 1);
            }

            for (int i = 0; i < 2; i++)
            {
                float f;
                FWC.ReadTRXEEPROMFloat((uint)(offset + 8 + i * 4), out f);
                //Thread.Sleep(40);
                utable[i] = (float)Math.Round(f, 1);
            }
        }

        public static byte ReadVULevelChecksum()
        {
            byte read;
            FWC.ReadTRXEEPROMByte(0x68E, out read);
            return read;
        }

        #endregion

        #region VU Power

        public static bool CheckVUPower() // assumes length of tables is V:14  U:22
        {
            const int V_LENGTH = 14;
            const int U_LENGTH = 22;

            Random rand = new Random();

            float[] vtable = new float[V_LENGTH];
            float[] utable = new float[U_LENGTH];

            for (int i = 0; i < V_LENGTH; i++)
                vtable[i] = (float)Math.Round(rand.NextDouble(), 4);

            for(int i=0; i< U_LENGTH; i++)
                utable[i] = (float)Math.Round(rand.NextDouble(), 4);


            byte temp;
            WriteVUPower(vtable, utable, out temp);

            float[] vtable_check = new float[V_LENGTH];
            float[] utable_check = new float[U_LENGTH];
            
            ReadVUPower(vtable_check, utable_check);

            for (int i = 0; i < V_LENGTH; i++)
            {
                if (vtable[i] != vtable_check[i])
                    return false;
            }

            for (int i = 0; i < U_LENGTH; i++)
            {
                if (utable[i] != utable_check[i])
                    return false;
            }

            return true;
        }

        public static void WriteVUPower(float[] vtable, float[] utable, out byte checksum)
        {
            WriteCalDateTime();
            Band[] bands = { Band.VHF0, Band.VHF1 };

            uint addr = 0x60;        //used to be 0x58
            ushort offset = 0x6A0;  //used to be 0x03F0
            ushort temp2;
            int error_count = 0;
            //int table_length = 0;
            do
            {
                FWC.WriteTRXEEPROMUshort(addr, offset);
                //Thread.Sleep(10);
                FWC.ReadTRXEEPROMUshort(addr, out temp2);
                //Thread.Sleep(40);

                if (temp2 != offset)
                {
                    if (error_count++ > NUM_WRITES_TO_TRY)
                    {
                        MessageBox.Show("Error writing PA Power pointer to EEPROM.\n" +
                            "Tried to write " + offset.ToString() + ", but read back " + temp2.ToString());
                        checksum = 0xFF;
                        return;
                    }
                }
            } while (temp2 != offset);

            for (int i = 0; i < vtable.Length; i++)
            {
                float val = (float)Math.Round(vtable[i], 4);

                if (val > 1.0f || val < 0.0f)
                {
                    /*MessageBox.Show("Error writing PA Power value to EEPROM.\n"+
                        "Value out of range 0.0 to 1.0 ("+table[(int)bands[i]][j].ToString("f4")+").");*/
                    TextWriter writer = new StreamWriter(app_data_path + "\\eeprom_error.log", true);
                    writer.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " " +
                        "Error writing PA Power value to EEPROM -- " +
                        "Value out of range 0.0 to 1.0 (" + val.ToString("f4") + ").");
                    writer.Close();
                    val = 0.0f;
                }
                error_count = 0;
                float temp = 0.0f;
                do
                {
                    FWC.WriteTRXEEPROMFloat((uint)(offset + i * 4), val);
                    //Thread.Sleep(10);

                    FWC.ReadTRXEEPROMFloat((uint)(offset + i * 4), out temp);
                    //Thread.Sleep(40);

                    if (temp != val)
                    {
                        if (error_count++ > NUM_WRITES_TO_TRY)
                        {
                            MessageBox.Show("Error writing PA Power value to EEPROM.\n" +
                                "Tried to write " + val.ToString("f4") + ", but read back " + temp.ToString("f4"));
                            checksum = 0xFF;
                            return;
                        }
                    }
                } while (temp != val);
            }

            for (int i = 0; i < utable.Length; i++)
            {
                float val = (float)Math.Round(utable[i], 4);

                if (val > 1.0f || val < 0.0f)
                {
                    /*MessageBox.Show("Error writing PA Power value to EEPROM.\n"+
                        "Value out of range 0.0 to 1.0 ("+table[(int)bands[i]][j].ToString("f4")+").");*/
                    TextWriter writer = new StreamWriter(app_data_path + "\\eeprom_error.log", true);
                    writer.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " " +
                        "Error writing PA Power value to EEPROM -- " +
                        "Value out of range 0.0 to 1.0 (" + val.ToString("f4") + ").");
                    writer.Close();
                    val = 0.0f;
                }
                error_count = 0;
                float temp = 0.0f;
                do
                {
                    FWC.WriteTRXEEPROMFloat((uint)(offset + 80 + i * 4), val);
                    //Thread.Sleep(10);

                    FWC.ReadTRXEEPROMFloat((uint)(offset + 80 + i * 4), out temp);
                    //Thread.Sleep(40);

                    if (temp != val)
                    {
                        if (error_count++ > NUM_WRITES_TO_TRY)
                        {
                            MessageBox.Show("Error writing PA Power value to EEPROM.\n" +
                                "Tried to write " + val.ToString("f4") + ", but read back " + temp.ToString("f4"));
                            checksum = 0xFF;
                            return;
                        }
                    }
                } while (temp != val);
            }

            // calculate and write checksum
            byte sum = Checksum.Calc(vtable);
            sum += Checksum.Calc(utable);
            WritePAPowerChecksum(sum);
            checksum = sum;
        }

        public static void WriteVUPowerChecksum(byte sum)
        {
            byte read, errors = 0;
            do
            {
                FWC.WriteTRXEEPROMByte(0x68D, sum);  //used to be 0x62F
                //Thread.Sleep(10);

                FWC.ReadTRXEEPROMByte(0x68D, out read);
                if (read != sum)
                {
                    if (errors++ > NUM_WRITES_TO_TRY)
                    {
                        MessageBox.Show("Error writing PA Power checksum to EEPROM.\n" +
                            "Tried to write " + sum + ", but read back " + read);
                        return;
                    }
                }
            } while (read != sum);
        }

        public static void ReadVUPower(float[] vtable, float[] utable)
        {
            ushort offset;

            FWC.ReadTRXEEPROMUshort(0x60, out offset);
            //Thread.Sleep(40);
            if (offset == 0 || offset == 0xFFFF) return;
            Debug.Assert(offset == 0x6A0);

            uint data;
            FWC.ReadTRXEEPROMUint(offset, out data);
            //Thread.Sleep(40);
            if (data == 0xFFFFFFFF) return;
                
            for (int i = 0; i < vtable.Length; i++)
            {
                float f;
                FWC.ReadTRXEEPROMFloat((uint)(offset + i * 4), out f);
                //Thread.Sleep(40);
                if (f > 1.0f || f < 0.0f)
                {
                    TextWriter writer = new StreamWriter(app_data_path + "\\eeprom_error.log", true);
                    writer.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " " +
                        "Bad data detected in EEPROM -- " +
                        "VU PA Power V[" + i + "] = " + f.ToString("f4"));
                    writer.Close();
                    vtable[i] = 0.0f;
                }
                else vtable[i] = (float)Math.Round(f, 4);
            }

            for (int i = 0; i < utable.Length; i++)
            {
                float f;
                FWC.ReadTRXEEPROMFloat((uint)(offset + 80 + i * 4), out f);
                //Thread.Sleep(40);
                if (f > 1.0f || f < 0.0f)
                {
                    TextWriter writer = new StreamWriter(app_data_path + "\\eeprom_error.log", true);
                    writer.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " " +
                        "Bad data detected in EEPROM -- " +
                        "VU PA Power U[" + i + "] = " + f.ToString("f4"));
                    writer.Close();
                    utable[i] = 0.0f;
                }
                else utable[i] = (float)Math.Round(f, 4);
            }
        }

        public static byte ReadVUPowerChecksum()
        {
            byte read;
            FWC.ReadTRXEEPROMByte(0x68D, out read);
            return read;
        }
        #endregion

        #endregion
    }
}