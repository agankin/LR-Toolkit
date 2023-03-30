using Optional.Collections;

namespace LRBee.GrammarDefinition
{
    public class Grammar<TSymbol>
    {
        private readonly IReadOnlyDictionary<TSymbol, IReadOnlyList<ProductionRule<TSymbol>>> _productions;

        public Grammar(
            TSymbol start,
            IReadOnlyDictionary<TSymbol, IReadOnlyList<ProductionRule<TSymbol>>> productions)
        {
            Start = start;
            _productions = productions;
        }

        public TSymbol Start { get; }

        public IReadOnlyList<ProductionRule<TSymbol>> this[TSymbol symbol] =>
            _productions.GetValueOrNone(symbol).ValueOr(Array.Empty<ProductionRule<TSymbol>>);
    }
}