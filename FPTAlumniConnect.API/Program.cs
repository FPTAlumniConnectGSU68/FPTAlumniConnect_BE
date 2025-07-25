using FPTAlumniConnect.API.Extensions;
using FPTAlumniConnect.API.Middlewares;
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

