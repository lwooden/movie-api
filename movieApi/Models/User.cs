using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace movieApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        // creates 1-to-Many Relationship between Users<-->Reservations table (Foreign Key)
        public ICollection<Reservation> Reservations { get; set; }
    }
}
