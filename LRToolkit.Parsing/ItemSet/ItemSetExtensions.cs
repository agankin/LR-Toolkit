namespace LRToolkit.Parsing
{
    internal static class StateItemSetExtensions
    {
        public static ItemSet<TSymbol> Include<TSymbol>(this ItemSet<TSymbol> itemSet, IEnumerable<Item<TSymbol>> items)
            where TSymbol : notnull
        {
            var resultItems = itemSet.Concat(items).ToHashSet();

            return new ItemSet<TSymbol>(resultItems);
        }

        public static ItemSet<TSymbol> Include<TSymbol>(this Item<TSymbol> item, IEnumerable<Item<TSymbol>> items)
            where TSymbol : notnull
        {
            var resultItems = items.Prepend(item).ToHashSet();

            return new ItemSet<TSymbol>(resultItems);
        }

        public static ItemSet<TSymbol> Produce<TSymbol>(this ClosureProducer<TSymbol> closureProducer, ItemSet<TSymbol> itemSet)
            where TSymbol : notnull
        {
            var closures = itemSet.SelectMany(closureProducer.Produce).ToHashSet();
            return new ItemSet<TSymbol>(closures);
        }
    }
}