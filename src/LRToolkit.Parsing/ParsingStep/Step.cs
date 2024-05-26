using PureMonads;

namespace LRToolkit.Parsing;

internal record Step<TSymbol>(
    StepType Type,
    Symbol<TSymbol> SymbolAhead,
    ItemSet<TSymbol> NextItemSet,
    Option<Item<TSymbol>> ReducedItem
)
where TSymbol : notnull;