using System.Globalization;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

var ptBR = new CultureInfo("pt-BR");

Console.Write("Valor (ex.: 1500,50): ");
if (!decimal.TryParse(Console.ReadLine(), NumberStyles.Number, ptBR, out var valor) || valor <= 0)
{
    Console.WriteLine("Valor inválido.");
    return;
}

Console.Write("Data de vencimento (dd/mm/aaaa): ");
if (!DateTime.TryParseExact(Console.ReadLine()?.Trim(), "dd/MM/yyyy", ptBR, DateTimeStyles.None, out var vencimento))
{
    Console.WriteLine("Data inválida.");
    return;
}

var hoje = DateTime.Today;
var dias = CalculadoraJuros.DiasDeAtraso(vencimento, hoje);
var juros = CalculadoraJuros.Calcular(valor, vencimento, hoje);

Console.WriteLine();
Console.WriteLine($"Data de hoje:     {hoje:dd/MM/yyyy}");
Console.WriteLine($"Dias de atraso:   {dias}");
Console.WriteLine($"Valor original:   {valor.ToString("C", ptBR)}");
Console.WriteLine($"Juros (2,5%/dia): {juros.ToString("C", ptBR)}");
Console.WriteLine($"Valor atualizado: {(valor + juros).ToString("C", ptBR)}");

static class CalculadoraJuros
{
    public const decimal TaxaDiaria = 0.025m;

    // Conta só dias corridos de atraso; se ainda não venceu, não há juros.
    public static int DiasDeAtraso(DateTime vencimento, DateTime hoje) =>
        Math.Max(0, (hoje.Date - vencimento.Date).Days);

    // Juros simples: valor x 2,5% x dias de atraso.
    public static decimal Calcular(decimal valor, DateTime vencimento, DateTime hoje) =>
        Math.Round(valor * TaxaDiaria * DiasDeAtraso(vencimento, hoje), 2, MidpointRounding.AwayFromZero);
}
