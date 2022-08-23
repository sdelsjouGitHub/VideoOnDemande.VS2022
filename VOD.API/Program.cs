using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VOD.API.Services;
using VOD.Common.DTOModels.Admin;
using VOD.Common.Entities;
using VOD.Common.Services;
using VOD.Database.Contexts;
using VOD.Database.Services;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<VODContext>(options =>
    options.UseSqlServer(connectionString));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddIdentityCore<VODUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<VODContext>();

#region JWT Token Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var signingKey = new SymmetricSecurityKey(Convert.FromBase64String(configuration["Jwt:SigningSecret"]));

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        ClockSkew = TimeSpan.Zero
    };
    options.RequireHttpsMetadata = false;
});
#endregion

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VODUser", policy => policy.RequireClaim("VODUser", "true"));
    options.AddPolicy("Admin", policy => policy.RequireClaim("Admin", "true"));
});




ConfigureAutoMapper(builder.Services);
RegisterServices(builder.Services);

var app = builder.Build();

void ConfigureAutoMapper(IServiceCollection services)
{

    var config = new AutoMapper.MapperConfiguration(cfg =>
    {
        cfg.CreateMap<Video, VideoDTO>();

        cfg.CreateMap<Course, CourseDTO>();
        cfg.CreateMap<Instructor, InstructorDTO>();
        cfg.CreateMap<Module, ModuleDTO>();
        cfg.CreateMap<Download, DownloadDTO>();

    });
    var mapper = config.CreateMapper();
    services.AddSingleton(mapper);
}



void RegisterServices(IServiceCollection services)
{
    services.AddScoped<IDbReadService, DbReadService>();
    services.AddScoped<IDbWriteService, DbWriteService>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IAdminService, AdminEFService>();
    services.AddTransient<ITokenService, TokenService>();

}

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
