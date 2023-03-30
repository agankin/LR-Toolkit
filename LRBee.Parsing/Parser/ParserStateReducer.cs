using DFAutomaton;

namespace LRBee.Parsing
{
    internal delegate ParsingState<TSymbol> ParserStateReducer<TSymbol>(
        AutomataRunState<Symbol<TSymbol>, ParsingState<TSymbol>> automataRunState,
        ParsingState<TSymbol> parsingState)
        where TSymbol : notnull;
}