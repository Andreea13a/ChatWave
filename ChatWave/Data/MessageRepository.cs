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
                    string query = @"INSERT INTO Messages 
                        (SenderId, SenderName, ReceiverId, ReceiverName, Text, SentAt) 
                        VALUES 
                        (@senderId, @senderName, @receiverId, @receiverName, @text, @sentAt)";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@senderId", message.SenderId);
                    cmd.Parameters.AddWithValue("@senderName", message.SenderName);
                    cmd.Parameters.AddWithValue("@receiverId", message.ReceiverId);
                    cmd.Parameters.AddWithValue("@receiverName", message.ReceiverName);
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
                            ReceiverId = reader.IsDBNull(reader.GetOrdinal("ReceiverId")) ? 0 : reader.GetInt32("ReceiverId"),
                            ReceiverName = reader.IsDBNull(reader.GetOrdinal("ReceiverName")) ? "" : reader.GetString("ReceiverName"),
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

        public static List<Message> GetMessagesBetweenUsers(int user1Id, int user2Id)
        {
            var messages = new List<Message>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT * FROM Messages 
                        WHERE (SenderId = @user1 AND ReceiverId = @user2)
                        OR (SenderId = @user2 AND ReceiverId = @user1)
                        ORDER BY SentAt ASC";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user1", user1Id);
                    cmd.Parameters.AddWithValue("@user2", user2Id);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        messages.Add(new Message
                        {
                            Id = reader.GetInt32("Id"),
                            SenderId = reader.GetInt32("SenderId"),
                            SenderName = reader.GetString("SenderName"),
                            ReceiverId = reader.GetInt32("ReceiverId"),
                            ReceiverName = reader.GetString("ReceiverName"),
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