using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using movieApi.Data;
using movieApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace movieApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class ReservationsController : ControllerBase
    {
        private MovieDbContext _dbContext;
        protected readonly ILogger<ReservationsController> _logger;


        public ReservationsController(MovieDbContext dbContext, ILogger<ReservationsController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;

        }

        [HttpPost]
        public IActionResult Post([FromBody] Reservation reservationObj)
        {
            reservationObj.ReservationTime = DateTime.Now;
            _dbContext.Reservations.Add(reservationObj);
            _dbContext.SaveChanges();
            _logger.LogInformation("Reservation has been added!");

            return StatusCode(StatusCodes.Status201Created);

        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("Getting All Reservations!");

            /**
             * Select the dataset of the first table -> IN Reservations Table
             * Use join to select the dataset of the second table -> IN User Table
             * Use join to select the dataset of the third table -> IN Movies 
             * the keyword "on" represents the foreign key I will use to access the joining table
             * reservation, customer, and movie are the variables I use to access the underlying datasets
             **/

            var reservations = from reservation in _dbContext.Reservations
                               join customer in _dbContext.User on reservation.UserId equals customer.Id
                               join movie in _dbContext.Movies on reservation.MovieId equals movie.Id
                               select new
                               {
                                   // these will be the values return to the caller
                                   Id = reservation.Id,
                                   ReservationTime = reservation.ReservationTime,
                                   CustomerName = customer.Id,
                                   MovieName = movie.Name
                               };

            return Ok(reservations);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult GetReservationDetail(int id)
        {

            /**
             * Adding the "where" clause helps me perform my doesExist check within my query
             * When I do not wrap the sql statement in parentheses, .NET always returns an "array" even if it is only 1 value returned. This is not a good api experience
             * Wrapping the statement in parens and invoking FirstOrDefault returns only a single element with no array syntax. This is the way to go!
             **/
            var reservationResult = (from reservation in _dbContext.Reservations
                                     join customer in _dbContext.User on reservation.UserId equals customer.Id
                                     join movie in _dbContext.Movies on reservation.MovieId equals movie.Id
                                     where reservation.Id == id
                                     select new
                                     {
                                         // these will be the values return to the caller
                                         Id = reservation.Id,
                                         ReservationTime = reservation.ReservationTime,
                                         CustomerName = customer.Id,
                                         MovieName = movie.Name,
                                         Email = customer.Email,
                                         Qty = reservation.Qty,
                                         Price = reservation.Price,
                                         Phone = reservation.Phone,
                                         PlayingDate = movie.PlayingDate,
                                         PlayingTime = movie.PlayingTime
                                     }).FirstOrDefault();

            if (reservationResult == null)
            {
                _logger.LogWarning("Reservation does not exist!");
            }

            return Ok(reservationResult);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteReservation(int id)
        {
            var reservation = _dbContext.Reservations.Find(id);

            if (reservation == null)
            {
                return NotFound("Reservation Does Not Exists!");
            }

            _dbContext.Reservations.Remove(reservation);
            _dbContext.SaveChanges();
            _logger.LogInformation("Deleted the following reservation: {0}", reservation.Id);

            return Ok("Record Deleted Successfully!");
        }

    }

}
