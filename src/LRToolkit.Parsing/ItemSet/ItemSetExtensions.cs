namespace LRToolkit.Parsing;

internal static class StateItemSetExtensions
{
    public static IReadOnlySet<Item<TSymbol>> Produce<TSymbol>(this ClosureProducer<TSymbol> closureProducer, IReadOnlySet<Item<TSymbol>> kernels)
        where TSymbol : notnull
    {
        var closures = kernels.SelectMany(closureProducer.Produce).ToHashSet();
        
        return closures;
    }
}