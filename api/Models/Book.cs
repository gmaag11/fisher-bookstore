using System; 

namespace Fisher.Bookstore.Api.Models
{
    public class Book
    {
        public int Id { get; set; } // shortcut to build a property: "prop" + tab

        public string Title { get; set; }

        public Author Author { get; set; }

        public string ISBN { get; set; }

        public DateTime PublishDate { get; set; }

        public string Publisher { get; set; }

        public void ChangePublicationDate(DateTime dateTime)
        {
            this.PublishDate = dateTime;
        }
    }
}