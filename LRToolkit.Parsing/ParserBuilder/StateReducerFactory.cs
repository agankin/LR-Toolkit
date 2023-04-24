using LRToolkit.Utilities;

namespace LRToolkit.Parsing
{
    internal static class StateReducerFactory
    {
        public static ParserStateReducer<TSymbol> Shift<TSymbol>(
            Symbol<TSymbol> symbolAhead,
            ItemSet<TSymbol> afterSymbolFullItemSet,
            ShiftListener<TSymbol> listener)
            where TSymbol : notnull
        {
            return (automatonRunState, parsingState) =>
            {
                listener(parsingState, symbolAhead, automatonRunState.TransitingTo);
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
            Item<TSymbol> reducedItem,
            ReduceListener<TSymbol> listener)
            where TSymbol : notnull
        {
            return (automatonRunState, parsingState) =>
            {
                listener(parsingState, symbolAhead, reducedItem, automatonRunState.TransitingTo);

                var reducedToSymbol = reducedItem.ForSymbol;
                var (parsedSymbols, parsedSymbolsItemSets) = parsingState;

                var (newParsedSymbols, reducedParsedSymbol) = symbolAhead.MapSymbol(
                    symbol => parsedSymbols.Reduce(symbol, reducedItem), 
                    () => throw new InvalidOperationException("Reduce called for END symbol."));
                var newParsedSymbolsItemSets = parsedSymbolsItemSets
                    .Push(afterSymbolFullItemSet)
                    .PopSkip(reducedItem.Count);

                var goToAfterReduce = newParsedSymbolsItemSets.Peek();
                var nextEmitForGoToAfterReduce = Symbol<TSymbol>.Create(reducedToSymbol, goToAfterReduce);
                automatonRunState.EmitNext(nextEmitForGoToAfterReduce);
                
                return parsingState with
                {
                    ParsedSymbols = newParsedSymbols,
                    ParsedSymbolsItemSets = newParsedSymbolsItemSets
                };
            };
        }

        public static ParserStateReducer<TSymbol> Accept<TSymbol>(
            Symbol<TSymbol> symbolAhead,
            ItemSet<TSymbol> afterSymbolFullItemSet,
            AcceptListener<TSymbol> listener)
            where TSymbol : notnull
        {
            return (automatonRunState, parsingState) =>
            {
                listener(parsingState, automatonRunState.TransitingTo);
                var (parsedSymbols, parsedSymbolsItemSets) = parsingState;

                return parsingState with
                {
                    ParsedSymbols = symbolAhead.MapSymbol(parsedSymbols.Shift, () => parsedSymbols),
                    ParsedSymbolsItemSets = parsedSymbolsItemSets.Push(afterSymbolFullItemSet)
                };
            };
        }

        public static ParserStateReducer<TSymbol> GoToAfterReduce<TSymbol>(
            Item<TSymbol> reducedItem,
            GoToAfterReduceListener<TSymbol> listener)
            where TSymbol : notnull
        {
            var reducedToSymbol = reducedItem.ForSymbol;
            var symbol = Symbol<TSymbol>.Create(reducedToSymbol);

            return (automatonRunState, parsingState) =>
            {
                listener(parsingState, symbol, automatonRunState.TransitingTo);
                
                automatonRunState.EmitNext(symbol);
                reducedItem.Lookahead.ForEach(automatonRunState.EmitNext);

                return parsingState;
            };
        }
    }
}