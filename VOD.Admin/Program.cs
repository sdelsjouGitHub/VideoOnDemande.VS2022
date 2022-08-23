using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VOD.Common.DTOModels.Admin;
using VOD.Common.Entities;
using VOD.Common.Services;
using VOD.Database.Contexts;
using VOD.Database.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<VODContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<VODUser>()
    .AddRoles<IdentityRole>()
    .AddDefaultUI()
    .AddEntityFrameworkStores<VODContext>();


builder.Services.AddHttpClient("AdminClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:6600");
    client.Timeout = new TimeSpan(0, 0, 30);
    client.DefaultRequestHeaders.Clear();
}).ConfigurePrimaryHttpMessageHandler(handler =>
    new HttpClientHandler()
    {
        AutomaticDecompression = System.Net.DecompressionMethods.GZip
    });

ConfigureAutoMapper(builder.Services);
RegisterServices(builder.Services);

builder.Services.AddRazorPages();

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
    //services.AddScoped<IAdminService, AdminEFService>();
    services.AddScoped<IAdminService, AdminAPIService>();
    services.AddScoped<IHttpClientFactoryService, HttpClientFactoryService>();
    services.AddScoped<IJwtTokenService, JwtTokenService>();

}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
