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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.udIFGlobalOffset = new System.Windows.Forms.NumericUpDownTS();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.udIFFSK = new System.Windows.Forms.NumericUpDownTS();
			this.udIFFM = new System.Windows.Forms.NumericUpDownTS();
			this.udIFAM = new System.Windows.Forms.NumericUpDownTS();
			this.udIFCW = new System.Windows.Forms.NumericUpDownTS();
			this.udIFUSB = new System.Windows.Forms.NumericUpDownTS();
			this.udIFLSB = new System.Windows.Forms.NumericUpDownTS();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.btnImportDB = new System.Windows.Forms.ButtonTS();
			this.btnResetDB = new System.Windows.Forms.ButtonTS();
			this.btnApply = new System.Windows.Forms.ButtonTS();
			this.btnCancel = new System.Windows.Forms.ButtonTS();
			this.btnOK = new System.Windows.Forms.ButtonTS();
			this.grpRigTypeBox = new System.Windows.Forms.GroupBox();
			this.comboRigType = new System.Windows.Forms.ComboBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
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
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize) (this.udIFGlobalOffset)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize) (this.udIFFSK)).BeginInit();
			((System.ComponentModel.ISupportInitialize) (this.udIFFM)).BeginInit();
			((System.ComponentModel.ISupportInitialize) (this.udIFAM)).BeginInit();
			((System.ComponentModel.ISupportInitialize) (this.udIFCW)).BeginInit();
			((System.ComponentModel.ISupportInitialize) (this.udIFUSB)).BeginInit();
			((System.ComponentModel.ISupportInitialize) (this.udIFLSB)).BeginInit();
			this.grpRigTypeBox.SuspendLayout();
			this.grpRigSerialBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Location = new System.Drawing.Point(12,12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(592,301);
			this.tabControl1.TabIndex = 27;
			// 
			// tabPage1
			// 
			this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
			this.tabPage1.Controls.Add(this.grpRigSerialBox);
			this.tabPage1.Controls.Add(this.grpRigTypeBox);
			this.tabPage1.Location = new System.Drawing.Point(4,22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(584,275);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Rig Connection";
			// 
			// tabPage2
			// 
			this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
			this.tabPage2.Controls.Add(this.groupBox2);
			this.tabPage2.Controls.Add(this.groupBox1);
			this.tabPage2.Location = new System.Drawing.Point(4,22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(584,275);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "IF Frequencies";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.udIFGlobalOffset);
			this.groupBox2.Location = new System.Drawing.Point(11,199);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(138,65);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "IF Frequency Global Offset (Hz)";
			// 
			// udIFGlobalOffset
			// 
			this.udIFGlobalOffset.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.udIFGlobalOffset.Location = new System.Drawing.Point(46,39);
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
			this.udIFGlobalOffset.Size = new System.Drawing.Size(79,20);
			this.udIFGlobalOffset.TabIndex = 18;
			this.udIFGlobalOffset.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.udIFGlobalOffset.ValueChanged += new System.EventHandler(this.udIFGlobalOffset_ValueChanged);
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
			this.groupBox1.Location = new System.Drawing.Point(11,6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(142,180);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "IF Frequency (Hz)";
			// 
			// udIFFSK
			// 
			this.udIFFSK.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.udIFFSK.Location = new System.Drawing.Point(46,140);
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
			this.udIFFSK.Size = new System.Drawing.Size(79,20);
			this.udIFFSK.TabIndex = 17;
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
            1,
            0,
            0,
            0});
			this.udIFFM.Location = new System.Drawing.Point(46,117);
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
			this.udIFFM.Size = new System.Drawing.Size(79,20);
			this.udIFFM.TabIndex = 16;
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
            1,
            0,
            0,
            0});
			this.udIFAM.Location = new System.Drawing.Point(46,95);
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
			this.udIFAM.Size = new System.Drawing.Size(79,20);
			this.udIFAM.TabIndex = 15;
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
            1,
            0,
            0,
            0});
			this.udIFCW.Location = new System.Drawing.Point(46,73);
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
			this.udIFCW.Size = new System.Drawing.Size(79,20);
			this.udIFCW.TabIndex = 14;
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
            1,
            0,
            0,
            0});
			this.udIFUSB.Location = new System.Drawing.Point(46,50);
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
			this.udIFUSB.Size = new System.Drawing.Size(79,20);
			this.udIFUSB.TabIndex = 13;
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
            1,
            0,
            0,
            0});
			this.udIFLSB.Location = new System.Drawing.Point(46,26);
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
			this.udIFLSB.Size = new System.Drawing.Size(79,20);
			this.udIFLSB.TabIndex = 12;
			this.udIFLSB.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.udIFLSB.ValueChanged += new System.EventHandler(this.udIFLSB_ValueChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(9,140);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(27,13);
			this.label6.TabIndex = 5;
			this.label6.Text = "FSK";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(11,117);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(22,13);
			this.label5.TabIndex = 4;
			this.label5.Text = "FM";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(11,95);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(23,13);
			this.label4.TabIndex = 3;
			this.label4.Text = "AM";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(11,73);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(25,13);
			this.label3.TabIndex = 2;
			this.label3.Text = "CW";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(9,50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(29,13);
			this.label2.TabIndex = 1;
			this.label2.Text = "USB";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(11,26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(27,13);
			this.label1.TabIndex = 0;
			this.label1.Text = "LSB";
			// 
			// tabPage3
			// 
			this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
			this.tabPage3.Location = new System.Drawing.Point(4,22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(584,275);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Audio";
			// 
			// btnImportDB
			// 
			this.btnImportDB.Image = null;
			this.btnImportDB.Location = new System.Drawing.Point(143,322);
			this.btnImportDB.Name = "btnImportDB";
			this.btnImportDB.Size = new System.Drawing.Size(112,23);
			this.btnImportDB.TabIndex = 26;
			this.btnImportDB.Text = "Import Database...";
			this.btnImportDB.Click += new System.EventHandler(this.btnImportDB_Click);
			// 
			// btnResetDB
			// 
			this.btnResetDB.Image = null;
			this.btnResetDB.Location = new System.Drawing.Point(23,322);
			this.btnResetDB.Name = "btnResetDB";
			this.btnResetDB.Size = new System.Drawing.Size(96,23);
			this.btnResetDB.TabIndex = 25;
			this.btnResetDB.Text = "Reset Database";
			this.btnResetDB.Click += new System.EventHandler(this.btnResetDB_Click);
			// 
			// btnApply
			// 
			this.btnApply.Image = null;
			this.btnApply.Location = new System.Drawing.Point(511,322);
			this.btnApply.Name = "btnApply";
			this.btnApply.Size = new System.Drawing.Size(75,23);
			this.btnApply.TabIndex = 24;
			this.btnApply.Text = "Apply";
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Image = null;
			this.btnCancel.Location = new System.Drawing.Point(423,322);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75,23);
			this.btnCancel.TabIndex = 23;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.Image = null;
			this.btnOK.Location = new System.Drawing.Point(335,322);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75,23);
			this.btnOK.TabIndex = 22;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// grpRigTypeBox
			// 
			this.grpRigTypeBox.Controls.Add(this.comboRigType);
			this.grpRigTypeBox.Location = new System.Drawing.Point(7,7);
			this.grpRigTypeBox.Name = "grpRigTypeBox";
			this.grpRigTypeBox.Size = new System.Drawing.Size(141,55);
			this.grpRigTypeBox.TabIndex = 0;
			this.grpRigTypeBox.TabStop = false;
			this.grpRigTypeBox.Text = "Rig Type";
			// 
			// comboRigType
			// 
			this.comboRigType.FormattingEnabled = true;
			this.comboRigType.Items.AddRange(new object[] {
            "Kenwood TS-940S"});
			this.comboRigType.Location = new System.Drawing.Point(9,20);
			this.comboRigType.Name = "comboRigType";
			this.comboRigType.Size = new System.Drawing.Size(123,21);
			this.comboRigType.TabIndex = 0;
			this.toolTip1.SetToolTip(this.comboRigType,"Defines what kind of rig will control PowerSDR");
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
			this.grpRigSerialBox.Location = new System.Drawing.Point(7,79);
			this.grpRigSerialBox.Name = "grpRigSerialBox";
			this.grpRigSerialBox.Size = new System.Drawing.Size(141,161);
			this.grpRigSerialBox.TabIndex = 91;
			this.grpRigSerialBox.TabStop = false;
			this.grpRigSerialBox.Text = "Rig Serial Connection";
			// 
			// comboRigPort
			// 
			this.comboRigPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboRigPort.DropDownWidth = 56;
			this.comboRigPort.Location = new System.Drawing.Point(60,19);
			this.comboRigPort.Name = "comboRigPort";
			this.comboRigPort.Size = new System.Drawing.Size(72,21);
			this.comboRigPort.TabIndex = 95;
			this.toolTip1.SetToolTip(this.comboRigPort,"Sets the COM port to be used for communicating with the Rig.");
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
			this.comboRigBaud.Location = new System.Drawing.Point(60,46);
			this.comboRigBaud.Name = "comboRigBaud";
			this.comboRigBaud.Size = new System.Drawing.Size(72,21);
			this.comboRigBaud.TabIndex = 93;
			this.comboRigBaud.SelectedIndexChanged += new System.EventHandler(this.comboRigBaud_SelectedIndexChanged);
			// 
			// lblCATBaud
			// 
			this.lblCATBaud.Image = null;
			this.lblCATBaud.Location = new System.Drawing.Point(14,46);
			this.lblCATBaud.Name = "lblCATBaud";
			this.lblCATBaud.Size = new System.Drawing.Size(40,23);
			this.lblCATBaud.TabIndex = 5;
			this.lblCATBaud.Text = "Baud:";
			this.lblCATBaud.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblCATPort
			// 
			this.lblCATPort.Image = null;
			this.lblCATPort.Location = new System.Drawing.Point(14,19);
			this.lblCATPort.Name = "lblCATPort";
			this.lblCATPort.Size = new System.Drawing.Size(40,23);
			this.lblCATPort.TabIndex = 3;
			this.lblCATPort.Text = "Port:";
			this.lblCATPort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblCATParity
			// 
			this.lblCATParity.Image = null;
			this.lblCATParity.Location = new System.Drawing.Point(6,73);
			this.lblCATParity.Name = "lblCATParity";
			this.lblCATParity.Size = new System.Drawing.Size(48,23);
			this.lblCATParity.TabIndex = 92;
			this.lblCATParity.Text = "Parity:";
			this.lblCATParity.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblCATData
			// 
			this.lblCATData.Image = null;
			this.lblCATData.Location = new System.Drawing.Point(14,100);
			this.lblCATData.Name = "lblCATData";
			this.lblCATData.Size = new System.Drawing.Size(40,23);
			this.lblCATData.TabIndex = 92;
			this.lblCATData.Text = "Data:";
			this.lblCATData.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lblCATStop
			// 
			this.lblCATStop.Image = null;
			this.lblCATStop.Location = new System.Drawing.Point(14,127);
			this.lblCATStop.Name = "lblCATStop";
			this.lblCATStop.Size = new System.Drawing.Size(40,23);
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
			this.comboRigParity.Location = new System.Drawing.Point(60,73);
			this.comboRigParity.Name = "comboRigParity";
			this.comboRigParity.Size = new System.Drawing.Size(72,21);
			this.comboRigParity.TabIndex = 92;
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
			this.comboRigDataBits.Location = new System.Drawing.Point(60,100);
			this.comboRigDataBits.Name = "comboRigDataBits";
			this.comboRigDataBits.Size = new System.Drawing.Size(72,21);
			this.comboRigDataBits.TabIndex = 93;
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
			this.comboRigStopBits.Location = new System.Drawing.Point(60,127);
			this.comboRigStopBits.Name = "comboRigStopBits";
			this.comboRigStopBits.Size = new System.Drawing.Size(72,21);
			this.comboRigStopBits.TabIndex = 94;
			this.comboRigStopBits.SelectedIndexChanged += new System.EventHandler(this.comboRigStopBits_SelectedIndexChanged);
			// 
			// SetupIF
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F,13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(616,357);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.btnImportDB);
			this.Controls.Add(this.btnResetDB);
			this.Controls.Add(this.btnApply);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Name = "SetupIF";
			this.Text = "PowerSDR/IF Stage Setup";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize) (this.udIFGlobalOffset)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize) (this.udIFFSK)).EndInit();
			((System.ComponentModel.ISupportInitialize) (this.udIFFM)).EndInit();
			((System.ComponentModel.ISupportInitialize) (this.udIFAM)).EndInit();
			((System.ComponentModel.ISupportInitialize) (this.udIFCW)).EndInit();
			((System.ComponentModel.ISupportInitialize) (this.udIFUSB)).EndInit();
			((System.ComponentModel.ISupportInitialize) (this.udIFLSB)).EndInit();
			this.grpRigTypeBox.ResumeLayout(false);
			this.grpRigSerialBox.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ButtonTS btnImportDB;
        private System.Windows.Forms.ButtonTS btnResetDB;
        private System.Windows.Forms.ButtonTS btnApply;
        private System.Windows.Forms.ButtonTS btnCancel;
        private System.Windows.Forms.ButtonTS btnOK;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDownTS udIFCW;
        private System.Windows.Forms.NumericUpDownTS udIFUSB;
        private System.Windows.Forms.NumericUpDownTS udIFLSB;
        private System.Windows.Forms.NumericUpDownTS udIFGlobalOffset;
        private System.Windows.Forms.NumericUpDownTS udIFFSK;
        private System.Windows.Forms.NumericUpDownTS udIFFM;
        private System.Windows.Forms.NumericUpDownTS udIFAM;
		private System.Windows.Forms.GroupBox grpRigTypeBox;
		private System.Windows.Forms.ComboBox comboRigType;
		private System.Windows.Forms.ToolTip toolTip1;
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
    }
}