using Optional;
using Optional.Collections;

namespace LRBee.Parsing
{
    internal class StateForItemSet<TSymbol> where TSymbol : notnull
    {
        private readonly Dictionary<ItemSet<TSymbol>, State<TSymbol>> _stateByItemSet = new();

        public Option<State<TSymbol>> this[ItemSet<TSymbol> itemSet] =>
            _stateByItemSet.GetValueOrNone(itemSet);

        public void Add(State<TSymbol> state, ItemSet<TSymbol> itemSet) =>
            _stateByItemSet.Add(itemSet, state);
    }
}