using System;
using System.Collections.Generic;

namespace Fisher.Bookstore.Models

{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public List<Book> Titles { get; set; }
    }
}