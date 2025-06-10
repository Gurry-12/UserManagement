using Microsoft.EntityFrameworkCore;
using UserManagement.Application.Services.AbstractServices;
using UserManagement.Application.Services.ConcreteServices;
using UserManagement.Infrastructure.DBContext;
using UserManagement.Infrastructure.Repository.AbstractRepository;
using UserManagement.Infrastructure.Repository.ConcreteRepository;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Swagger/OpenAPI setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
