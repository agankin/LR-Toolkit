﻿using LRToolkit.GrammarDefinition;
using DFAutomaton;
using Optional;
using Optional.Unsafe;

namespace LRToolkit.Parsing
{
    public class ParserBuilder<TSymbol> where TSymbol : notnull
    {
        private readonly Grammar<TSymbol> _grammar;
        private readonly ILookaheadFactory<TSymbol> _lookaheadFactory;
        private readonly ParserTransitionsObserver<TSymbol> _observer;

        private readonly ClosureItemSetGenerator<TSymbol> _closureGenerator;
        private readonly StepCalculator<TSymbol> _stepCalculator;
        private readonly StateForStepBuilder<TSymbol> _stateForStepBuilder;

        private ParserBuilder(
            Grammar<TSymbol> grammar,
            ILookaheadFactory<TSymbol> lookaheadFactory,
            ParserTransitionsObserver<TSymbol> observer)
        {
            _grammar = grammar;
            _lookaheadFactory = lookaheadFactory;
            _observer = observer;

            _closureGenerator = new ClosureItemSetGenerator<TSymbol>(grammar, lookaheadFactory);
            
            _stepCalculator = new StepCalculator<TSymbol>(_closureGenerator, _observer);
            _stateForStepBuilder = new StateForStepBuilder<TSymbol>(BuildForSymbolsAhead, _observer);
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

            var statesLog = StatesLog<TSymbol>.Empty.Push(startState);
            var errorOption = BuildForSymbolsAhead(startState, statesLog);

            return errorOption.Map(Option.None<Parser<TSymbol>, BuilderError>)
                .ValueOr(() =>
                {
                    var automaton = automatonBuilder.Build();

                    return new Parser<TSymbol>(automaton.ValueOrFailure(), startItemSet)
                        .Some<Parser<TSymbol>, BuilderError>();
                });
        }

        private Option<BuilderError> BuildForSymbolsAhead(
            State<TSymbol> fromState,
            StatesLog<TSymbol> statesLog)
        {
            var symbolsAhead = fromState.FullItemSet.GetSymbolsAhead();

            return symbolsAhead.Aggregate(
                Option.None<BuilderError>(),
                (errorOption, symbolAhead) => errorOption.Match(
                    error => error.Some(),
                    () => BuildForSymbolAhead(fromState, symbolAhead, statesLog)));
        }

        private Option<BuilderError> BuildForSymbolAhead(
            State<TSymbol> fromState,
            Symbol<TSymbol> symbolAhead,
            StatesLog<TSymbol> statesLog)
        {
            return _stepCalculator.GetForSymbolAhead(fromState.FullItemSet, symbolAhead)
                .Match(
                    step => _stateForStepBuilder.BuildStepState(fromState, step, statesLog),
                    Option.Some);
        }

        private ItemSet<TSymbol> CreateStartItemSet()
        {
            var start = _grammar.Start;
            var startLookahead = NoLookahead<TSymbol>.Instance;

            var kernelItem = Item<TSymbol>.ForRoot(start, startLookahead);
            var closureItems = _closureGenerator.GetClosureItems(kernelItem);
            var fullItemSet = kernelItem.Include(closureItems);

            return fullItemSet;
        }
    }
}