using LRToolkit.Utilities;
using Optional;

namespace LRToolkit.Parsing;

internal class StepCalculator<TSymbol> where TSymbol : notnull
{
    private readonly ClosureProducer<TSymbol> _closureProducer;

    public StepCalculator(ClosureProducer<TSymbol> closureProducer)
    {
        _closureProducer = closureProducer;
    }

    public Option<Step<TSymbol>, BuilderError> GetStep(ItemSet<TSymbol> itemSet, Symbol<TSymbol> symbolAhead)
    {
        var afterStepForward = GetStepForwardItemSet(itemSet, symbolAhead);
        var stepItemSet = new StepItemSet<TSymbol>(afterStepForward);

        return Validate(stepItemSet).Map(_ => CalculateStep(symbolAhead, stepItemSet));
    }

    private Step<TSymbol> CalculateStep(Symbol<TSymbol> symbolAhead, StepItemSet<TSymbol> stepItemSet)
    {            
        var isShift = stepItemSet.ShiftItems.Any();
        if (isShift)
            return CreateShift(symbolAhead, stepItemSet.ItemSet);

        return stepItemSet.ReduceItems[0].IsStart
            ? CreateAccept(symbolAhead, stepItemSet.ItemSet)
            : CreateReduce(symbolAhead, stepItemSet);
    }

    private Step<TSymbol> CreateShift(Symbol<TSymbol> symbolAhead, ItemSet<TSymbol> itemSet) =>
        new Step<TSymbol>(StepType.Shift, symbolAhead, itemSet, Option.None<Item<TSymbol>>());

    private Step<TSymbol> CreateReduce(Symbol<TSymbol> symbolAhead, StepItemSet<TSymbol> stepItemSet)
    {
        var reducedItem = stepItemSet.ReduceItems[0];            
        return new Step<TSymbol>(StepType.Reduce, symbolAhead, stepItemSet.ItemSet, reducedItem.Some());
    }

    private Step<TSymbol> CreateAccept(Symbol<TSymbol> symbolAhead, ItemSet<TSymbol> nextItemSet) =>
        new Step<TSymbol>(StepType.Accept, symbolAhead, nextItemSet, Option.None<Item<TSymbol>>());

    private ItemSet<TSymbol> GetStepForwardItemSet(ItemSet<TSymbol> itemSet, Symbol<TSymbol> symbolAhead)
    {
        var kernels = itemSet.StepForward(symbolAhead);
        var closures = _closureProducer.Produce(kernels);
        
        var next = new ItemSet<TSymbol>(kernels, closures);

        return next;
    }

    private static Option<Nothing, BuilderError> Validate(StepItemSet<TSymbol> stepItemSet)
    {
        var (shiftItems, reduceItems) = (stepItemSet.ShiftItems, stepItemSet.ReduceItems);

        if (shiftItems.Any() && reduceItems.Any())
            return Option.None<Nothing, BuilderError>(BuilderError.ShiftReduceConflict);

        var reducedSymbols = reduceItems.Select(item => item.ForSymbol).Distinct();
        if (reducedSymbols.Count() > 1)
            return Option.None<Nothing, BuilderError>(BuilderError.ReduceConflict);

        return new Nothing().Some<Nothing, BuilderError>();
    }
}