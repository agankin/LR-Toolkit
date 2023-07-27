using System.Collections.Immutable;
using DFAutomaton;

namespace LRToolkit.Parsing
{
    public record ParsingState<TSymbol>(
        ParsedSymbolsStack<TSymbol> ParsedSymbols,
        ImmutableStack<IState<Symbol<TSymbol>, ParsingState<TSymbol>>> PriorStates
    )
    where TSymbol : notnull
    {
        public static ParsingState<TSymbol> CreateNew(State<TSymbol> start)
        {
            var parsedSymbols = ParsedSymbolsStack<TSymbol>.Empty;
            var priorStates = ImmutableStack<IState<Symbol<TSymbol>, ParsingState<TSymbol>>>.Empty
                .Push(start.DFAState);

            return new ParsingState<TSymbol>(parsedSymbols, priorStates);
        }
    }
}