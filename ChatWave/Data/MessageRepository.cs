using ChatWave.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Message = ChatWave.Models.Message;

namespace ChatWave.Data
{
    public class MessageRepository
    {
        public static bool AddMessage(Message message)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO Messages (SenderId, SenderName, Text, SentAt) VALUES (@senderId, @senderName, @text, @sentAt)";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@senderId", message.SenderId);
                    cmd.Parameters.AddWithValue("@senderName", message.SenderName);
                    cmd.Parameters.AddWithValue("@text", message.Text);
                    cmd.Parameters.AddWithValue("@sentAt", message.SentAt);
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

        public static List<Message> GetAllMessages()
        {
            var messages = new List<Message>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Messages ORDER BY SentAt ASC";
                    var cmd = new MySqlCommand(query, conn);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        messages.Add(new Message
                        {
                            Id = reader.GetInt32("Id"),
                            SenderId = reader.GetInt32("SenderId"),
                            SenderName = reader.GetString("SenderName"),
                            Text = reader.GetString("Text"),
                            SentAt = reader.GetDateTime("SentAt")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare: " + ex.Message);
            }
            return messages;
        }

        public static bool DeleteMessage(int id)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Messages WHERE Id = @id";
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