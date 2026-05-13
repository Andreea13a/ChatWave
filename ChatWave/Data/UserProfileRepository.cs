using MySql.Data.MySqlClient;
using ChatWave.Models;

namespace ChatWave.Data
{
    public class UserProfileRepository
    {
        public static bool AddProfile(UserProfile profile)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        INSERT INTO UserProfiles
                        (UserId, Email, Phone, Bio, UpdatedAt)
                        VALUES
                        (@userId, @email, @phone, @bio, @updatedAt)";

                    var cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@userId", profile.UserId);
                    cmd.Parameters.AddWithValue("@email", profile.Email);
                    cmd.Parameters.AddWithValue("@phone", profile.Phone);
                    cmd.Parameters.AddWithValue("@bio", profile.Bio);
                    cmd.Parameters.AddWithValue("@updatedAt", profile.UpdatedAt);

                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}