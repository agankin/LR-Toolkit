using DFAutomaton;
using LRToolkit.Utilities;
using PureMonads;

namespace LRToolkit.Parsing;

internal class StateReducerFactory<TSymbol> where TSymbol : notnull
{
    private readonly ParserTransitionsObserver<TSymbol> _observer;

    public StateReducerFactory(ParserTransitionsObserver<TSymbol> observer) => _observer = observer;

    public Reduce<Symbol<TSymbol>, ParsingState<TSymbol>> Shift()
    {
        return (parsingState, symbol) =>
        {
            var nextStateOption = Automaton<Symbol<TSymbol>, ParsingState<TSymbol>>.CurrentTransition.Value.TransitsTo;
            var nextState = nextStateOption.ValueOrFailure();

            _observer.ShiftListener(parsingState, symbol, nextState.Id);
            
            var (parsingStack, priorStateIds) = parsingState;
            return parsingState with
            {
                ParsingStack = symbol.Type switch
                {
                    SymbolType.Symbol => parsingStack.Shift(symbol),
                    _ => parsingStack
                },
                PriorStateIds = priorStateIds.Push(nextState.Id),
            };
        };
    }

    public Reduce<Symbol<TSymbol>, ParsingState<TSymbol>> Reduce(Item<TSymbol> reducedItem)
    {
        return (parsingState, symbol) =>
        {
            var lookaheadCount = reducedItem.Lookahead.Count;
            var (parsingStack, priorStateIds) = parsingState;

            Symbol<TSymbol> reducedToSymbol = null!;
            IReadOnlyCollection<Symbol<TSymbol>> poppedLookaheads = null!;
            var resultParsingStack = parsingStack
                .Pipe(stack => lookaheadCount == 0 ? stack.Shift(symbol) : stack)
                .Pipe(stack => stack.Reduce(reducedItem, out reducedToSymbol, out poppedLookaheads));
            
            var resultPriorStateIds = priorStateIds
                .PopSkip(reducedItem.Count - 1)
                .Peek(out var goToStateId);

            _observer.ReduceListener(parsingState, symbol, reducedItem, goToStateId);

            var resultParsingState = parsingState with
            {
                ParsingStack = resultParsingStack,
                PriorStateIds = resultPriorStateIds
            };

            var currentTransition = Automaton<Symbol<TSymbol>, ParsingState<TSymbol>>.CurrentTransition.Value;
            currentTransition.YieldNext(reducedToSymbol);
            poppedLookaheads.ForEach(currentTransition.YieldNext);

            if (lookaheadCount > 0)
                currentTransition.YieldNext(symbol);

            currentTransition.DynamiclyGoTo(goToStateId.Some());

            return resultParsingState;
        };
    }

    public Reduce<Symbol<TSymbol>, ParsingState<TSymbol>> Accept()
    {
        return (parsingState, symbol) =>
        {
            var nextStateOption = Automaton<Symbol<TSymbol>, ParsingState<TSymbol>>.CurrentTransition.Value.TransitsTo;
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