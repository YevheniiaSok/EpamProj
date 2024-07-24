
namespace WebApplication2.Models
{
      public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string  Author { get; set; }= string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; 
        public DateTime Date { get; set; }= DateTime.Now;
        public string? FileName { get; set; } = string.Empty;
        public Article()
        {

        }
        public Article(int id, string title, string author, string category, string description, DateTime date, string fileName)
        {
            Id = id;
            Title = title;
            Author = author;
            Category = category;
            Description = description;
            Date = date;
            FileName = fileName;
        }
    }
}
