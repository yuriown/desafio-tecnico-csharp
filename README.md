# Desafio Técnico – C#

Solução em C# (.NET 10), com um projeto de console para cada questão.

## Como rodar

Requer o .NET SDK 10.

```bash
dotnet run --project Comissao
dotnet run --project Estoque
dotnet run --project Juros
```

## 1. Comissão (`Comissao/`)

Lê `vendas.json`, calcula a comissão de cada venda e soma por vendedor.

| Valor da venda | Comissão |
|---|---|
| abaixo de R$ 100,00 | 0% |
| de R$ 100,00 até R$ 499,99 | 1% |
| a partir de R$ 500,00 | 5% |

- Valores em `decimal`, para não ter erro de arredondamento de ponto flutuante.
- A comissão é arredondada no total de cada vendedor (2 casas, meio para cima).

Resultado:

| Vendedor | Comissão |
|---|---|
| João Silva | R$ 495,68 |
| Maria Souza | R$ 465,95 |
| Ana Lima | R$ 404,98 |
| Carlos Oliveira | R$ 379,37 |

## 2. Estoque (`Estoque/`)

Menu no console para listar produtos, lançar entradas/saídas e ver o histórico.
Cada movimentação recebe um ID sequencial único, uma descrição e o tipo (entrada ou saída),
e o programa mostra o estoque final do produto movimentado.

Validações: produto inexistente, quantidade menor ou igual a zero, descrição vazia
e saída maior que o estoque disponível.

As movimentações ficam em memória durante a execução (o `estoque.json` não é alterado).

## 3. Juros (`Juros/`)

Recebe um valor e uma data de vencimento e calcula os juros até a data de hoje.

- O enunciado fala em "juros" e "multa de 2,5% ao dia". Considerei **juros simples de 2,5% por dia de atraso**:
  `juros = valor × 0,025 × dias de atraso`.
- Conta dias corridos. Se a data ainda não venceu, os juros são zero.
- Valor no formato brasileiro (`1500,50`) e data em `dd/mm/aaaa`.

## Observação

Desenvolvido com auxílio de IA (Claude). Revisei o código e entendo cada parte da solução.
