//=============================================================================
// SetupIF.cs
//=============================================================================
// Author(s): Scott McClements (WU2X), Chad Gatesman (W1CEG)
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.IO.Ports;
using SDRSerialSupportII;


namespace PowerSDR
{

	public partial class SetupIF : Form
	{

		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private Console console;
		private RigHW hw = null;

		public SetupIF(Console c, AbstractHW hw)
		{
			this.console = c;

			if (hw is RigHW)
				this.hw = (RigHW) hw;

			this.InitializeComponent();

			this.RefreshCOMPortLists();

			// Set Default Values before reading in from database...
			this.comboRigType.Text = "Kenwood TS-940S";

			if (comboRigPort.Items.Count > 0)
				this.comboRigPort.SelectedIndex = 0;
			this.comboRigBaud.Text = "4800";
			this.comboRigParity.Text = "none";
			this.comboRigDataBits.Text = "8";
			this.comboRigStopBits.Text = "1";

			// Read from database...
			this.GetOptions();
		}

		private void btnCancel_Click(object sender,System.EventArgs e)
		{
			Thread t = new Thread(new ThreadStart(GetOptions));
			t.Name = "Save Options Thread";
			t.IsBackground = true;
			t.Priority = ThreadPriority.Lowest;
			t.Start();
			this.Hide();
		}

		private void btnApply_Click(object sender,System.EventArgs e)
		{
			Thread t = new Thread(new ThreadStart(ApplyOptions));
			t.Name = "Save Options Thread";
			t.IsBackground = true;
			t.Priority = ThreadPriority.Lowest;
			t.Start();
		}

		private void ApplyOptions()
		{
			if (saving) return;
			SaveOptions();
			DB.Update();
		}

		private void btnOK_Click(object sender,System.EventArgs e)
		{
			Thread t = new Thread(new ThreadStart(SaveOptions));
			t.Name = "Save Options Thread";
			t.IsBackground = true;
			t.Priority = ThreadPriority.Lowest;
			t.Start();
			this.Hide();
		}

		private static bool saving = false;

		private void ControlList(Control c,ref ArrayList a)
		{
			if (c.Controls.Count > 0)
			{
				foreach (Control c2 in c.Controls)
					ControlList(c2,ref a);
			}

			if (c.GetType() == typeof(CheckBoxTS) || c.GetType() == typeof(CheckBoxTS) ||
				c.GetType() == typeof(ComboBoxTS) || c.GetType() == typeof(ComboBox) ||
				c.GetType() == typeof(NumericUpDownTS) || c.GetType() == typeof(NumericUpDown) ||
				c.GetType() == typeof(RadioButtonTS) || c.GetType() == typeof(RadioButton) ||
				c.GetType() == typeof(TextBoxTS) || c.GetType() == typeof(TextBox) ||
				c.GetType() == typeof(TrackBarTS) || c.GetType() == typeof(TrackBar) ||
				c.GetType() == typeof(ColorButton))
				a.Add(c);
		}

		private void btnImportDB_Click(object sender,System.EventArgs e)
		{
			string path = Application.StartupPath;
			path = path.Substring(0,path.LastIndexOf("\\"));
			openFileDialog1.InitialDirectory = path;
			openFileDialog1.ShowDialog();
		}

		private void btnResetDB_Click(object sender,System.EventArgs e)
		{
			DialogResult dr = MessageBox.Show("This will close the program, make a copy of the current\n" +
				"database to your desktop, and reset the active database\n" +
				"the next time PowerSDR is launched.\n\n" +
				"Are you sure you want to reset the database?",
				"Reset Database?",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Warning);

			if (dr == DialogResult.No) return;

			// WU2X: TODO: ??? This is probably reseting the wrong database ..
			console.reset_db = true;
			console.Close();
		}

		public void SaveOptions()
		{
			// Automatically saves all control settings to the database in the tab
			// pages on this form of the following types: CheckBoxTS, ComboBox,
			// NumericUpDown, RadioButton, TextBox, and TrackBar (slider)

			saving = true;

			ArrayList a = new ArrayList();
			ArrayList temp = new ArrayList();

			ControlList(this,ref temp);

			foreach (Control c in temp)				// For each control
			{
				if (c.GetType() == typeof(CheckBoxTS))
					a.Add(c.Name + "/" + ((CheckBoxTS) c).Checked.ToString());
				else if (c.GetType() == typeof(ComboBoxTS))
				{
					//if(((ComboBox)c).SelectedIndex >= 0)
					a.Add(c.Name + "/" + ((ComboBoxTS) c).Text);
				}
				else if (c.GetType() == typeof(NumericUpDownTS))
					a.Add(c.Name + "/" + ((NumericUpDownTS) c).Value.ToString());
				else if (c.GetType() == typeof(RadioButtonTS))
					a.Add(c.Name + "/" + ((RadioButtonTS) c).Checked.ToString());
				else if (c.GetType() == typeof(TextBoxTS))
					a.Add(c.Name + "/" + ((TextBoxTS) c).Text);
				else if (c.GetType() == typeof(TrackBarTS))
					a.Add(c.Name + "/" + ((TrackBarTS) c).Value.ToString());
				else if (c.GetType() == typeof(ColorButton))
				{
					Color clr = ((ColorButton) c).Color;
					a.Add(c.Name + "/" + clr.R + "." + clr.G + "." + clr.B + "." + clr.A);
				}

			}

			DatabaseIF.SaveVars("Options",ref a);		// save the values to the DB
			saving = false;
		}

		public void GetOptions()
		{
			// Automatically restores all controls from the database in the
			// tab pages on this form of the following types: CheckBoxTS, ComboBox,
			// NumericUpDown, RadioButton, TextBox, and TrackBar (slider)

			// get list of live controls
			ArrayList temp = new ArrayList();		// list of all first level controls
			ControlList(this,ref temp);

			ArrayList checkbox_list = new ArrayList();
			ArrayList combobox_list = new ArrayList();
			ArrayList numericupdown_list = new ArrayList();
			ArrayList radiobutton_list = new ArrayList();
			ArrayList textbox_list = new ArrayList();
			ArrayList trackbar_list = new ArrayList();
			ArrayList colorbutton_list = new ArrayList();

			//ArrayList controls = new ArrayList();	// list of controls to restore
			foreach (Control c in temp)
			{
				if (c.GetType() == typeof(CheckBoxTS))			// the control is a CheckBoxTS
					checkbox_list.Add(c);
				else if (c.GetType() == typeof(ComboBoxTS))		// the control is a ComboBox
					combobox_list.Add(c);
				else if (c.GetType() == typeof(NumericUpDownTS))	// the control is a NumericUpDown
					numericupdown_list.Add(c);
				else if (c.GetType() == typeof(RadioButtonTS))	// the control is a RadioButton
					radiobutton_list.Add(c);
				else if (c.GetType() == typeof(TextBoxTS))		// the control is a TextBox
					textbox_list.Add(c);
				else if (c.GetType() == typeof(TrackBarTS))		// the control is a TrackBar (slider)
					trackbar_list.Add(c);
				else if (c.GetType() == typeof(ColorButton))
					colorbutton_list.Add(c);
			}
			temp.Clear();	// now that we have the controls we want, delete first list 

			ArrayList a = DatabaseIF.GetVars("Options");						// Get the saved list of controls
			a.Sort();
			int num_controls = checkbox_list.Count + combobox_list.Count +
				numericupdown_list.Count + radiobutton_list.Count +
				textbox_list.Count + trackbar_list.Count +
				colorbutton_list.Count;


			// restore saved values to the controls
			foreach (string s in a)				// string is in the format "name,value"
			{
				string[] vals = s.Split('/');
				if (vals.Length > 2)
				{
					for (int i = 2; i < vals.Length; i++)
						vals[1] += "/" + vals[i];
				}

				string name = vals[0];
				string val = vals[1];

				if (s.StartsWith("chk"))			// control is a CheckBoxTS
				{
					for (int i = 0; i < checkbox_list.Count; i++)
					{	// look through each control to find the matching name
						CheckBoxTS c = (CheckBoxTS) checkbox_list[i];
						if (c.Name.Equals(name))		// name found
						{
							c.Checked = bool.Parse(val);	// restore value
							i = checkbox_list.Count + 1;
						}
						if (i == checkbox_list.Count)
							MessageBox.Show("Control not found: " + name);
					}
				}
				else if (s.StartsWith("combo"))	// control is a ComboBox
				{
					for (int i = 0; i < combobox_list.Count; i++)
					{	// look through each control to find the matching name
						ComboBoxTS c = (ComboBoxTS) combobox_list[i];
						if (c.Name.Equals(name))		// name found
						{
							if (c.Items.Count > 0 && c.Items[0].GetType() == typeof(string))
							{
								c.Text = val;
							}
							else
							{
								foreach (object o in c.Items)
								{
									if (o.ToString() == val)
										c.Text = val;	// restore value
								}
							}
							i = combobox_list.Count + 1;
						}
						if (i == combobox_list.Count)
							MessageBox.Show("Control not found: " + name);
					}
				}
				else if (s.StartsWith("ud"))
				{
					for (int i = 0; i < numericupdown_list.Count; i++)
					{	// look through each control to find the matching name
						NumericUpDownTS c = (NumericUpDownTS) numericupdown_list[i];
						if (c.Name.Equals(name))		// name found
						{
							decimal num = decimal.Parse(val);

							if (num > c.Maximum) num = c.Maximum;		// check endpoints
							else if (num < c.Minimum) num = c.Minimum;
							c.Value = num;			// restore value
							i = numericupdown_list.Count + 1;
						}
						if (i == numericupdown_list.Count)
							MessageBox.Show("Control not found: " + name);
					}
				}
				else if (s.StartsWith("rad"))
				{	// look through each control to find the matching name
					for (int i = 0; i < radiobutton_list.Count; i++)
					{
						RadioButtonTS c = (RadioButtonTS) radiobutton_list[i];
						if (c.Name.Equals(name))		// name found
						{
							c.Checked = bool.Parse(val);	// restore value
							i = radiobutton_list.Count + 1;
						}
						if (i == radiobutton_list.Count)
							MessageBox.Show("Control not found: " + name);
					}
				}
				else if (s.StartsWith("txt"))
				{	// look through each control to find the matching name
					for (int i = 0; i < textbox_list.Count; i++)
					{
						TextBoxTS c = (TextBoxTS) textbox_list[i];
						if (c.Name.Equals(name))		// name found
						{
							c.Text = val;	// restore value
							i = textbox_list.Count + 1;
						}
						if (i == textbox_list.Count)
							MessageBox.Show("Control not found: " + name);
					}
				}
				else if (s.StartsWith("tb"))
				{
					// look through each control to find the matching name
					for (int i = 0; i < trackbar_list.Count; i++)
					{
						TrackBarTS c = (TrackBarTS) trackbar_list[i];
						if (c.Name.Equals(name))		// name found
						{
							c.Value = Int32.Parse(val);
							i = trackbar_list.Count + 1;
						}
						if (i == trackbar_list.Count)
							MessageBox.Show("Control not found: " + name);
					}
				}
				else if (s.StartsWith("clrbtn"))
				{
					string[] colors = val.Split('.');
					if (colors.Length == 4)
					{
						int R,G,B,A;
						R = Int32.Parse(colors[0]);
						G = Int32.Parse(colors[1]);
						B = Int32.Parse(colors[2]);
						A = Int32.Parse(colors[3]);

						for (int i = 0; i < colorbutton_list.Count; i++)
						{
							ColorButton c = (ColorButton) colorbutton_list[i];
							if (c.Name.Equals(name))		// name found
							{
								c.Color = Color.FromArgb(A,R,G,B);
								i = colorbutton_list.Count + 1;
							}
							if (i == colorbutton_list.Count)
								MessageBox.Show("Control not found: " + name);
						}
					}
				}
			}

			foreach (ColorButton c in colorbutton_list)
				c.Automatic = "";
		}

		private void RefreshCOMPortLists()
		{
			string[] comPorts = SerialPort.GetPortNames();
			Array.Sort(comPorts);

			this.comboRigPort.Items.Clear();

			foreach (string s in comPorts)
				this.comboRigPort.Items.Add(s);
		}

		private void udIFGlobalOffset_ValueChanged(object sender,System.EventArgs e)
		{
			console.globalIFOffset = (double) udIFGlobalOffset.Value * 1e-6;
		}

		private void udIFLSB_ValueChanged(object sender,System.EventArgs e)
		{
			console.IFLSB = (double) udIFLSB.Value * 1e-6;
		}

		private void udIFUSB_ValueChanged(object sender,System.EventArgs e)
		{
			console.IFUSB = (double) udIFUSB.Value * 1e-6;
		}

		private void udIFCW_ValueChanged(object sender,System.EventArgs e)
		{
			console.IFCW = (double) udIFCW.Value * 1e-6;
		}

		private void udIFAM_ValueChanged(object sender,System.EventArgs e)
		{
			console.IFAM = (double) udIFAM.Value * 1e-6;
		}

		private void udIFFM_ValueChanged(object sender,System.EventArgs e)
		{
			console.IFFM = (double) udIFFM.Value * 1e-6;
		}

		private void udIFFSK_ValueChanged(object sender,System.EventArgs e)
		{
			console.IFFSK = (double) udIFFSK.Value * 1e-6;
		}

		private void comboRigPort_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.hw == null)
				return;

			if (this.comboRigPort.Text.StartsWith("COM"))
				this.console.RigCOMPort = Int32.Parse(this.comboRigPort.Text.Substring(3));
		}

		private void comboRigParity_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.hw == null)
				return;

			string selection = this.comboRigParity.SelectedText;

			if (selection != null)
				this.console.RigCOMParity = SDRSerialPort.stringToParity(selection);
		}

		private void comboRigBaud_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.hw == null)
				return;

			if (this.comboRigBaud.SelectedIndex >= 0)
				this.console.RigCOMBaudRate = Int32.Parse(this.comboRigBaud.Text);
		}

		private void comboRigDataBits_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.hw == null)
				return;

			if (this.comboRigDataBits.SelectedIndex >= 0)
				this.console.RigCOMDataBits = SDRSerialPort.stringToDataBits(this.comboRigDataBits.Text);
		}

		private void comboRigStopBits_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.hw == null)
				return;

			if (this.comboRigStopBits.SelectedIndex >= 0)
				this.console.RigCOMStopBits = SDRSerialPort.stringToStopBits(this.comboRigStopBits.Text);
		}

		private void udRigPollingInterval_ValueChanged(object sender, EventArgs e)
		{
			if (this.hw == null)
				return;

			this.console.RigPollingInterval = (int) this.udRigPollingInterval.Value;
		}

		private void udRigTuningPollingInterval_ValueChanged(object sender, EventArgs e)
		{
			if (this.hw == null)
				return;

			this.console.RigPollingInterval = (int) this.udRigTuningPollingInterval.Value;
		}

		private void udRigPollingLockoutTime_ValueChanged(object sender, EventArgs e)
		{
			if (this.hw == null)
				return;

			this.console.RigPollingInterval = (int) this.udRigPollingLockoutTime.Value;
		}

		private void chkRigPollVFOB_CheckedChanged(object sender, EventArgs e)
		{
			if (this.hw == null)
				return;

			this.console.RigPollVFOB = this.chkRigPollVFOB.Checked;
		}

		private void chkRigPollIFFreq_CheckedChanged(object sender, EventArgs e)
		{
			if (this.hw == null)
				return;

			this.console.RigPollIFFreq = this.chkRigPollIFFreq.Checked;
		}

		private void comboRigType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.hw == null)
				return;

			// :TODO: Give the option to power off PowerSDR to make this change.
			if (this.console.PowerOn)
			{
				this.comboRigType.Text = this.console.RigType;
				return;
			}

			this.console.RigType = this.comboRigType.Text;

			this.comboRigBaud.Text = this.hw.defaultBaudRate().ToString();
			this.chkRigPollVFOB.Checked = this.hw.needsPollVFOB();

			if (this.hw.supportsIFFreq())
			{
				this.chkRigPollIFFreq.Enabled = true;
				this.chkRigPollIFFreq.Checked = true;
			}
			else
			{
				this.chkRigPollIFFreq.Enabled = false;
				this.chkRigPollIFFreq.Checked = false;
			}

		}
	}
}
