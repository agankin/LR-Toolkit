using DFAutomaton;
using LRToolkit.Utilities;
using Optional.Unsafe;

namespace LRToolkit.Parsing
{
    internal static class StateReducerFactory
    {
        public static StateReducer<Symbol<TSymbol>, ParsingState<TSymbol>> Shift<TSymbol>(
            Symbol<TSymbol> symbol,
            ItemSet<TSymbol> nextItemSet,
            ShiftListener<TSymbol> listener)
            where TSymbol : notnull
        {
            return (automatonRunState, parsingState) =>
            {
                listener(parsingState, symbol, automatonRunState.TransitingTo);
                var (parsedSymbols, parsedSymbolsItemSets) = parsingState;

                return parsingState with
                {
                    ParsedSymbols = symbol.Type switch
                    {
                        SymbolType.Symbol => parsedSymbols.Shift(symbol.Value.ValueOrFailure()),
                        _ => parsedSymbols
                    },
                    ParsedSymbolsItemSets = parsedSymbolsItemSets.Push(nextItemSet),
                };
            };
        }

        public static StateReducer<Symbol<TSymbol>, ParsingState<TSymbol>> Reduce<TSymbol>(
            Symbol<TSymbol> symbol,
            ItemSet<TSymbol> nextItemSet,
            Item<TSymbol> reducedItem,
            ReduceListener<TSymbol> listener)
            where TSymbol : notnull
        {
            return (automatonRunState, parsingState) =>
            {
                listener(parsingState, symbol, reducedItem, automatonRunState.TransitingTo);

                var reducedToSymbol = reducedItem.ForSymbol;
                var (parsedSymbols, parsedSymbolsItemSets) = parsingState;

                var (newParsedSymbols, reducedParsedSymbol) = symbol.Type switch
                {
                    SymbolType.Symbol => parsedSymbols
                        .Shift(symbol.Value.ValueOrFailure())
                        .Reduce(reducedItem), 
                    _ => parsedSymbols.Reduce(reducedItem)
                };
                
                var newParsedSymbolsItemSets = parsedSymbolsItemSets
                    .Push(nextItemSet)
                    .PopSkip(reducedItem.Count);

                var goToAfterReduce = newParsedSymbolsItemSets.Peek();
                var nextEmitForGoToAfterReduce = Symbol<TSymbol>.CreateReduced(reducedToSymbol, goToAfterReduce);
                automatonRunState.EmitNext(nextEmitForGoToAfterReduce);
                
                return parsingState with
                {
                    ParsedSymbols = newParsedSymbols,
                    ParsedSymbolsItemSets = newParsedSymbolsItemSets
                };
            };
        }

        public static StateReducer<Symbol<TSymbol>, ParsingState<TSymbol>> Accept<TSymbol>(
            Symbol<TSymbol> symbol,
            ItemSet<TSymbol> nextItemSet,
            AcceptListener<TSymbol> listener)
            where TSymbol : notnull
        {
            return (automatonRunState, parsingState) =>
            {
                listener(parsingState, automatonRunState.TransitingTo);
                var (parsedSymbols, parsedSymbolsItemSets) = parsingState;

                return parsingState with
                {
                    ParsedSymbols = symbol.Type switch
                    {
                        SymbolType.Symbol => parsedSymbols.Shift(symbol.Value.ValueOrFailure()),
                        _ => parsedSymbols
                    },
                    ParsedSymbolsItemSets = parsedSymbolsItemSets.Push(nextItemSet)
                };
            };
        }

        public static StateReducer<Symbol<TSymbol>, ParsingState<TSymbol>> GoToAfterReduce<TSymbol>(
            Item<TSymbol> reducedItem,
            GoToAfterReduceListener<TSymbol> listener)
            where TSymbol : notnull
        {
            var symbol = Symbol<TSymbol>.Create(reducedItem.ForSymbol);

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