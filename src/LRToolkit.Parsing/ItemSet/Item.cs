using LRToolkit.GrammarDefinition;
using Optional;
using Optional.Collections;

namespace LRToolkit.Parsing;

public record Item<TSymbol> where TSymbol : notnull
{
    private Item(TSymbol forSymbol, Production<TSymbol> production, ILookahead<TSymbol> lookahead, int position, bool isKernel)
    {
        ForSymbol = forSymbol;
        Production = production;
        Lookahead = lookahead;
        Position = position;
        IsKernel = isKernel;
    }

    public TSymbol ForSymbol { get; init; }

    public Production<TSymbol> Production { get; init; }

    public ILookahead<TSymbol> Lookahead { get; init; }

    public int ProductionCount => Production.Count;

    public int Count => Production.Count + Lookahead.Count;

    public int Position { get; init; }

    public bool IsStart { get; init; }

    public bool IsKernel { get; init; }

    public static Item<TSymbol> ForStart(TSymbol start, ILookahead<TSymbol> lookahead)
    {
        var production = new Production<TSymbol>(start);

        return new Item<TSymbol>(start, production, lookahead, position: 0, isKernel: true)
        {
            IsStart = true
        };
    }

    public static Item<TSymbol> ClosureFromRule(ProductionRule<TSymbol> rule, ILookahead<TSymbol> lookahead)
    {
        if (rule == null)
            throw new ArgumentNullException(nameof(rule));

        var forSymbol = rule.ForSymbol;
        var production = rule.Production;

        return new Item<TSymbol>(forSymbol, production, lookahead, position: 0, isKernel: false);
    }

    public Option<Symbol<TSymbol>> GetSymbolAhead(int lookaheadPosition = 0)
    {
        var symbolAheadPosition = Position + lookaheadPosition;

        return Production.ElementAtOrNone(symbolAheadPosition)
            .Map(Symbol<TSymbol>.Create)
            .Else(Lookahead[symbolAheadPosition - Production.Count]);
    }

    public Option<Item<TSymbol>> StepForward() => HasSymbolAhead()
        ? (this with { Position = Position + 1, IsKernel = true }).Some()
        : Option.None<Item<TSymbol>>();

    public bool HasSymbolAhead() => Position < Count;

    public bool ProductionFinished() => Position >= Production.Count;

    public override string ToString() => $"{ForSymbol} -> {GetFormattedProductions()}";

    private string GetFormattedProductions()
    {
        var formattedProductions = string.Join(
            ", ",
            Production.Select((symbol, idx) => idx == Position ? $"*{symbol}" : symbol.ToString()));

        var lookaheadPos = Position - Production.Count;
        var formattedLookaheads = string.Join(
            ", ",
            Lookahead.Select((symbol, idx) => idx == lookaheadPos ? $"*{symbol}" : symbol.ToString()));
        var lastPos = Position == Count ? "*" : string.Empty;

        return Lookahead.Count > 0
            ? $"{formattedProductions}; {formattedLookaheads}{lastPos}"
            : $"{formattedProductions}{lastPos}";
    }
}