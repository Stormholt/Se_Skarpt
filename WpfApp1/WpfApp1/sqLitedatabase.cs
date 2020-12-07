using System;
using System.Data.SQLite;
using System.IO;
using System.Data;

namespace SQLiteDatabase
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

        public void OpenConnection() //Opening the connection to the SQLite database
        {
            if (myConnection.State != System.Data.ConnectionState.Open)
            {
                myConnection.Open();
            }
        }

        public void CloseConnection() //Closing  the connection to the SQLite database
        {
            if (myConnection.State != System.Data.ConnectionState.Closed)
            {
                myConnection.Close();
            }
        }

        /// <summary>
        ///  Taking the value of the temp and light combining it with the current time of the measurment 
        ///  And storing it in the SQLite Database 
        /// </summary>
        public void sendData(int temp, int light)
        {
            string query = "INSERT INTO TempAndHumit (`temp`, `light`, `datetime`) VALUES (@temp, @light, @datetime)"; //setting up a query with the info for hte table and values 
            SQLiteCommand myCommand = new SQLiteCommand(query, myConnection); 
            myCommand.Parameters.AddWithValue("@temp", temp);
            myCommand.Parameters.AddWithValue("@light", light);
            myCommand.Parameters.AddWithValue("@datetime", ToJulianDate(DateTime.Now)); // taking the current time and converting ti to julian time so it fit in a real
            myCommand.ExecuteNonQuery(); // executing the data query
            
        }

        //https://stackoverflow.com/questions/5248827/convert-datetime-to-julian-date-in-c-sharp-tooadate-safe
        public static double ToJulianDate(DateTime date) // converting the current time to OA date and adding time for it to be Julian.
        {
            return date.ToOADate() + 2415018.5;
        }

        

        public DataTable Filldata() // Pulling Data from the Database 
        {
            SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM TempAndLight", myConnection); // Selecting the data table in the SQlite database
            SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd); // setting up a data adapter with the command 
            DataTable dt = new DataTable("Dataset");  // creating hte datatable to sen dthe data
            sda.Fill(dt); // Filling out the data
            return dt; // retuning the datatable
        }
    }
}
