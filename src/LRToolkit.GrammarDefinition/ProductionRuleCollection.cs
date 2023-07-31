using System.Collections;

namespace LRToolkit.GrammarDefinition;

public class ProductionRuleCollection<TSymbol> : IReadOnlyList<ProductionRule<TSymbol>>
{
    private readonly IList<ProductionRule<TSymbol>> _rules = new List<ProductionRule<TSymbol>>();

    public ProductionRuleCollection(TSymbol forSymbol) => ForSymbol = forSymbol;

    public TSymbol ForSymbol { get; }

    public int Count => _rules.Count;

    public ProductionRule<TSymbol> this[int index] => _rules[index];

    public void Add(ProductionRule<TSymbol> rule) => _rules.Add(rule);

    public IEnumerator<ProductionRule<TSymbol>> GetEnumerator() => _rules.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString()
    {
        var rules = string.Join(" | ", _rules.Select(rule => $"[{rule.Production}]"));

        return $"{ForSymbol} -> {rules}";
    }
}