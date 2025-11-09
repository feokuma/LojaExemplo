using LojaExemplo.Repositorios;
using LojaExemplo.Servicos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Register application services
builder.Services.AddSingleton<IRepositorioDeProdutos, RepositorioDeProdutos>();
builder.Services.AddSingleton<IRepositorioDePedidos, RepositorioDePedidos>();
builder.Services.AddScoped<IServicoDePedidos, ServicoDePedidos>();
builder.Services.AddSingleton<IServicoDePagamentos, ServicoDePagamentos>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Make the implicit Program class accessible for testing
public partial class Program { }
