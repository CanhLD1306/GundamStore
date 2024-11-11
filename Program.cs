using GundamStore.Data;
using GundamStore.Interfaces;
using GundamStore.Models;
using GundamStore.Services;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GundamStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>options.UseSqlServer(connectionString));

            
            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

            builder.Services.AddTransient<IEmailSenderService, EmailSenderService>();

            builder.Services.Configure<FirebaseSettings>(builder.Configuration.GetSection("FirebaseSettings"));

            builder.Services.AddTransient<IFirebaseStorageService, FirebaseStorageService>();

            builder.Services.AddScoped<ICategoryService, CategoryService>();

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();            

            builder.Services.AddRouting();
            builder.Services.AddRazorPages();
            builder.Services.AddAuthentication()
                .AddGoogle(GoogleOptions =>
                {
                    IConfigurationSection googleAuth = builder.Configuration.GetSection("Authentication:Google");

                    GoogleOptions.ClientId = googleAuth["ClientId"];
                    GoogleOptions.ClientSecret = googleAuth["ClientSecret"];
                    GoogleOptions.CallbackPath = "/Google-SignIn";
                    GoogleOptions.Events = new OAuthEvents
                    {
                        OnRemoteFailure = context =>
                        {
                            context.Response.Redirect("/Account/Login");
                            context.HandleResponse();
                            return Task.CompletedTask;
                        }
                    };
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.Use(async (context, next) =>
            {
                context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                context.Response.Headers["Pragma"] = "no-cache";
                context.Response.Headers["Expires"] = "-1";

                await next();
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Home}/{action=Index}/{id?}"
                );
            });
            app.MapRazorPages();

            // using (var scope = app.Services.CreateScope())
            // {
            //     var services = scope.ServiceProvider;

            //     try
            //     {
            //         SeedData.Initialize(services).Wait();
            //     }
            //     catch (Exception ex)
            //     {
            //         var logger = services.GetRequiredService<ILogger<Program>>();
            //         logger.LogError(ex, "An error occurred seeding the DB.");
            //     }
            // }
            app.Run();
        }
    }
}