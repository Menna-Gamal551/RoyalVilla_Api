using Microsoft.AspNetCore.Authentication.Cookies;
using RoyalVillaDTO;
using RoyalVillaWeb.Services;
using RoyalVillaWeb.Services.IServices;

namespace RoyalVillaWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(op=>
            {
                op.IdleTimeout=TimeSpan.FromMinutes(90);
                op.Cookie.HttpOnly = true;
                op.Cookie.IsEssential = true;
            }
            );

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddAutoMapper(op =>
            {
                op.CreateMap<VillaCreateDTO, VillaDTO>().ReverseMap();

                op.CreateMap<VillaUpdateDTO, VillaDTO>().ReverseMap();
            }               
                );
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie( op =>
            {
                op.Cookie.HttpOnly=true;
                op.ExpireTimeSpan=TimeSpan.FromMinutes(90);
                op.SlidingExpiration=true;
                op.LoginPath="/auth/login";
                op.AccessDeniedPath="/auth/accessdenied";
               }
                );
            builder.Services.AddHttpClient("RoyalVillaApi",op=>
            {
                var villaUrl = builder.Configuration.GetValue<string>("serviceUrls:VillaUrl");
                op.BaseAddress=new Uri(villaUrl);
                op.DefaultRequestHeaders.Add("Accept", "application/json");

            });
            builder.Services.AddScoped<IVillaService,VillaService>();
            builder.Services.AddScoped<IAuthService,AuthService>();

                var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
