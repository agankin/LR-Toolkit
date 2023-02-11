namespace LRBee.GrammarDefinition
{
    public class Grammar<TSymbol>
    {
        private readonly IReadOnlyDictionary<TSymbol, ProductionRuleCollection<TSymbol>> _productions;

        public Grammar(
            TSymbol start,
            IReadOnlyDictionary<TSymbol, ProductionRuleCollection<TSymbol>> productions)
        {
            Start = start;
            _productions = productions;
        }

        public TSymbol Start { get; }

        public IEnumerable<ProductionRule<TSymbol>> this[TSymbol symbol] => _productions[symbol];
    }
}