using Corinthians.Data;
using Corinthians.Models;

class HelloWeb
{
    static void Main(string[] args)
    {
        //Criação da aplicação e configurações do banco de dados

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApiDocument(config =>
        {
            config.DocumentName = "Corinthians";
            config.Title = "Corinthians, o mais pica";
            config.Version = "v1";
        });
        
        builder.Services.AddDbContext<CorinthiansDb>();
        WebApplication app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi(config =>
            {
                config.DocumentTitle = "Corinthians API";
                config.Path = "/swagger";
                config.DocumentPath = "/swagger/{documentName}/swagger.json";
                config.DocExpansion = "list";
            });
        }

        //Métodos

        app.MapGet("/api/jogadores", (CorinthiansDb context) =>
            {
                var jogadores = context.Jogadores;

                return jogadores != null ? Results.Ok(jogadores) : Results.NotFound();
            }
        ).Produces<Jogador>();


        app.MapPost("/api/jogadores", (CorinthiansDb context, string nome, int idade, string posicao, bool bagre) => 
            {
                var novoJogador =  new Jogador(Guid.NewGuid(), nome, idade, posicao, bagre);

                context.Jogadores.Add(novoJogador);
                context.SaveChanges();

                return Results.Ok();
            }
        ).Produces<Jogador>();

        app.MapPut("/api/jogadores", (CorinthiansDb context, Jogador buscaJogador) =>
            {
                var jogador = context.Jogadores.Find(buscaJogador.Id);

                if (jogador == null) return Results.NotFound();

                var entry = context.Entry(jogador).CurrentValues;

                entry.SetValues(buscaJogador);
                context.SaveChanges();

                return Results.Ok(jogador);

            }
        ).Produces<Jogador>();

        app.MapDelete("/api/jogadores", (CorinthiansDb context, Guid Id) =>
            {
                var jogador = context.Jogadores.Find(Id);

                if (jogador == null) return Results.NotFound();

                context.Jogadores.Remove(jogador);
                context.SaveChanges();

                return Results.Ok(jogador);
            }
        ).Produces<Jogador>();

        app.MapGet("/api/jogadores/Id", (CorinthiansDb context, Guid id) =>
            {
                foreach (var j in context.Jogadores)
                {
                    if (j.Id == id)
                    {
                         return Results.Ok(j);
                    }
                }
               return Results.NotFound();
            }
        ).Produces<Jogador>();

        app.MapPatch("/api/jogadores", (CorinthiansDb context, Guid id, Bagre bagre) =>
            {
                var jogador = context.Jogadores.Find(id);

                if (jogador == null) return Results.NotFound();

                var entry = context.Entry(jogador).CurrentValues;
                //setvalues espera um objeto, portanto não é possivel adicionar um valor booleano para bagre
                entry.SetValues(bagre);
                context.SaveChanges();

                return Results.Ok(jogador);

            }
        ).Produces<Jogador>();

        app.MapGet("/api/jogadores/bagres", (CorinthiansDb context) =>
            {
                //crio um objeto bagres, e seu valor recebe o contexto de jogadores, ou seja, os jogadores do banco de dados, where (aonde) > (realiza um foreach procurando o jogador que tenha o atributo bagre como true, e depois transforma numa lista, ou seja "bagres" é uma lista de bagres)
                var bagres = context.Jogadores.Where(j => j.bagre == true).ToList();
                    if (bagres == null)
                    {
                        return Results.NotFound("Impossível não ter bagre nesse time, tu fez cagada");
                    }

                return Results.Ok(bagres);
            }
        ).Produces<Jogador>();

        app.Run();
    }
}
