using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DigitalBookHistoryAPI.Interface;

namespace DigitalBookHistoryAPI.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public HomeController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        // GET: api/Home
        [HttpGet]
        //[Route("/[controller]/[action]")]
        public IActionResult GetBooks() => Ok(_bookRepository.GetBooks());


        // GET: api/Home/5
        [HttpGet] // [HttpGet("{id}", Name = "Get")]
        [Route("/[controller]/[action]/{id}")]
        public IActionResult GetBookById(int id) => Ok(_bookRepository.GetBookById(id));

        [HttpGet]
        [Route("/[controller]/[action]/{authorName}")]
        public IActionResult GetBooksByAuthor(string authorName) => Ok(_bookRepository.GetBooksByAuthor(authorName));

        //// POST: api/Home
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT: api/Home/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
