﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Dynamic;
using System.IO;
using System.Windows.Forms;

namespace TransitServer
{
    public class SQLite
    {
        private String dbFileName;
        private SQLiteConnection m_dbConn;
        private SQLiteCommand m_sqlCmd;

        private static readonly SQLite instance = new SQLite();

        public SQLite()
        {
            m_dbConn = new SQLiteConnection();
            m_sqlCmd = new SQLiteCommand();
            dbFileName = "transitServer.sqlite";
        }

        public static SQLite Instance
        {
            get
            {
                return instance;
            }
        }


        public void CreateDB()
        {
            if (!File.Exists(dbFileName))
                SQLiteConnection.CreateFile(dbFileName);

            try
            {
                m_dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
                m_dbConn.Open();
                m_sqlCmd.Connection = m_dbConn;

                m_sqlCmd.CommandText = "CREATE TABLE IF NOT EXISTS dbEvents (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT, date TEXT, message TEXT, quite TEXT)";
                //m_sqlCmd.CommandText = "CREATE TABLE IF NOT EXISTS modems (id INTEGER PRIMARY KEY AUTOINCREMENT, imei TEXT, port TEXT, name TEXT, quite TEXT)";

                m_sqlCmd.ExecuteNonQuery();
                
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public void CreateModemsTable()
        {
            if (!File.Exists(dbFileName))
                SQLiteConnection.CreateFile(dbFileName);

            try
            {
                m_dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
                m_dbConn.Open();
                m_sqlCmd.Connection = m_dbConn;

                m_sqlCmd.CommandText = "CREATE TABLE IF NOT EXISTS modems (id INTEGER PRIMARY KEY AUTOINCREMENT, imei TEXT, port TEXT, name TEXT, lastConnection TEXT, activeConnection INTEGER)";
                m_sqlCmd.ExecuteNonQuery();

            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public void InsertModems(string imei, string port, string name)
        {
            // записываем информацию в базу данных
            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                string commandText = "INSERT INTO [modems] ([imei], [port], [name]) VALUES(@imei, @port, @name, @activeConnection)";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connect);
                Command.Parameters.AddWithValue("@imei", imei);
                Command.Parameters.AddWithValue("@port", port);
                Command.Parameters.AddWithValue("@name", name);
                Command.Parameters.AddWithValue("@activeConnection", 0);
                Connect.Open();
                Command.ExecuteNonQuery();
                Connect.Close();
            }
        }
        public void InsertModems(string imei, string port)
        {
            // записываем информацию в базу данных
            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                string commandText = "INSERT INTO [modems] ([imei], [port], [name],[activeConnection]) VALUES(@imei, @port, @name, @activeConnection)";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connect);
                Command.Parameters.AddWithValue("@imei", imei);
                Command.Parameters.AddWithValue("@port", port);
                Command.Parameters.AddWithValue("@name", "Not name");
                Command.Parameters.AddWithValue("@activeConnection", 0);
                Connect.Open();
                Command.ExecuteNonQuery();
                Connect.Close();
            }
        }

        public void DeleteModems(string id)
        {
            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                string commandText = "DELETE FROM [modems] WHERE [id]=@id";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connect);
                Command.Parameters.AddWithValue("@id", id);
                Connect.Open();
                Command.ExecuteNonQuery(); 
                Connect.Close();
            }
        }
        public void DeleteModemsByImei(string imei)
        {
            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                string commandText = "DELETE FROM [modems] WHERE [imei]=@imei";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connect);
                Command.Parameters.AddWithValue("@imei", imei);
                Connect.Open();
                Command.ExecuteNonQuery();
                Connect.Close();
            }
        }
        //UPDATE table1 SET name = ‘Людмила Иванова’ WHERE id = 2;
        public void UpdateNameModemsbyImei(string imei, string name)
        {
            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                string commandText = "UPDATE modems SET name = @name WHERE imei = @imei;";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connect);
                Command.Parameters.AddWithValue("@imei", imei);
                Command.Parameters.AddWithValue("@name", name);
                Connect.Open();
                Command.ExecuteNonQuery();
                Connect.Close();
            }
        }
        public void UpdateActiveConnectionModemsbyImei(string imei, int activeConnection)
        {
            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                string commandText = "UPDATE modems SET activeConnection = @activeConnection WHERE imei = @imei;";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connect);
                Command.Parameters.AddWithValue("@imei", imei);
                Command.Parameters.AddWithValue("@activeConnection", activeConnection);
                Connect.Open();
                Command.ExecuteNonQuery();
                Connect.Close();
            }
        }

        public void UpdatePortModemsbyImei(string imei, int port)
        {
            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                string commandText = "UPDATE modems SET port = @port WHERE imei = @imei;";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connect);
                Command.Parameters.AddWithValue("@imei", imei);
                Command.Parameters.AddWithValue("@port", port);
                Connect.Open();
                Command.ExecuteNonQuery();
                Connect.Close();
            }
        }

        public List<dynamic> GetModems()
        {
            List<dynamic> records = new List<dynamic>();

            if (!File.Exists(dbFileName)) return records;

            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                Connect.Open();
                SQLiteCommand Command = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = @"SELECT * FROM [modems]"
                };
                SQLiteDataReader sqlReader = Command.ExecuteReader();

                while (sqlReader.Read()) // считываем и вносим в лист все параметры
                {
                    dynamic record = new ExpandoObject();
                    record.id = sqlReader["id"];
                    record.imei = (string)sqlReader["imei"];
                    record.port= (string)sqlReader["port"];
                    record.name= (string)sqlReader["name"];
                    record.lastConnection = sqlReader["lastConnection"];
                    record.activeConnection = sqlReader["activeConnection"];
                    records.Add(record);
                }
                Connect.Close();
            }
            return records;
        }
        public List<int> GetModemsPort()
        {
            List<int> ports = new List<int>();

            if (!File.Exists(dbFileName)) return ports;

            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                Connect.Open();
                SQLiteCommand Command = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = @"SELECT * FROM [modems]"
                };
                try
                {
                    SQLiteDataReader sqlReader = Command.ExecuteReader();
                    while (sqlReader.Read()) // считываем и вносим в лист все параметры
                    {
                        try
                        {
                            int port = Int32.Parse((string)sqlReader["port"]);
                            ports.Add(port);
                        }
                        catch { }
                    }
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                Connect.Close();
            }
            return ports;
        }
        public List<string> GetModemsImei()
        {
            List<string> modemsImei = new List<string>();

            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                Connect.Open();
                SQLiteCommand Command = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = @"SELECT * FROM [modems]"
                };
                SQLiteDataReader sqlReader = Command.ExecuteReader();

                while (sqlReader.Read()) // считываем и вносим в лист все параметры
                {
                    try
                    {
                        string imei = (string)sqlReader["imei"];
                        modemsImei.Add(imei);
                    }
                    catch { }
                }
                Connect.Close();
            }
            return modemsImei;
        }
        public int GetModemsPortByImei(string imei)
        {
            List<dynamic> records = new List<dynamic>();
            int port = 0;
            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                Connect.Open();
                SQLiteCommand Command = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = @"SELECT * FROM [modems] WHERE [imei]=@imei"
                };
                Command.Parameters.AddWithValue("@imei", imei);
                SQLiteDataReader sqlReader = Command.ExecuteReader();
                while (sqlReader.Read()) // считываем и вносим в лист все параметры
                {
                    port = Int32.Parse((string)sqlReader["port"]);
                }
                Connect.Close();
            }
            return port;
        }
        public string GetModemsNameByImei(string imei)
        {
            string name = "---";
            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                Connect.Open();
                SQLiteCommand Command = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = @"SELECT * FROM [modems] WHERE [imei]=@imei"
                };
                Command.Parameters.AddWithValue("@imei", imei);
                SQLiteDataReader sqlReader = Command.ExecuteReader();
                while (sqlReader.Read())
                {
                    name = (string)sqlReader["name"];
                }
                Connect.Close();
            }
            return name;
        }
        public List<dynamic> GetNotQuite()
        {
            // получаем данные их БД
            // сделав запрос к БД мы получим множество строк в ответе, поэтому мы их записываем в массивы/List
            List<dynamic> records = new List<dynamic>();

            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                Connect.Open();
                SQLiteCommand Command = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = @"SELECT * FROM [dbEvents] WHERE [quite] IS NULL"
                };
                SQLiteDataReader sqlReader = Command.ExecuteReader();

                while (sqlReader.Read()) // считываем и вносим в лист все параметры
                {
                    dynamic record = new ExpandoObject();
                    record.name = (string)sqlReader["name"];
                    record.date = (string)sqlReader["date"];
                    record.message = (string)sqlReader["message"];
                    records.Add(record);
                }
                Connect.Close();
            }
            return records;
        }
        public List<dynamic> GetAll()
        {
            // получаем данные их БД
            // сделав запрос к БД мы получим множество строк в ответе, поэтому мы их записываем в массивы/List
            List<dynamic> records = new List<dynamic>();

            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                Connect.Open();
                SQLiteCommand Command = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = @"SELECT * FROM [dbEvents]" 
                };
                SQLiteDataReader sqlReader = Command.ExecuteReader();

                while (sqlReader.Read()) // считываем и вносим в лист все параметры
                {
                    dynamic record = new ExpandoObject();
                    record.name = (string)sqlReader["name"];
                    record.date = (string)sqlReader["date"];
                    record.message = (string)sqlReader["message"];
                    record.quite = sqlReader["quite"];
                    records.Add(record);
                }
                Connect.Close();
            }
            return records;
        }

        public List<dynamic> GetRowForCheck(string name, DateTime dateTime, string message)
        {
            List<dynamic> records = new List<dynamic>();

            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                Connect.Open();
                SQLiteCommand Command = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = @"SELECT * FROM [dbEvents] WHERE [name]=@name AND [date]=@date AND [message]=@message"
                };
                Command.Parameters.AddWithValue("@name", name);
                Command.Parameters.AddWithValue("@date", dateTime.ToString());
                Command.Parameters.AddWithValue("@message", message);
                SQLiteDataReader sqlReader = Command.ExecuteReader();

                while (sqlReader.Read()) // считываем и вносим в лист все параметры
                {
                    dynamic record = new ExpandoObject();
                    record.name = (string)sqlReader["name"];
                    record.date = (string)sqlReader["date"];
                    record.message = (string)sqlReader["message"];
                    records.Add(record);
                }
                Connect.Close();
            }
            return records;
        }

        public void InsertRow(string name, DateTime date, string message)
        {
            // записываем информацию в базу данных
            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                string commandText = "INSERT INTO [dbEvents] ([name], [date], [message]) VALUES(@name, @date, @message)";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connect);
                Command.Parameters.AddWithValue("@name", name);
                Command.Parameters.AddWithValue("@date", date.ToString());
                Command.Parameters.AddWithValue("@message", message);
                Connect.Open();
                Command.ExecuteNonQuery();
                Connect.Close();
            }
        }
        public void QuiteRow(string name, string date, string message)
        {
            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                string commandText = "UPDATE [dbEvents] SET [quite] = @quite WHERE [name]=@name and [date]=@date and [message]=@message";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connect);
                Command.Parameters.AddWithValue("@quite", DateTime.Now.ToString());
                Command.Parameters.AddWithValue("@name", name);
                Command.Parameters.AddWithValue("@date", date);
                Command.Parameters.AddWithValue("@message", message);
                Connect.Open();
                Command.ExecuteNonQuery(); // можно эту строку вместо двух последующих строк
                Connect.Close();
            }
        }
    }
}
