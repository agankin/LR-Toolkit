namespace LRBee.Parsing
{
    public readonly record struct ParsedSymbol<TSymbol>
    {
        public required TSymbol Symbol { get; init; }

        public required IEnumerable<ParsedSymbol<TSymbol>> ReducedFromSymbols { get; init; }

        public static ParsedSymbol<TSymbol> ForShift(TSymbol symbol) =>
            new ParsedSymbol<TSymbol>
            {
                Symbol = symbol,
                ReducedFromSymbols = Enumerable.Empty<ParsedSymbol<TSymbol>>()
            };

        public static ParsedSymbol<TSymbol> ForReduce(TSymbol reduceToSymbol, IEnumerable<ParsedSymbol<TSymbol>> reducedSymbols) =>
            new ParsedSymbol<TSymbol>
            {
                Symbol = reduceToSymbol,
                ReducedFromSymbols = reducedSymbols
            };
    }
}