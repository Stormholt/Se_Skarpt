using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.IO;
using System.Data;

namespace WpfApp1
{
    class sqLitedatabase
    {
        public SQLiteConnection myConnection;

        public sqLitedatabase()
        {
            myConnection = new SQLiteConnection("Data Source=database.sqlite");

            if (!File.Exists("./database.sqlite"))
            {
                SQLiteConnection.CreateFile("database.sqlite");
                System.Console.WriteLine("Database created");
            }
        }

        public void OpenConnection()
        {
            if (myConnection.State != System.Data.ConnectionState.Open)
            {
                myConnection.Open();
            }
        }

        public void CloseConnection()
        {
            if (myConnection.State != System.Data.ConnectionState.Closed)
            {
                myConnection.Close();
            }
        }

        public void sendData(int temp, int humit)
        {
            
            string query = "INSERT INTO TempAndHumit (`temp`, `humit`, `datetime`) VALUES (@temp, @humit, @datetime)";
            SQLiteCommand myCommand = new SQLiteCommand(query, myConnection);
            myCommand.Parameters.AddWithValue("@temp", temp);
            myCommand.Parameters.AddWithValue("@humit", humit);
            myCommand.Parameters.AddWithValue("@datetime", ToJulianDate(DateTime.Now));
            myCommand.ExecuteNonQuery();
            
        }

        //https://stackoverflow.com/questions/5248827/convert-datetime-to-julian-date-in-c-sharp-tooadate-safe
        public static double ToJulianDate(DateTime date)
        {
            return date.ToOADate() + 2415018.5;
        }

        

        public DataTable Filldata()
        {
            SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM TempAndHumit", myConnection);
            SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
            DataTable dt = new DataTable("Dataset");

            sda.Fill(dt);

            //Dataset.ItemsSource = dt.DefaultView;
            return dt;
        }
    }
}
