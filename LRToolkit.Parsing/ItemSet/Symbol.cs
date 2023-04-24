﻿using LRToolkit.Utilities;
using Optional;
using Optional.Unsafe;

namespace LRToolkit.Parsing
{
    public readonly record struct Symbol<TSymbol> where TSymbol : notnull
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

        public static Symbol<TSymbol> Create(TSymbol symbol) => new Symbol<TSymbol>(
            SymbolType.Symbol,
            symbol.Some(),
            Option.None<ItemSet<TSymbol>>());

        public static Symbol<TSymbol> Create(TSymbol symbol, ItemSet<TSymbol> goToAfterReduce) =>
            new Symbol<TSymbol>(
                SymbolType.SymbolReduced,
                symbol.Some(),
                goToAfterReduce.Some());

        public static Symbol<TSymbol> End() => new Symbol<TSymbol>(
            SymbolType.End,
            Option.None<TSymbol>(),
            Option.None<ItemSet<TSymbol>>());

        public TResult MapSymbol<TResult>(Func<TSymbol, TResult> mapSymbol, Func<TResult> mapEnd) =>
            MapType(mapSymbol, (symbol, _) => mapSymbol(symbol), mapEnd);

        public override string? ToString() => MapType(
            symbol => symbol?.ToString(),
            (symbol, goToAfterReduce) => $"{symbol}, GOTO {goToAfterReduce}",
            () => "$");

        private TResult MapType<TResult>(
            Func<TSymbol, TResult> mapSymbol,
            Func<TSymbol, ItemSet<TSymbol>, TResult> mapSymbolReduced,
            Func<TResult> mapEnd) =>
            _type switch
            {
                SymbolType.Symbol => mapSymbol(_symbol.ValueOrFailure()),
                SymbolType.SymbolReduced => mapSymbolReduced(_symbol.ValueOrFailure(), _goToAfterReduce.ValueOrFailure()),
                SymbolType.End => mapEnd(),
                _ => throw new UnsupportedEnumValueException<SymbolType>(_type)
            };

        private enum SymbolType
        {
            Symbol = 1,

            SymbolReduced,

            End
        }
    }
}