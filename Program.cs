
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RoyalVilla_Api.Data;
using RoyalVilla_Api.Data.Models;
using RoyalVillaDTO;
using RoyalVilla_Api.Recervice;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Scalar.AspNetCore;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Identity;


namespace RoyalVilla_Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("jwtsettings")["secret"]);

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
            builder.Services.AddAuthentication(op =>
            {
                op.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
                op.DefaultForbidScheme=JwtBearerDefaults.AuthenticationScheme;
                op.DefaultScheme=JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(op =>
            {
                op.RequireHttpsMetadata=false;
                op.SaveToken=true;
                op.TokenValidationParameters=new TokenValidationParameters
                {
                    ValidateIssuerSigningKey=true,
                    IssuerSigningKey=new SymmetricSecurityKey(key),
                    ValidateIssuer=false,
                    ValidateAudience=false,
                    ValidateLifetime=true,
                    ClockSkew=TimeSpan.Zero
                };
            }
            );

            builder.Services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            

            builder.Services.AddCors();
            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>
                (option => option.UseSqlServer(builder.Configuration.GetConnectionString("MyCon")));
            builder.Services.AddControllers();
           
            builder.Services.AddAutoMapper(op =>
            { 
                op.CreateMap<VillaCreateDTO, Villa>().ReverseMap();

                op.CreateMap<VillaUpdateDTO, Villa>().ReverseMap();
            
                op.CreateMap<VillaDTO, Villa>().ReverseMap();
           
                op.CreateMap<VillaDTO, VillaUpdateDTO>().ReverseMap();
    
                op.CreateMap<ApplicationUser, UserDTO>().ReverseMap();
                op.CreateMap<VillaAmenities, VillaAmentyCreateDTO>().ReverseMap();
                op.CreateMap<VillaAmenities, VillaAmentyUpdateDTO>().ReverseMap();
                op.CreateMap<VillaAmentyDTO, VillaAmenities>();
                op.CreateMap<VillaAmenities, VillaAmentyDTO>().ForMember(
                    dest => dest.VillaName,
                    op => op.MapFrom(src => src.villa!=null ? src.villa.Name : null)
                );

            
            });
           

            builder.Services.AddScoped<IAuthService, AuthService>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddTransient<
    IConfigureOptions<SwaggerGenOptions>,
    ConfigureSwaggerOptions>();

            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            var versionProvider = app.Services
    .GetRequiredService<IApiVersionDescriptionProvider>();
            foreach (var description in versionProvider.ApiVersionDescriptions)
            {
                Console.WriteLine(description.GroupName);
            }

            // Configure the HTTP request pipeline.


            if (app.Environment.IsDevelopment())
            {
                app.MapSwagger("/openapi/{documentName}.json");

                app.MapScalarApiReference(options =>
                {
                    options.WithTitle("RoyalVilla API");

                    foreach (var description in
                             versionProvider.ApiVersionDescriptions)
                    {
                        var versionName = description.GroupName;
                        var versionNumber =
                            description.ApiVersion.ToString();

                        options.AddDocument(
                            versionName,
                            $"RoyalVilla API - {versionNumber}",
                            $"/openapi/{versionName}.json",
                            isDefault: versionName == "v1");
                    }
                });
            }
            app.UseCors(op=>op.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("*"));
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
