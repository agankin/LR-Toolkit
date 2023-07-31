using LRToolkit.GrammarDefinition;
using DFAutomaton;
using Optional;
using Optional.Unsafe;
using LRToolkit.Utilities;

namespace LRToolkit.Parsing;

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
        
        _stepCalculator = new StepCalculator<TSymbol>(_closureProducer);
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

        var startState = State<TSymbol>.CreateStart(automatonBuilder.Start, startItemSet);
        startState.Tag = startItemSet;

        return BuildNext(startState).Match(
            Option.None<Parser<TSymbol>, BuilderError>,
            () => 
            {
                var automaton = automatonBuilder.Build(c => c.TurnOffAnyReachesAcceptedValidation()).ValueOrFailure();
                return new Parser<TSymbol>(automaton, startState).Some<Parser<TSymbol>, BuilderError>();
            });
    }

    private Option<BuilderError> BuildNext(State<TSymbol> state)
    {
        var symbols = state.FullItemSet.GetSymbolsAhead();

        return symbols.Aggregate(
            Option.None<BuilderError>(),
            (error, symbol) => error.MapNone(() => BuildNext(state, symbol)));
    }

    private Option<BuilderError> BuildNext(State<TSymbol> state, Symbol<TSymbol> symbol) =>
        _stepCalculator.GetStep(state.FullItemSet, symbol).Match(
            step => _stateBuilder.BuildNext(state, step),
            error => error.Some());

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