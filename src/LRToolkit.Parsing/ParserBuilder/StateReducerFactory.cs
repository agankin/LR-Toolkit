using DFAutomaton;
using LRToolkit.Utilities;
using Optional;
using Optional.Unsafe;

namespace LRToolkit.Parsing;

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
            
            var (parsingStack, priorStates) = parsingState;
            return parsingState with
            {
                ParsingStack = symbol.Type switch
                {
                    SymbolType.Symbol => parsingStack.Shift(symbol),
                    _ => parsingStack
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
            var (parsingStack, priorStates) = parsingState;

            var (nextParsingStack, node) = symbol.Type switch
            {
                SymbolType.Symbol => parsingStack
                    .Shift(symbol)
                    .Reduce(reducedItem), 
                _ => parsingStack.Reduce(reducedItem)
            };
            
            var nextPriorStates = priorStates.PopSkip(reducedItem.Count - 1);
            var goToState = nextPriorStates.Peek();

            _observer.ReduceListener(parsingState, symbol, reducedItem, goToState.Id);

            var nextParsingState = parsingState with
            {
                ParsingStack = nextParsingStack,
                PriorStates = nextPriorStates
            };

            emitNext(Symbol<TSymbol>.Create(reducedToSymbol));
            foreach (var symbol in reducedItem.Lookahead)
                emitNext(symbol);

            return new(nextParsingState, goToState.Some());
        };
    }

    public ReduceValue<Symbol<TSymbol>, ParsingState<TSymbol>> Accept(Symbol<TSymbol> symbol)
    {
        return automatonState =>
        {
            var (parsingState, nextStateOption, _) = automatonState;
            var nextState = nextStateOption.ValueOrFailure();
            
            _observer.AcceptListener(parsingState, nextState.Id);

            var (parsingStack, _) = parsingState;
            return parsingState with
            {
                ParsingStack = symbol.Type switch
                {
                    SymbolType.Symbol => parsingStack.Shift(symbol),
                    _ => parsingStack
                }
            };
        };
    }
}