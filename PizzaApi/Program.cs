using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PizzaApi.Data;
using PizzaApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Entity Framework
builder.Services.AddDbContext<PizzaContext>(opt => opt.UseInMemoryDatabase("PizzaList"));

builder.Services.AddDbContext<UserContext>(opt => opt.UseInMemoryDatabase("UserList"));
builder.Services.AddDbContext<OrderContext>(opt => opt.UseInMemoryDatabase("OrderList"));

//bespoke services
builder.Services.AddScoped<OrderHandler>();
builder.Services.AddScoped<PizzaItemHandler>();


//auth
var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.SaveToken = true;
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = jwtIssuer,
         ValidAudience = jwtIssuer,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
     };
     options.Events = new JwtBearerEvents
     {
         OnMessageReceived = ctx =>
         {
             if (ctx.Request.Cookies.ContainsKey("AccessToken")) ctx.Token = ctx.Request.Cookies["AccessToken"];
             return Task.CompletedTask;
         }
     }; 
 });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Role", "Admin"));
});

//plumbing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
