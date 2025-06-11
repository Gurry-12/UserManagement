using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using UserManagement.Application.Services.AbstractServices;
using UserManagement.Application.Services.ConcreteServices;
using UserManagement.Infrastructure.DBContext;
using UserManagement.Infrastructure.Repository.AbstractRepository;
using UserManagement.Infrastructure.Repository.ConcreteRepository;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });



// Swagger/OpenAPI setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});


// Add your EF Core database context
builder.Services.AddDbContext<UserManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

#region Application Interfaces
builder.Services.AddScoped<IUserService, UserService>();
#endregion

#region Repository Interfaces
builder.Services.AddScoped<IUserRepository, UserRepository>();
#endregion


var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // 🔧 This shows detailed errors on 500
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
