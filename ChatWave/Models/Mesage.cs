using System;

namespace ChatWave.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public string Text { get; set; }
        public DateTime SentAt { get; set; }
    }
}