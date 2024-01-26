using DFAutomaton;
using LRToolkit.Lexing;
using Optional;

namespace LRToolkit.Parsing;

public class Parser<TSymbol> where TSymbol : notnull
{
    private readonly Automaton<Symbol<TSymbol>, ParsingState<TSymbol>> _automaton;
    private readonly State<TSymbol> _startState;

    internal Parser(Automaton<Symbol<TSymbol>, ParsingState<TSymbol>> automaton, State<TSymbol> startState)
    {
        _automaton = automaton;
        _startState = startState;
    }

    public IState<Symbol<TSymbol>, ParsingState<TSymbol>> StartState => _automaton.Start;

    public Option<Symbol<TSymbol>, AutomatonError<Symbol<TSymbol>, ParsingState<TSymbol>>> Run(IEnumerable<Lexem<TSymbol>> lexems)
    {
        var endLexem = Symbol<TSymbol>.End();
        var startValue = ParsingState<TSymbol>.CreateNew(_startState);
        var symbolsWithEnd = lexems
            .Select(Symbol<TSymbol>.Create)
            .Append(endLexem);

        var result = _automaton.Run(startValue, symbolsWithEnd);
        var startNode = result.Map(GetRootNode);

        return startNode;
    }

    private Symbol<TSymbol> GetRootNode(ParsingState<TSymbol> state) => state.ParsingStack.Peek();
}