using System.Collections;
using Optional;
using Optional.Collections;

namespace LRToolkit.GrammarDefinition
{
    public class Production<TSymbol> : IReadOnlyCollection<TSymbol>
    {
        private readonly IReadOnlyList<TSymbol> _productionSymbols;

        public Production(IReadOnlyList<TSymbol> productionSymbols) =>
            _productionSymbols = productionSymbols;

        public Production(params TSymbol[] productionSymbols) : this(productionSymbols.AsReadOnly()) { }

        public Option<TSymbol> this[int index] => _productionSymbols.ElementAtOrNone(index);

        public TSymbol First => _productionSymbols[0];

        public int Count => _productionSymbols.Count;

        public IEnumerator<TSymbol> GetEnumerator() => _productionSymbols.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _productionSymbols.GetEnumerator();

        public override string ToString() => string.Join(", ", _productionSymbols);
    }
}