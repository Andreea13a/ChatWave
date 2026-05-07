using MySql.Data.MySqlClient;
using System;


namespace ChatWave.Data
{

    public class DatabaseHelper
    {
        private static string connectionString =
            "Server=localhost;Database=chatwave;Uid=root;Pwd=789321andreeaVlad;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public static void TestConnection()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    Console.WriteLine("Conexiune reusita!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare: " + ex.Message);
            }
        }
    }
}
