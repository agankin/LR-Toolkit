using LRBee.GrammarDefinition;
using DFAutomaton;
using Optional;
using Optional.Unsafe;

namespace LRBee.Parsing
{
    public class ParserBuilder<TSymbol> where TSymbol : notnull
    {
        private readonly Grammar<TSymbol> _grammar;

        private readonly ClosureItemSetGenerator<TSymbol> _closureGenerator;
        private readonly StepCalculator<TSymbol> _stepCalculator;
        private readonly StateForStepBuilder<TSymbol> _stateForStepBuilder;

        private ParserBuilder(Grammar<TSymbol> grammar)
        {
            _grammar = grammar;

            _closureGenerator = new ClosureItemSetGenerator<TSymbol>(grammar);
            
            _stepCalculator = new StepCalculator<TSymbol>(_closureGenerator);
            _stateForStepBuilder = new StateForStepBuilder<TSymbol>(BuildForSymbolsAhead);
        }

        public static Option<Parser<TSymbol>, BuilderError> Build(Grammar<TSymbol> grammar)
        {
            var builder = new ParserBuilder<TSymbol>(grammar);

            return builder.Build();
        }

        private Option<Parser<TSymbol>, BuilderError> Build()
        {
            var automataBuilder = AutomataBuilder<Symbol<TSymbol>, ParsingState<TSymbol>>.Create();
            var startItemSet = CreateStartItemSet();

            var startState = new State<TSymbol>(automataBuilder.StartState, startItemSet);

            var statesLog = StatesLog<TSymbol>.Empty.Push(startState);
            var errorOption = BuildForSymbolsAhead(startState, statesLog);

            return errorOption.Map(Option.None<Parser<TSymbol>, BuilderError>)
                .ValueOr(() =>
                {
                    var automata = automataBuilder.Build();

                    return new Parser<TSymbol>(automata.ValueOrFailure(), startItemSet)
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

            var kernelItem = Item<TSymbol>.ForRoot(start);
            var closureItems = _closureGenerator.GetClosureItems(kernelItem);
            var fullItemSet = kernelItem.Include(closureItems);

            return new ItemSet<TSymbol>(fullItemSet);
        }
    }
}