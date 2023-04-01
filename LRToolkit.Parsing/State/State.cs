using DFAutomaton;

namespace LRToolkit.Parsing
{
    public class State<TSymbol> where TSymbol : notnull
    {
        private readonly State<Symbol<TSymbol>, ParsingState<TSymbol>> _dfaState;

        public State(State<Symbol<TSymbol>, ParsingState<TSymbol>> dfaState, ItemSet<TSymbol> fullItemSet)
        {
            _dfaState = dfaState;
            FullItemSet = fullItemSet;
        }

        public IState<Symbol<TSymbol>, ParsingState<TSymbol>> DFAState => _dfaState;

        public ItemSet<TSymbol> FullItemSet { get; }

        public object? Tag
        {
            get => _dfaState.Tag;
            set => _dfaState.Tag = value;
        }

        public State<TSymbol> ToNewState(
            Symbol<TSymbol> symbolAhead,
            ItemSet<TSymbol> stateItemSet,
            StateReducer<Symbol<TSymbol>, ParsingState<TSymbol>> reducer)
        {
            var newDFAState = _dfaState.ToNewState(symbolAhead, reducer);
            return new State<TSymbol>(newDFAState, stateItemSet);
        }

        public void LinkState(
            Symbol<TSymbol> symbolAhead,
            State<TSymbol> toState,
            StateReducer<Symbol<TSymbol>, ParsingState<TSymbol>> reducer)
        {
            _dfaState.LinkState(symbolAhead, toState._dfaState, reducer);
        }

        public AcceptedStateHandle<Symbol<TSymbol>, ParsingState<TSymbol>> ToNewAccepted(
            Symbol<TSymbol> symbol,
            StateReducer<Symbol<TSymbol>, ParsingState<TSymbol>> reducer)
        {
            return _dfaState.ToNewAccepted(symbol, reducer);
        }

        public override string? ToString() => _dfaState.ToString();
    }
}