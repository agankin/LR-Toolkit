using DFAutomaton;

namespace LRToolkit.Parsing;

public class State<TSymbol> where TSymbol : notnull
{
    private State(
        State<Symbol<TSymbol>, ParsingState<TSymbol>> dfaState,
        ItemSet<TSymbol> fullItemSet,
        StateItemSetDict<TSymbol> stateItemSetDict)
    {
        DFAState = dfaState;
        FullItemSet = fullItemSet;
        StateItemSetDict = stateItemSetDict;
    }

    public State<Symbol<TSymbol>, ParsingState<TSymbol>> DFAState { get; }

    public ItemSet<TSymbol> FullItemSet { get; }

    public long Id => DFAState.Id;

    public object? Tag
    {
        get => DFAState.Tag;
        set => DFAState.Tag = value;
    }

    internal StateItemSetDict<TSymbol> StateItemSetDict { get; }

    public static State<TSymbol> CreateStart(State<Symbol<TSymbol>, ParsingState<TSymbol>> dfaState, ItemSet<TSymbol> fullItemSet) =>
        new State<TSymbol>(dfaState, fullItemSet, new());

    public State<TSymbol> ToNewFixedState(
        Symbol<TSymbol> symbolAhead,
        ItemSet<TSymbol> fullItemSet,
        ReduceValue<Symbol<TSymbol>, ParsingState<TSymbol>> reduce)
    {
        var newDFAState = DFAState.ToNewFixedState(symbolAhead, reduce);
        return new State<TSymbol>(newDFAState, fullItemSet, StateItemSetDict);
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