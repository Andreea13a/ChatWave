using System;

namespace ChatWave.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        public string Avatar { get; set; }
        public string Bio { get; set; }

        public DateTime UpdatedAt { get; set; }
        // Proprietăți suplimentare pentru profil
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}