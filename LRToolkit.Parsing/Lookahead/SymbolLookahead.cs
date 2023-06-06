namespace LRToolkit.Parsing;

public readonly record struct SymbolLookahead<TSymbol>(Symbol<TSymbol> Symbol, ILookahead<TSymbol> Lookahead)
    where TSymbol : notnull 
{
}