using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using movieApi.Models;

namespace movieApi.Models
{
    public class Movie
    {

        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public string Language { get; set; }

        public string Duration { get; set; }

        public DateTime PlayingDate { get; set; }

        public DateTime PlayingTime { get; set; }

        public double TicketPrice { get; set; }

        public double Rating { get; set; }

        public string Genre { get; set; }

        public string TrailerUrl { get; set; }

        public string ImageUrl { get; set; }

        // we do not want to store images in our database schema; only pointers to them (paths)
        [NotMapped]
        public IFormFile Image { get; set; }

        // creates 1-to-Many Relationship between Movies<-->Reservations table (Foreign Key)
        public ICollection<Reservation> Reservations { get; set; }

    }
}
