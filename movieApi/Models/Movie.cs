using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace movieApi.Models
{
    public class Movie
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }

        public double Rating { get; set; }

        // we do not want to store images in our database schema; only pointers to them (paths)
        [NotMapped]
        public IFormFile Image { get; set; }

        public string ImageUrl { get; set; }

    }
}
