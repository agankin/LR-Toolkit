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

    public Option<Step<TSymbol>, BuilderError> GetStep(ItemSet<TSymbol> itemSet, Symbol<TSymbol> symbol)
    {
        var (kernelItemSet, fullItemSet) = GetNextItemSets(itemSet, symbol);
        var stepItemSet = new StepItemSet<TSymbol>(kernelItemSet, fullItemSet);

        return Validate(stepItemSet).Map(_ => CalculateStep(symbol, stepItemSet));
    }

    private Step<TSymbol> CalculateStep(Symbol<TSymbol> symbol, StepItemSet<TSymbol> stepItemSet)
    {            
        var isShift = stepItemSet.ShiftItems.Any();
        if (isShift)
            return CreateShift(symbol, stepItemSet.ItemSet);

        return stepItemSet.ReduceItems[0].IsStart
            ? CreateAccept(symbol, stepItemSet.ItemSet)
            : CreateReduce(symbol, stepItemSet);
    }

    private Step<TSymbol> CreateShift(Symbol<TSymbol> symbol, ItemSet<TSymbol> itemSet) =>
        new Step<TSymbol>(StepType.Shift, symbol, itemSet, Option.None<Item<TSymbol>>());

    private Step<TSymbol> CreateReduce(Symbol<TSymbol> symbol, StepItemSet<TSymbol> stepItemSet)
    {
        var reducedItem = stepItemSet.ReduceItems[0];            
        return new Step<TSymbol>(StepType.Reduce, symbol, stepItemSet.ItemSet, reducedItem.Some());
    }

    private Step<TSymbol> CreateAccept(Symbol<TSymbol> symbol, ItemSet<TSymbol> nextItemSet) =>
        new Step<TSymbol>(StepType.Accept, symbol, nextItemSet, Option.None<Item<TSymbol>>());

    private (ItemSet<TSymbol> Kernel, ItemSet<TSymbol> Full) GetNextItemSets(ItemSet<TSymbol> itemSet, Symbol<TSymbol> symbol)
    {
        var kernelItemSet = itemSet.StepForward(symbol);
        var closureItemSet = _closureProducer.Produce(kernelItemSet);
        var fullItemSet = kernelItemSet.Include(closureItemSet);

        return (kernelItemSet, fullItemSet);
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