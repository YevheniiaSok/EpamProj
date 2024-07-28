using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using WebApplication2.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<ArticlesContext>
    (options => options.UseNpgsql(builder.Configuration.GetConnectionString("ArticlesContext")).UseLowerCaseNamingConvention());

// Додайте Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ArticlesContext>()
    .AddDefaultTokenProviders();

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


app.UseAuthentication();
app.UseAuthorization();

//app.MapRazorPages();

//// Реєстрація
//app.MapPost("/register", async (RegisterViewModel model, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) =>
//{
//    if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.Password) && model.Password == model.ConfirmPassword)
//    {
//        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
//        var result = await userManager.CreateAsync(user, model.Password);
//        if (result.Succeeded)
//        {
//            await signInManager.SignInAsync(user, isPersistent: false);
//            return Results.Ok();
//        }
//        return Results.BadRequest(result.Errors);
//    }
//    return Results.BadRequest("Invalid registration details");
//});

// Вхід
app.MapPost("/login", async (LoginViewModel model, SignInManager<IdentityUser> signInManager) =>
{
    if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.Password))
    {
        var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            return Results.Ok();
        }
        return Results.BadRequest("Invalid login attempt.");
    }
    return Results.BadRequest("Invalid login details");
});

// Вихід
app.MapPost("/logout", async (SignInManager<IdentityUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Ok();
});

//читання статей
app.MapGet(pattern: "/articles", async(ArticlesContext context) => await context.Articles.ToListAsync());

//додавання статті
app.MapPost(pattern: "articles", async (Article article, ArticlesContext context) =>
{
    context.Articles.Add(article); ;
    await context.SaveChangesAsync();
    return Results.Created(uri: $"/articles/{article.Id}", article);
}
).RequireAuthorization();

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

//пошук за Id
app.MapGet(pattern: "/articles/id/{Id}", async (int Id, ArticlesContext context) =>
{
    var articles = await context.Articles.Where(a => a.Id == Id).ToListAsync();
    if (articles == null || !articles.Any())
    {
        return Results.NotFound();
    }

    return Results.Ok(articles);
});

// Обробка запитів пошуку за частковим співпадінням
app.MapGet("/articles/search", async (string query, ArticlesContext context) =>
{
    if (string.IsNullOrWhiteSpace(query))
    {
        return Results.BadRequest("Query parameter is missing.");
    }

    var articles = await context.Articles
        .Where(a => EF.Functions.ILike(a.Title, $"%{query}%")
                    || EF.Functions.ILike(a.Description, $"%{query}%")
                    || EF.Functions.ILike(a.Category, $"%{query}%"))
        .ToListAsync();

    if (articles == null || !articles.Any())
    {
        return Results.NotFound();
    }

    return Results.Ok(articles);
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
}).RequireAuthorization();

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
}).RequireAuthorization(); 


app.MapControllers();

app.Run();
