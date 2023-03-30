namespace LRBee.GrammarDefinition
{
    public class ProductionRule<TSymbol>
    {
        public ProductionRule(TSymbol forSymbol, Production<TSymbol> production)
        {
            ForSymbol = forSymbol;
            Production = ValidateProduction(production);
        }

        public TSymbol ForSymbol { get; }

        public Production<TSymbol> Production { get; }

        private Production<TSymbol> ValidateProduction(Production<TSymbol> production)
        {
            if (production == null)
                throw new ArgumentNullException(nameof(production));

            if (production.Count == 0)
                throw new ArgumentException("Production doesn't contain symbols.", nameof(production));

            return production;
        }
    }
}