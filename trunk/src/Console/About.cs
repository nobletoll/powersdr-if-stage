//=================================================================
// About.cs
//=================================================================
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
// ================================================================
//
// Author: S.McClements - WU2X, 7/30/2007
//
// PowerSDR/IF Stage About Window
//=================================================================


using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PowerSDR
{
	/// <summary>
	/// Summary description for About.
	/// </summary>
	public class About : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label AboutText;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public About()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(About));
			this.AboutText = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// AboutText
			// 
			this.AboutText.BackColor = System.Drawing.Color.Transparent;
			this.AboutText.ForeColor = System.Drawing.Color.White;
			this.AboutText.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
			this.AboutText.Location = new System.Drawing.Point(24, 208);
			this.AboutText.Name = "AboutText";
			this.AboutText.Size = new System.Drawing.Size(352, 48);
			this.AboutText.TabIndex = 0;
			this.AboutText.Text = "PowerSDR/IF Stage by WU2X.  Version: 0.92. Build Date: 7/28/2008. Based on  Power" +
				"SDR Version: 1.9.0 . http://www.wu2x.com/";
			this.AboutText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// About
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = new System.Drawing.Size(400, 266);
			this.Controls.Add(this.AboutText);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "About";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About";
			this.Click += new System.EventHandler(this.About_Click);
			this.ResumeLayout(false);

		}
		#endregion

		private void About_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

	}
}
