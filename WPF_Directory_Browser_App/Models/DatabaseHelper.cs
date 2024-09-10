using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MyApp.Models
{
    public static class DatabaseHelper
    {
        private static string dbPath = "Images.db";

        static DatabaseHelper()
        {
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
                var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;");
                conn.Open();
                string createTableQuery = "CREATE TABLE Images (Id INTEGER PRIMARY KEY AUTOINCREMENT, FileName TEXT, ImageData BLOB)";
                var cmd = new SQLiteCommand(createTableQuery, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public static void SaveImage(string fileName, byte[] imageData)
        {
            var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            conn.Open();
            string insertQuery = "INSERT INTO Images (FileName, ImageData) VALUES (@FileName, @ImageData)";
            var cmd = new SQLiteCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@FileName", fileName);
            cmd.Parameters.AddWithValue("@ImageData", imageData);
            cmd.ExecuteNonQuery();
        }

        public static SQLiteDataReader GetImages()
        {
            var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            conn.Open();
            string selectQuery = "SELECT * FROM Images";
            var cmd = new SQLiteCommand(selectQuery, conn);
            return cmd.ExecuteReader();
        }
    }
}