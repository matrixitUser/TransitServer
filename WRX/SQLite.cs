using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TransitServer
{
    public partial class Form1 : Form
    {
        private String dbFileName;
        private SQLiteConnection m_dbConn;
        private SQLiteCommand m_sqlCmd;
        
        private void CreateDB()
        {
            if (!File.Exists(dbFileName))
                SQLiteConnection.CreateFile(dbFileName);

            try
            {
                m_dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
                m_dbConn.Open();
                m_sqlCmd.Connection = m_dbConn;

                m_sqlCmd.CommandText = "CREATE TABLE IF NOT EXISTS dbEvents (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT, date TEXT, message TEXT, quite VARCHAR(10))";
                m_sqlCmd.ExecuteNonQuery();
                
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        
        public List<dynamic> GetNotQuite()
        {
            // получаем данные их БД
            // сделав запрос к БД мы получим множество строк в ответе, поэтому мы их записываем в массивы/List
            List<dynamic> records = new List<dynamic>(); // изображение в байтах

            using (SQLiteConnection Connect = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;"))
            {
                Connect.Open();
                SQLiteCommand Command = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = @"SELECT * FROM [dbEvents] WHERE [quite] IS NULL" // выборка записей с заполненной ячейкой формата изображения, можно другой запрос составить
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
                Command.Parameters.AddWithValue("@quite", "кв");
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
