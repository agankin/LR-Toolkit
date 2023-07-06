using LRToolkit.Utilities;
using Optional;
using Optional.Unsafe;

namespace LRToolkit.Parsing
{
    public readonly record struct Symbol<TSymbol>(
        SymbolType Type,
        Option<TSymbol> Value,
        Option<ItemSet<TSymbol>> GoToAfterReduce
    ) where TSymbol : notnull
    {
        private readonly Option<ItemSet<TSymbol>> _goToAfterReduce = GoToAfterReduce;

        public static Symbol<TSymbol> Create(TSymbol symbol) =>
            new Symbol<TSymbol>(SymbolType.Symbol, symbol.Some(), Option.None<ItemSet<TSymbol>>());

        public static Symbol<TSymbol> CreateLookahead(TSymbol symbol) =>
            new Symbol<TSymbol>(SymbolType.Lookahead, symbol.Some(), Option.None<ItemSet<TSymbol>>());

        public static Symbol<TSymbol> CreateReduced(TSymbol symbol, ItemSet<TSymbol> goToAfterReduce) =>
            new Symbol<TSymbol>(SymbolType.ReducedSymbol, symbol.Some(), goToAfterReduce.Some());

        public static Symbol<TSymbol> End() =>
            new Symbol<TSymbol>(SymbolType.End, Option.None<TSymbol>(), Option.None<ItemSet<TSymbol>>());

        public override string? ToString()
        {
            return Type switch
            {
                SymbolType.Symbol => Value.ValueOrFailure().ToString(),
                SymbolType.Lookahead => Value.ValueOrFailure().ToString(),
                SymbolType.ReducedSymbol => $"{Value.ValueOrFailure()}, GOTO {_goToAfterReduce.ValueOrFailure()}",
                SymbolType.End => "$",
                _ => throw new UnsupportedEnumValueException<SymbolType>(Type)
            };
        }
    }
}