using FPTAlumniConnect.API.Services;
using FPTAlumniConnect.API.Services.Implements;
using FPTAlumniConnect.API.Services.Implements.FPTAlumniConnect.API.Services.Implements;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Repository.Implement;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StringUpdateApi.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

namespace FPTAlumniConnect.API.Extensions
{
    public static class DependencyServices
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork<AlumniConnectContext>, UnitOfWork<AlumniConnectContext>>();
            return services;
        }

        //public static IServiceCollection AddDatabase(this IServiceCollection services, WebApplicationBuilder builder)
        //{
        //    builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
        //         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        //         .Build();

        //    services.AddDbContext<AlumniConnectContext>(options =>
        //    {
        //        options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerDatabase"));
        //    });

        //    return services;
        //}

        public static IServiceCollection AddDatabase(this IServiceCollection services, WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("SQLServerDatabase");

            services.AddDbContext<AlumniConnectContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure();
                });
            });

            return services;
        }


        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            //var pathToKey = Path.Combine(Directory.GetCurrentDirectory(), "firebaseConfig.json");
            //FirebaseApp.Create(new AppOptions
            //{
            //    Credential = GoogleCredential.FromFile(pathToKey)
            //}, "[DEFAULT]");

            //services.AddScoped<IFirebaseService, FirebaseService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IPostReportService, PostReportService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IMentorshipService, MentorshipService>();
            services.AddScoped<ICVService, CVService>();
            services.AddScoped<ISkillService, SkillService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IMajorCodeService, MajorCodeService>();
            services.AddScoped<ISpMajorCodeService, SpMajorCodeService>();
            services.AddScoped<IJobPostService, JobPostService>();
            services.AddScoped<IJobApplicationService, JobApplicationService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<ITimeLineService, TimeLineService>();
            services.AddScoped<IUserJoinEventService, UserJoinEventService>();
            services.AddScoped<INotificationSettingService, NotificationSettingService>();
            services.AddScoped<IPrivacySettingService, PrivacySettingService>();
            services.AddScoped<ISocialLinkService, SocialLinkService>();
            services.AddScoped<IEducationHistoryService, EducationHistoryService>();
            services.AddScoped<IMessageGroupChatService, MessageGroupChatService>();
            services.AddScoped<IGroupChatService, GroupChatService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IEducationService, EducationService>();
            services.AddScoped<IWorkExperienceService, WorkExperienceService>();
            services.AddScoped<IPerspectiveService, PerspectiveService>();
            services.AddScoped<IPhoBertService, PhoBertService>();

            services.AddScoped<IRecruiterInfoService, RecruiterInfoService>();

            services.AddScoped<ICvSkillService, CvSkillService>();
            services.AddScoped<IJobPostSkillService, JobPostSkillService>();

            services.AddScoped<IEmploymentHistoryService, EmploymentHistoryService>();

            services.AddHostedService<JobPostCleanupService>();
            services.AddSingleton<JobPostCleanupService>();

            services.AddSingleton<MentorshipCleanupService>();
            services.AddHostedService<MentorshipCleanupService>();

            services.AddSingleton<ITimeService, TimeService>();

            services.AddSingleton<VersionService>();
            services.AddSingleton<StringCounterService>();
            return services;
        }

        public static IServiceCollection AddJwtValidation(this IServiceCollection services)
        {

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = "AlumniConnect",
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes("AlumniConnectSuperSecretKey123456"))
                    };
                });
            return services;
        }

        public static IServiceCollection AddConfigSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo() { Title = "FPT Alumni Connect API", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
                    });
                options.MapType<TimeOnly>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "time",
                    Example = OpenApiAnyFactory.CreateFromJson("\"13:45:42.0000000\"")
                });
            });
            return services;
        }
    }
}