using LRToolkit.Utilities;
using PureMonads;

namespace LRToolkit.Grammaring;

public class Grammar<TSymbol>
{
    private readonly IReadOnlyDictionary<TSymbol, ProductionRuleCollection<TSymbol>> _productions;

    public Grammar(TSymbol start, IReadOnlyDictionary<TSymbol, ProductionRuleCollection<TSymbol>> productions)
    {
        Start = start;
        _productions = productions;
    }

    public TSymbol Start { get; }

    public IReadOnlyList<ProductionRule<TSymbol>> this[TSymbol symbol] =>
        _productions.GetOrNone(symbol).Or(new ProductionRuleCollection<TSymbol>(symbol));
}