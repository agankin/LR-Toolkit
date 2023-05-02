using LRToolkit.Utilities;
using Optional;
using Optional.Unsafe;

namespace LRToolkit.Parsing
{
    public readonly struct Symbol<TSymbol> : IEquatable<Symbol<TSymbol>>
        where TSymbol : notnull
    {
        private readonly SymbolType _type;
        private readonly Option<TSymbol> _symbol;
        private readonly Option<ItemSet<TSymbol>> _goToAfterReduce;

        private Symbol(SymbolType type, Option<TSymbol> symbol, Option<ItemSet<TSymbol>> goToAfterReduce)
        {
            _type = type;
            _symbol = symbol;
            _goToAfterReduce = goToAfterReduce;
        }

        public static Symbol<TSymbol> Create(TSymbol symbol) =>
            new Symbol<TSymbol>(SymbolType.Symbol, symbol.Some(), Option.None<ItemSet<TSymbol>>());

        public static Symbol<TSymbol> CreateLookahead(TSymbol symbol) =>
            new Symbol<TSymbol>(SymbolType.LookaheadSymbol, symbol.Some(), Option.None<ItemSet<TSymbol>>());

        public static Symbol<TSymbol> CreateReduced(TSymbol symbol, ItemSet<TSymbol> goToAfterReduce) =>
            new Symbol<TSymbol>(SymbolType.ReducedSymbol, symbol.Some(), goToAfterReduce.Some());

        public static Symbol<TSymbol> End() =>
            new Symbol<TSymbol>(SymbolType.End, Option.None<TSymbol>(), Option.None<ItemSet<TSymbol>>());

        public TResult MapByType<TResult>(
            Func<TSymbol, TResult> mapSymbol,
            Func<TSymbol, TResult> mapLookaheadSymbol,
            Func<TResult> mapEnd)
        {
            return MapByType(
                mapSymbol,
                mapLookaheadSymbol,
                mapReducedSymbol: (symbol, _) => mapSymbol(symbol),
                mapEnd);
        }

        public bool Equals(Symbol<TSymbol> other) =>
            _symbol == other._symbol && _goToAfterReduce == other._goToAfterReduce;

        public static bool operator ==(Symbol<TSymbol> first, Symbol<TSymbol> second) =>
            first.Equals(second);

        public static bool operator !=(Symbol<TSymbol> first, Symbol<TSymbol> second) =>
            !first.Equals(second);

        public override string? ToString()
        {
            return MapByType(
                mapSymbol: symbol => symbol?.ToString(),
                mapLookaheadSymbol: symbol => symbol?.ToString(),
                mapReducedSymbol: (symbol, goToAfterReduce) => $"{symbol}, GOTO {goToAfterReduce}",
                mapEnd: () => "$");
        }

        public override bool Equals(object? obj) =>
            obj is Symbol<TSymbol> other && Equals(other);

        public override int GetHashCode() => Hash.Calculate(_symbol, _goToAfterReduce);

        private TResult MapByType<TResult>(
            Func<TSymbol, TResult> mapSymbol,
            Func<TSymbol, TResult> mapLookaheadSymbol,
            Func<TSymbol, ItemSet<TSymbol>, TResult> mapReducedSymbol,
            Func<TResult> mapEnd)
        {
            return _type switch
            {
                SymbolType.Symbol => mapSymbol(_symbol.ValueOrFailure()),
                SymbolType.LookaheadSymbol => mapLookaheadSymbol(_symbol.ValueOrFailure()),
                SymbolType.ReducedSymbol => mapReducedSymbol(_symbol.ValueOrFailure(), _goToAfterReduce.ValueOrFailure()),
                SymbolType.End => mapEnd(),
                _ => throw new UnsupportedEnumValueException<SymbolType>(_type)
            };
        }

        private enum SymbolType
        {
            Symbol = 1,

            LookaheadSymbol,

            ReducedSymbol,

            End
        }
    }
}