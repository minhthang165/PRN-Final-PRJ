using CloudinaryDotNet;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using PRN_Final_Project.Business.Data;
using PRN_Final_Project.Hubs;
using PRN_Final_Project.Repositories;
using PRN_Final_Project.Repositories.Interface;
using PRN_Final_Project.Service;
using PRN_Final_Project.Service.Interface;
using StackExchange.Redis;
using System.Text.Json.Serialization;
using VNPAY.NET;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer(); // Bắt buộc cho Swagger
builder.Services.AddSwaggerGen();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.MaxDepth = 999;
    });

builder.Services.AddDbContext<PRNDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                     new MySqlServerVersion(new Version(8, 0, 2))));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});


builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var host = Environment.GetEnvironmentVariable("REDIS_HOST");
    var password = Environment.GetEnvironmentVariable("REDIS_PASSWORD");

    var configuration = $"{host},password={password}";

    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddHttpContextAccessor();

// .env config
DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
builder.Configuration["Gemini:ApiKey"] = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
Cloudinary cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
cloudinary.Api.Secure = true;
builder.Services.AddSingleton(cloudinary);

// Inject service and repository
builder.Services.AddScoped<EmailService>();

builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<IClassRepository, ClassRepository>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IFileRepository, FileRepository>();

builder.Services.AddScoped<IRecruitmentService, RecruitmentService>();
builder.Services.AddScoped<IRecruitmentRepository, RecruitmentRepository>();

builder.Services.AddScoped<ICVInfoService, CVInfoService>();
builder.Services.AddScoped<ICVInfoRepository, CVInfoRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IAIExtractor, AIExtractorService>();

builder.Services.AddSingleton<IVnpay, Vnpay>();
builder.Services.AddScoped<VnpayPayment>();

builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();

builder.Services.AddScoped<IConversationUserService, ConversationUserService>();
builder.Services.AddScoped<IConversationUserRepository, ConversationUserRepository>();

builder.Services.AddSignalR().AddHubOptions<ChatHub>(options =>
{
    options.EnableDetailedErrors = true;
});

var vnpay = new Vnpay();
vnpay.Initialize(
    builder.Configuration["Vnpay:TmnCode"],
    builder.Configuration["Vnpay:HashSecret"],
    builder.Configuration["Vnpay:BaseUrl"],
    builder.Configuration["Vnpay:ReturnUrl"]
);

builder.Services.AddSingleton<IVnpay>(vnpay);

builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();

builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();

builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();

// Add services to the container.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Index";
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.Cookie.Name = "PRN-Final.AuthCookie";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
    })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = Environment.GetEnvironmentVariable("CLIENT_ID");
        options.ClientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");
        options.CallbackPath = "/login/oauth2/code/google";
        options.SaveTokens = true;
        options.CorrelationCookie.SameSite = SameSiteMode.Lax;
    });
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthorization();

builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Enable Swagger UI for API documentation
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseSession();

// Add SignalR services
app.MapHub<ChatHub>("/chatHub");

app.UseAuthentication();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
