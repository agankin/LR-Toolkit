namespace LRToolkit.GrammarDefinition;

public static class GrammarUtils
{
    public static Production<TSymbol> _<TSymbol>(params TSymbol[] symbols)
        => new Production<TSymbol>(symbols);
}