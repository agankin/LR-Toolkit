using LRToolkit.Utilities;
using Optional;

namespace LRToolkit.Parsing
{
    internal delegate Option<VoidValue, BuilderError> BuildNext<TSymbol>(State<TSymbol> fromState) where TSymbol : notnull;

    internal class StateForStepBuilder<TSymbol> where TSymbol : notnull
    {
        private readonly StateByItemSet<TSymbol> _stateByItemSet = new();
        private readonly BuildNext<TSymbol> _buildNext;
        private readonly ParserTransitionsObserver<TSymbol> _observer;

        public StateForStepBuilder(BuildNext<TSymbol> buildNext, ParserTransitionsObserver<TSymbol> observer)
        {
            _buildNext = buildNext;
            _observer = observer;
        }

        public Option<VoidValue, BuilderError> BuildNext(State<TSymbol> state, Step<TSymbol> step) =>
            step.Type switch
            {
                StepType.Shift => BuildShiftReduce(state, step),
                StepType.Reduce => BuildShiftReduce(state, step),
                StepType.Accept => BuildAccepted(state, step),
                _ => throw new UnsupportedEnumValueException<StepType>(step.Type)
            };

        private Option<VoidValue, BuilderError> BuildShiftReduce(State<TSymbol> state, Step<TSymbol> step)
        {
            var existingNextState = _stateByItemSet[step.NextItemSet];

            return existingNextState.Match(
                nextState => LinkExisting(state, step, nextState),
                () => CreateNew(state, step));
        }

        private Option<VoidValue, BuilderError> BuildAccepted(State<TSymbol> state, Step<TSymbol> step)
        {
            var nextState = state.ToNewAccepted(step.Symbol, step.Reducer);
            nextState.Tag = step.NextItemSet;

            return VoidValue.Instance.Some<VoidValue, BuilderError>();
        }

        private Option<VoidValue, BuilderError> CreateNew(State<TSymbol> state, Step<TSymbol> step)
        {
            var nextState = state.ToNewState(step.Symbol, step.NextItemSet, step.Reducer);
            nextState.Tag = step.NextItemSet;
            _stateByItemSet.Add(nextState, step.NextItemSet);
            
            return _buildNext(nextState);
        }

        private Option<VoidValue, BuilderError> LinkExisting(State<TSymbol> state, Step<TSymbol> step, State<TSymbol> nextState)
        {
            state.LinkState(step.Symbol, nextState, step.Reducer);

            return VoidValue.Instance.Some<VoidValue, BuilderError>();
        }

        private Func<Step<TSymbol>, VoidValue> AddGoToAfterReduce(State<TSymbol> state)
        {
            return reduceStep =>
            {
                /*
                var reducedItem = reduceStep.ReducedItem;
                var afterReduceState = GetAfterReduceState(reducedItem);

                var reducedToSymbol = reducedItem.ForSymbol;
                var emitedNext = Symbol<TSymbol>.CreateReduced(reducedToSymbol, afterReduceState.FullItemSet);
                var goToAfterReduce = StateReducerFactory.GoToAfterReduce(
                    reducedItem,
                    _observer.GoToAfterReduceListener);
                
                state.LinkState(
                    emitedNext,
                    afterReduceState,
                    (runState, state) => goToAfterReduce(runState, state));
                */

                return VoidValue.Instance;
            };
        }
    }
}