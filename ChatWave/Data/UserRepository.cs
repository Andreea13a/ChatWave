using ChatWave.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

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

                    string userQuery = @"
            INSERT INTO Users
            (Username, Password, Role, CreatedAt)
            VALUES
            (@username, @password, @role, @createdAt)";

                    var userCmd = new MySqlCommand(userQuery, conn);

                    userCmd.Parameters.AddWithValue("@username", user.Username);
                    userCmd.Parameters.AddWithValue("@password", user.Password);
                    userCmd.Parameters.AddWithValue("@role", user.Role);
                    userCmd.Parameters.AddWithValue("@createdAt", user.CreatedAt);

                    userCmd.ExecuteNonQuery();

                    int userId = (int)userCmd.LastInsertedId;

                    string profileQuery = @"
            INSERT INTO UserProfiles
            (UserId, Email, Phone, Bio, UpdatedAt)
            VALUES
            (@userId, @email, @phone, @bio, @updatedAt)";

                    var profileCmd = new MySqlCommand(profileQuery, conn);

                    profileCmd.Parameters.AddWithValue("@userId", userId);
                    profileCmd.Parameters.AddWithValue("@email", user.Email);
                    profileCmd.Parameters.AddWithValue("@phone", user.Phone);
                    profileCmd.Parameters.AddWithValue("@bio", "");
                    profileCmd.Parameters.AddWithValue("@updatedAt", DateTime.Now);

                    profileCmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public static List<User> GetAllUsers()
        {
            var users = new List<User>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
                SELECT 
                    u.Id,
                    u.Username,
                    u.Password,
                    u.Role,
                    u.CreatedAt,
                    up.Email,
                    up.Phone
                FROM Users u
                LEFT JOIN UserProfiles up 
                    ON u.Id = up.UserId";

                    var cmd = new MySqlCommand(query, conn);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt32("Id"),
                            Username = reader.GetString("Username"),
                            Password = reader.GetString("Password"),

                            Email = reader["Email"] != DBNull.Value
                                ? reader["Email"].ToString()
                                : "",

                            Phone = reader["Phone"] != DBNull.Value
                                ? reader["Phone"].ToString()
                                : "",

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

                    // șterge profilul mai întâi (obligatoriu)
                    string q1 = "DELETE FROM UserProfiles WHERE UserId = @id";
                    var cmd1 = new MySqlCommand(q1, conn);
                    cmd1.Parameters.AddWithValue("@id", id);
                    cmd1.ExecuteNonQuery();

                    // apoi șterge userul
                    string q2 = "DELETE FROM Users WHERE Id = @id";
                    var cmd2 = new MySqlCommand(q2, conn);
                    cmd2.Parameters.AddWithValue("@id", id);
                    cmd2.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public static bool UpdateProfile(int id, string email, string phone)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string checkQuery = "SELECT COUNT(*) FROM UserProfiles WHERE UserId = @id";
                    var checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@id", id);
                    int profileCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                    string query = profileCount > 0
                        ? @"UPDATE UserProfiles
                            SET Email = @email, Phone = @phone, UpdatedAt = @updatedAt
                            WHERE UserId = @id"
                        : @"INSERT INTO UserProfiles (UserId, Email, Phone, Bio, UpdatedAt)
                            VALUES (@id, @email, @phone, '', @updatedAt)";

                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    cmd.Parameters.AddWithValue("@updatedAt", DateTime.Now);
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

        public static bool UpdatePassword(int id, string newPassword)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE Users SET Password=@password WHERE Id=@id";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@password", newPassword);
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

        // ========== NOILE METODE PENTRU IMAGINEA DE PROFIL ==========

        /// <summary>
        /// Salvează imaginea de profil pentru un utilizator
        /// </summary>
        public static bool SaveProfileImage(int userId, byte[] imageData)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Verifică dacă există deja o înregistrare
                    string checkQuery = "SELECT COUNT(*) FROM UserProfiles WHERE UserId = @userId";
                    var checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@userId", userId);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count > 0)
                    {
                        // Actualizează existent
                        string updateQuery = "UPDATE UserProfiles SET ProfileImage = @image, UpdatedAt = @updatedAt WHERE UserId = @userId";
                        var updateCmd = new MySqlCommand(updateQuery, conn);
                        updateCmd.Parameters.AddWithValue("@image", imageData);
                        updateCmd.Parameters.AddWithValue("@updatedAt", DateTime.Now);
                        updateCmd.Parameters.AddWithValue("@userId", userId);
                        updateCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        // Inserează nou
                        string insertQuery = "INSERT INTO UserProfiles (UserId, ProfileImage, UpdatedAt) VALUES (@userId, @image, @updatedAt)";
                        var insertCmd = new MySqlCommand(insertQuery, conn);
                        insertCmd.Parameters.AddWithValue("@userId", userId);
                        insertCmd.Parameters.AddWithValue("@image", imageData);
                        insertCmd.Parameters.AddWithValue("@updatedAt", DateTime.Now);
                        insertCmd.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare la salvarea imaginii: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Încarcă imaginea de profil pentru un utilizator
        /// </summary>
        public static byte[] GetProfileImage(int userId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT ProfileImage FROM UserProfiles WHERE UserId = @userId";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return (byte[])result;
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare la încărcarea imaginii: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Obține toți utilizatorii cu imaginile lor de profil
        /// </summary>
        public static List<User> GetAllUsersWithImages()
        {
            var users = new List<User>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
                SELECT 
                    u.Id,
                    u.Username,
                    u.Password,
                    u.Role,
                    u.CreatedAt,
                    up.Email,
                    up.Phone,
                    up.ProfileImage
                FROM Users u
                LEFT JOIN UserProfiles up 
                    ON u.Id = up.UserId";

                    var cmd = new MySqlCommand(query, conn);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var user = new User
                        {
                            Id = reader.GetInt32("Id"),
                            Username = reader.GetString("Username"),
                            Password = reader.GetString("Password"),
                            Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "",
                            Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : "",
                            Role = reader.GetString("Role"),
                            CreatedAt = reader.GetDateTime("CreatedAt")
                        };

                        // Adaugă imaginea dacă există
                        if (reader["ProfileImage"] != DBNull.Value)
                        {
                            user.ProfileImage = (byte[])reader["ProfileImage"];
                        }

                        users.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare: " + ex.Message);
            }

            return users;
        }

        /// <summary>
        /// Obține un singur utilizator după ID cu imaginea sa
        /// </summary>
        public static User GetUserById(int id)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
                SELECT 
                    u.Id,
                    u.Username,
                    u.Password,
                    u.Role,
                    u.CreatedAt,
                    up.Email,
                    up.Phone,
                    up.ProfileImage
                FROM Users u
                LEFT JOIN UserProfiles up 
                    ON u.Id = up.UserId
                WHERE u.Id = @id";

                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        var user = new User
                        {
                            Id = reader.GetInt32("Id"),
                            Username = reader.GetString("Username"),
                            Password = reader.GetString("Password"),
                            Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "",
                            Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : "",
                            Role = reader.GetString("Role"),
                            CreatedAt = reader.GetDateTime("CreatedAt")
                        };

                        if (reader["ProfileImage"] != DBNull.Value)
                        {
                            user.ProfileImage = (byte[])reader["ProfileImage"];
                        }

                        return user;
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Șterge imaginea de profil a unui utilizator
        /// </summary>
        public static bool DeleteProfileImage(int userId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE UserProfiles SET ProfileImage = NULL WHERE UserId = @userId";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare la ștergerea imaginii: " + ex.Message);
                return false;
            }
        }
    }
}
