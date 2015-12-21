﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;
using System.Threading;

namespace SQLite_Basics
{
    public partial class Form1 : Form
    {
        #region Class Variables
        
        public static string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SQLiteBasics");
        public static string db = Path.Combine(dir, "test.sqlite");
        public static string password = "SuperStrongUniquePassword";
        //private SQLiteConnection dbConnection = new SQLiteConnection("Data Source=" + db + ";Version=3;Password=" + password + ";");
        private SQLiteConnection dbConnection = new SQLiteConnection("Data Source=" + db + ";Version=3;");
        #endregion

        public Form1()
        {
            InitializeComponent();
            txtResult.Text = "The following DB will be created: " + Form1.db.ToString() + Environment.NewLine;
            //try
            //{
            //    //dbConnection.Open();
            //    dbConnection.SetPassword(password);
            //    //dbConnection.Close();
            //    txtResult.AppendText(Environment.NewLine + "DB password set: " + password + Environment.NewLine);
            //    //password = "newPassword";
            //    //dbConnection.ChangePassword(password);
            //}
            //catch(Exception e)
            //{
            //    txtResult.AppendText(Environment.NewLine + "ERROR:" + Environment.NewLine + e.ToString());
            //    dbConnection.Close();
            //    txtResult.AppendText(Environment.NewLine + Environment.NewLine + "SQLite connection closed: " + dbConnection.State.ToString() + Environment.NewLine);
            //}
        }

        #region User click events
        private void btnCreate_Click(object sender, EventArgs e)
        {
            _createDB();
        }

        private void btnCreateTable_Click(object sender, EventArgs e)
        {
            _createDBTable();
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            _writeToDB();
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            _readFromDB();
        }
        #endregion

        #region Functions
        private void _createDB()
        {
            string result;
            txtResult.AppendText(Environment.NewLine);
            txtResult.AppendText(Environment.NewLine);
            try
            {
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                if (!File.Exists(db))
                {
                    SQLiteConnection.CreateFile(db);
                    result = "DB Created Succesfully - " + db;
                }
                else
                {
                    //MessageBox.Show("DB is already there - don't need to create it again.");
                    result = "DB already exists - " + db;
                }
                txtResult.AppendText(result + Environment.NewLine);
            }
            catch(Exception e)
            {
                txtResult.AppendText(Environment.NewLine + "ERROR:" + Environment.NewLine + e.ToString());
            }
        }

        private void _createDBTable()
        {
            try {
                txtResult.AppendText(Environment.NewLine);
                dbConnection.Open();
                txtResult.AppendText("SQLite connection opened: " + dbConnection.State.ToString() + Environment.NewLine);
                string sql = "create table highscores (name varchar(20), score int)";
                txtResult.AppendText("SQL defined: " + sql.ToString() + Environment.NewLine);
                SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
                txtResult.AppendText("SQLiteCommand defined: " + command.ToString() + Environment.NewLine);
                command.ExecuteNonQuery();
                txtResult.AppendText("SQLiteCommand executed: " + command.ToString() + Environment.NewLine);
                dbConnection.Close();
                txtResult.AppendText("SQLite connection closed: " + dbConnection.State.ToString() + Environment.NewLine);
            }
            catch(Exception e)
            {
                txtResult.AppendText(Environment.NewLine + "ERROR:" + Environment.NewLine + e.ToString());
                dbConnection.Close();
                txtResult.AppendText(Environment.NewLine + Environment.NewLine + "SQLite connection closed: " + dbConnection.State.ToString() + Environment.NewLine);
            }
        }

        private void _writeToDB()
        {
            try
            {
                txtResult.AppendText(Environment.NewLine);
                dbConnection.Open();
                txtResult.AppendText("SQLite connection opened: " + dbConnection.State.ToString() + Environment.NewLine);

                int x = 0;
                string sql = "";
                Random rnd = new Random();
                while (x < 1000)
                {
                    x += 1;
                    //generate a random name
                    int rsn = GetRandomNumber(7, 12);
                    string rs = RandomString(rsn);
                    //generate a random score
                    int rn = GetRandomNumber(5, 5000000);

                    sql = "insert into highscores (name, score) values ('" + rs + "', " + rn + " )";

                    SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
                    command.ExecuteNonQuery();
                    txtResult.AppendText("SQL " + x.ToString() + " written: " + sql + Environment.NewLine);
                    //insert into DB
                }
                
                dbConnection.Close();
                txtResult.AppendText("SQLite connection closed: " + dbConnection.State.ToString() + Environment.NewLine);
            }
            catch(Exception e)
            {
                txtResult.AppendText(Environment.NewLine + "ERROR:" + Environment.NewLine + e.ToString());
                dbConnection.Close();
                txtResult.AppendText(Environment.NewLine + Environment.NewLine + "SQLite connection closed: " + dbConnection.State.ToString() + Environment.NewLine);
            }
        }

        private void _readFromDB()
        {
            try
            {
                txtResult.AppendText(Environment.NewLine);
                dbConnection.Open();
                txtResult.AppendText("SQLite connection opened: " + dbConnection.State.ToString() + Environment.NewLine);

                string sql = "select * from highscores order by score desc";
                SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    txtResult.AppendText(Environment.NewLine + "\tName:\t" + reader["name"] + "\tScore:\t" + reader["score"] + Environment.NewLine);
                }
                
                dbConnection.Close();
                txtResult.AppendText("SQLite connection closed: " + dbConnection.State.ToString() + Environment.NewLine);
            }
            catch (Exception e)
            {
                txtResult.AppendText(Environment.NewLine + "ERROR:" + Environment.NewLine + e.ToString());
                dbConnection.Close();
                txtResult.AppendText(Environment.NewLine + Environment.NewLine + "SQLite connection closed: " + dbConnection.State.ToString() + Environment.NewLine);
            }
        }

        #endregion

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private static readonly Random getrandom = new Random();
        private static readonly object syncLock = new object();
        public static int GetRandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return getrandom.Next(min, max);
            }
        }

    }
}
