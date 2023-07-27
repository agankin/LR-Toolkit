using DFAutomaton;
using LRToolkit.Utilities;
using Optional;
using Optional.Unsafe;

namespace LRToolkit.Parsing
{
    internal class StateReducerFactory<TSymbol> where TSymbol : notnull
    {
        private readonly ParserTransitionsObserver<TSymbol> _observer;

        public StateReducerFactory(ParserTransitionsObserver<TSymbol> observer) => _observer = observer;

        public ReduceValue<Symbol<TSymbol>, ParsingState<TSymbol>> Shift(Symbol<TSymbol> symbol)
        {
            return automatonState =>
            {
                var (parsingState, nextStateOption, _) = automatonState;
                var nextState = nextStateOption.ValueOrFailure();

                _observer.ShiftListener(parsingState, symbol, nextState.Id);
                
                var (parsedSymbols, priorStates) = parsingState;
                return parsingState with
                {
                    ParsedSymbols = symbol.Type switch
                    {
                        SymbolType.Symbol => parsedSymbols.Shift(symbol.Value.ValueOrFailure()),
                        _ => parsedSymbols
                    },
                    PriorStates = priorStates.Push(nextState),
                };
            };
        }

        public Reduce<Symbol<TSymbol>, ParsingState<TSymbol>> Reduce(Symbol<TSymbol> symbol, Item<TSymbol> reducedItem)
        {
            return automatonState =>
            {
                var (parsingState, _, emitNext) = automatonState;

                var reducedToSymbol = reducedItem.ForSymbol;
                var (parsedSymbols, priorStates) = parsingState;

                var (newParsedSymbols, reducedParsedSymbol) = symbol.Type switch
                {
                    SymbolType.Symbol => parsedSymbols
                        .Shift(symbol.Value.ValueOrFailure())
                        .Reduce(reducedItem), 
                    _ => parsedSymbols.Reduce(reducedItem)
                };
                
                var reducedPriorStates = priorStates.PopSkip(reducedItem.Count);
                var goToState = reducedPriorStates.Peek();

                _observer.ReduceListener(parsingState, symbol, reducedItem, goToState.Id);

                var reducedParsingState = parsingState with
                {
                    ParsedSymbols = newParsedSymbols,
                    PriorStates = reducedPriorStates
                };

                return new(reducedParsingState, goToState.Some());
            };
        }

        public ReduceValue<Symbol<TSymbol>, ParsingState<TSymbol>> Accept(Symbol<TSymbol> symbol)
        {
            return automatonState =>
            {
                var (parsingState, nextStateOption, _) = automatonState;
                var nextState = nextStateOption.ValueOrFailure();
                
                _observer.AcceptListener(parsingState, nextState.Id);

                var (parsedSymbols, _) = parsingState;
                return parsingState with
                {
                    ParsedSymbols = symbol.Type switch
                    {
                        SymbolType.Symbol => parsedSymbols.Shift(symbol.Value.ValueOrFailure()),
                        _ => parsedSymbols
                    }
                };
            };
        }
    }
}