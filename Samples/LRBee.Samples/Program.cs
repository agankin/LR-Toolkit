using LRBee.GrammarDefinition;
using LRBee.Parsing.ItemBuilding;
using LRBee.Utilities.Extensions;

using static LRBee.GrammarDefinition.GrammarUtils;

var grammar = new GrammarBuilder<Symbol>(Symbol.S)
{
    [Symbol.S] = _(Symbol.A, Symbol.A),
    [Symbol.A] = _(Symbol.a, Symbol.A),
    [Symbol.A] = _(Symbol.b)
}.Build();

var itemSetBuilder = new ClosureItemSetBuilder<Symbol>(grammar);

var initialProduction = new ProductionRule<Symbol>(Symbol.S, new[] { Symbol.S });
var initialItem = Item<Symbol>.Init(initialProduction);

var itemSet0 = itemSetBuilder.GetClosure(initialItem);

PrintItemSet(itemSet0);

var itemSet1 = itemSetBuilder.GetClosure(itemSet0.AfterNextSymbol(Symbol.a));
PrintItemSet(itemSet1);

Console.Write("To exit press any key...");
Console.ReadKey(true);

void PrintItemSet(ItemSet<Symbol> itemSet)
{
    Console.WriteLine("Item Set:");
    foreach(var item in itemSet)
    {
        PrintItem(item);
        Console.WriteLine();
    }
}

void PrintItem(Item<Symbol> item)
{
    var productionRule = item.ProductionRule;

    Console.Write(productionRule.Symbol);
    Console.Write(" -> ");

    int position = 0;
    foreach (var symbol in productionRule)
    {
        if (position++ == item.Position)
            Console.Write("*");

        Console.Write(symbol);
        Console.Write(", ");
    }

    if (position++ == item.Position)
        Console.Write("*");
}

void PrintSymbols(IEnumerable<Symbol> symbols)
{
    symbols.ForEach(symbol => Console.WriteLine($"{symbol}, "));
}

public enum Symbol
{
    Start,
    S,
    A,
    a,
    b
}
