using RoadsideAssistance.Models.Interfaces;
using RoadsideAssistance.Models;
using RoadsideAssistance.Services.Interfaces;
using RoadsideAssistance.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddTransient<IGeolocation, Geolocation>();
builder.Services.AddScoped<IGeolocation>(_ => new Geolocation(0,0));
builder.Services.AddTransient<IAssistant>(provider =>
{
    string name = ""; // Set the desired name value
    IGeolocation location = provider.GetRequiredService<IGeolocation>(); // Resolve the IGeolocation dependency

    return new Assistant(name, location);
});
//builder.Services.AddTransient<IAssistant, Assistant>();
builder.Services.AddTransient<ICustomer>(provider =>
{
    string name = ""; // Set the desired name value
    IGeolocation location = provider.GetRequiredService<IGeolocation>(); // Resolve the IGeolocation dependency

    return new Customer(name, location);
});
//builder.Services.AddTransient<ICustomer, Customer>();
// Retrieve the allAssistants list from your desired source
List<Assistant> allAssistants = new List<Assistant>();

builder.Services.AddTransient<IRoadsideAssistanceService>(provider =>
{
    IAssistant assistant = provider.GetRequiredService<IAssistant>(); 
    IGeolocation geolocation = provider.GetRequiredService<IGeolocation>(); 
    ICustomer customer = provider.GetRequiredService<ICustomer>(); 
   // List<Assistant> allAssistants = provider.GetRequiredService<List<Assistant>>(); // Resolve the List<Assistant> dependency

    return new RoadsideAssistanceService(assistant, geolocation, customer);
});
builder.Services.AddTransient<IRoadsideAssistanceService, RoadsideAssistanceService>();    

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
