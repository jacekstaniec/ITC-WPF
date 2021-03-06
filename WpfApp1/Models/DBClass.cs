﻿namespace WpfApp1.ViewModels
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows;

    class DBClass
    {
        static string _dbName;
        static string _dbPath;
        static string _dbConString;

        public static string sql;

        public static SqlConnection con = new SqlConnection();
        public static SqlCommand cmd = new SqlCommand("", con);

        public static SqlDataReader rdHeader;
        public static SqlDataReader rdDetail;
        public static DataTable dtHeader;
        public static DataTable dtDetail;
        public static SqlDataAdapter daHeader;
        public static SqlDataAdapter daDetail;


        /// <summary>
        /// inicjalizacja zmiennych prywatnych
        /// </summary>
        static void setSettings()
        {
            _dbName = "Database1.mdf";
            _dbPath = System.IO.Path.GetFullPath(_dbName);
            _dbConString = "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = " + _dbPath + "; Integrated Security = True";
        }

        /// <summary>
        /// static void open connection
        /// </summary>
        public static void openConnection()
        {
            setSettings();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.ConnectionString = _dbConString;
                    con.Open();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// static void close connection
        /// </summary>
        public static void closeConnection()
        {
            setSettings();
            try
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}