using Microsoft.EntityFrameworkCore;
using mvc_app.Services;
using Microsoft.AspNetCore.Identity;
using mvc_app.DBContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace mvc_app
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            //Add service customer to DI container
            builder.Services.AddScoped<IServiceCustomers, ServiceCustomer>();
            builder.Services.AddDbContext<CustomerContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //Identity
            builder.Services.AddDbContext<UserContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                //Confirmed email is required to sign in
                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = false;
            })
              .AddEntityFrameworkStores<UserContext>()
              .AddDefaultTokenProviders();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder.WithOrigins("http://127.0.0.1:5500")
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });

            // JWT Configuration for API authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };

                // Event handlers for logging
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogError("Authentication failed: {Message}", context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogInformation("Token validated successfully.");
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Add cookie-based authentication for Identity
            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultScheme = IdentityConstants.ApplicationScheme; // Cookie-based auth for Identity
            //    options.DefaultSignInScheme = IdentityConstants.ExternalScheme; // For external login providers (optional)
            //}).AddCookie();

            

            //builder.Services.ConfigureApplicationCookie(options =>
            //{
            //    options.LoginPath = "/User/Login";
            //    options.AccessDeniedPath = "/User/AccessDenied";
            //});

            //builder.Services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("admin", policy => policy.RequireRole("admin"));
            //});

            

            //if (!app.Environment.IsDevelopment())
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    app.UseHsts();
            //}

            //app.UseHttpsRedirection();
            //app.UseStaticFiles();

            app.UseRouting();

            //Identity
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllers();
            app.Run();
        }
    }
}
