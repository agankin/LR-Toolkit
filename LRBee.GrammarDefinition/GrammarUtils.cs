namespace LRBee.GrammarDefinition
{
    public static class GrammarUtils
    {
        public static IReadOnlyList<TSymbol> _<TSymbol>(params TSymbol[] symbols)
            => symbols;
    }
}