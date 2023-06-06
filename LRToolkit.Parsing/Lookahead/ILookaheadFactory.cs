using LRToolkit.GrammarDefinition;
using Optional;

namespace LRToolkit.Parsing
{
    public interface ILookaheadFactory<TSymbol> where TSymbol : notnull
    {
        Option<SymbolLookahead<TSymbol>> GetAhead(Item<TSymbol> item);

        IReadOnlySet<ILookahead<TSymbol>> GetFullSet(ILookahead<TSymbol> symbolLookahead);
    }
}