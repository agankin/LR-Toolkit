namespace LRToolkit.Utilities;

public static class Hash
{
    private const ulong OffsetBasis = 0xCBF29CE484222325;
    private const ulong Prime = 0x100000001B3;

    public static int FNV<TValue>(IEnumerable<TValue> items) => items.Select(item => item?.GetHashCode() ?? 0).AggregateToFNVHash();

    private static int AggregateToFNVHash(this IEnumerable<int> itemsHashes) =>
        (int)itemsHashes.Aggregate(OffsetBasis, (resultHash, itemHash) => (resultHash ^ (ulong)itemHash) * Prime);
}