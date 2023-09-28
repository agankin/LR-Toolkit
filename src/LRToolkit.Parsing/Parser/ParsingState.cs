using System.Collections.Immutable;
using DFAutomaton;

namespace LRToolkit.Parsing;

public record ParsingState<TSymbol>(
    ParsingStack<TSymbol> ParsingStack,
    ImmutableStack<IState<Symbol<TSymbol>, ParsingState<TSymbol>>> PriorStates
)
where TSymbol : notnull
{
    public static ParsingState<TSymbol> CreateNew(State<TSymbol> start)
    {
        var parsingStack = new ParsingStack<TSymbol>();
        var priorStates = ImmutableStack<IState<Symbol<TSymbol>, ParsingState<TSymbol>>>.Empty.Push(start.DFAState);

        return new ParsingState<TSymbol>(parsingStack, priorStates);
    }
}