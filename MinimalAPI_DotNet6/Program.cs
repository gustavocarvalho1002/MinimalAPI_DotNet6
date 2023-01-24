using Microsoft.EntityFrameworkCore;
using MinimalAPI_DotNet6.Data;
using MinimalAPI_DotNet6.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MinimalContextDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/supplier", async
    (MinimalContextDb context) =>
    await context.Suppliers.ToListAsync())
    .WithName("GetSuppliers")
    .WithTags("Supplier");

app.MapGet("/supplier/{id}", async (
    Guid id,
    MinimalContextDb context) =>

    await context.Suppliers.FindAsync(id)
        is Supplier supplier
            ? Results.Ok(supplier)
            : Results.NotFound())
    .Produces<Supplier>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("GetSuppliersById")
    .WithTags("Supplier");

app.MapPost("/supplier", async (
    MinimalContextDb context,
    Supplier supplier) =>
    {
        context.Suppliers.Add(supplier);
        var result = await context.SaveChangesAsync();
    })
    .Produces<Supplier>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("PostSupplier")
    .WithTags("Supplier"); ;

app.Run();