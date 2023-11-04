using System.Diagnostics.CodeAnalysis;

namespace LRToolkit.Utilities;

public class FuncEqualityComparer<TValue> : IEqualityComparer<TValue>
{
    private readonly Func<TValue, TValue, bool> _equals;
    private readonly Func<TValue, int> _getHashCode;

    public FuncEqualityComparer(Func<TValue, TValue, bool> equals, Func<TValue, int> getHashCode)
    {
        _equals = equals ?? throw new ArgumentNullException(nameof(equals));
        _getHashCode = getHashCode ?? throw new ArgumentNullException(nameof(getHashCode));
    }

    public bool Equals(TValue? first, TValue? second)
    {
        if (first is null && second is null)
            return true;

        if (first is null || second is null)
            return false;

        return _equals(first, second);
    }

    public int GetHashCode([DisallowNull] TValue value) => _getHashCode(value);
}