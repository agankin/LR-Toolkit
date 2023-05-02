using System.Collections;
using System.Collections.Immutable;
using LRToolkit.Utilities;
using Optional;

namespace LRToolkit.Parsing
{
    public readonly record struct ParsedSymbolsStack<TSymbol> : IEnumerable<ParsedSymbol<TSymbol>>
        where TSymbol : notnull
    {
        private ImmutableStack<ParsedSymbol<TSymbol>> ParsedSymbols { get; init; }

        public static readonly ParsedSymbolsStack<TSymbol> Empty = new ParsedSymbolsStack<TSymbol>
        {
            ParsedSymbols = ImmutableStack<ParsedSymbol<TSymbol>>.Empty
        };

        public ParsedSymbolsStack<TSymbol> Shift(TSymbol symbolAhead)
        {
            var parsedSymbol = ParsedSymbol<TSymbol>.ForShift(symbolAhead);

            return this with { ParsedSymbols = ParsedSymbols.Push(parsedSymbol) };
        }

        public (ParsedSymbolsStack<TSymbol>, ParsedSymbol<TSymbol>) Reduce(Item<TSymbol> reducedItem) =>
            Reduce(reducedItem, Option.None<TSymbol>());

        public (ParsedSymbolsStack<TSymbol>, ParsedSymbol<TSymbol>) Reduce(Item<TSymbol> reducedItem, Option<TSymbol> symbolAheadOption)
        {
            var reducedToSymbol = reducedItem.ForSymbol;
            var reducedSymbolsCount = reducedItem.ProductionCount;

            var @this = this;
            var parsedSymbols = symbolAheadOption.Map(ParsedSymbol<TSymbol>.ForShift)
                .Match(@this.ParsedSymbols.Push, () => @this.ParsedSymbols);

            var (newParsedSymbols, reducedSymbols) = parsedSymbols.Pop(reducedSymbolsCount);
            var reducedParsedSymbol = ParsedSymbol<TSymbol>.ForReduce(reducedToSymbol, reducedSymbols);

            return (this with { ParsedSymbols = newParsedSymbols }, reducedParsedSymbol);
        }

        public IEnumerator<ParsedSymbol<TSymbol>> GetEnumerator() => ParsedSymbols.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}