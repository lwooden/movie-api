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
using Microsoft.Extensions.Logging;

namespace movieApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesDbController : ControllerBase
    {
        protected readonly ILogger<MoviesDbController> _logger; // create ILogger property of type controller
        private MovieDbContext _dbContext;

        public MoviesDbController(MovieDbContext dbContext, ILogger<MoviesDbController> logger)
        {
            _logger = logger; // add ILogger as a parameter in the constructor and initialize it
            _dbContext = dbContext;


        }

        /** 
         * 
         IActionResult allows me to send status codes back in addition to an object(like our list of Movies) in the form of a response
        
         **/

        [Authorize]
        [HttpGet("[action]")]
        public IActionResult AllMovies(string sort, int? pageNumber, int? pageSize)
        {
            // this style returns every property for every movie; I don't need all of the properties
            //return Ok(_dbContext.Movies);

            // instead loop through all of the movies returned from the db and only get the values that I really need instead
            // of returning the entire dataset

            // int? syntax marks a parameter as "nullable"

            // sets default values in case no values are sent by the caller
            // functions kind of like a ternary operator
            var currentPageNumber = pageNumber ?? 1;
            var currentPageSize = pageSize ?? 3;

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

            // implement paging using Skip & Take Algorithm
            // pageSize = number of elements per page to display

            switch (sort)
            {
                case "desc":
                    return Ok(movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize).OrderByDescending(m => m.Rating));
                case "asc":
                    return Ok(movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize).OrderBy(m => m.Rating));
                default:
                    return Ok(movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize));

            }

            //return Ok(movies);
        }


        [HttpGet("[action]")]
        public IActionResult FindMovies(string searchTerm)
        {
            // sql style query that queries the database within the specific context with these conditions and saves the results to a variable
            var movies = from movie in _dbContext.Movies
                         where movie.Name.StartsWith(searchTerm)
                         select new
                         {
                             Id = movie.Id,
                             Name = movie.Name,
                             ImageUrl = movie.ImageUrl
                         };

            return Ok(movies);


        }


        // GET: api/<MovieDbController>
        [HttpGet]

        public IActionResult Get()
        {
            _logger.LogInformation("Getting all movies!");
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
                _logger.LogWarning("Movie does not appear to exist!");
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
