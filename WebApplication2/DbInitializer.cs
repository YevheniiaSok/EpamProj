using WebApplication2.Models;
using Npgsql;
public static class DbInitializer
{
    
    public static void Initialize(ArticlesContext context)
    {
        
        var articles = new List <Article>();
       
        context.Articles.AddRange(articles);
        context.SaveChanges();

    }
}
