﻿using LRToolkit.GrammarDefinition;
using DFAutomaton;
using Optional;
using Optional.Unsafe;
using LRToolkit.Utilities;

namespace LRToolkit.Parsing
{
    public class ParserBuilder<TSymbol> where TSymbol : notnull
    {
        private readonly Grammar<TSymbol> _grammar;
        private readonly ILookaheadFactory<TSymbol> _lookaheadFactory;
        private readonly ParserTransitionsObserver<TSymbol> _observer;

        private readonly ClosureProducer<TSymbol> _closureProducer;
        private readonly StepCalculator<TSymbol> _stepCalculator;
        private readonly StateForStepBuilder<TSymbol> _stateBuilder;

        private ParserBuilder(Grammar<TSymbol> grammar, ILookaheadFactory<TSymbol> lookaheadFactory, ParserTransitionsObserver<TSymbol> observer)
        {
            _grammar = grammar;
            _lookaheadFactory = lookaheadFactory;
            _observer = observer;

            _closureProducer = new ClosureProducer<TSymbol>(grammar, lookaheadFactory);
            
            _stepCalculator = new StepCalculator<TSymbol>(_closureProducer, _observer);
            _stateBuilder = new StateForStepBuilder<TSymbol>(BuildNext, _observer);
        }

        public static Option<Parser<TSymbol>, BuilderError> Build(
            Grammar<TSymbol> grammar,
            ILookaheadFactory<TSymbol> lookaheadFactory,
            Func<ParserTransitionsObserver<TSymbol>, ParserTransitionsObserver<TSymbol>>? configureObserver = null)
        {
            var emptyObserver = ParserTransitionsObserver<TSymbol>.Create();
            var observer = configureObserver?.Invoke(emptyObserver) ?? emptyObserver;

            var builder = new ParserBuilder<TSymbol>(grammar, lookaheadFactory, observer);

            return builder.Build();
        }

        private Option<Parser<TSymbol>, BuilderError> Build()
        {
            var automatonBuilder = AutomatonBuilder<Symbol<TSymbol>, ParsingState<TSymbol>>.Create();
            var startItemSet = CreateStartItemSet();

            var startState = new State<TSymbol>(automatonBuilder.StartState, startItemSet);
            startState.Tag = startItemSet;

            return BuildNext(startState)
                .Map(_ => 
                {
                    var automaton = automatonBuilder.Build(c => c.TurnOffAnyReachesAcceptedValidation()).ValueOrFailure();
                    
                    return new Parser<TSymbol>(automaton, startItemSet);
                });
        }

        private Option<VoidValue, BuilderError> BuildNext(State<TSymbol> state)
        {
            var symbols = state.FullItemSet.GetSymbolsAhead();

            return symbols.Aggregate(
                VoidValue.Instance.Some<VoidValue, BuilderError>(),
                (error, symbol) => error.FlatMap(_ => BuildNext(state, symbol)));
        }

        private Option<VoidValue, BuilderError> BuildNext(State<TSymbol> state, Symbol<TSymbol> symbol)
        {
            return _stepCalculator.GetStep(state.FullItemSet, symbol)
                .FlatMap(step => _stateBuilder.BuildNext(state, step));
        }

        private ItemSet<TSymbol> CreateStartItemSet()
        {
            var start = _grammar.Start;
            var startLookahead = _lookaheadFactory.GetStart();

            var kernelItem = Item<TSymbol>.ForStart(start, startLookahead);
            var closureItems = _closureProducer.Produce(kernelItem);
            var fullItemSet = kernelItem.Include(closureItems);

            return fullItemSet;
        }
    }
}