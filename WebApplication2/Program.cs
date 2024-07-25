using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using WebApplication2.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ArticlesContext>
    (options => options.UseNpgsql(builder.Configuration.GetConnectionString("ArticlesContext")).UseLowerCaseNamingConvention());
    
   var app = builder.Build();
using (var scope=app.Services.CreateScope())
{
    var services= scope.ServiceProvider;
    var context= services.GetRequiredService<ArticlesContext>();
    context.Database.EnsureCreated();

    DbInitializer.Initialize(context);
    
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//читання статей
app.MapGet(pattern: "/articles", async(ArticlesContext context) => await context.Articles.ToListAsync());

//додавання статті
app.MapPost(pattern: "articles", async (Article article, ArticlesContext context) =>
{
    context.Articles.Add(article); ;
    await context.SaveChangesAsync();
    return Results.Created(uri: $"/articles/{article.Id}", article);
}
);

//пошук за назвою
app.MapGet(pattern: "/articles/title/{Title}", async (string Title, ArticlesContext context) =>
{
    var articles = await context.Articles.Where(a => a.Title == Title).ToListAsync();
    if (articles == null || !articles.Any())
    {
        return Results.NotFound();
    }

    return Results.Ok(articles);
});

//пошук за категорією
app.MapGet(pattern: "/articles/category/{Category}", async (string Category, ArticlesContext context) =>
{
    var articles = await context.Articles.Where(a => a.Category == Category).ToListAsync();
    if (articles == null || !articles.Any())
    {
        return Results.NotFound();
    }

    return Results.Ok(articles);
});

//пошук за описом
app.MapGet(pattern: "/articles/description/{Description}", async (string Description, ArticlesContext context) =>
{
    var articles = await context.Articles.Where(a => a.Description == Description).ToListAsync();
    if (articles == null || !articles.Any())
    {
        return Results.NotFound();
    }

    return Results.Ok(articles);
});

//пошук за ID
app.MapGet("/articles/{id:int}", async (int id, ArticlesContext context) =>
{
    var article = await context.Articles.FindAsync(id);
    if (article == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(article);
});

app.MapDelete(pattern: "/articles/{id:int}", async (int id, ArticlesContext context) =>
{
    var article = await context.Articles.FindAsync(id);
    if (article == null)
    {
        return Results.NotFound();
    }

    context.Articles.Remove(article);
    await context.SaveChangesAsync();

    return Results.NoContent();
});

// редагування статті
app.MapPut("/articles/{id:int}", async (int id, Article updatedArticle, ArticlesContext context) =>
{
    if (id != updatedArticle.Id)
    {
        return Results.BadRequest();
    }

    var article = await context.Articles.FindAsync(id);
    if (article == null)
    {
        return Results.NotFound();
    }

    // Оновлення полів статті
    article.Title = updatedArticle.Title;
    article.Author = updatedArticle.Author;
    article.Category = updatedArticle.Category;
    article.Description= updatedArticle.Description;
    article.Date = updatedArticle.Date;
    article.FileName = updatedArticle.FileName;
   
    try
    {
        await context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!context.Articles.Any(e => e.Id == id))
        {
            return Results.NotFound();
        }
        else
        {
            throw;
        }
    }

    return Results.NoContent();
});


//app.UseAuthorization();

app.MapControllers();

app.Run();
