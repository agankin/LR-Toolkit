namespace LRToolkit.Parsing
{
    public static class ItemSetExtensions
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
    }
}