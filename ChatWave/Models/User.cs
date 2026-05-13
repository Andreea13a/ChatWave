using System;

namespace ChatWave.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public byte[] ProfileImage { get; set; }      // Pentru stocare în baza de date (byte array)
        public string ProfileImagePath { get; set; }  // Sau pentru cale către fișier
    }
}