//=================================================================
// database.cs
//=================================================================
// PowerSDR is a C# implementation of a Software Defined Radio.
// Copyright (C) 2004-2009  FlexRadio Systems
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
// You may contact us via email at: sales@flex-radio.com.
// Paper mail may be sent to: 
//    FlexRadio Systems
//    8900 Marybank Dr.
//    Austin, TX 78750
//    USA
//=================================================================

using System;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;


namespace PowerSDR
{
    class DatabaseIF
    {
        #region Variable Declaration

        private static DataSet ds;

		private static string app_data_path = "";
		public static string AppDataPath
		{
			set { app_data_path = value; }
		}

        #endregion

        #region Private Member Functions
        // ======================================================
        // Private Member Functions
        // ======================================================

        private static void AddFormTable(string name)
        {
            ds.Tables.Add(name);
            ds.Tables[name].Columns.Add("Key", typeof(string));
            ds.Tables[name].Columns.Add("Value", typeof(string));
        }

        #endregion

        #region Public Member Functions
        // ======================================================
        // Public Member Functions 
        // ======================================================

        public static void Init()
        {
            ds = new DataSet("Data");

            if (File.Exists(app_data_path + "\\databaseIF.xml"))
				ds.ReadXml(app_data_path + "\\databaseIF.xml");
        }

        public static void Update()
        {
            ds.WriteXml(app_data_path + "\\databaseIF.xml", XmlWriteMode.WriteSchema);
        }

		public static void Exit()
        {
            Update();
            ds = null;
        }

        public static void SaveVars(string tableName, ref ArrayList list)
        {
            if (!ds.Tables.Contains(tableName))
                AddFormTable(tableName);

            foreach (string s in list)
            {
                string[] vals = s.Split('/');
                if (vals.Length > 2)
                {
                    for (int i = 2; i < vals.Length; i++)
                        vals[1] += "/" + vals[i];
                }

                DataRow[] rows = ds.Tables[tableName].Select("Key = '" + vals[0] + "'");
                if (rows.Length == 0)	// name is not in list
                {
                    DataRow newRow = ds.Tables[tableName].NewRow();
                    newRow[0] = vals[0];
                    newRow[1] = vals[1];
                    ds.Tables[tableName].Rows.Add(newRow);
                }
                else if (rows.Length == 1)
                {
                    rows[0][1] = vals[1];
                }
            }
        }

        public static ArrayList GetVars(string tableName)
        {
            ArrayList list = new ArrayList();
            if (!ds.Tables.Contains(tableName))
                return list;

            DataTable t = ds.Tables[tableName];

            for (int i = 0; i < t.Rows.Count; i++)
            {
                list.Add(t.Rows[i][0].ToString() + "/" + t.Rows[i][1].ToString());
            }

            return list;
        }

		public static bool ImportDatabase(string filename)
		{
			if (!File.Exists(filename))
				return false;

			DataSet file = new DataSet();

			try
			{
				file.ReadXml(filename);
			}
			catch (Exception)
			{
				return false;
			}

			ds = file;
			return true;
		}

		public static void ExportDatabase(string filename)
		{
			ds.WriteXml(filename,XmlWriteMode.WriteSchema);
		}

		#endregion
	}
}
