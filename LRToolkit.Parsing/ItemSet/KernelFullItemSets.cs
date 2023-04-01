namespace LRToolkit.Parsing
{
    internal readonly record struct KernelFullItemSets<TSymbol>(
        ItemSet<TSymbol> Kernel,
        ItemSet<TSymbol> Full)
        where TSymbol : notnull;
}