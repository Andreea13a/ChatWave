using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ChatWave.Models;

namespace ChatWave.Data
{
    public class UserRepository
    {
        public static bool AddUser(User user)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO Users (Username, Password, Role, CreatedAt) VALUES (@username, @password, @role, @createdAt)";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", user.Username);
                    cmd.Parameters.AddWithValue("@password", user.Password);
                    cmd.Parameters.AddWithValue("@role", user.Role);
                    cmd.Parameters.AddWithValue("@createdAt", user.CreatedAt);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare: " + ex.Message);
                return false;
            }
        }

        public static User GetUserByUsername(string username)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Users WHERE Username = @username";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32("Id"),
                            Username = reader.GetString("Username"),
                            Password = reader.GetString("Password"),
                            Role = reader.GetString("Role"),
                            CreatedAt = reader.GetDateTime("CreatedAt")
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare: " + ex.Message);
            }
            return null;
        }

        public static List<User> GetAllUsers()
        {
            var users = new List<User>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Users";
                    var cmd = new MySqlCommand(query, conn);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt32("Id"),
                            Username = reader.GetString("Username"),
                            Password = reader.GetString("Password"),
                            Role = reader.GetString("Role"),
                            CreatedAt = reader.GetDateTime("CreatedAt")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare: " + ex.Message);
            }
            return users;
        }

        public static bool DeleteUser(int id)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Users WHERE Id = @id";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare: " + ex.Message);
                return false;
            }
        }
    }
}