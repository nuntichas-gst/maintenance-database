using dashboardtask.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;

    });


builder.Services.AddHttpContextAccessor();

//session ของ program
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
});

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

//#region  Account Log In
//builder.Services.AddScoped<IAccountUser, AccountUsersRepo>();
//builder.Services.AddScoped<IAccountUserLog, AccountUserLogRepo>();
//builder.Services.AddScoped<IADUsers, ADUsersRepo>();

//#endregion

//// ตั้งค่าการเชื่อมต่อกับฐานข้อมูล
//#region Database Connections
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection"))
);


//builder.Services.AddDbContext<AppDbGSecure>(
//    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnectionSecure"))
//    );

////Connect AccountRoles 
//builder.Services.AddDbContext<AppDbMaRoles>(
//    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnectionAccountRoles"))
//    );


//builder.Services.AddDbContext<AppDbHRIS>(
//    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnectionGHRIS"))
//    );



//#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();

