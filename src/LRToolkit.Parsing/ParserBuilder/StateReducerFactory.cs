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

            var lookaheadCount = reducedItem.Lookahead.Count;
            var (parsingStack, priorStates) = parsingState;

            Symbol<TSymbol> reducedToSymbol = null!;
            IReadOnlyCollection<Symbol<TSymbol>> poppedLookaheads = null!;
            var resultParsingStack = parsingStack
                .Pipe(stack => lookaheadCount == 0 ? stack.Shift(symbol) : stack)
                .Pipe(stack => stack.Reduce(reducedItem, out reducedToSymbol, out poppedLookaheads));
            
            var resultPriorStates = priorStates
                .PopSkip(reducedItem.Count - 1)
                .Peek(out var goToState);

            _observer.ReduceListener(parsingState, symbol, reducedItem, goToState.Id);

            var resultParsingState = parsingState with
            {
                ParsingStack = resultParsingStack,
                PriorStates = resultPriorStates
            };

            emitNext(reducedToSymbol);
            poppedLookaheads.ForEach(emitNext);

            if (lookaheadCount > 0)
                emitNext(symbol);

            return new(resultParsingState, goToState.Some());
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