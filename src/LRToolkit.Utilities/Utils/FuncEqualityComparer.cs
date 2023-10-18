using System.Diagnostics.CodeAnalysis;

namespace LRToolkit.Utilities;

public class FuncEqualityComparer<TValue> : IEqualityComparer<TValue>
{
    private readonly Func<TValue, TValue, bool> _equals;

    public FuncEqualityComparer(Func<TValue, TValue, bool> equals) => _equals = equals ?? throw new ArgumentNullException(nameof(equals));

    public bool Equals(TValue? first, TValue? second)
    {
        if (first is null && second is null)
            return true;

        if (first is null || second is null)
            return false;

        return _equals(first, second);
    }

    public int GetHashCode([DisallowNull] TValue value) => value?.GetHashCode() ?? 0;
}