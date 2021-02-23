using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using movieApi.Data;
using movieApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        // GET: api/<MovieDbController>
        // IActionResult allows me to send status codes back in addition to an object(like our list of Movies)
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
        [HttpPost]
        public IActionResult Post([FromBody] Movie movieObj)
        {
            var nameExists = _dbContext.Movies.FirstOrDefault(m => m.Name == movieObj.Name);
            Console.WriteLine(nameExists);

            if (nameExists.Name == movieObj.Name)
            {

                return StatusCode(StatusCodes.Status409Conflict, "Something Went Wrong!");
            }

            _dbContext.Movies.Add(movieObj);
            _dbContext.SaveChanges();
            Console.WriteLine("Added {0} to the database!", movieObj.Name);
            return StatusCode(StatusCodes.Status201Created);

            
        }

        //public void Post([FromBody] Movie movieObj)
        //{
        //    _dbContext.Movies.Add(movieObj);
        //    _dbContext.SaveChanges();
        //    Console.WriteLine("Added {0} to the database!", movieObj.Name);

        //}

        // PUT api/<MovieDbController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Movie movieObj)
        {
            var movie = _dbContext.Movies.Find(id);

            if (movie == null)
            {
                return NotFound("Record Does Not Exists!");
            }
            else
            {
                movie.Name = movieObj.Name;
                movie.Language = movieObj.Language;
                _dbContext.SaveChanges();
                return Ok("Record Updated Successfully!");

            }
            
        }

        // DELETE api/<MovieDbController>/5
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
