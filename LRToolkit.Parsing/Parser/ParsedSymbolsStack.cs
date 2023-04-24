using System.Collections;
using System.Collections.Immutable;
using LRToolkit.Utilities;

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

        public (ParsedSymbolsStack<TSymbol>, ParsedSymbol<TSymbol>) Reduce(TSymbol symbolAhead, Item<TSymbol> reducedItem)
        {
            var reducedToSymbol = reducedItem.ForSymbol;
            var reducedSymbolsCount = reducedItem.Count;

            var parsedSymbolAhead = ParsedSymbol<TSymbol>.ForShift(symbolAhead);

            var (newParsedSymbols, reducedSymbols) = ParsedSymbols
                .Push(parsedSymbolAhead)
                .Pop(reducedSymbolsCount);
            var reducedParsedSymbol = ParsedSymbol<TSymbol>.ForReduce(reducedToSymbol, reducedSymbols);

            return (this with { ParsedSymbols = newParsedSymbols }, reducedParsedSymbol);
        }

        public IEnumerator<ParsedSymbol<TSymbol>> GetEnumerator() => ParsedSymbols.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}