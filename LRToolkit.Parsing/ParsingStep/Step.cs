using DFAutomaton;

namespace LRToolkit.Parsing
{
    internal record Step<TSymbol>(
        StepType Type,
        Symbol<TSymbol> Symbol,
        StateReducer<Symbol<TSymbol>, ParsingState<TSymbol>> Reducer,
        ItemSet<TSymbol> NextItemSet
    )
    where TSymbol : notnull;
}