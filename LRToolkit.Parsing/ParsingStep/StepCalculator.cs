using System.Collections.Immutable;
using Optional;
using Optional.Unsafe;

namespace LRToolkit.Parsing
{
    internal class StepCalculator<TSymbol> where TSymbol : notnull
    {
        private readonly ClosureItemSetGenerator<TSymbol> _closureGenerator;
        private readonly ParserTransitionsObserver<TSymbol> _observer;

        public StepCalculator(
            ClosureItemSetGenerator<TSymbol> closureGenerator,
            ParserTransitionsObserver<TSymbol> observer)
        {
            _closureGenerator = closureGenerator;
            _observer = observer;
        }

        public Option<Step<TSymbol>, BuilderError> GetForSymbolAhead(
            ItemSet<TSymbol> beforeSymbolItemSet,
            Symbol<TSymbol> symbolAhead)
        {
            var afterSymbolItemSets = GetItemSetsAfterSymbol(beforeSymbolItemSet, symbolAhead);

            var shiftReduceItems = afterSymbolItemSets.Kernel.Aggregate(
                ShiftReduceItems.Empty,
                (shiftReduce, item) => item.HasSymbolAhead() 
                    ? shiftReduce with { ShiftItems = shiftReduce.ShiftItems.Add(item) }
                    : shiftReduce with { ReduceItems = shiftReduce.ReduceItems.Add(item) });

            var (shiftItems, reduceItems) = shiftReduceItems;
            
            var validationResult = Validate(shiftItems, reduceItems);
            if (validationResult.HasValue)
                return Option.None<Step<TSymbol>, BuilderError>(validationResult.ValueOrFailure());

            return CalculateStep(symbolAhead, afterSymbolItemSets.Full, shiftReduceItems)
                .Some<Step<TSymbol>, BuilderError>();
        }

        private Step<TSymbol> CalculateStep(
            Symbol<TSymbol> symbolAhead,
            ItemSet<TSymbol> afterSymbolFullItemSet,
            ShiftReduceItems shiftReduceItems)
        {
            var (shiftItems, reduceItems) = shiftReduceItems;
            var isShift = shiftItems.Any();

            if (isShift)
                return CreateShift(symbolAhead, afterSymbolFullItemSet);

            var productionItem = reduceItems[0];
            return productionItem.IsRoot
                ? CreateAccept(symbolAhead, afterSymbolFullItemSet)
                : CreateReduce(symbolAhead, afterSymbolFullItemSet, productionItem);
        }

        private Step<TSymbol> CreateShift(Symbol<TSymbol> symbolAhead, ItemSet<TSymbol> afterSymbolFullItemSet)
        {
            var shift = StateReducerFactory.Shift(symbolAhead, afterSymbolFullItemSet, _observer.ShiftListener);

            return Step<TSymbol>.CreateShiftStep(symbolAhead, shift, afterSymbolFullItemSet);
        }

        private Step<TSymbol> CreateReduce(
            Symbol<TSymbol> symbolAhead,
            ItemSet<TSymbol> afterSymbolFullItemSet,
            Item<TSymbol> reducedItem)
        {
            var reduce = StateReducerFactory.Reduce(symbolAhead, afterSymbolFullItemSet, reducedItem, _observer.ReduceListener);
            
            return Step<TSymbol>.CreateReduceStep(symbolAhead, reduce, afterSymbolFullItemSet, reducedItem);
        }

        private Step<TSymbol> CreateAccept(Symbol<TSymbol> symbolAhead, ItemSet<TSymbol> afterSymbolFullItemSet)
        {
            var accept = StateReducerFactory.Accept(symbolAhead, afterSymbolFullItemSet, _observer.AcceptListener);

            return Step<TSymbol>.CreateAcceptStep(symbolAhead, accept, afterSymbolFullItemSet);
        }

        private KernelFullItemSets<TSymbol> GetItemSetsAfterSymbol(ItemSet<TSymbol> stateItemSet, Symbol<TSymbol> symbolAhead)
        {
            var kernelItemSet = stateItemSet.StepForward(symbolAhead);
            var closureItemSet = _closureGenerator.GetClosureItems(kernelItemSet);
            var fullItemSet = kernelItemSet.Include(closureItemSet);

            return new KernelFullItemSets<TSymbol>(kernelItemSet, fullItemSet);
        }

        private static Option<BuilderError> Validate(
            ImmutableList<Item<TSymbol>> shiftItems,
            ImmutableList<Item<TSymbol>> reduceItems)
        {
            if (shiftItems.Any() && reduceItems.Any())
                return BuilderError.ShiftReduceConflict.Some();

            var reducedSymbols = reduceItems.Select(item => item.ForSymbol).Distinct();
            if (reducedSymbols.Count() > 1)
                return BuilderError.ReduceConflict.Some();

            return Option.None<BuilderError>();
        }

        private readonly record struct ShiftReduceItems(
            ImmutableList<Item<TSymbol>> ShiftItems,
            ImmutableList<Item<TSymbol>> ReduceItems)
        {
            public static readonly ShiftReduceItems Empty = new ShiftReduceItems(
                ImmutableList<Item<TSymbol>>.Empty,
                ImmutableList<Item<TSymbol>>.Empty);
        }
    }
}