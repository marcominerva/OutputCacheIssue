using SimpleAuthentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSimpleAuthentication(builder.Configuration);

builder.Services.AddOutputCache();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSimpleAuthentication(builder.Configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseOutputCache();

app.MapGet("/api/not-using-cacheoutput", () => DateTime.UtcNow)
.WithOpenApi(operation => new(operation)
{
    Description = "This endpoint does not call CacheOuput(), but according to the documentation, it should be cached because Output caching has been registerd with 'builder.Services.AddOutputCache()'"
});

app.MapGet("/api/withcache", () => DateTime.UtcNow)
.RequireAuthorization().CacheOutput()
.WithOpenApi(operation => new(operation)
{
    Description = "Use the value 'f1I7S5GXa4wQDgLQWgz0' (without quotes) for Authorization. This endpoint calls CacheOutput(). According to the documentation, it should be cached because Output caching has been registerd with 'builder.Services.AddOutputCache()', that removes all policy defaults (including the one regarding authenticated requests)"
});

app.Run();
