using DFAutomaton;

namespace LRToolkit.Parsing;

public class State<TSymbol> where TSymbol : notnull
{
    private State(
        State<Symbol<TSymbol>, ParsingState<TSymbol>> dfaState,
        ItemSet<TSymbol> fullItemSet,
        MergeableStateDict<TSymbol> mergeableStateDict)
    {
        DFAState = dfaState;
        FullItemSet = fullItemSet;
        MergeableStateDict = mergeableStateDict;
    }

    public State<Symbol<TSymbol>, ParsingState<TSymbol>> DFAState { get; }

    public ItemSet<TSymbol> FullItemSet { get; }

    public long Id => DFAState.Id;

    public object? Tag
    {
        get => DFAState.Tag;
        set => DFAState.Tag = value;
    }

    internal MergeableStateDict<TSymbol> MergeableStateDict { get; }

    public static State<TSymbol> CreateStart(
        State<Symbol<TSymbol>, ParsingState<TSymbol>> dfaState,
        ItemSet<TSymbol> fullItemSet,
        ILRParserBuilderBehavior<TSymbol> parserBuilderBehavior)
    {
        var mergeableStateDict = new MergeableStateDict<TSymbol>(parserBuilderBehavior);
        var startState = new State<TSymbol>(dfaState, fullItemSet, mergeableStateDict);

        return startState;
    }

    public State<TSymbol> ToNewFixedState(
        Symbol<TSymbol> symbolAhead,
        ItemSet<TSymbol> fullItemSet,
        ReduceValue<Symbol<TSymbol>, ParsingState<TSymbol>> reduce)
    {
        var newDFAState = DFAState.ToNewFixedState(symbolAhead, reduce);
        return new State<TSymbol>(newDFAState, fullItemSet, MergeableStateDict);
    }

    public void LinkFixedState(Symbol<TSymbol> symbolAhead, State<TSymbol> toState, ReduceValue<Symbol<TSymbol>, ParsingState<TSymbol>> reduce) =>
        DFAState.LinkFixedState(symbolAhead, toState.DFAState, reduce);

    public void LinkDynamicState(Symbol<TSymbol> symbol, Reduce<Symbol<TSymbol>, ParsingState<TSymbol>> reduce)
    {
        DFAState.LinkDynamic(symbol, reduce);
    }

    public AcceptedState<Symbol<TSymbol>, ParsingState<TSymbol>> ToNewFixedAccepted(
        Symbol<TSymbol> symbol,
        ReduceValue<Symbol<TSymbol>, ParsingState<TSymbol>> reduce)
    {
        return DFAState.ToNewFixedAccepted(symbol, reduce);
    }

    public override string? ToString() => DFAState.ToString();
}