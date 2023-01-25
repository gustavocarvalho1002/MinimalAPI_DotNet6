using Microsoft.EntityFrameworkCore;
using MinimalAPI_DotNet6.Data;
using MinimalAPI_DotNet6.Model;
using MiniValidation;

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
        if (!MiniValidator.TryValidate(supplier, out var errors))
            return Results.ValidationProblem(errors);

        context.Suppliers.Add(supplier);
        var result = await context.SaveChangesAsync();

        return result > 0
            ? Results.Created($"/supplier/{supplier.Id}", supplier)
            : Results.BadRequest("An error ocurred while trying to save supplier");
    })
    .Produces<Supplier>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("PostSupplier")
    .WithTags("Supplier");

app.MapPut("/supplier/{id}", async (
    Guid id,
    MinimalContextDb context,
    Supplier supplier) =>
{
    var supplierFromDatabase = await context.Suppliers.AsNoTracking<Supplier>().FirstOrDefaultAsync(s => s.Id == id);

    if (supplierFromDatabase == null)
        return Results.NotFound($"No supplier with id '{supplier.Id}' was found");

    if (!MiniValidator.TryValidate(supplier, out var errors))
        return Results.ValidationProblem(errors);

    context.Suppliers.Update(supplier);
    var result = await context.SaveChangesAsync();

    return result > 0
        ? Results.NoContent()
        : Results.BadRequest("An error ocurred while trying to save supplier");
})
    .Produces<Supplier>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("PutSupplier")
    .WithTags("Supplier");


app.MapDelete("/supplier/{id}", async (
    Guid id,
    MinimalContextDb context) =>
    {
        var supplierFromDatabase = await context.Suppliers.FindAsync(id);
        if (supplierFromDatabase == null)
            return Results.NotFound();

        context.Suppliers.Remove(supplierFromDatabase);
        var result = await context.SaveChangesAsync();

        return result > 0
        ? Results.NoContent()
        : Results.BadRequest("An error ocurred while trying to delete supplier");
    }).Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("DeleteSupplier")
    .WithTags("Supplier");

app.Run();