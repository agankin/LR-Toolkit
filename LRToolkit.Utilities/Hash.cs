namespace LRToolkit.Utilities
{
    public static class Hash
    {
        private const ulong OffsetBasis = 0xCBF29CE484222325;
        private const ulong Prime = 0x100000001B3;

        public static int Calculate(object first, object second) =>
            31 * (first?.GetHashCode() ?? 0) + (second?.GetHashCode() ?? 0);

        public static int CalculateFNV<TItem>(IEnumerable<TItem> items) =>
            (int)items.Aggregate(
                OffsetBasis,
                (hash, item) => (hash ^ (ulong)(item?.GetHashCode() ?? 0)) * Prime);
    }
}