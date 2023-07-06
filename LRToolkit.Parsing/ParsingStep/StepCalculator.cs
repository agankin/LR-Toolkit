using LRToolkit.Utilities;
using Optional;

namespace LRToolkit.Parsing
{
    internal class StepCalculator<TSymbol> where TSymbol : notnull
    {
        private readonly ClosureProducer<TSymbol> _closureGenerator;
        private readonly ParserTransitionsObserver<TSymbol> _observer;

        public StepCalculator(
            ClosureProducer<TSymbol> closureGenerator,
            ParserTransitionsObserver<TSymbol> observer)
        {
            _closureGenerator = closureGenerator;
            _observer = observer;
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

        private Step<TSymbol> CreateShift(Symbol<TSymbol> symbol, ItemSet<TSymbol> itemSet)
        {
            var shift = StateReducerFactory.Shift(symbol, itemSet, _observer.ShiftListener);

            return new Step<TSymbol>(StepType.Shift, symbol, shift, itemSet);
        }

        private Step<TSymbol> CreateReduce(Symbol<TSymbol> symbol, StepItemSet<TSymbol> stepItemSet)
        {
            var reduce = StateReducerFactory.Reduce(symbol, stepItemSet.ItemSet, stepItemSet.ReduceItems[0], _observer.ReduceListener);
            
            return new Step<TSymbol>(StepType.Reduce, symbol, reduce, stepItemSet.ItemSet);
        }

        private Step<TSymbol> CreateAccept(Symbol<TSymbol> symbol, ItemSet<TSymbol> nextItemSet)
        {
            var accept = StateReducerFactory.Accept(symbol, nextItemSet, _observer.AcceptListener);

            return new Step<TSymbol>(StepType.Accept, symbol, accept, nextItemSet);
        }

        private (ItemSet<TSymbol> Kernel, ItemSet<TSymbol> Full) GetNextItemSets(ItemSet<TSymbol> itemSet, Symbol<TSymbol> symbol)
        {
            var kernelItemSet = itemSet.StepForward(symbol);
            var closureItemSet = _closureGenerator.Produce(kernelItemSet);
            var fullItemSet = kernelItemSet.Include(closureItemSet);

            return (kernelItemSet, fullItemSet);
        }

        private static Option<VoidValue, BuilderError> Validate(StepItemSet<TSymbol> stepItemSet)
        {
            var (shiftItems, reduceItems) = (stepItemSet.ShiftItems, stepItemSet.ReduceItems);

            if (shiftItems.Any() && reduceItems.Any())
                return Option.None<VoidValue, BuilderError>(BuilderError.ShiftReduceConflict);

            var reducedSymbols = reduceItems.Select(item => item.ForSymbol).Distinct();
            if (reducedSymbols.Count() > 1)
                return Option.None<VoidValue, BuilderError>(BuilderError.ReduceConflict);

            return VoidValue.Instance.Some<VoidValue, BuilderError>();
        }
    }
}