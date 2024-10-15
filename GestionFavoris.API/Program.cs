var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//Ajout du service de mise en cache des réponses HTTP
builder.Services.AddResponseCaching();
// Ajout du service de mise en cache dans le conteneur d'IoC
builder.Services.AddMemoryCache(options=>
{
    options.SizeLimit = 5000000;
}); 

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//Ajout du middleware de mise en cache des réponses HTTP
app.UseResponseCaching();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
