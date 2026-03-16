using Microsoft.EntityFrameworkCore;
using TodoApi; 

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ToDoDbContext>();

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



// הפעלת Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll"); 

//Routes

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

    // עדכון רק אם נשלח ערך, או שמירה על הערך הקיים
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