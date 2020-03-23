using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationAPI.DAL;
using AuthorizationAPI.Models;
using AuthorizationAPI.Repositories;
using AuthorizationAPI.Services;
using AuthorizationAPI.Utilities;
using Cache.Infrastructure;
using Cache.Redis.Service;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace AuthorizationAPI
{
    
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            Configuration = configuration;
            var credentialsPath = "C:\\Google\\firebase-account-key.json";// configuration.GetValue<string>("Firebase:KeyLocation");

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.GetApplicationDefault()
            });
            
            InitialDataSetup();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddScoped<EducationPortalModel.EducationPortalModel, EducationPortalModel.EducationPortalModel>();
            
            
            services.AddScoped<FirebaseUtils, FirebaseUtils>();
            services.AddScoped<InstitutionService, InstitutionService>();

            services.AddScoped<PackageConfigurationService, PackageConfigurationService>();
            services.AddScoped<InstitutionRepository, InstitutionRepository>();

            services.AddScoped<ICache, RedisCache>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
               .AddCookie(options =>
               {
                   options.Cookie.HttpOnly = true;
                   options.Cookie.SecurePolicy = _env.IsDevelopment()
                       ? CookieSecurePolicy.SameAsRequest
                       : CookieSecurePolicy.Always;
                   options.Cookie.SameSite = _env.IsDevelopment() ? SameSiteMode.None : SameSiteMode.Strict;
                   options.SlidingExpiration = true;
                   options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                   options.Cookie.Name = "token";
                   options.LoginPath = ""; // let the client handle this
                   options.Events.OnRedirectToLogin = context =>
                  {
                      context.Response.StatusCode = 401;
                      return Task.CompletedTask;
                  };
                   options.Events.OnRedirectToAccessDenied = context =>
                   {
                       context.Response.StatusCode = 403;
                       return Task.CompletedTask;
                   };
                   options.Events.OnRedirectToLogout = context =>
                   {
                       context.Response.StatusCode = 401;
                       return Task.CompletedTask;
                   };
               });

            services.AddSwaggerGen((options) =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "StudentEducationPortal API",
                    Version = "v1",
                    Description = "This Service to perform Student Education Portal related work."
                });

                //options.OperationFilter<Filters.CustomFilters.AuthHeaderFilter>();
                options.AddSecurityDefinition("Authorization", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Description = "Please enter the API Key provided to you"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Authorization" }
                        }, new List<string>() }
                });

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDeveloperExceptionPage();
                app.UseCors(builder => builder.WithOrigins("*"));
                //app.UseSwaggerDocumentation();
                app.UseSwagger();
                //specify the Swagger JSON endpoint
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Authorization API");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


        // This is Temp code for users
        private void InitialDataSetup()
        {
            InMemoryRepository repository = new InMemoryRepository();

            var superUserRole = new ApplicationRole
            {
                Id = 1,
                Role = "Super User",
                Permissions = { AppPermissions.VIEW_STUDENT_PROFILES, AppPermissions.VIEW_OWN_ADMIN_PROFILE, AppPermissions.MANAGE_STUDENT_PROFILE, AppPermissions.VIEW_ADMINISTRATOR_PROFILES, AppPermissions.MANAGE_STUDENT_PROFILE, AppPermissions.MANAGE_PERMISSIONS, AppPermissions.MANAGE_ROLES, AppPermissions.MANAGE_ADMINISTRATOR_PROFILE }
            };
            var adminManagerRole = new ApplicationRole
            {
                Id = 2,
                Role = "Administrators Manager",
                Permissions = { AppPermissions.VIEW_ADMINISTRATOR_PROFILES, AppPermissions.MANAGE_STUDENT_PROFILE }
            };
            var adminManagerRole2 = new ApplicationRole
            {
                Id = 5,
                Role = "Administrators2",
                Permissions = { AppPermissions.VIEW_ADMINISTRATOR_PROFILES, AppPermissions.MANAGE_STUDENT_PROFILE, AppPermissions.VIEW_OWN_ADMIN_PROFILE, AppPermissions.MANAGE_STUDENT_PROFILE }
            };
            var studentsAdministratorRole = new ApplicationRole
            {
                Id = 3,
                Role = "District",
                //Role = "Students Manager",
                Permissions = { AppPermissions.VIEW_OWN_ADMIN_PROFILE, AppPermissions.MANAGE_STUDENT_PROFILE }
            };
            var teachingAssistantRole = new ApplicationRole
            {
                Id = 4,

                Role = "School",
                //Role = "Teaching Assistant",
                Permissions = { AppPermissions.VIEW_STUDENT_PROFILES, AppPermissions.MANAGE_STUDENT_PROFILE }
            };
            var studentRole = new ApplicationRole
            {
                Id = 5,
                Role = "Class Room",
                //Role = "Student",
                Permissions = { AppPermissions.VIEW_OWN_STUDENT_PROFILE }
            };



            repository.Add(superUserRole);
            repository.Add(adminManagerRole);
            repository.Add(studentsAdministratorRole);
            repository.Add(teachingAssistantRole);
            repository.Add(studentRole);

            var superuser = new ApplicationUser
            {
                Id = 1,
                Username = "superuser"
            };

            var superadmin = new ApplicationUser
            {
                Id = 2,
                Username = "superadmin"

            };

            var superadmin2 = new ApplicationUser
            {
                Id = 5,
                Username = "superadmin2"

            };


            var admin = new ApplicationUser
            {
                Id = 4,
                Username = "admin"

            };

            var teacher = new ApplicationUser
            {
                Id = 5,
                Guid = "teacher",
                Username = "student1",   //This student is TA who can view other student's profiles

            };

            var student2 = new ApplicationUser
            {
                Id = 6,
                Email= "ashok-admin@scantron.edu.portal.com",
                Password= "test1234",
                Username = "jared"//,
                //Roles = { studentRole }
                

            };

            var student3 = new ApplicationUser
            {
                Id = 7,
                Username = "ashok"

            };

            repository.Add(superuser);
            repository.Add(superadmin);
            repository.Add(superadmin2);
            repository.Add(admin);
            repository.Add(teacher);
            repository.Add(student2);
            repository.Add(student3);

            repository.Add(new Administrator { Id = 1, Name = "Bob Smith", User = superuser });
            repository.Add(new Administrator { Id = 2, Name = "Paul Smith", User = superadmin });
            repository.Add(new Administrator { Id = 3, Name = "Paul J. Smith", User = superadmin2 });
            repository.Add(new Administrator { Id = 4, Name = "Michael Sindhu", User = admin });
            //repository.Add(new Student { Id = 5, Guid = "AAAA", Name = "Jeff Studants", User = student1 });
            repository.Add(new Student { Id = 6, Guid = "AAAA", Name = "Jared", User = student2 });
            repository.Add(new Student { Id = 7, Guid = "BBBB", Name = "Ashok", User = student3 });

            // institutions
            var system = new Institution()
            {
                Guid = "0000000",
                ObjectType = "System",
                Level = 0
            };
            repository.Add(system);
            var scantron = new Institution()
            {
                Guid = "1111111111",
                ObjectType = "Scantron",
                ParentGuid = "0000000",
                Level = 1
            };
            repository.Add(scantron);
            var client1 = new Institution()
            {
                Guid = "ABC1",
                ObjectType = "District",
                ParentGuid = "0000000",
                Level = 1
            };
            repository.Add(client1);
            var School1 = new Institution()
            {
                Guid = "BCD2",
                ObjectType = "School",
                ParentGuid = "ABC1",
                Level = 2
            };
            repository.Add(School1);
            var Class1 = new Institution()
            {
                Guid = "FED4",
                ObjectType = "Classroom",
                ParentGuid = "BCD2",
                Level = 3
            };
            repository.Add(Class1);
            var Class2 = new Institution()
            {
                Guid = "JDH5",
                ObjectType = "SchClassroomool",
                ParentGuid = "BCD2",
                Level = 3
            };
            repository.Add(Class2);
            var School2 = new Institution()
            {
                Guid = "CDE3",
                ObjectType = "School",
                ParentGuid = "ABC1",
                Level = 2
            };
            repository.Add(School2);
            var studentSchoolAssociation = new StudentAssociation()
            {
                InstitutionGuid = "BCD2",
                StudentGuid = "AAAA"
            };
            repository.Add(studentSchoolAssociation);
            var studentClassroomAssociation = new StudentAssociation()
            {
                InstitutionGuid = "FED4",
                StudentGuid = "BBBB"
            };
            repository.Add(studentClassroomAssociation);
            var teacherSchoolPermissions = new UserPermissions()
            {
                InstitutionGuid = "BCD2",
                Roles = new List<ApplicationRole>(),
                Permissions = new List<string>(),
                UserGuid = "teacher"
            };
            teacherSchoolPermissions.Permissions.Add(AppPermissions.VIEW_STUDENT_PROFILES);
            teacherSchoolPermissions.Permissions.Add(AppPermissions.MANAGE_STUDENT_PROFILE);

            repository.Add(teacherSchoolPermissions);
        }
    }
}
