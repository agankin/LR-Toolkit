using DFAutomaton;
using Optional;

namespace LRBee.Parsing
{
    public class Parser<TSymbol> where TSymbol : notnull
    {
        private readonly Automata<Symbol<TSymbol>, ParsingState<TSymbol>> _automata;
        private readonly ItemSet<TSymbol> _startItemSet;

        public Parser(Automata<Symbol<TSymbol>, ParsingState<TSymbol>> automata, ItemSet<TSymbol> startItemSet)
        {
            _automata = automata;
            _startItemSet = startItemSet;
        }

        public IState<Symbol<TSymbol>, ParsingState<TSymbol>> StartState => _automata.Start;

        public Option<ParsingState<TSymbol>, AutomataError<Symbol<TSymbol>, ParsingState<TSymbol>>> Run(IEnumerable<TSymbol> symbols)
        {
            var startValue = ParsingState<TSymbol>.CreateNew(_startItemSet);
            var symbolsWithEnd = symbols.Select(Symbol<TSymbol>.Create).Append(Symbol<TSymbol>.End());

            return _automata.Run(startValue, symbolsWithEnd);
        }
    }
}