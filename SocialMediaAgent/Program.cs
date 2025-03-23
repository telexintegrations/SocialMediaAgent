using SocialMediaAgent.Repositories.Implementation;
using SocialMediaAgent.Repositories.Interfaces;
using SocialMediaAgent.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<PostSchedulingService>();
builder.Services.AddScoped<ITelexService, TelexRepository>();
builder.Services.AddScoped<IGroqService, GroqService>();
builder.Services.AddHttpClient<IGroqService, GroqService>();

builder.Services.AddCors(
    opts => opts.AddPolicy("AllowAll", builder => {
        builder.AllowAnyOrigin();
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
    }
));


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
