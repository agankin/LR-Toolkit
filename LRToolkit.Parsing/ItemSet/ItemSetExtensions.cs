namespace LRToolkit.Parsing
{
    public static class ItemSetExtensions
    {
        public static ItemSet<TSymbol> Include<TSymbol>(this ItemSet<TSymbol> itemSet, IEnumerable<Item<TSymbol>> items)
            where TSymbol : notnull
        {
            var itemsUnion = new HashSet<Item<TSymbol>>(itemSet.Concat(items));

            return new ItemSet<TSymbol>(itemsUnion);
        }

        public static ItemSet<TSymbol> Include<TSymbol>(this Item<TSymbol> item, IEnumerable<Item<TSymbol>> items)
            where TSymbol : notnull
        {
            var itemsUnion = new HashSet<Item<TSymbol>>(items.Append(item));

            return new ItemSet<TSymbol>(itemsUnion);
        }
    }
}