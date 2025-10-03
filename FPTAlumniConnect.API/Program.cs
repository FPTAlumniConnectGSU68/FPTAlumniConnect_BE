using FPTAlumniConnect.API.Extensions;
using FPTAlumniConnect.API.Middlewares;
using FPTAlumniConnect.API.Services;
using FPTAlumniConnect.API.Services.Implements;
using FPTAlumniConnect.BusinessTier.Configurations;
using FPTAlumniConnect.BusinessTier.Payload.JobPost;
using FPTAlumniConnect.BusinessTier.Payload.Mentorship;
using FPTAlumniConnect.BusinessTier.Payload.Schedule;
using Polly;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy(name: CorsConstant.PolicyName,
//        policy =>
//        {
//            policy.WithOrigins("http://localhost:3000",
//                    "https://fpt-allumni.vercel.app")
//                  .AllowAnyHeader()
//                  .AllowAnyMethod()
//                  .AllowCredentials();
//        });
//});
builder.Services.AddCors(x => x.AddPolicy("AllowAll", p =>
{
    p.SetIsOriginAllowed(_ => true)
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials();
}));

builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    x.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
});

builder.Services.AddHttpClient<PhoBertService>(client =>
{
    client.BaseAddress = new Uri("https://api-inference.huggingface.co/models/sentence-transformers/all-MiniLM-L6-v2");
    var apiToken = builder.Configuration["HuggingFace:ApiToken"]
        ?? Environment.GetEnvironmentVariable("HUGGINGFACE_API_TOKEN")
        ?? throw new ArgumentNullException("HuggingFace:ApiToken", "API token is not configured.");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
})
.AddStandardResilienceHandler();

builder.Services.AddDatabase(builder);
builder.Services.AddUnitOfWork();
builder.Services.AddServices();
builder.Services.AddJwtValidation();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddConfigSwagger();
builder.Services.AddSwaggerGen();

// Configure MentorshipCleanupOptions
builder.Services.Configure<MentorshipCleanupOptions>(builder.Configuration.GetSection("MentorshipCleanup"));
builder.Services.Configure<JobPostCleanupOptions>(builder.Configuration.GetSection("JobPostCleanup"));
builder.Services.Configure<MentorshipSettings>(builder.Configuration.GetSection("MentorshipSettings"));
builder.Services.Configure<ScheduleSettings>(builder.Configuration.GetSection("ScheduleSettings"));
builder.Services.Configure<MentorshipCleanupSettings>(builder.Configuration.GetSection("MentorshipCleanup"));
builder.Services.Configure<JobPostCleanupSettings>(builder.Configuration.GetSection("JobPostCleanup"));

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {

    });

var app = builder.Build();

app.UseSwagger();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FPT Alumni Connect API V1");
    });
}
else
{
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FPT Alumni Connect API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Ensure CORS, Authentication, and Authorization middlewares are in correct order
//app.UseCors(CorsConstant.PolicyName);
app.UseCors("AllowAll");
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<NotificationHub>("/notificationHub");
});

app.Run();

