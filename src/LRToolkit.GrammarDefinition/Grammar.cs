using Optional.Collections;

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
        _productions.GetValueOrNone(symbol).ValueOr(new ProductionRuleCollection<TSymbol>(symbol));
}