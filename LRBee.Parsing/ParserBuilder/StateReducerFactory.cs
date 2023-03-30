using LRBee.Utilities;

namespace LRBee.Parsing
{
    internal static class StateReducerFactory
    {
        public static ParserStateReducer<TSymbol> Shift<TSymbol>(
            Symbol<TSymbol> symbolAhead,
            ItemSet<TSymbol> afterSymbolFullItemSet)
            where TSymbol : notnull
        {
            return (automataRunState, parsingState) =>
            {
                var (parsedSymbols, parsedSymbolsItemSets) = parsingState;

                return parsingState with
                {
                    ParsedSymbols = symbolAhead.MapSymbol(
                        parsedSymbols.Shift,
                        () => throw new InvalidOperationException("Shift called for END symbol.")),
                    ParsedSymbolsItemSets = parsedSymbolsItemSets.Push(afterSymbolFullItemSet),
                };
            };
        }

        public static ParserStateReducer<TSymbol> Reduce<TSymbol>(
            Symbol<TSymbol> symbolAhead,
            ItemSet<TSymbol> afterSymbolFullItemSet,
            Item<TSymbol> reducedItem)
            where TSymbol : notnull
        {
            return (automataRunState, parsingState) =>
            {
                var reducedToSymbol = reducedItem.ForSymbol;
                var reducedSymbolsCount = reducedItem.Production.Count;
                var (parsedSymbols, parsedSymbolsItemSets) = parsingState;

                var (newParsedSymbols, reducedParsedSymbol) = symbolAhead.MapSymbol(
                    symbol => parsedSymbols.Reduce(symbol, reducedItem), 
                    () => throw new InvalidOperationException("Reduce called for END symbol."));
                var newParsedSymbolsItemSets = parsedSymbolsItemSets
                    .Push(afterSymbolFullItemSet)
                    .PopSkip(reducedSymbolsCount);

                var goToAfterReduce = newParsedSymbolsItemSets.Peek();
                var nextEmitForGoToAfterReduce = Symbol<TSymbol>.Create(reducedToSymbol, goToAfterReduce);
                automataRunState.EmitNext(nextEmitForGoToAfterReduce);

                return parsingState with
                {
                    ParsedSymbols = newParsedSymbols,
                    ParsedSymbolsItemSets = newParsedSymbolsItemSets
                };
            };
        }

        public static ParserStateReducer<TSymbol> Accept<TSymbol>(
            Symbol<TSymbol> symbolAhead,
            ItemSet<TSymbol> afterSymbolFullItemSet)
            where TSymbol : notnull
        {
            return (automataRunState, parsingState) =>
            {
                var (parsedSymbols, parsedSymbolsItemSets) = parsingState;

                return parsingState with
                {
                    ParsedSymbols = symbolAhead.MapSymbol(parsedSymbols.Shift, () => parsedSymbols),
                    ParsedSymbolsItemSets = parsedSymbolsItemSets.Push(afterSymbolFullItemSet)
                };
            };
        }

        public static ParserStateReducer<TSymbol> GoToAfterReduce<TSymbol>(TSymbol reducedToSymbol)
            where TSymbol : notnull
        {
            var symbol = Symbol<TSymbol>.Create(reducedToSymbol);

            return (automataRunState, parsingState) =>
            {
                automataRunState.EmitNext(symbol);

                return parsingState;
            };
        }
    }
}