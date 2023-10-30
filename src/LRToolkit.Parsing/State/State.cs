using DFAutomaton;

namespace LRToolkit.Parsing;

internal record State<TSymbol> where TSymbol : notnull
{
    private readonly State<Symbol<TSymbol>, ParsingState<TSymbol>> _dfaState;
    private ItemSet<TSymbol> _fullItemSet;

    public required State<Symbol<TSymbol>, ParsingState<TSymbol>> DFAState
    {
        get => _dfaState;
        init
        {
            _dfaState = value;
            _dfaState.Tag = FullItemSet;
        }
    }

    public required ItemSet<TSymbol> FullItemSet
    {
        get => _fullItemSet;
        set => _dfaState.Tag = _fullItemSet = value;
    }

    internal required MergeableStateDict<TSymbol> MergeableStateDict { get; init; }

    public long Id => DFAState.Id;

    public static State<TSymbol> CreateStart(
        State<Symbol<TSymbol>, ParsingState<TSymbol>> dfaState,
        ItemSet<TSymbol> fullItemSet,
        ILRParserBuilderBehavior<TSymbol> parserBuilderBehavior)
    {
        var mergeableStateDict = new MergeableStateDict<TSymbol>(parserBuilderBehavior);
        var startState = new State<TSymbol>
        {
            DFAState = dfaState,
            FullItemSet = fullItemSet,
            MergeableStateDict = mergeableStateDict
        };

        return startState;
    }

    public State<TSymbol> ToNewFixedState(
        Symbol<TSymbol> symbolAhead,
        ItemSet<TSymbol> fullItemSet,
        ReduceValue<Symbol<TSymbol>, ParsingState<TSymbol>> reduce)
    {
        var newDFAState = DFAState.ToNewFixedState(symbolAhead, reduce);
        return this with
        {
            DFAState = newDFAState,
            FullItemSet = fullItemSet
        };
    }

    public void LinkFixedState(Symbol<TSymbol> symbolAhead, State<TSymbol> toState, ReduceValue<Symbol<TSymbol>, ParsingState<TSymbol>> reduce) =>
        DFAState.LinkFixedState(symbolAhead, toState.DFAState, reduce);

    public void LinkDynamicState(Symbol<TSymbol> symbol, Reduce<Symbol<TSymbol>, ParsingState<TSymbol>> reduce) =>
        DFAState.LinkDynamic(symbol, reduce);

    public AcceptedState<Symbol<TSymbol>, ParsingState<TSymbol>> ToNewFixedAccepted(
        Symbol<TSymbol> symbol,
        ReduceValue<Symbol<TSymbol>, ParsingState<TSymbol>> reduce)
    {
        return DFAState.ToNewFixedAccepted(symbol, reduce);
    }

    public override string? ToString() => DFAState.ToString();
}