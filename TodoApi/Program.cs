using Microsoft.EntityFrameworkCore;
using TodoApi; 

var builder = WebApplication.CreateBuilder(args);

// --- עדכון הגדרת ה-Database ---
// שליפת מחרוזת החיבור ממשתני הסביבה של Render
var connectionString = builder.Configuration.GetConnectionString("ToDoDb");

// הגדרת גרסה קבועה כדי למנוע שגיאות חיבור בעלייה (AutoDetect)
var serverVersion = new MySqlServerVersion(new Version(8, 0, 33)); 

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, serverVersion, 
        mysqlOptions => mysqlOptions.EnableRetryOnFailure()));
// ------------------------------

// הגדרת CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// הגדרת Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- קוד ליצירת טבלאות אוטומטית ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
    db.Database.EnsureCreated(); 
}
// ---------------------------------

// הפעלת Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll"); 

// Routes
app.MapGet("/", () => "ToDo API is running!");

// שליפת כל המשימות
app.MapGet("/items", async (ToDoDbContext db) => 
    await db.Items.ToListAsync());

// הוספת משימה חדשה
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

// מחיקת משימה
app.MapDelete("/items/{id}", async (ToDoDbContext db, int id) => {
    if (await db.Items.FindAsync(id) is Item item) {
        db.Items.Remove(item);
        await db.SaveChangesAsync();
        return Results.Ok(item);
    }
    return Results.NotFound();
});

app.Run();