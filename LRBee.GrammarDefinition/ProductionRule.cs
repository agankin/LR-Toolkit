using System.Collections;

namespace LRBee.GrammarDefinition
{
    public class ProductionRule<TSymbol> : IReadOnlyList<TSymbol>
    {
        private IReadOnlyList<TSymbol> _production;

        public ProductionRule(TSymbol symbol, IReadOnlyList<TSymbol> production)
        {
            Symbol = symbol;
            _production = production;
        }

        public TSymbol Symbol { get; }

        public int Count => _production.Count;

        public TSymbol this[int index] => _production[index];

        public IEnumerator<TSymbol> GetEnumerator() => _production.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _production.GetEnumerator();
    }
}