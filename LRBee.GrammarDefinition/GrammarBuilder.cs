namespace LRBee.GrammarDefinition
{
    public class GrammarBuilder<TSymbol>
        where TSymbol : notnull
    {
        private readonly Dictionary<TSymbol, ProductionRuleCollection<TSymbol>> _productions = new();

        public GrammarBuilder(TSymbol start) => Start = start;

        public TSymbol Start { get; }

        public IEnumerable<TSymbol> this[TSymbol symbol]
        {
            set
            {
                var production = new ProductionRule<TSymbol>(symbol, value);

                if (!_productions.TryGetValue(symbol, out var productions))
                    _productions.Add(symbol, productions = new());

                productions.Add(production);
            }
        }

        public Grammar<TSymbol> Build()
        {
            return new Grammar<TSymbol>(Start, _productions);
        }
    }
}