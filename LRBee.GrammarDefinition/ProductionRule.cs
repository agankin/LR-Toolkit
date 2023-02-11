using System.Collections;

namespace LRBee.GrammarDefinition
{
    public class ProductionRule<TSymbol> : IReadOnlyList<Symbol<TSymbol>>
    {
        public ProductionRule(TSymbol symbol, IEnumerable<TSymbol> production)
        {
            Symbol = symbol;
            Production = production
                .Select((symbol, pos) => new Symbol<TSymbol>(symbol, this, pos))
                .ToList();
        }

        public TSymbol Symbol { get; }

        public IReadOnlyList<Symbol<TSymbol>> Production { get; }

        public int Count => Production.Count;

        public Symbol<TSymbol> this[int index] => Production[index];

        public IEnumerator<Symbol<TSymbol>> GetEnumerator() => Production.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Production.GetEnumerator();
    }
}