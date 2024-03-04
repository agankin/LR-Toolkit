using LRToolkit.Utilities;
using Optional;
using Optional.Unsafe;

namespace LRToolkit.Parsing;

internal delegate Option<BuilderError> BuildNext<TSymbol>(State<TSymbol> fromState) where TSymbol : notnull;

internal class StateForStepBuilder<TSymbol> where TSymbol : notnull
{
    private readonly BuildNext<TSymbol> _buildNext;
    private readonly IItemSetMerger<TSymbol> _itemSetMerger;
    private readonly StateReducerFactory<TSymbol> _reducerFactory;

    public StateForStepBuilder(
        BuildNext<TSymbol> buildNext,
        IItemSetMerger<TSymbol> itemSetMerger,
        ParserTransitionsObserver<TSymbol> observer)
    {
        _buildNext = buildNext;
        _itemSetMerger = itemSetMerger;
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
        var symbolAhead = step.SymbolAhead;
        var nextItemSet = step.NextItemSet;

        var reduce = _reducerFactory.Shift();
        var existingToState = state.MergeableStateDict.GetMergeableOrNone(nextItemSet);

        return existingToState.Match(
            toState => 
            {
                state.TransitsToExisting(symbolAhead, toState, reduce);
                var errorOrNone = _itemSetMerger.Merge(toState.FullItemSet, nextItemSet)
                    .Map(mergedItemSet => toState.FullItemSet = mergedItemSet)
                    .Match(_ => Option.None<BuilderError>(), error => error.Some());
                
                return errorOrNone;
            },
            () =>
            {
                var nextState = state.TransitsToNew(symbolAhead, nextItemSet, reduce);
                state.MergeableStateDict.AddNew(nextState, nextItemSet);
                
                return _buildNext(nextState);
            });
    }

    private Option<BuilderError> BuildReduce(State<TSymbol> state, Step<TSymbol> step)
    {
        var reducedItem = step.ReducedItem.ValueOrFailure();
        var reduce = _reducerFactory.Reduce(reducedItem);
        var existingToState = state.MergeableStateDict.GetMergeableOrNone(step.NextItemSet);

        state.TransitsDynamicly(step.SymbolAhead, reduce);
        return Option.None<BuilderError>();
    }

    private Option<BuilderError> BuildAccepted(State<TSymbol> state, Step<TSymbol> step)
    {
        var reduce = _reducerFactory.Accept();
        var nextState = state.ToNewFixedAccepted(step.SymbolAhead, reduce);
        nextState.Tag = step.NextItemSet;

        return Option.None<BuilderError>();
    }
}