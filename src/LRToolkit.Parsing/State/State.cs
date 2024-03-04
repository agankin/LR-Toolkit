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
        IItemSetMerger<TSymbol> itemSetMerger)
    {
        var mergeableStateDict = new MergeableStateDict<TSymbol>(itemSetMerger);
        var startState = new State<TSymbol>
        {
            DFAState = dfaState,
            FullItemSet = fullItemSet,
            MergeableStateDict = mergeableStateDict
        };

        return startState;
    }

    public State<TSymbol> TransitsToNew(Symbol<TSymbol> symbol, ItemSet<TSymbol> fullItemSet, Reduce<Symbol<TSymbol>, ParsingState<TSymbol>> reduce)
    {
        var newDFAState = DFAState.TransitsBy(symbol).WithReducingBy(reduce).ToNew();
        return this with
        {
            DFAState = newDFAState,
            FullItemSet = fullItemSet
        };
    }

    public void TransitsToExisting(Symbol<TSymbol> symbolAhead, State<TSymbol> toState, Reduce<Symbol<TSymbol>, ParsingState<TSymbol>> reduce) =>
        DFAState.TransitsBy(symbolAhead).WithReducingBy(reduce).To(toState.DFAState);

    public void TransitsDynamicly(Symbol<TSymbol> symbol, Reduce<Symbol<TSymbol>, ParsingState<TSymbol>> reduce) =>
        DFAState.TransitsBy(symbol).Dynamicly().WithReducing(reduce);

    public AcceptedState<Symbol<TSymbol>, ParsingState<TSymbol>> ToNewFixedAccepted(
        Symbol<TSymbol> symbolAhead,
        Reduce<Symbol<TSymbol>, ParsingState<TSymbol>> reduce)
    {
        return DFAState.TransitsBy(symbolAhead).WithReducingBy(reduce).ToAccepted();
    }

    public override string? ToString() => DFAState.ToString();
}