using Microsoft.EntityFrameworkCore;
using TodoApi; 

var builder = WebApplication.CreateBuilder(args);

// 1. שליפת מחרוזת החיבור
var connectionString = builder.Configuration.GetConnectionString("ToDoDb");

// 2. הגדרת גרסה קבועה (במקום AutoDetect) כדי למנוע קריסה בעלייה
var serverVersion = new MySqlServerVersion(new Version(8, 0, 33)); 

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, serverVersion, 
        mysqlOptions => mysqlOptions.EnableRetryOnFailure()));

// הגדרת CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. יצירת הטבלאות באופן אוטומטי אם הן חסרות
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
    db.Database.EnsureCreated(); 
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAll"); 

app.MapGet("/", () => "ToDo API is running!");

app.MapGet("/items", async (ToDoDbContext db) => 
    await db.Items.ToListAsync());

app.MapPost("/items", async (ToDoDbContext db, Item newItem) => {
    db.Items.Add(newItem);
    await db.SaveChangesAsync();
    return Results.Created($"/items/{newItem.Id}", newItem);
});

app.MapPut("/items/{id}", async (ToDoDbContext db, int id, Item inputItem) => {
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    item.Name = inputItem.Name ?? item.Name; 
    item.IsComplete = inputItem.IsComplete;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/items/{id}", async (ToDoDbContext db, int id) => {
    if (await db.Items.FindAsync(id) is Item item) {
        db.Items.Remove(item);
        await db.SaveChangesAsync();
        return Results.Ok(item);
    }
    return Results.NotFound();
});

app.Run();