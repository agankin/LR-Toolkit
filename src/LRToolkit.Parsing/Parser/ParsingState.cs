using System.Collections.Immutable;
using DFAutomaton;

namespace LRToolkit.Parsing;

public record ParsingState<TSymbol>(
    ParsingStack<TSymbol> ParsingStack,
    ImmutableStack<uint> PriorStateIds
)
where TSymbol : notnull
{
    internal static ParsingState<TSymbol> CreateNew(State<TSymbol> start)
    {
        var parsingStack = new ParsingStack<TSymbol>();
        var priorStateIds = ImmutableStack<uint>.Empty.Push(start.DFAState.Id);

        return new ParsingState<TSymbol>(parsingStack, priorStateIds);
    }
}