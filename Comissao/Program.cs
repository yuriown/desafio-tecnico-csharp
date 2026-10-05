using System.Globalization;
using System.Text;
using System.Text.Json;

Console.OutputEncoding = Encoding.UTF8;

var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
var caminho = Path.Combine(AppContext.BaseDirectory, "vendas.json");
var dados = JsonSerializer.Deserialize<DadosVendas>(File.ReadAllText(caminho), opcoes)
    ?? throw new InvalidOperationException("Não foi possível ler o arquivo vendas.json.");

var ptBR = new CultureInfo("pt-BR");

var resumo = dados.Vendas
    .GroupBy(v => v.Vendedor)
    .Select(grupo => new
    {
        Vendedor = grupo.Key,
        QtdeVendas = grupo.Count(),
        TotalVendido = grupo.Sum(v => v.Valor),
        Comissao = Math.Round(grupo.Sum(v => RegraComissao.Calcular(v.Valor)), 2, MidpointRounding.AwayFromZero)
    })
    .OrderByDescending(r => r.Comissao)
    .ToList();

Console.WriteLine($"{"Vendedor",-18}{"Vendas",8}{"Total vendido",18}{"Comissão",14}");
Console.WriteLine(new string('-', 58));

foreach (var r in resumo)
{
    Console.WriteLine($"{r.Vendedor,-18}{r.QtdeVendas,8}{r.TotalVendido.ToString("C", ptBR),18}{r.Comissao.ToString("C", ptBR),14}");
}

Console.WriteLine(new string('-', 58));
Console.WriteLine($"{"Total",-18}{resumo.Sum(r => r.QtdeVendas),8}{resumo.Sum(r => r.TotalVendido).ToString("C", ptBR),18}{resumo.Sum(r => r.Comissao).ToString("C", ptBR),14}");

static class RegraComissao
{
    // Abaixo de 100: sem comissão. De 100 até 499,99: 1%. A partir de 500: 5%.
    public static decimal Percentual(decimal valor) => valor switch
    {
        < 100m => 0m,
        < 500m => 0.01m,
        _ => 0.05m
    };

    public static decimal Calcular(decimal valor) => valor * Percentual(valor);
}

record Venda(string Vendedor, decimal Valor);
record DadosVendas(List<Venda> Vendas);
