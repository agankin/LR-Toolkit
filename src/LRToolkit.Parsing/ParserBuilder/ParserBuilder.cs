using LRToolkit.GrammarDefinition;
using DFAutomaton;
using Optional;
using Optional.Unsafe;
using LRToolkit.Utilities;

namespace LRToolkit.Parsing;

public class ParserBuilder<TSymbol> where TSymbol : notnull
{
    private readonly Grammar<TSymbol> _grammar;
    private readonly ILRParserBuilderBehavior<TSymbol> _parserBuilderBehavior;
    private readonly ILookaheadFactory<TSymbol> _lookaheadFactory;
    private readonly ParserTransitionsObserver<TSymbol> _observer;

    private readonly ClosureProducer<TSymbol> _closureProducer;
    private readonly StepCalculator<TSymbol> _stepCalculator;
    private readonly StateForStepBuilder<TSymbol> _stateBuilder;

    private ParserBuilder(
        Grammar<TSymbol> grammar,
        ILRParserBuilderBehavior<TSymbol> parserBuilderBehavior,
        ParserTransitionsObserver<TSymbol> observer)
    {
        _grammar = grammar;
        _parserBuilderBehavior = parserBuilderBehavior;
        _lookaheadFactory = _parserBuilderBehavior.GetLookaheadFactory();
        _observer = observer;

        _closureProducer = new ClosureProducer<TSymbol>(grammar, _lookaheadFactory);
        
        _stepCalculator = new StepCalculator<TSymbol>(_closureProducer);
        _stateBuilder = new StateForStepBuilder<TSymbol>(BuildNextStates, _parserBuilderBehavior, _observer);
    }

    public static Option<Parser<TSymbol>, BuilderError> Build(
        Grammar<TSymbol> grammar,
        ILRParserBuilderBehavior<TSymbol> parserBuilderBehavior,
        Func<ParserTransitionsObserver<TSymbol>, ParserTransitionsObserver<TSymbol>>? configureObserver = null)
    {
        var emptyObserver = ParserTransitionsObserver<TSymbol>.Create();
        var observer = configureObserver?.Invoke(emptyObserver) ?? emptyObserver;

        var builder = new ParserBuilder<TSymbol>(grammar, parserBuilderBehavior, observer);

        return builder.Build();
    }

    private Option<Parser<TSymbol>, BuilderError> Build()
    {
        var automatonBuilder = AutomatonBuilder<Symbol<TSymbol>, ParsingState<TSymbol>>.Create();
        var startItemSet = CreateStartItemSet();

        var startState = State<TSymbol>.CreateStart(automatonBuilder.Start, startItemSet, _parserBuilderBehavior);
        var error = BuildNextStates(startState);

        return error.Match(
            Option.None<Parser<TSymbol>, BuilderError>,
            () => 
            {
                var automaton = automatonBuilder.Build(c => c.TurnOffAnyReachesAcceptedValidation()).ValueOrFailure();
                return new Parser<TSymbol>(automaton, startState).Some<Parser<TSymbol>, BuilderError>();
            });
    }

    private Option<BuilderError> BuildNextStates(State<TSymbol> state)
    {
        var symbols = state.FullItemSet.GetSymbolsAhead();

        return symbols.Aggregate(
            Option.None<BuilderError>(),
            (error, symbol) => error.MapNone(() => BuildNextStates(state, symbol)));
    }

    private Option<BuilderError> BuildNextStates(State<TSymbol> state, Symbol<TSymbol> symbolAhead)
    {
        var stepOrError = _stepCalculator.GetStep(state.FullItemSet, symbolAhead);
        
        var error = stepOrError.Match(
            step => _stateBuilder.BuildNext(state, step),
            error => error.Some());

        return error;
    }

    private ItemSet<TSymbol> CreateStartItemSet()
    {
        var start = _grammar.Start;
        var startLookahead = _lookaheadFactory.GetStart();

        var kernel = Item<TSymbol>.ForStart(start, startLookahead);
        var kernels = new HashSet<Item<TSymbol>> { kernel };
        var closures = _closureProducer.Produce(kernel);
        
        var itemSet = new ItemSet<TSymbol>(kernels, closures);

        return itemSet;
    }
}