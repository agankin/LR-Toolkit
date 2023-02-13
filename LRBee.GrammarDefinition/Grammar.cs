using LRBee.Utilities.Extensions;

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

        public IReadOnlyList<ProductionRule<TSymbol>> this[TSymbol symbol] => _productions.Get(
            symbol,
            Array.Empty<ProductionRule<TSymbol>>);
    }
}