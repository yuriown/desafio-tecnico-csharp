using System.Text;
using System.Text.Json;

Console.OutputEncoding = Encoding.UTF8;

var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
var caminho = Path.Combine(AppContext.BaseDirectory, "estoque.json");
var dados = JsonSerializer.Deserialize<DadosEstoque>(File.ReadAllText(caminho), opcoes)
    ?? throw new InvalidOperationException("Não foi possível ler o arquivo estoque.json.");

var deposito = new Deposito(dados.Estoque);

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1 - Listar produtos");
    Console.WriteLine("2 - Lançar movimentação");
    Console.WriteLine("3 - Ver movimentações");
    Console.WriteLine("0 - Sair");

    var opcao = Ler("Opção: ");
    if (opcao is null or "0")
        break;

    switch (opcao)
    {
        case "1": ListarProdutos(); break;
        case "2": LancarMovimentacao(); break;
        case "3": ListarMovimentacoes(); break;
        default: Console.WriteLine("Opção inválida."); break;
    }
}

void ListarProdutos()
{
    Console.WriteLine();
    Console.WriteLine($"{"Código",-8}{"Produto",-28}{"Estoque",8}");
    foreach (var p in deposito.Produtos)
        Console.WriteLine($"{p.CodigoProduto,-8}{p.DescricaoProduto,-28}{p.Estoque,8}");
}

void LancarMovimentacao()
{
    var codigo = LerInteiro("Código do produto: ");
    if (codigo is null) return;

    var tipoDigitado = Ler("Tipo (E = entrada, S = saída): ")?.ToUpperInvariant();
    TipoMovimentacao tipo;
    if (tipoDigitado == "E") tipo = TipoMovimentacao.Entrada;
    else if (tipoDigitado == "S") tipo = TipoMovimentacao.Saida;
    else
    {
        Console.WriteLine("Tipo inválido. Use E ou S.");
        return;
    }

    var quantidade = LerInteiro("Quantidade: ");
    if (quantidade is null) return;

    var descricao = Ler("Descrição da movimentação: ") ?? "";

    try
    {
        var mov = deposito.Movimentar(codigo.Value, tipo, quantidade.Value, descricao);
        var produto = deposito.BuscarProduto(mov.CodigoProduto)!;
        Console.WriteLine($"Movimentação #{mov.Id} registrada ({mov.Tipo}: {mov.Quantidade} un.).");
        Console.WriteLine($"Estoque final de {produto.DescricaoProduto}: {mov.EstoqueFinal}");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Erro: {ex.Message}");
    }
}

void ListarMovimentacoes()
{
    Console.WriteLine();
    if (deposito.Movimentacoes.Count == 0)
    {
        Console.WriteLine("Nenhuma movimentação lançada.");
        return;
    }

    foreach (var m in deposito.Movimentacoes)
        Console.WriteLine($"#{m.Id} {m.Data:dd/MM/yyyy HH:mm} | Produto {m.CodigoProduto} | {m.Tipo} {m.Quantidade} | Saldo {m.EstoqueFinal} | {m.Descricao}");
}

static string? Ler(string mensagem)
{
    Console.Write(mensagem);
    return Console.ReadLine()?.Trim();
}

static int? LerInteiro(string mensagem)
{
    var texto = Ler(mensagem);
    if (texto is null) return null;

    if (int.TryParse(texto, out var numero))
        return numero;

    Console.WriteLine("Digite um número inteiro.");
    return null;
}

enum TipoMovimentacao { Entrada, Saida }

record Movimentacao(
    int Id,
    DateTime Data,
    int CodigoProduto,
    TipoMovimentacao Tipo,
    int Quantidade,
    string Descricao,
    int EstoqueFinal);

class Produto
{
    public int CodigoProduto { get; init; }
    public string DescricaoProduto { get; init; } = "";
    public int Estoque { get; set; }
}

record DadosEstoque(List<Produto> Estoque);

class Deposito
{
    private readonly Dictionary<int, Produto> _produtos;
    private readonly List<Movimentacao> _movimentacoes = new();
    private int _ultimoId;

    public Deposito(IEnumerable<Produto> produtos)
    {
        _produtos = produtos.ToDictionary(p => p.CodigoProduto);
    }

    public IEnumerable<Produto> Produtos => _produtos.Values;
    public IReadOnlyList<Movimentacao> Movimentacoes => _movimentacoes;

    public Produto? BuscarProduto(int codigo) => _produtos.GetValueOrDefault(codigo);

    public Movimentacao Movimentar(int codigoProduto, TipoMovimentacao tipo, int quantidade, string descricao)
    {
        if (!_produtos.TryGetValue(codigoProduto, out var produto))
            throw new InvalidOperationException($"Produto {codigoProduto} não encontrado.");

        if (quantidade <= 0)
            throw new InvalidOperationException("A quantidade deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(descricao))
            throw new InvalidOperationException("Informe uma descrição para a movimentação.");

        if (tipo == TipoMovimentacao.Saida && quantidade > produto.Estoque)
            throw new InvalidOperationException(
                $"Estoque insuficiente. {produto.DescricaoProduto} tem {produto.Estoque} unidade(s).");

        produto.Estoque += tipo == TipoMovimentacao.Entrada ? quantidade : -quantidade;

        var movimentacao = new Movimentacao(
            ++_ultimoId, DateTime.Now, codigoProduto, tipo, quantidade, descricao.Trim(), produto.Estoque);

        _movimentacoes.Add(movimentacao);
        return movimentacao;
    }
}
