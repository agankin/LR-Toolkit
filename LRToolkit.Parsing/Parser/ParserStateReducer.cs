using DFAutomaton;

namespace LRToolkit.Parsing
{
    internal delegate ParsingState<TSymbol> ParserStateReducer<TSymbol>(
        AutomatonRunState<Symbol<TSymbol>, ParsingState<TSymbol>> automatonRunState,
        ParsingState<TSymbol> parsingState)
        where TSymbol : notnull;
}