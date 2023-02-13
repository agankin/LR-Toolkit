using LRBee.Utilities.Extensions;

namespace LRBee.GrammarDefinition
{
    public class GrammarBuilder<TSymbol>
        where TSymbol : notnull
    {
        private readonly Dictionary<TSymbol, ProductionRuleCollection<TSymbol>> _productions = new();

        public GrammarBuilder(TSymbol start) => Start = start;

        public TSymbol Start { get; }

        public IReadOnlyList<TSymbol> this[TSymbol symbol]
        {
            set
            {
                var productions = _productions.Get(symbol, () => _productions[symbol] = new());
                var production = new ProductionRule<TSymbol>(symbol, value);

                productions.Add(production);
            }
        }

        public Grammar<TSymbol> Build()
        {
            var productions = _productions.ToDictionary(
                entry => entry.Key,
                entry => (IReadOnlyList<ProductionRule<TSymbol>>)entry.Value.ToList());

            return new Grammar<TSymbol>(Start, productions);
        }
    }
}