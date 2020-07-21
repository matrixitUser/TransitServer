using System;
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

        #region CreateDB
        public void CreateDB()
        {
            if (!File.Exists(dbFileName))
                SQLiteConnection.CreateFile(dbFileName);

            try
            {
                m_dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
                m_dbConn.Open();
                SQLiteCommand m_sqlCmd1 = new SQLiteCommand(m_dbConn);
                SQLiteCommand m_sqlCmd2 = new SQLiteCommand(m_dbConn);
                SQLiteCommand m_sqlCmd3 = new SQLiteCommand(m_dbConn);

                m_sqlCmd1.CommandText = "CREATE TABLE IF NOT EXISTS dbEvents (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT, date TEXT, message TEXT, quite TEXT)";
                m_sqlCmd2.CommandText = "CREATE TABLE IF NOT EXISTS modems (id INTEGER PRIMARY KEY AUTOINCREMENT, imei TEXT, port TEXT, name TEXT, lastConnection TEXT, activeConnection INTEGER, groups TEXT DEFAULT '0')";
                m_sqlCmd3.CommandText = "CREATE TABLE IF NOT EXISTS nodesTree (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT, parent TEXT)";

                m_sqlCmd1.ExecuteNonQuery();
                m_sqlCmd2.ExecuteNonQuery();
                m_sqlCmd3.ExecuteNonQuery();

            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        #endregion

        #region TreeView
        public void DeleteNode(string id)
        {
            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                string commandText = "DELETE FROM [nodesTree] WHERE [id]=@id";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connect);
                Command.Parameters.AddWithValue("@id", id);
                Connect.Open();
                Command.ExecuteNonQuery();
                Connect.Close();
            }
        }
        public void InsertNode(string nameNode, string parent)
        {
            // записываем информацию в базу данных
            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                string commandText = "INSERT INTO [nodesTree] ([name], [parent]) VALUES(@nameNode, @parent)";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connect);
                Command.Parameters.AddWithValue("@nameNode", nameNode);
                Command.Parameters.AddWithValue("@parent", parent);
                Connect.Open();
                Command.ExecuteNonQuery();
                Connect.Close();
            }
        }
        public List<dynamic> GetAllNodesTree()
        {
            List<dynamic> listNodesTree = new List<dynamic>();

            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                Connect.Open();
                SQLiteCommand Command = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = @"SELECT * FROM [nodesTree]"
                };
                SQLiteDataReader sqlReader = Command.ExecuteReader();

                while (sqlReader.Read()) // считываем и вносим в лист все параметры
                {
                    try
                    {
                        dynamic record = new ExpandoObject();
                        record.id = sqlReader["id"];
                        record.name = (string)sqlReader["name"];
                        record.parent = (string)sqlReader["parent"];
                        listNodesTree.Add(record);
                    }
                    catch { }
                }
                Connect.Close();
            }
            return listNodesTree;
        }

        public List<string> GetParentsNodeForCheck(string textNode)
        {
            List<string> listNodesTree = new List<string>();

            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                Connect.Open();
                SQLiteCommand Command = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = @"SELECT * FROM [nodesTree]  WHERE [parent]=@textNode"
                };
                Command.Parameters.AddWithValue("@textNode", textNode);
                SQLiteDataReader sqlReader = Command.ExecuteReader();

                while (sqlReader.Read()) // считываем и вносим в лист все параметры
                {
                    try
                    {
                        string name = (string)sqlReader["name"];
                        listNodesTree.Add(name);
                    }
                    catch { }
                }
                Connect.Close();
            }
            return listNodesTree;
        }

        public string GetIdGroup(string imei)
        {
            string id = "";

            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                Connect.Open();
                SQLiteCommand Command = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = @"SELECT * FROM [modems]  WHERE [imei]=@imei"
                };
                Command.Parameters.AddWithValue("@imei", imei);
                SQLiteDataReader sqlReader = Command.ExecuteReader();

                while (sqlReader.Read()) // считываем и вносим в лист все параметры
                {
                    try
                    {
                        id = (string)sqlReader["groups"];
                    }
                    catch(Exception e) { MessageBox.Show(e.Message); }
                }
                Connect.Close();
            }
            return id;
        }
        public string GetCurrentGroup(string id)
        {
            string nameGroup = String.Empty;
            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                Connect.Open();
                SQLiteCommand Command = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = @"SELECT * FROM [nodesTree] WHERE [id]=@id"
                };
                Command.Parameters.AddWithValue("@id", id);
                SQLiteDataReader sqlReader = Command.ExecuteReader();

                while (sqlReader.Read()) // считываем и вносим в лист все параметры
                {
                    try
                    {
                        nameGroup = (string)sqlReader["name"];
                    }
                    catch { }
                }
                Connect.Close();
            }
            return nameGroup;
        }
        #endregion

        #region ModemsWork
        //update
        public void UpdateGroupModems(string imei, string groups)
        {
            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                string commandText = "UPDATE modems SET groups = @groups WHERE imei = @imei;";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connect);
                Command.Parameters.AddWithValue("@imei", imei);
                Command.Parameters.AddWithValue("@groups", groups);
                Connect.Open();
                Command.ExecuteNonQuery();
                Connect.Close();
            }
        }
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
        public void ZeroingActiveConnection()
        {
            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                string commandText = "UPDATE modems SET activeConnection = @activeConnection";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connect);
                Command.Parameters.AddWithValue("@activeConnection", 0);
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
        public void UpdateLastConnectionModemsbyImei(string imei, string lastConnection)
        {
            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                string commandText = "UPDATE modems SET lastConnection = @lastConnection WHERE imei = @imei;";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connect);
                Command.Parameters.AddWithValue("@imei", imei);
                Command.Parameters.AddWithValue("@lastConnection", lastConnection);
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
        //Insert
        public void InsertModems(string imei, string port, string name)
        {
            // записываем информацию в базу данных
            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                string commandText = "INSERT INTO [modems] ([imei], [port], [name], [activeConnection]) VALUES(@imei, @port, @name, @activeConnection)";
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
                Command.Parameters.AddWithValue("@name", "Новый объект");
                Command.Parameters.AddWithValue("@activeConnection", 0);
                Connect.Open();
                Command.ExecuteNonQuery();
                Connect.Close();
            }
        }
        //Delete
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
        //Get
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
                    record.port = (string)sqlReader["port"];
                    record.name = (string)sqlReader["name"];
                    record.lastConnection = sqlReader["lastConnection"];
                    record.activeConnection = sqlReader["activeConnection"];
                    record.group = sqlReader["groups"];
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
                catch (Exception e)
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
        public string GetModemsLastConnectionByImei(string imei)
        {
            string lastConnection = "---";
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
                    lastConnection = (string)sqlReader["lastConnection"];
                }
                Connect.Close();
            }
            return lastConnection;
        }
        #endregion

        #region dbEvents
        public List<dynamic> GetNotQuite()
        {
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
        #endregion
    }
}
