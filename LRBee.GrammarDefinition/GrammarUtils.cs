namespace LRBee.GrammarDefinition
{
    public static class GrammarUtils
    {
        public static Production<TSymbol> Prod<TSymbol>(params TSymbol[] symbols)
            => new Production<TSymbol>(symbols);
    }
}