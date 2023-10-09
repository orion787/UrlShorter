using Microsoft.EntityFrameworkCore;
using UrlShorter.DbAccess;
using UrlShorter.DTOs;
using UrlShorter.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<ApiDbContext>(options => options.UseSqlite(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/urlShoter", async (UrlDto url, HttpContext _ctx, ApiDbContext _db) =>
{
    if (!Uri.TryCreate(url.Url, UriKind.Absolute, out var inpitUri))
        return Results.BadRequest("Invalid Url");

    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";

    var random = new Random();

    var randomStr = new string(Enumerable.Repeat(chars, 10)
                              .Select(x => x[random.Next(chars.Length)])
                              .ToArray());

    var shortUrl = new UrlManager
    {
        Url = url.Url,
        ShortUrl = randomStr
    };

    _db.Urls.Add(shortUrl);
    await _db.SaveChangesAsync();

    string result = $"{_ctx.Request.Scheme}://{_ctx.Request.Host}/{shortUrl.ShortUrl}";

    return Results.Ok(new UrlResponseDto
    {
        Url = result
    });
});

app.MapFallback(async (ApiDbContext _db, HttpContext _ctx) =>
{
    var path = _ctx.Request.Path.ToUriComponent().Trim('/');

    var url = await _db.Urls.FirstOrDefaultAsync(x => x.ShortUrl.Trim() == path.Trim());

    if (url == null)
        return Results.BadRequest("invalid short url");

    return Results.Redirect(url.Url);
});

app.Run();