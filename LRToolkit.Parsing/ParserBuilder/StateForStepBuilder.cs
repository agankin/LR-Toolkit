using LRToolkit.Utilities;
using Optional;

namespace LRToolkit.Parsing
{
    internal delegate Option<BuilderError> BuildForNextSymbolsAhead<TSymbol>(
        State<TSymbol> fromState,
        StatesLog<TSymbol> statesLog)
        where TSymbol : notnull;

    internal class StateForStepBuilder<TSymbol> where TSymbol : notnull
    {
        private readonly StateForItemSet<TSymbol> _stateForItemSet = new();
        private readonly BuildForNextSymbolsAhead<TSymbol> _buildForNextSymbolsAhead;
        private readonly ParserTransitionsObserver<TSymbol> _observer;

        public StateForStepBuilder(
            BuildForNextSymbolsAhead<TSymbol> buildForNextSymbolsAhead,
            ParserTransitionsObserver<TSymbol> observer)
        {
            _buildForNextSymbolsAhead = buildForNextSymbolsAhead;
            _observer = observer;
        }

        public Option<BuilderError> BuildStepState(
            State<TSymbol> fromState,
            Step<TSymbol> step,
            StatesLog<TSymbol> statesLog)
        {
            var toStateItemSet = step.AfterSymbolFullItemSet;
            var existingToState = _stateForItemSet[toStateItemSet];

            return step.Map(
                mapShift: _ => BuildShiftReduceStepState(fromState, step, statesLog),
                mapReduce: _ => BuildShiftReduceStepState(fromState, step, statesLog),
                mapAccept: acceptState => BuildAcceptedStepState(fromState, acceptState));
        }

        private Option<BuilderError> BuildShiftReduceStepState(
            State<TSymbol> fromState,
            Step<TSymbol> step,
            StatesLog<TSymbol> statesLog)
        {
            var afterSymbolFullItemSet = step.AfterSymbolFullItemSet;
            var existingToState = _stateForItemSet[afterSymbolFullItemSet];

            return existingToState.Match(
                toState => LinkExistingForShiftOrReduce(fromState, step, toState, statesLog),
                () => CreateNewForShiftOrReduce(fromState, step, statesLog));
        }

        private Option<BuilderError> BuildAcceptedStepState(State<TSymbol> fromState, Step<TSymbol>.Accept step)
        {
            var symbolAhead = step.SymbolAhead;
            var afterSymbolFullItemSet = step.AfterSymbolFullItemSet;

            var toState = fromState.ToNewAccepted(
                symbolAhead,
                (runState, parserState) => step.ParserStateReducer(runState, parserState));
            toState.Tag = afterSymbolFullItemSet;

            return Option.None<BuilderError>();
        }

        private Option<BuilderError> CreateNewForShiftOrReduce(
            State<TSymbol> fromState,
            Step<TSymbol> step,
            StatesLog<TSymbol> statesLog)
        {
            var symbolAhead = step.SymbolAhead;
            var afterSymbolFullItemSet = step.AfterSymbolFullItemSet;

            var toState = fromState.ToNewState(
                symbolAhead,
                afterSymbolFullItemSet,
                (runState, parserState) => step.ParserStateReducer(runState, parserState));
            toState.Tag = afterSymbolFullItemSet;
            
            _stateForItemSet.Add(toState, afterSymbolFullItemSet);
            statesLog = statesLog.Push(toState);

            step.Map(
                mapShift: _ => new VoidValue(),
                mapReduce: AddGoToAfterReduce(toState, statesLog),
                mapAccept: _ => new VoidValue());
            
            return _buildForNextSymbolsAhead(toState, statesLog);
        }

        private Option<BuilderError> LinkExistingForShiftOrReduce(
            State<TSymbol> fromState,
            Step<TSymbol> step,
            State<TSymbol> toState,
            StatesLog<TSymbol> statesLog)
        {
            var symbolAhead = step.SymbolAhead;
            fromState.LinkState(
                symbolAhead,
                toState,
                (runState, parserState) => step.ParserStateReducer(runState, parserState));
            
            var hasVisited = statesLog.Contains(toState);
            statesLog = statesLog.Push(toState);

            step.Map(
                mapShift: _ => new VoidValue(),
                mapReduce: AddGoToAfterReduce(toState, statesLog),
                mapAccept: _ => new VoidValue());

            return hasVisited
                ? Option.None<BuilderError>()
                : _buildForNextSymbolsAhead(toState, statesLog);
        }

        private Func<Step<TSymbol>.Reduce, VoidValue> AddGoToAfterReduce(
            State<TSymbol> reduceStepState,
            StatesLog<TSymbol> statesLog)
        {
            return reduceStep =>
            {
                var reducedItem = reduceStep.ReducedItem;
                var afterReduceState = GetAfterReduceState(reducedItem, statesLog);

                var reducedToSymbol = reducedItem.ForSymbol;
                var emitedNext = Symbol<TSymbol>.Create(reducedToSymbol, afterReduceState.FullItemSet);
                var goToAfterReduce = StateReducerFactory.GoToAfterReduce(
                    reducedToSymbol,
                    _observer.GoToAfterReduceListener);
                
                reduceStepState.LinkState(
                    emitedNext,
                    afterReduceState,
                    (runState, state) => goToAfterReduce(runState, state));

                return new VoidValue();
            };
        }

        private State<TSymbol> GetAfterReduceState(
            Item<TSymbol> reducedItem,
            StatesLog<TSymbol> statesLog)
        {
            var production = reducedItem.Production;
            return statesLog[production.Count];
        }
    }
}