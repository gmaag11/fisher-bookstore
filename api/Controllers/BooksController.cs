using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks; 
using Fisher.Bookstore.Api.Data;
using Fisher.Bookstore.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 

namespace Fisher.Bookstore.Api.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookstoreContext db; 

        public BooksController(BookstoreContext db)
        {
            this.db = db; 

            if (this.db.Books.Count() == 0)
            {
                this.db.Books.Add(new Book()
                {
                    Id = 1, 
                    Title = "Design Patterns", 
                    Author = new Author() {
                        Id = 1,
                        Name = "Eric Gemma"
                    }, 
                    ISBN = "978-0201633610"
                }); 
                this.db.Books.Add(new Book()
                {
                    Id = 2, 
                    Title = "Continuous Delivery", 
                    Author = new Author() {
                        Id = 2,
                        Name = "Jez Humble"
                    }, 
                    ISBN = "978-0321601919"
                }); 
                this.db.Books.Add(new Book()
                {
                    Id = 3, 
                    Title = "The DevOps Handbook", 
                    Author = new Author() {
                        Id = 3,
                        Name =  "Gene Kim"
                    },
                    ISBN = "978-1942788003"
                }); 
            }
            this.db.SaveChanges(); 
        }

        [HttpGet] 
        public IActionResult Get()
        {
            return Ok(db.Books); 
        }

        [HttpGet("{id}", Name = "GetBook")]
        public IActionResult GetBook(int id)
        {
            // try to find the correct book
            var book = db.Books.FirstOrDefault(b => b.Id == id); 

            // if no book is found with the id key, return HTTP 404 Not Found
            if (book == null) 
            {
                return NotFound(); 
            }

            // return the Book inside HTTP 200 OK
            return Ok(book); 
        }

        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody]Book book) 
        {
            if(book == null) 
            {
                return BadRequest();
            }

            db.Books.Add(book);
            db.SaveChanges();

            return CreatedAtRoute("GetBook", new { id = book.Id }, book); 
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id) 
        {
            var book = db.Books.FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            db.Books.Remove(book); 
            db.SaveChanges();
            
            return NoContent(); 
        }
    }
}