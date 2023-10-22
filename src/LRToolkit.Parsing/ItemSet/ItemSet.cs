using System.Collections;
using LRToolkit.Utilities;

namespace LRToolkit.Parsing;

public readonly record struct ItemSet<TSymbol>(
    IReadOnlySet<Item<TSymbol>> Kernels,
    IReadOnlySet<Item<TSymbol>> Closures
) : IEnumerable<Item<TSymbol>> where TSymbol : notnull
{
    public IEnumerable<Symbol<TSymbol>> GetSymbolsAhead()
    {
        var fromKernels = Kernels.SelectOnlySome(item => item.GetSymbolAhead()).Distinct();
        var fromClosures = Closures.SelectOnlySome(item => item.GetSymbolAhead()).Distinct();
        
        return fromKernels.Concat(fromClosures);
    }

    public IReadOnlySet<Item<TSymbol>> StepForward(Symbol<TSymbol> symbolAhead)
    {
        var kernelsHavingAhead = Kernels.Where(item => item.GetSymbolAhead().SomeEquals(symbolAhead));
        var closuresHavingAhead = Closures.Where(item => item.GetSymbolAhead().SomeEquals(symbolAhead));

        var stepForwardKernels = kernelsHavingAhead.Concat(closuresHavingAhead).SelectOnlySome(item => item.StepForward()).ToHashSet();

        return stepForwardKernels;
    }

    public IEnumerator<Item<TSymbol>> GetEnumerator() => AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => AsEnumerable().GetEnumerator();

    public bool Equals(ItemSet<TSymbol> other) => Kernels.SetEquals(other.Kernels);

    public override int GetHashCode() => Hash.FNV(Kernels);

    public override string ToString()
    {        
        var kernels = string.Join(" | ", Kernels);

        return $"[{kernels}]";
    }

    private IEnumerable<Item<TSymbol>> AsEnumerable()
    {
        foreach (var kernel in Kernels)
            yield return kernel;

        foreach (var closure in Closures)
            yield return closure;
    }
}