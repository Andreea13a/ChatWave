using System;

namespace ChatWave.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }

        // Proprietăți de navigare (pentru afișare)
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }

        public ChatMessage()
        {
            SentAt = DateTime.Now;
            IsRead = false;
        }
    }
}