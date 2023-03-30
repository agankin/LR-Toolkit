using System.Collections.Immutable;

namespace LRBee.Parsing
{
    public record ParsingState<TSymbol>(
        ParsedSymbolsStack<TSymbol> ParsedSymbols,
        ImmutableStack<ItemSet<TSymbol>> ParsedSymbolsItemSets)
        where TSymbol : notnull
    {
        public static ParsingState<TSymbol> CreateNew(ItemSet<TSymbol> startItemSet) => new ParsingState<TSymbol>(
            ParsedSymbolsStack<TSymbol>.Empty,
            ImmutableStack<ItemSet<TSymbol>>.Empty.Push(startItemSet));
    }
}