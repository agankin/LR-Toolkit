using DFAutomaton;
using Optional;

namespace LRToolkit.Parsing
{
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

        public Option<ParsingState<TSymbol>, AutomatonError<Symbol<TSymbol>, ParsingState<TSymbol>>> Run(IEnumerable<TSymbol> symbols)
        {
            var startValue = ParsingState<TSymbol>.CreateNew(_startState);
            var symbolsWithEnd = symbols.Select(Symbol<TSymbol>.Create).Append(Symbol<TSymbol>.End());

            return _automaton.Run(startValue, symbolsWithEnd);
        }
    }
}