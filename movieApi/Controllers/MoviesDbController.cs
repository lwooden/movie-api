using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using movieApi.Data;
using movieApi.Models;
using Microsoft.AspNetCore.Authorization;


namespace movieApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesDbController : ControllerBase
    {
        private MovieDbContext _dbContext;

        public MoviesDbController(MovieDbContext dbContext)
        {
            _dbContext = dbContext;

        }

        // IActionResult allows me to send status codes back in addition to an object(like our list of Movies)

        [Authorize]
        [HttpGet("[action]")]
        public IActionResult AllMovies(string sort)
        {
            // this style returns every property for every movie; I don't need all of the properties
            //return Ok(_dbContext.Movies);

            // instead loop through all of the movies returned from the db and only get the values that I really need instead
            // of returning the entire dataset

            var movies = from movie in _dbContext.Movies
            select new
            {
                Id = movie.Id,
                Name = movie.Name,
                Duration = movie.Duration,
                Language = movie.Language,
                Rating = movie.Rating,
                Genre = movie.Genre,
                ImageUrl = movie.ImageUrl
            };

            switch (sort)
            {
                case "desc":
                    return Ok(movies.OrderByDescending(m => m.Rating));
                case "asc":
                    return Ok(movies.OrderBy(m => m.Rating));
                default:
                    return Ok(movies);

            }

            //return Ok(movies);
        }
        // GET: api/<MovieDbController>
        [HttpGet]

        public IActionResult Get()
        {
            return Ok(_dbContext.Movies);
            //return BadRequest()
            //return NotFound()
            //return StatusCode(StatusCodes.Status201Created)
        }

        //public IEnumerable<Movie> Get()
        //{
        //    return _dbContext.Movies;
        //}

        [HttpGet("[action]/{id}")]
        public IActionResult MovieDetails(int id)
        {
           var movie = _dbContext.Movies.Find(id);

            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        // GET api/<MovieDbController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            
            var movie = _dbContext.Movies.Find(id);

            if (movie == null)
            {
                return NotFound("Record Does Not Exists!");
            }

            return Ok(movie);
        }

        // POST api/<MovieDbController>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Post([FromForm] Movie movieObj)
        {
            var guid = Guid.NewGuid();
            var filePath = Path.Combine("wwwroot", guid + ".jpg");

            if (movieObj.Image != null)
            {
                var fileStream = new FileStream(filePath, FileMode.Create);
                movieObj.Image.CopyTo(fileStream);
            }

            movieObj.ImageUrl = filePath.Remove(0, 7);
            _dbContext.Movies.Add(movieObj);
            _dbContext.SaveChanges();

            return StatusCode(StatusCodes.Status201Created);


        }

        //<-- Version 2 -->
        //public IActionResult Post([FromBody] Movie movieObj)
        //{
        //    var nameExists = _dbContext.Movies.FirstOrDefault(m => m.Name == movieObj.Name);
        //    Console.WriteLine(nameExists);

        //    if (nameExists.Name == movieObj.Name)
        //    {

        //        return StatusCode(StatusCodes.Status409Conflict, "Something Went Wrong!");
        //    }

        //    _dbContext.Movies.Add(movieObj);
        //    _dbContext.SaveChanges();
        //    Console.WriteLine("Added {0} to the database!", movieObj.Name);
        //    return StatusCode(StatusCodes.Status201Created);


        //}

        //<-- Version 1 -->
        //public void Post([FromBody] Movie movieObj)
        //{
        //    _dbContext.Movies.Add(movieObj);
        //    _dbContext.SaveChanges();
        //    Console.WriteLine("Added {0} to the database!", movieObj.Name);

        //}

        // PUT api/<MovieDbController>/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromForm] Movie movieObj)
        {
            var movie = _dbContext.Movies.Find(id);

            if (movie == null)
            {
                return NotFound("Record Does Not Exists!");
            }
            else
            {
                var guid = Guid.NewGuid();
                var filePath = Path.Combine("wwwroot", guid + ".jpg");

                if (movieObj.Image != null)
                {
                    var fileStream = new FileStream(filePath, FileMode.Create);
                    movieObj.Image.CopyTo(fileStream);
                    movieObj.ImageUrl = filePath.Remove(0, 7);
                }

               
                movie.Name = movieObj.Name;
                movie.Description = movieObj.Description;
                movie.Language = movieObj.Language;
                movie.Duration = movieObj.Duration;
                movie.PlayingDate = movieObj.PlayingDate;
                movie.PlayingTime = movieObj.PlayingTime;
                movie.TicketPrice = movieObj.TicketPrice;
                movie.Rating = movieObj.Rating;
                movie.Genre = movieObj.Genre;
                movie.TrailerUrl = movieObj.TrailerUrl;
                _dbContext.SaveChanges();
                return Ok("Record Updated Successfully!");

            }
            
        }

        // DELETE api/<MovieDbController>/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var movie = _dbContext.Movies.Find(id);

            if (movie == null)
            {
                return NotFound("Record Does Not Exists!");
            }

            _dbContext.Movies.Remove(movie);
            _dbContext.SaveChanges();
            Console.WriteLine("Deleted the following movie: {0}", movie.Name);
            return Ok("Record Deleted Successfully!");
        }

        //public void Delete(int id)
        //{
        //   var movie = _dbContext.Movies.Find(id);
        //   _dbContext.Movies.Remove(movie);
        //   _dbContext.SaveChanges();
        //   Console.WriteLine("Deleted the following movie: {0}", movie.Name);
        //}
    }
}
