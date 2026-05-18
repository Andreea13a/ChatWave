using ChatWave.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ChatWave
{
    public class LoggedUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
    }

    public static class UserService
    {

        public static LoggedUser Login(string username, string password)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"
                        SELECT
                            u.Id,
                            u.Username,
                            u.Role,
                            up.Email,
                            up.Phone,
                            u.Password
                        FROM Users u
                        LEFT JOIN UserProfiles up ON u.Id = up.UserId
                        WHERE u.Username = @u AND u.Password = @p";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@p", password);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new LoggedUser
                                {
                                    Id = reader.GetInt32("Id"),
                                    Username = reader.GetString("Username"),
                                    Role = reader.GetString("Role"),
                                    Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString("Email"),
                                    Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? "" : reader.GetString("Phone"),
                                    Password = reader.GetString("Password")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare DB: " + ex.Message);
            }
            return null;
        }

        public static bool Register(string username, string password, string email, string phone = "")
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string sqlUser = @"INSERT INTO Users (Username, Password, Role, CreatedAt)
                                       VALUES (@u, @p, 'user', @d);
                                       SELECT LAST_INSERT_ID();";

                    int newId;
                    using (var cmd = new MySqlCommand(sqlUser, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@p", password);
                        cmd.Parameters.AddWithValue("@d", DateTime.Now);
                        newId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    string sqlProfile = @"INSERT INTO UserProfiles (UserId, Email, Phone, UpdatedAt)
                                          VALUES (@uid, @email, @phone, @d)";

                    using (var cmd = new MySqlCommand(sqlProfile, conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", newId);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@phone", phone);
                        cmd.Parameters.AddWithValue("@d", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare: " + ex.Message);
                return false;
            }
        }

       

        // ← înlocuiești metoda veche cu asta:
        public static List<(int UserId, string Username)> GetConversations(int currentUserId)
        {
            var list = new List<(int, string)>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT DISTINCT " +
                                 "CASE WHEN cr.User1Id = @uid THEN cr.User2Id ELSE cr.User1Id END AS OtherUserId, " +
                                 "u.Username " +
                                 "FROM ChatRooms cr " +
                                 "JOIN Users u ON u.Id = CASE WHEN cr.User1Id = @uid THEN cr.User2Id ELSE cr.User1Id END " +
                                 "WHERE cr.User1Id = @uid OR cr.User2Id = @uid";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", currentUserId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                list.Add((reader.GetInt32("OtherUserId"), reader.GetString("Username")));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare: " + ex.Message);
            }
            return list;
        }

    } // ← închide UserService
} // ← închide namespace
