﻿using LRToolkit.Utilities;
using Optional;
using Optional.Unsafe;

namespace LRToolkit.Parsing;

internal delegate Option<BuilderError> BuildNext<TSymbol>(State<TSymbol> fromState) where TSymbol : notnull;

internal class StateForStepBuilder<TSymbol> where TSymbol : notnull
{
    private readonly BuildNext<TSymbol> _buildNext;
    private readonly StateReducerFactory<TSymbol> _reducerFactory;

    public StateForStepBuilder(BuildNext<TSymbol> buildNext, ParserTransitionsObserver<TSymbol> observer)
    {
        _buildNext = buildNext;
        _reducerFactory = new(observer);
    }

    public Option<BuilderError> BuildNext(State<TSymbol> state, Step<TSymbol> step) =>
        step.Type switch
        {
            StepType.Shift => BuildShift(state, step),
            StepType.Reduce => BuildReduce(state, step),
            StepType.Accept => BuildAccepted(state, step),
            _ => throw new UnsupportedEnumValueException<StepType>(step.Type)
        };

    private Option<BuilderError> BuildShift(State<TSymbol> state, Step<TSymbol> step)
    {
        var symbol = step.Symbol;
        var nextItemSet = step.NextItemSet;

        var reduce = _reducerFactory.Shift(symbol);
        var existingToState = state.StateItemSetDict[nextItemSet];

        return existingToState.Match(
            toState => 
            {
                state.LinkFixedState(symbol, toState, reduce);
                return Option.None<BuilderError>();
            },
            () =>
            {
                var nextState = state.ToNewFixedState(symbol, nextItemSet, reduce);
                nextState.Tag = nextItemSet;

                state.StateItemSetDict.Add(nextState, nextItemSet);
                
                return _buildNext(nextState);
            });
    }

    private Option<BuilderError> BuildReduce(State<TSymbol> state, Step<TSymbol> step)
    {
        var symbol = step.Symbol;
        var nextItemSet = step.NextItemSet;
        var reducedItem = step.ReducedItem.ValueOrFailure();

        var reduce = _reducerFactory.Reduce(symbol, reducedItem);
        var existingToState = state.StateItemSetDict[nextItemSet];

        state.LinkDynamicState(symbol, reduce);
        return Option.None<BuilderError>();
    }

    private Option<BuilderError> BuildAccepted(State<TSymbol> state, Step<TSymbol> step)
    {
        var symbol = step.Symbol;
        var nextItemSet = step.NextItemSet;

        var reduce = _reducerFactory.Accept(symbol);
        var nextState = state.ToNewFixedAccepted(step.Symbol, reduce);
        nextState.Tag = step.NextItemSet;

        return Option.None<BuilderError>();
    }
}