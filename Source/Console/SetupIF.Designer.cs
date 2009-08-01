//=============================================================================
// SetupIF.Designer.cs
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

namespace PowerSDR
{
    partial class SetupIF
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupIF));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.grpMeterTimingBox = new System.Windows.Forms.GroupBox();
            this.grpRigTimingBox = new System.Windows.Forms.GroupBox();
            this.grpOptionalPollingCommandsBox = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.grpMeterTypeBox = new System.Windows.Forms.GroupBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.grpRigTypeBox = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.chkRigPollIFFreq = new System.Windows.Forms.CheckBoxTS();
            this.chkRigPollVFOB = new System.Windows.Forms.CheckBoxTS();
            this.labelTS6 = new System.Windows.Forms.LabelTS();
            this.udRigTuningCATInterval = new System.Windows.Forms.NumericUpDownTS();
            this.labelRigTuningPollingInterval = new System.Windows.Forms.LabelTS();
            this.udRigTuningPollingInterval = new System.Windows.Forms.NumericUpDownTS();
            this.labelRigPollingLockoutTime = new System.Windows.Forms.LabelTS();
            this.udRigPollingLockoutTime = new System.Windows.Forms.NumericUpDownTS();
            this.labelRigPollingInterval = new System.Windows.Forms.LabelTS();
            this.udRigPollingInterval = new System.Windows.Forms.NumericUpDownTS();
            this.grpRigSerialBox = new System.Windows.Forms.GroupBoxTS();
            this.comboRigPort = new System.Windows.Forms.ComboBoxTS();
            this.comboRigBaud = new System.Windows.Forms.ComboBoxTS();
            this.lblCATBaud = new System.Windows.Forms.LabelTS();
            this.lblCATPort = new System.Windows.Forms.LabelTS();
            this.lblCATParity = new System.Windows.Forms.LabelTS();
            this.lblCATData = new System.Windows.Forms.LabelTS();
            this.lblCATStop = new System.Windows.Forms.LabelTS();
            this.comboRigParity = new System.Windows.Forms.ComboBoxTS();
            this.comboRigDataBits = new System.Windows.Forms.ComboBoxTS();
            this.comboRigStopBits = new System.Windows.Forms.ComboBoxTS();
            this.comboRigType = new System.Windows.Forms.ComboBoxTS();
            this.chkSwapIQ = new System.Windows.Forms.CheckBoxTS();
            this.udSwapFrequency = new System.Windows.Forms.NumericUpDownTS();
            this.udLOCenterFreq = new System.Windows.Forms.NumericUpDownTS();
            this.udIFGlobalOffset = new System.Windows.Forms.NumericUpDownTS();
            this.udIFFSK = new System.Windows.Forms.NumericUpDownTS();
            this.udIFFM = new System.Windows.Forms.NumericUpDownTS();
            this.udIFAM = new System.Windows.Forms.NumericUpDownTS();
            this.udIFCW = new System.Windows.Forms.NumericUpDownTS();
            this.udIFUSB = new System.Windows.Forms.NumericUpDownTS();
            this.udIFLSB = new System.Windows.Forms.NumericUpDownTS();
            this.comboMeterTimingInterval = new System.Windows.Forms.ComboBoxTS();
            this.labelTS8 = new System.Windows.Forms.LabelTS();
            this.chkUseMeter = new System.Windows.Forms.CheckBoxTS();
            this.comboMeterType = new System.Windows.Forms.ComboBoxTS();
            this.grpMeterSerialBox = new System.Windows.Forms.GroupBoxTS();
            this.comboMeterPort = new System.Windows.Forms.ComboBoxTS();
            this.comboMeterBaud = new System.Windows.Forms.ComboBoxTS();
            this.labelTS1 = new System.Windows.Forms.LabelTS();
            this.labelTS2 = new System.Windows.Forms.LabelTS();
            this.labelTS3 = new System.Windows.Forms.LabelTS();
            this.labelTS4 = new System.Windows.Forms.LabelTS();
            this.labelTS5 = new System.Windows.Forms.LabelTS();
            this.comboMeterParity = new System.Windows.Forms.ComboBoxTS();
            this.comboMeterDataBits = new System.Windows.Forms.ComboBoxTS();
            this.comboMeterStopBits = new System.Windows.Forms.ComboBoxTS();
            this.btnImportDB = new System.Windows.Forms.ButtonTS();
            this.btnResetDB = new System.Windows.Forms.ButtonTS();
            this.btnApply = new System.Windows.Forms.ButtonTS();
            this.btnCancel = new System.Windows.Forms.ButtonTS();
            this.btnOK = new System.Windows.Forms.ButtonTS();
            this.grpMeterTimingBox.SuspendLayout();
            this.grpRigTimingBox.SuspendLayout();
            this.grpOptionalPollingCommandsBox.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.grpMeterTypeBox.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.grpRigTypeBox.SuspendLayout();
            this.tabControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udRigTuningCATInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udRigTuningPollingInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udRigPollingLockoutTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udRigPollingInterval)).BeginInit();
            this.grpRigSerialBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udSwapFrequency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udLOCenterFreq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udIFGlobalOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udIFFSK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udIFFM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udIFAM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udIFCW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udIFUSB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udIFLSB)).BeginInit();
            this.grpMeterSerialBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpMeterTimingBox
            // 
            this.grpMeterTimingBox.Controls.Add(this.comboMeterTimingInterval);
            this.grpMeterTimingBox.Controls.Add(this.labelTS8);
            this.grpMeterTimingBox.Enabled = false;
            this.grpMeterTimingBox.Location = new System.Drawing.Point(153, 87);
            this.grpMeterTimingBox.Name = "grpMeterTimingBox";
            this.grpMeterTimingBox.Size = new System.Drawing.Size(194, 53);
            this.grpMeterTimingBox.TabIndex = 94;
            this.grpMeterTimingBox.TabStop = false;
            this.grpMeterTimingBox.Text = "Meter Timing (ms)";
            this.toolTip1.SetToolTip(this.grpMeterTimingBox, "Configurable Timeouts and Intervals used for polling information from the externa" +
                    "l meter");
            // 
            // grpRigTimingBox
            // 
            this.grpRigTimingBox.Controls.Add(this.labelTS6);
            this.grpRigTimingBox.Controls.Add(this.udRigTuningCATInterval);
            this.grpRigTimingBox.Controls.Add(this.labelRigTuningPollingInterval);
            this.grpRigTimingBox.Controls.Add(this.udRigTuningPollingInterval);
            this.grpRigTimingBox.Controls.Add(this.labelRigPollingLockoutTime);
            this.grpRigTimingBox.Controls.Add(this.udRigPollingLockoutTime);
            this.grpRigTimingBox.Controls.Add(this.labelRigPollingInterval);
            this.grpRigTimingBox.Controls.Add(this.udRigPollingInterval);
            this.grpRigTimingBox.Location = new System.Drawing.Point(154, 6);
            this.grpRigTimingBox.Name = "grpRigTimingBox";
            this.grpRigTimingBox.Size = new System.Drawing.Size(194, 166);
            this.grpRigTimingBox.TabIndex = 92;
            this.grpRigTimingBox.TabStop = false;
            this.grpRigTimingBox.Text = "Rig Timing (ms)";
            this.toolTip1.SetToolTip(this.grpRigTimingBox, "Configurable Timeouts and Intervals used for polling information from the Rig");
            // 
            // grpOptionalPollingCommandsBox
            // 
            this.grpOptionalPollingCommandsBox.Controls.Add(this.chkRigPollIFFreq);
            this.grpOptionalPollingCommandsBox.Controls.Add(this.chkRigPollVFOB);
            this.grpOptionalPollingCommandsBox.Location = new System.Drawing.Point(154, 178);
            this.grpOptionalPollingCommandsBox.Name = "grpOptionalPollingCommandsBox";
            this.grpOptionalPollingCommandsBox.Size = new System.Drawing.Size(194, 90);
            this.grpOptionalPollingCommandsBox.TabIndex = 93;
            this.grpOptionalPollingCommandsBox.TabStop = false;
            this.grpOptionalPollingCommandsBox.Text = "Optional Information To Poll";
            this.toolTip1.SetToolTip(this.grpOptionalPollingCommandsBox, "Additional Commands to polled on the Rig");
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(6, 45);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(97, 20);
            this.label8.TabIndex = 10;
            this.label8.Text = "LO Center:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.label8, "Local Oscillator Center Frequency of External SDR");
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.Transparent;
            this.tabPage3.Controls.Add(this.grpMeterTimingBox);
            this.tabPage3.Controls.Add(this.grpMeterTypeBox);
            this.tabPage3.Controls.Add(this.grpMeterSerialBox);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(584, 275);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Meter Connection";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // grpMeterTypeBox
            // 
            this.grpMeterTypeBox.Controls.Add(this.chkUseMeter);
            this.grpMeterTypeBox.Controls.Add(this.comboMeterType);
            this.grpMeterTypeBox.Location = new System.Drawing.Point(6, 6);
            this.grpMeterTypeBox.Name = "grpMeterTypeBox";
            this.grpMeterTypeBox.Size = new System.Drawing.Size(183, 75);
            this.grpMeterTypeBox.TabIndex = 92;
            this.grpMeterTypeBox.TabStop = false;
            this.grpMeterTypeBox.Text = "Meter Type";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Transparent;
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(584, 275);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "IF Frequencies";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkSwapIQ);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.udSwapFrequency);
            this.groupBox3.Location = new System.Drawing.Point(215, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(198, 91);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Swap I/Q @ Frequency  (Mhz)";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(6, 47);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(66, 20);
            this.label9.TabIndex = 4;
            this.label9.Text = "Frequency";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.udLOCenterFreq);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.udIFGlobalOffset);
            this.groupBox2.Location = new System.Drawing.Point(11, 192);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(198, 77);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Global IF Frequency Adjustments (Hz)";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(6, 19);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(97, 20);
            this.label7.TabIndex = 8;
            this.label7.Text = "Global Offset:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.udIFFSK);
            this.groupBox1.Controls.Add(this.udIFFM);
            this.groupBox1.Controls.Add(this.udIFAM);
            this.groupBox1.Controls.Add(this.udIFCW);
            this.groupBox1.Controls.Add(this.udIFUSB);
            this.groupBox1.Controls.Add(this.udIFLSB);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(11, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(198, 180);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "IF Frequency (Hz)";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(76, 149);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 20);
            this.label6.TabIndex = 5;
            this.label6.Text = "FSK";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(81, 123);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(22, 20);
            this.label5.TabIndex = 4;
            this.label5.Text = "FM";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(80, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "AM";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(78, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "CW";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(74, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "USB";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(76, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "LSB";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.Controls.Add(this.grpOptionalPollingCommandsBox);
            this.tabPage1.Controls.Add(this.grpRigTimingBox);
            this.tabPage1.Controls.Add(this.grpRigSerialBox);
            this.tabPage1.Controls.Add(this.grpRigTypeBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(584, 275);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Rig Connection";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // grpRigTypeBox
            // 
            this.grpRigTypeBox.Controls.Add(this.comboRigType);
            this.grpRigTypeBox.Location = new System.Drawing.Point(6, 6);
            this.grpRigTypeBox.Name = "grpRigTypeBox";
            this.grpRigTypeBox.Size = new System.Drawing.Size(141, 55);
            this.grpRigTypeBox.TabIndex = 0;
            this.grpRigTypeBox.TabStop = false;
            this.grpRigTypeBox.Text = "Rig Type";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(592, 301);
            this.tabControl1.TabIndex = 27;
            // 
            // chkRigPollIFFreq
            // 
            this.chkRigPollIFFreq.AutoSize = true;
            this.chkRigPollIFFreq.Enabled = false;
            this.chkRigPollIFFreq.Image = null;
            this.chkRigPollIFFreq.Location = new System.Drawing.Point(9, 42);
            this.chkRigPollIFFreq.Name = "chkRigPollIFFreq";
            this.chkRigPollIFFreq.Size = new System.Drawing.Size(88, 17);
            this.chkRigPollIFFreq.TabIndex = 10;
            this.chkRigPollIFFreq.Text = "IF Frequency";
            this.toolTip1.SetToolTip(this.chkRigPollIFFreq, "Poll for IF Frequency");
            this.chkRigPollIFFreq.UseVisualStyleBackColor = true;
            this.chkRigPollIFFreq.CheckedChanged += new System.EventHandler(this.chkRigPollIFFreq_CheckedChanged);
            // 
            // chkRigPollVFOB
            // 
            this.chkRigPollVFOB.AutoSize = true;
            this.chkRigPollVFOB.Image = null;
            this.chkRigPollVFOB.Location = new System.Drawing.Point(9, 19);
            this.chkRigPollVFOB.Name = "chkRigPollVFOB";
            this.chkRigPollVFOB.Size = new System.Drawing.Size(57, 17);
            this.chkRigPollVFOB.TabIndex = 9;
            this.chkRigPollVFOB.Text = "VFO-B";
            this.toolTip1.SetToolTip(this.chkRigPollVFOB, "Poll for VFO-B");
            this.chkRigPollVFOB.UseVisualStyleBackColor = true;
            this.chkRigPollVFOB.CheckedChanged += new System.EventHandler(this.chkRigPollVFOB_CheckedChanged);
            // 
            // labelTS6
            // 
            this.labelTS6.Image = null;
            this.labelTS6.Location = new System.Drawing.Point(6, 71);
            this.labelTS6.Name = "labelTS6";
            this.labelTS6.Size = new System.Drawing.Size(120, 20);
            this.labelTS6.TabIndex = 20;
            this.labelTS6.Text = "Tuning CAT Interval:";
            this.labelTS6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udRigTuningCATInterval
            // 
            this.udRigTuningCATInterval.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udRigTuningCATInterval.Location = new System.Drawing.Point(132, 71);
            this.udRigTuningCATInterval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.udRigTuningCATInterval.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udRigTuningCATInterval.Name = "udRigTuningCATInterval";
            this.udRigTuningCATInterval.Size = new System.Drawing.Size(51, 20);
            this.udRigTuningCATInterval.TabIndex = 8;
            this.toolTip1.SetToolTip(this.udRigTuningCATInterval, "Interval at which frequency information is sent to the Rig while tuning in PowerS" +
                    "DR");
            this.udRigTuningCATInterval.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.udRigTuningCATInterval.ValueChanged += new System.EventHandler(this.udRigTuningCATInterval_ValueChanged);
            // 
            // labelRigTuningPollingInterval
            // 
            this.labelRigTuningPollingInterval.Image = null;
            this.labelRigTuningPollingInterval.Location = new System.Drawing.Point(6, 45);
            this.labelRigTuningPollingInterval.Name = "labelRigTuningPollingInterval";
            this.labelRigTuningPollingInterval.Size = new System.Drawing.Size(120, 20);
            this.labelRigTuningPollingInterval.TabIndex = 18;
            this.labelRigTuningPollingInterval.Text = "Tuning Polling Interval:";
            this.labelRigTuningPollingInterval.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udRigTuningPollingInterval
            // 
            this.udRigTuningPollingInterval.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udRigTuningPollingInterval.Location = new System.Drawing.Point(132, 45);
            this.udRigTuningPollingInterval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.udRigTuningPollingInterval.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udRigTuningPollingInterval.Name = "udRigTuningPollingInterval";
            this.udRigTuningPollingInterval.Size = new System.Drawing.Size(51, 20);
            this.udRigTuningPollingInterval.TabIndex = 7;
            this.toolTip1.SetToolTip(this.udRigTuningPollingInterval, "Interval at which frequency information is polled from the Rig while tuning on th" +
                    "e Rig\'s VFO knob");
            this.udRigTuningPollingInterval.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.udRigTuningPollingInterval.ValueChanged += new System.EventHandler(this.udRigTuningPollingInterval_ValueChanged);
            // 
            // labelRigPollingLockoutTime
            // 
            this.labelRigPollingLockoutTime.Image = null;
            this.labelRigPollingLockoutTime.Location = new System.Drawing.Point(6, 97);
            this.labelRigPollingLockoutTime.Name = "labelRigPollingLockoutTime";
            this.labelRigPollingLockoutTime.Size = new System.Drawing.Size(120, 20);
            this.labelRigPollingLockoutTime.TabIndex = 16;
            this.labelRigPollingLockoutTime.Text = "Polling Lockout Time:";
            this.labelRigPollingLockoutTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udRigPollingLockoutTime
            // 
            this.udRigPollingLockoutTime.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udRigPollingLockoutTime.Location = new System.Drawing.Point(132, 97);
            this.udRigPollingLockoutTime.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.udRigPollingLockoutTime.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.udRigPollingLockoutTime.Name = "udRigPollingLockoutTime";
            this.udRigPollingLockoutTime.Size = new System.Drawing.Size(51, 20);
            this.udRigPollingLockoutTime.TabIndex = 9;
            this.toolTip1.SetToolTip(this.udRigPollingLockoutTime, "Amount of time to lockout Rig Polling while Rig commands are executed as a result" +
                    " of UI changes");
            this.udRigPollingLockoutTime.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.udRigPollingLockoutTime.ValueChanged += new System.EventHandler(this.udRigPollingLockoutTime_ValueChanged);
            // 
            // labelRigPollingInterval
            // 
            this.labelRigPollingInterval.Image = null;
            this.labelRigPollingInterval.Location = new System.Drawing.Point(6, 19);
            this.labelRigPollingInterval.Name = "labelRigPollingInterval";
            this.labelRigPollingInterval.Size = new System.Drawing.Size(120, 20);
            this.labelRigPollingInterval.TabIndex = 14;
            this.labelRigPollingInterval.Text = "Polling Interval:";
            this.labelRigPollingInterval.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udRigPollingInterval
            // 
            this.udRigPollingInterval.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udRigPollingInterval.Location = new System.Drawing.Point(132, 19);
            this.udRigPollingInterval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.udRigPollingInterval.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.udRigPollingInterval.Name = "udRigPollingInterval";
            this.udRigPollingInterval.Size = new System.Drawing.Size(51, 20);
            this.udRigPollingInterval.TabIndex = 6;
            this.toolTip1.SetToolTip(this.udRigPollingInterval, "Interval at which information is polled from the Rig");
            this.udRigPollingInterval.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.udRigPollingInterval.ValueChanged += new System.EventHandler(this.udRigPollingInterval_ValueChanged);
            // 
            // grpRigSerialBox
            // 
            this.grpRigSerialBox.Controls.Add(this.comboRigPort);
            this.grpRigSerialBox.Controls.Add(this.comboRigBaud);
            this.grpRigSerialBox.Controls.Add(this.lblCATBaud);
            this.grpRigSerialBox.Controls.Add(this.lblCATPort);
            this.grpRigSerialBox.Controls.Add(this.lblCATParity);
            this.grpRigSerialBox.Controls.Add(this.lblCATData);
            this.grpRigSerialBox.Controls.Add(this.lblCATStop);
            this.grpRigSerialBox.Controls.Add(this.comboRigParity);
            this.grpRigSerialBox.Controls.Add(this.comboRigDataBits);
            this.grpRigSerialBox.Controls.Add(this.comboRigStopBits);
            this.grpRigSerialBox.Location = new System.Drawing.Point(6, 67);
            this.grpRigSerialBox.Name = "grpRigSerialBox";
            this.grpRigSerialBox.Size = new System.Drawing.Size(141, 201);
            this.grpRigSerialBox.TabIndex = 91;
            this.grpRigSerialBox.TabStop = false;
            this.grpRigSerialBox.Text = "Rig Serial Connection";
            // 
            // comboRigPort
            // 
            this.comboRigPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboRigPort.DropDownWidth = 56;
            this.comboRigPort.Location = new System.Drawing.Point(60, 19);
            this.comboRigPort.Name = "comboRigPort";
            this.comboRigPort.Size = new System.Drawing.Size(72, 21);
            this.comboRigPort.TabIndex = 1;
            this.toolTip1.SetToolTip(this.comboRigPort, "Sets the COM port to be used for communicating with the Rig.");
            this.comboRigPort.SelectedIndexChanged += new System.EventHandler(this.comboRigPort_SelectedIndexChanged);
            // 
            // comboRigBaud
            // 
            this.comboRigBaud.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboRigBaud.DropDownWidth = 56;
            this.comboRigBaud.Items.AddRange(new object[] {
            "300",
            "1200",
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "57600"});
            this.comboRigBaud.Location = new System.Drawing.Point(60, 46);
            this.comboRigBaud.Name = "comboRigBaud";
            this.comboRigBaud.Size = new System.Drawing.Size(72, 21);
            this.comboRigBaud.TabIndex = 2;
            this.comboRigBaud.SelectedIndexChanged += new System.EventHandler(this.comboRigBaud_SelectedIndexChanged);
            // 
            // lblCATBaud
            // 
            this.lblCATBaud.Image = null;
            this.lblCATBaud.Location = new System.Drawing.Point(14, 46);
            this.lblCATBaud.Name = "lblCATBaud";
            this.lblCATBaud.Size = new System.Drawing.Size(40, 23);
            this.lblCATBaud.TabIndex = 5;
            this.lblCATBaud.Text = "Baud:";
            this.lblCATBaud.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCATPort
            // 
            this.lblCATPort.Image = null;
            this.lblCATPort.Location = new System.Drawing.Point(14, 19);
            this.lblCATPort.Name = "lblCATPort";
            this.lblCATPort.Size = new System.Drawing.Size(40, 23);
            this.lblCATPort.TabIndex = 3;
            this.lblCATPort.Text = "Port:";
            this.lblCATPort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCATParity
            // 
            this.lblCATParity.Image = null;
            this.lblCATParity.Location = new System.Drawing.Point(6, 73);
            this.lblCATParity.Name = "lblCATParity";
            this.lblCATParity.Size = new System.Drawing.Size(48, 23);
            this.lblCATParity.TabIndex = 92;
            this.lblCATParity.Text = "Parity:";
            this.lblCATParity.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCATData
            // 
            this.lblCATData.Image = null;
            this.lblCATData.Location = new System.Drawing.Point(14, 100);
            this.lblCATData.Name = "lblCATData";
            this.lblCATData.Size = new System.Drawing.Size(40, 23);
            this.lblCATData.TabIndex = 92;
            this.lblCATData.Text = "Data:";
            this.lblCATData.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCATStop
            // 
            this.lblCATStop.Image = null;
            this.lblCATStop.Location = new System.Drawing.Point(14, 127);
            this.lblCATStop.Name = "lblCATStop";
            this.lblCATStop.Size = new System.Drawing.Size(40, 23);
            this.lblCATStop.TabIndex = 92;
            this.lblCATStop.Text = "Stop:";
            this.lblCATStop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboRigParity
            // 
            this.comboRigParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboRigParity.DropDownWidth = 56;
            this.comboRigParity.Items.AddRange(new object[] {
            "none",
            "odd ",
            "even",
            "mark",
            "space"});
            this.comboRigParity.Location = new System.Drawing.Point(60, 73);
            this.comboRigParity.Name = "comboRigParity";
            this.comboRigParity.Size = new System.Drawing.Size(72, 21);
            this.comboRigParity.TabIndex = 3;
            this.comboRigParity.SelectedIndexChanged += new System.EventHandler(this.comboRigParity_SelectedIndexChanged);
            // 
            // comboRigDataBits
            // 
            this.comboRigDataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboRigDataBits.DropDownWidth = 56;
            this.comboRigDataBits.Items.AddRange(new object[] {
            "8",
            "7",
            "6"});
            this.comboRigDataBits.Location = new System.Drawing.Point(60, 100);
            this.comboRigDataBits.Name = "comboRigDataBits";
            this.comboRigDataBits.Size = new System.Drawing.Size(72, 21);
            this.comboRigDataBits.TabIndex = 4;
            this.comboRigDataBits.SelectedIndexChanged += new System.EventHandler(this.comboRigDataBits_SelectedIndexChanged);
            // 
            // comboRigStopBits
            // 
            this.comboRigStopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboRigStopBits.DropDownWidth = 56;
            this.comboRigStopBits.Items.AddRange(new object[] {
            "1",
            "1.5",
            "2"});
            this.comboRigStopBits.Location = new System.Drawing.Point(60, 127);
            this.comboRigStopBits.Name = "comboRigStopBits";
            this.comboRigStopBits.Size = new System.Drawing.Size(72, 21);
            this.comboRigStopBits.TabIndex = 5;
            this.comboRigStopBits.SelectedIndexChanged += new System.EventHandler(this.comboRigStopBits_SelectedIndexChanged);
            // 
            // comboRigType
            // 
            this.comboRigType.FormattingEnabled = true;
            this.comboRigType.Items.AddRange(new object[] {
            "Elecraft K3",
            "Kenwood TS-940S",
            "Ham Radio Deluxe"});
            this.comboRigType.Location = new System.Drawing.Point(9, 20);
            this.comboRigType.Name = "comboRigType";
            this.comboRigType.Size = new System.Drawing.Size(123, 21);
            this.comboRigType.TabIndex = 0;
            this.toolTip1.SetToolTip(this.comboRigType, "Defines what kind of rig will control PowerSDR");
            this.comboRigType.SelectedIndexChanged += new System.EventHandler(this.comboRigType_SelectedIndexChanged);
            // 
            // chkSwapIQ
            // 
            this.chkSwapIQ.AutoSize = true;
            this.chkSwapIQ.Image = null;
            this.chkSwapIQ.Location = new System.Drawing.Point(22, 24);
            this.chkSwapIQ.Name = "chkSwapIQ";
            this.chkSwapIQ.Size = new System.Drawing.Size(119, 17);
            this.chkSwapIQ.TabIndex = 5;
            this.chkSwapIQ.Text = "Swap I/Q Channels";
            this.chkSwapIQ.UseVisualStyleBackColor = true;
            this.chkSwapIQ.CheckedChanged += new System.EventHandler(this.swapIQ_CheckedChanged);
            // 
            // udSwapFrequency
            // 
            this.udSwapFrequency.DecimalPlaces = 6;
            this.udSwapFrequency.Enabled = false;
            this.udSwapFrequency.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udSwapFrequency.Location = new System.Drawing.Point(78, 49);
            this.udSwapFrequency.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udSwapFrequency.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.udSwapFrequency.Name = "udSwapFrequency";
            this.udSwapFrequency.Size = new System.Drawing.Size(78, 20);
            this.udSwapFrequency.TabIndex = 3;
            this.toolTip1.SetToolTip(this.udSwapFrequency, "Frequency calibration reference frequency");
            this.udSwapFrequency.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.udSwapFrequency.ValueChanged += new System.EventHandler(this.swapFrequency_ValueChanged);
            // 
            // udLOCenterFreq
            // 
            this.udLOCenterFreq.Enabled = false;
            this.udLOCenterFreq.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udLOCenterFreq.Location = new System.Drawing.Point(109, 45);
            this.udLOCenterFreq.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.udLOCenterFreq.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.udLOCenterFreq.Name = "udLOCenterFreq";
            this.udLOCenterFreq.Size = new System.Drawing.Size(79, 20);
            this.udLOCenterFreq.TabIndex = 8;
            this.toolTip1.SetToolTip(this.udLOCenterFreq, "Local Oscillator Center Frequency of External SDR");
            this.udLOCenterFreq.Value = new decimal(new int[] {
            8215000,
            0,
            0,
            0});
            this.udLOCenterFreq.ValueChanged += new System.EventHandler(this.udLOCenterFreq_ValueChanged);
            // 
            // udIFGlobalOffset
            // 
            this.udIFGlobalOffset.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udIFGlobalOffset.Location = new System.Drawing.Point(109, 19);
            this.udIFGlobalOffset.Maximum = new decimal(new int[] {
            96000,
            0,
            0,
            0});
            this.udIFGlobalOffset.Minimum = new decimal(new int[] {
            96000,
            0,
            0,
            -2147483648});
            this.udIFGlobalOffset.Name = "udIFGlobalOffset";
            this.udIFGlobalOffset.Size = new System.Drawing.Size(79, 20);
            this.udIFGlobalOffset.TabIndex = 7;
            this.udIFGlobalOffset.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.udIFGlobalOffset.ValueChanged += new System.EventHandler(this.udIFGlobalOffset_ValueChanged);
            // 
            // udIFFSK
            // 
            this.udIFFSK.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udIFFSK.Location = new System.Drawing.Point(109, 149);
            this.udIFFSK.Maximum = new decimal(new int[] {
            96000,
            0,
            0,
            0});
            this.udIFFSK.Minimum = new decimal(new int[] {
            96000,
            0,
            0,
            -2147483648});
            this.udIFFSK.Name = "udIFFSK";
            this.udIFFSK.Size = new System.Drawing.Size(79, 20);
            this.udIFFSK.TabIndex = 6;
            this.udIFFSK.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.udIFFSK.ValueChanged += new System.EventHandler(this.udIFFSK_ValueChanged);
            // 
            // udIFFM
            // 
            this.udIFFM.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udIFFM.Location = new System.Drawing.Point(109, 123);
            this.udIFFM.Maximum = new decimal(new int[] {
            96000,
            0,
            0,
            0});
            this.udIFFM.Minimum = new decimal(new int[] {
            96000,
            0,
            0,
            -2147483648});
            this.udIFFM.Name = "udIFFM";
            this.udIFFM.Size = new System.Drawing.Size(79, 20);
            this.udIFFM.TabIndex = 5;
            this.udIFFM.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.udIFFM.ValueChanged += new System.EventHandler(this.udIFFM_ValueChanged);
            // 
            // udIFAM
            // 
            this.udIFAM.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udIFAM.Location = new System.Drawing.Point(109, 97);
            this.udIFAM.Maximum = new decimal(new int[] {
            96000,
            0,
            0,
            0});
            this.udIFAM.Minimum = new decimal(new int[] {
            96000,
            0,
            0,
            -2147483648});
            this.udIFAM.Name = "udIFAM";
            this.udIFAM.Size = new System.Drawing.Size(79, 20);
            this.udIFAM.TabIndex = 4;
            this.udIFAM.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.udIFAM.ValueChanged += new System.EventHandler(this.udIFAM_ValueChanged);
            // 
            // udIFCW
            // 
            this.udIFCW.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udIFCW.Location = new System.Drawing.Point(109, 71);
            this.udIFCW.Maximum = new decimal(new int[] {
            96000,
            0,
            0,
            0});
            this.udIFCW.Minimum = new decimal(new int[] {
            96000,
            0,
            0,
            -2147483648});
            this.udIFCW.Name = "udIFCW";
            this.udIFCW.Size = new System.Drawing.Size(79, 20);
            this.udIFCW.TabIndex = 3;
            this.udIFCW.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.udIFCW.ValueChanged += new System.EventHandler(this.udIFCW_ValueChanged);
            // 
            // udIFUSB
            // 
            this.udIFUSB.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udIFUSB.Location = new System.Drawing.Point(109, 45);
            this.udIFUSB.Maximum = new decimal(new int[] {
            96000,
            0,
            0,
            0});
            this.udIFUSB.Minimum = new decimal(new int[] {
            96000,
            0,
            0,
            -2147483648});
            this.udIFUSB.Name = "udIFUSB";
            this.udIFUSB.Size = new System.Drawing.Size(79, 20);
            this.udIFUSB.TabIndex = 2;
            this.udIFUSB.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.udIFUSB.ValueChanged += new System.EventHandler(this.udIFUSB_ValueChanged);
            // 
            // udIFLSB
            // 
            this.udIFLSB.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udIFLSB.Location = new System.Drawing.Point(109, 19);
            this.udIFLSB.Maximum = new decimal(new int[] {
            96000,
            0,
            0,
            0});
            this.udIFLSB.Minimum = new decimal(new int[] {
            96000,
            0,
            0,
            -2147483648});
            this.udIFLSB.Name = "udIFLSB";
            this.udIFLSB.Size = new System.Drawing.Size(79, 20);
            this.udIFLSB.TabIndex = 1;
            this.udIFLSB.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.udIFLSB.ValueChanged += new System.EventHandler(this.udIFLSB_ValueChanged);
            // 
            // comboMeterTimingInterval
            // 
            this.comboMeterTimingInterval.FormattingEnabled = true;
            this.comboMeterTimingInterval.Items.AddRange(new object[] {
            "35",
            "70",
            "140",
            "280"});
            this.comboMeterTimingInterval.Location = new System.Drawing.Point(116, 19);
            this.comboMeterTimingInterval.Name = "comboMeterTimingInterval";
            this.comboMeterTimingInterval.Size = new System.Drawing.Size(70, 21);
            this.comboMeterTimingInterval.TabIndex = 15;
            this.comboMeterTimingInterval.Text = "35";
            this.comboMeterTimingInterval.SelectedIndexChanged += new System.EventHandler(this.comboMeterTimingInterval_SelectedIndexChanged);
            // 
            // labelTS8
            // 
            this.labelTS8.Image = null;
            this.labelTS8.Location = new System.Drawing.Point(6, 19);
            this.labelTS8.Name = "labelTS8";
            this.labelTS8.Size = new System.Drawing.Size(104, 20);
            this.labelTS8.TabIndex = 14;
            this.labelTS8.Text = "Data Interval:";
            this.labelTS8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkUseMeter
            // 
            this.chkUseMeter.AutoSize = true;
            this.chkUseMeter.Image = null;
            this.chkUseMeter.Location = new System.Drawing.Point(9, 20);
            this.chkUseMeter.Name = "chkUseMeter";
            this.chkUseMeter.Size = new System.Drawing.Size(116, 17);
            this.chkUseMeter.TabIndex = 0;
            this.chkUseMeter.Text = "Use External Meter";
            this.chkUseMeter.UseVisualStyleBackColor = true;
            this.chkUseMeter.CheckedChanged += new System.EventHandler(this.chkUseMeter_CheckedChanged);
            // 
            // comboMeterType
            // 
            this.comboMeterType.Enabled = false;
            this.comboMeterType.FormattingEnabled = true;
            this.comboMeterType.Items.AddRange(new object[] {
            "Array Solutions PowerMaster"});
            this.comboMeterType.Location = new System.Drawing.Point(9, 43);
            this.comboMeterType.Name = "comboMeterType";
            this.comboMeterType.Size = new System.Drawing.Size(165, 21);
            this.comboMeterType.TabIndex = 1;
            this.toolTip1.SetToolTip(this.comboMeterType, "Defines what kind of meter will provide TX information to PowerSDR");
            this.comboMeterType.SelectedIndexChanged += new System.EventHandler(this.comboMeterType_SelectedIndexChanged);
            // 
            // grpMeterSerialBox
            // 
            this.grpMeterSerialBox.Controls.Add(this.comboMeterPort);
            this.grpMeterSerialBox.Controls.Add(this.comboMeterBaud);
            this.grpMeterSerialBox.Controls.Add(this.labelTS1);
            this.grpMeterSerialBox.Controls.Add(this.labelTS2);
            this.grpMeterSerialBox.Controls.Add(this.labelTS3);
            this.grpMeterSerialBox.Controls.Add(this.labelTS4);
            this.grpMeterSerialBox.Controls.Add(this.labelTS5);
            this.grpMeterSerialBox.Controls.Add(this.comboMeterParity);
            this.grpMeterSerialBox.Controls.Add(this.comboMeterDataBits);
            this.grpMeterSerialBox.Controls.Add(this.comboMeterStopBits);
            this.grpMeterSerialBox.Enabled = false;
            this.grpMeterSerialBox.Location = new System.Drawing.Point(6, 87);
            this.grpMeterSerialBox.Name = "grpMeterSerialBox";
            this.grpMeterSerialBox.Size = new System.Drawing.Size(141, 181);
            this.grpMeterSerialBox.TabIndex = 93;
            this.grpMeterSerialBox.TabStop = false;
            this.grpMeterSerialBox.Text = "Meter Serial Connection";
            // 
            // comboMeterPort
            // 
            this.comboMeterPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMeterPort.DropDownWidth = 56;
            this.comboMeterPort.Location = new System.Drawing.Point(60, 19);
            this.comboMeterPort.Name = "comboMeterPort";
            this.comboMeterPort.Size = new System.Drawing.Size(72, 21);
            this.comboMeterPort.TabIndex = 2;
            this.toolTip1.SetToolTip(this.comboMeterPort, "Sets the COM port to be used for communicating with the external meter.");
            this.comboMeterPort.SelectedIndexChanged += new System.EventHandler(this.comboMeterPort_SelectedIndexChanged);
            // 
            // comboMeterBaud
            // 
            this.comboMeterBaud.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMeterBaud.DropDownWidth = 56;
            this.comboMeterBaud.Items.AddRange(new object[] {
            "300",
            "1200",
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "57600"});
            this.comboMeterBaud.Location = new System.Drawing.Point(60, 46);
            this.comboMeterBaud.Name = "comboMeterBaud";
            this.comboMeterBaud.Size = new System.Drawing.Size(72, 21);
            this.comboMeterBaud.TabIndex = 3;
            this.comboMeterBaud.SelectedIndexChanged += new System.EventHandler(this.comboMeterBaud_SelectedIndexChanged);
            // 
            // labelTS1
            // 
            this.labelTS1.Image = null;
            this.labelTS1.Location = new System.Drawing.Point(14, 46);
            this.labelTS1.Name = "labelTS1";
            this.labelTS1.Size = new System.Drawing.Size(40, 23);
            this.labelTS1.TabIndex = 5;
            this.labelTS1.Text = "Baud:";
            this.labelTS1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTS2
            // 
            this.labelTS2.Image = null;
            this.labelTS2.Location = new System.Drawing.Point(14, 19);
            this.labelTS2.Name = "labelTS2";
            this.labelTS2.Size = new System.Drawing.Size(40, 23);
            this.labelTS2.TabIndex = 3;
            this.labelTS2.Text = "Port:";
            this.labelTS2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTS3
            // 
            this.labelTS3.Image = null;
            this.labelTS3.Location = new System.Drawing.Point(6, 73);
            this.labelTS3.Name = "labelTS3";
            this.labelTS3.Size = new System.Drawing.Size(48, 23);
            this.labelTS3.TabIndex = 92;
            this.labelTS3.Text = "Parity:";
            this.labelTS3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTS4
            // 
            this.labelTS4.Image = null;
            this.labelTS4.Location = new System.Drawing.Point(14, 100);
            this.labelTS4.Name = "labelTS4";
            this.labelTS4.Size = new System.Drawing.Size(40, 23);
            this.labelTS4.TabIndex = 92;
            this.labelTS4.Text = "Data:";
            this.labelTS4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTS5
            // 
            this.labelTS5.Image = null;
            this.labelTS5.Location = new System.Drawing.Point(14, 127);
            this.labelTS5.Name = "labelTS5";
            this.labelTS5.Size = new System.Drawing.Size(40, 23);
            this.labelTS5.TabIndex = 92;
            this.labelTS5.Text = "Stop:";
            this.labelTS5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboMeterParity
            // 
            this.comboMeterParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMeterParity.DropDownWidth = 56;
            this.comboMeterParity.Items.AddRange(new object[] {
            "none",
            "odd ",
            "even",
            "mark",
            "space"});
            this.comboMeterParity.Location = new System.Drawing.Point(60, 73);
            this.comboMeterParity.Name = "comboMeterParity";
            this.comboMeterParity.Size = new System.Drawing.Size(72, 21);
            this.comboMeterParity.TabIndex = 4;
            this.comboMeterParity.SelectedIndexChanged += new System.EventHandler(this.comboMeterParity_SelectedIndexChanged);
            // 
            // comboMeterDataBits
            // 
            this.comboMeterDataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMeterDataBits.DropDownWidth = 56;
            this.comboMeterDataBits.Items.AddRange(new object[] {
            "8",
            "7",
            "6"});
            this.comboMeterDataBits.Location = new System.Drawing.Point(60, 100);
            this.comboMeterDataBits.Name = "comboMeterDataBits";
            this.comboMeterDataBits.Size = new System.Drawing.Size(72, 21);
            this.comboMeterDataBits.TabIndex = 5;
            this.comboMeterDataBits.SelectedIndexChanged += new System.EventHandler(this.comboMeterDataBits_SelectedIndexChanged);
            // 
            // comboMeterStopBits
            // 
            this.comboMeterStopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMeterStopBits.DropDownWidth = 56;
            this.comboMeterStopBits.Items.AddRange(new object[] {
            "1",
            "1.5",
            "2"});
            this.comboMeterStopBits.Location = new System.Drawing.Point(60, 127);
            this.comboMeterStopBits.Name = "comboMeterStopBits";
            this.comboMeterStopBits.Size = new System.Drawing.Size(72, 21);
            this.comboMeterStopBits.TabIndex = 6;
            this.comboMeterStopBits.SelectedIndexChanged += new System.EventHandler(this.comboMeterStopBits_SelectedIndexChanged);
            // 
            // btnImportDB
            // 
            this.btnImportDB.Image = null;
            this.btnImportDB.Location = new System.Drawing.Point(143, 322);
            this.btnImportDB.Name = "btnImportDB";
            this.btnImportDB.Size = new System.Drawing.Size(112, 23);
            this.btnImportDB.TabIndex = 26;
            this.btnImportDB.Text = "Import Database...";
            this.btnImportDB.Click += new System.EventHandler(this.btnImportDB_Click);
            // 
            // btnResetDB
            // 
            this.btnResetDB.Image = null;
            this.btnResetDB.Location = new System.Drawing.Point(23, 322);
            this.btnResetDB.Name = "btnResetDB";
            this.btnResetDB.Size = new System.Drawing.Size(96, 23);
            this.btnResetDB.TabIndex = 25;
            this.btnResetDB.Text = "Reset Database";
            this.btnResetDB.Click += new System.EventHandler(this.btnResetDB_Click);
            // 
            // btnApply
            // 
            this.btnApply.Image = null;
            this.btnApply.Location = new System.Drawing.Point(511, 322);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 24;
            this.btnApply.Text = "Apply";
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Image = null;
            this.btnCancel.Location = new System.Drawing.Point(423, 322);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 23;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Image = null;
            this.btnOK.Location = new System.Drawing.Point(335, 322);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 22;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // SetupIF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 357);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnImportDB);
            this.Controls.Add(this.btnResetDB);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SetupIF";
            this.Text = "PowerSDR/IF Stage Setup";
            this.grpMeterTimingBox.ResumeLayout(false);
            this.grpRigTimingBox.ResumeLayout(false);
            this.grpOptionalPollingCommandsBox.ResumeLayout(false);
            this.grpOptionalPollingCommandsBox.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.grpMeterTypeBox.ResumeLayout(false);
            this.grpMeterTypeBox.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.grpRigTypeBox.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.udRigTuningCATInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udRigTuningPollingInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udRigPollingLockoutTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udRigPollingInterval)).EndInit();
            this.grpRigSerialBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.udSwapFrequency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udLOCenterFreq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udIFGlobalOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udIFFSK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udIFFM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udIFAM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udIFCW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udIFUSB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udIFLSB)).EndInit();
            this.grpMeterSerialBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ButtonTS btnImportDB;
        private System.Windows.Forms.ButtonTS btnResetDB;
        private System.Windows.Forms.ButtonTS btnApply;
        private System.Windows.Forms.ButtonTS btnCancel;
		private System.Windows.Forms.ButtonTS btnOK;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.GroupBox grpMeterTimingBox;
		private System.Windows.Forms.ComboBoxTS comboMeterTimingInterval;
		private System.Windows.Forms.LabelTS labelTS8;
		private System.Windows.Forms.GroupBoxTS grpMeterSerialBox;
		private System.Windows.Forms.ComboBoxTS comboMeterPort;
		private System.Windows.Forms.ComboBoxTS comboMeterBaud;
		private System.Windows.Forms.LabelTS labelTS1;
		private System.Windows.Forms.LabelTS labelTS2;
		private System.Windows.Forms.LabelTS labelTS3;
		private System.Windows.Forms.LabelTS labelTS4;
		private System.Windows.Forms.LabelTS labelTS5;
		private System.Windows.Forms.ComboBoxTS comboMeterParity;
		private System.Windows.Forms.ComboBoxTS comboMeterDataBits;
		private System.Windows.Forms.ComboBoxTS comboMeterStopBits;
		private System.Windows.Forms.GroupBox grpMeterTypeBox;
		private System.Windows.Forms.CheckBoxTS chkUseMeter;
		private System.Windows.Forms.ComboBoxTS comboMeterType;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.NumericUpDownTS udIFGlobalOffset;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.NumericUpDownTS udIFFSK;
		private System.Windows.Forms.NumericUpDownTS udIFFM;
		private System.Windows.Forms.NumericUpDownTS udIFAM;
		private System.Windows.Forms.NumericUpDownTS udIFCW;
		private System.Windows.Forms.NumericUpDownTS udIFUSB;
		private System.Windows.Forms.NumericUpDownTS udIFLSB;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.GroupBox grpOptionalPollingCommandsBox;
		private System.Windows.Forms.CheckBoxTS chkRigPollIFFreq;
		private System.Windows.Forms.CheckBoxTS chkRigPollVFOB;
		private System.Windows.Forms.GroupBox grpRigTimingBox;
		private System.Windows.Forms.LabelTS labelRigTuningPollingInterval;
		private System.Windows.Forms.NumericUpDownTS udRigTuningPollingInterval;
		private System.Windows.Forms.LabelTS labelRigPollingLockoutTime;
		private System.Windows.Forms.NumericUpDownTS udRigPollingLockoutTime;
		private System.Windows.Forms.LabelTS labelRigPollingInterval;
		private System.Windows.Forms.NumericUpDownTS udRigPollingInterval;
		private System.Windows.Forms.GroupBoxTS grpRigSerialBox;
		private System.Windows.Forms.ComboBoxTS comboRigPort;
		private System.Windows.Forms.ComboBoxTS comboRigBaud;
		private System.Windows.Forms.LabelTS lblCATBaud;
		private System.Windows.Forms.LabelTS lblCATPort;
		private System.Windows.Forms.LabelTS lblCATParity;
		private System.Windows.Forms.LabelTS lblCATData;
		private System.Windows.Forms.LabelTS lblCATStop;
		private System.Windows.Forms.ComboBoxTS comboRigParity;
		private System.Windows.Forms.ComboBoxTS comboRigDataBits;
		private System.Windows.Forms.ComboBoxTS comboRigStopBits;
		private System.Windows.Forms.GroupBox grpRigTypeBox;
		private System.Windows.Forms.ComboBoxTS comboRigType;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.NumericUpDownTS udLOCenterFreq;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.LabelTS labelTS6;
		private System.Windows.Forms.NumericUpDownTS udRigTuningCATInterval;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDownTS udSwapFrequency;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBoxTS chkSwapIQ;
    }
}