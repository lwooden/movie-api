using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movieApi.Models;

namespace movieApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {

        // Creates a list of "movies" that's accessible to this controller
        private static List<Movie> movies = new List<Movie>
        {
            new Movie() { Id = 0, Name = "Blue Streak", Language = "English"},
            new Movie() { Id = 1, Name = "National Security", Language = "English" }
        };


        [HttpGet]
        // IENumberable is a way to return a number of elements from a List
        // In this case I am returning the "list" of "movies" that I initialized above
        public IEnumerable<Movie> Get()
        {
            Console.WriteLine("Here are all the movies!");
            return movies;
        }

        [HttpPost]
        // This method is void because it does not return anything
        // [FromBody] attribute tells .NET to accept the data passed in the "body" of the request
        // What is passed has to conform to the standards of my movie "model
        public void Post([FromBody]Movie movie)
        {
            Console.WriteLine("Got a request!");
            movies.Add(movie);
        }


        [HttpPut("{id}")]
        // localhost:5000/api/Movie/{id}
        // Accept an integer as a parameter to represent the index of the item I was to update
        // Accept the data in the request body to overwrite/update the element
        public void Put(int id, [FromBody]Movie movie)
        {
            movies[id] = movie;
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            movies.RemoveAt(id);
            Console.WriteLine("Deleted movie id: {0}", id);
        }
    }
}