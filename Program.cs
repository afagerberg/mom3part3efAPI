//DT191G Moment 3 del 3 API, av Alice Fagerberg

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TuneDb>(options =>
    options.UseSqlite("Data Source=MusiclistDb.db")
);

//Kommenterat bort anv av swagger - Gillar Thunder Client mer
/*
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
*/
var app = builder.Build();

/*if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/

// Route och metoder
app.MapGet("/", () => "Välkommen till låtlista API -\nBörja att söka dig vidare med /tunes");

//Get - hämta alla låtar(tunes)
app.MapGet("/tunes", async(TuneDb db) =>
    await db.Tunes.ToListAsync());

// Get by id - Hämta specifik låt
app.MapGet("tunes/{id}", async (int id, TuneDb db) =>

    await db.Tunes.FindAsync(id)
    is Tune tune
    ? Results.Ok(tune)
    : Results.NotFound()
);

// post to database - Lägg till låt
app.MapPost("/tunes", async(Tune tune, TuneDb db) => {

    db.Tunes.Add(tune);
    await db.SaveChangesAsync();

    return Results.Created("Tune created:", tune);
});

//Put - Uppdatera en låt
app.MapPut("/tunes/{id}", async(int id, Tune inputTune, TuneDb db) =>
{
    var tune = await db.Tunes.FindAsync(id);
    if(tune is null) return Results.NotFound();

    //updat input
    tune.Artist = inputTune.Artist;
    tune.Title = inputTune.Title;
    tune.Length = inputTune.Length;
    tune.Category = inputTune.Category;

    await db.SaveChangesAsync();

    return Results.Ok(tune);

}

);

// delete - Radera låt
app.MapDelete("/tunes/{id}", async(int id, TuneDb db) =>
 {
     if(await db.Tunes.FindAsync(id) is Tune tune)
     {
         db.Tunes.Remove(tune);
         await db.SaveChangesAsync();
         return Results.Ok(tune);
     }

     return Results.NotFound();
 });




app.Run();


//Klass för en låt till låtlista
class Tune {

    //Properties
    public int Id { get; set; }

    public string? Artist { get; set; }

    public string? Title{ get; set; }

    public int? Length { get; set; }

    public string? Category { get; set; }
}

// DB Context
class TuneDb : DbContext
{
    public TuneDb(DbContextOptions<TuneDb> options)
        : base(options) { }
    
    public DbSet<Tune> Tunes => Set<Tune>();
}

