using LRToolkit.GrammarDefinition;
using Optional;
using Optional.Collections;

namespace LRToolkit.Parsing
{
    public record Item<TSymbol> where TSymbol : notnull
    {
        private Item(TSymbol forSymbol, SymbolCollection<TSymbol> production, ILookahead<TSymbol> lookahead, int position = 0)
        {
            ForSymbol = forSymbol;
            Production = production;
            Lookahead = lookahead;
            Position = position;
        }

        public TSymbol ForSymbol { get; init; }

        public SymbolCollection<TSymbol> Production { get; init; }

        public ILookahead<TSymbol> Lookahead { get; init; }

        public int ProductionCount => Production.Count;

        public int Count => Production.Count + Lookahead.Count;

        public int Position { get; init; }

        public bool IsRoot { get; init; }

        public static Item<TSymbol> ForRoot(TSymbol root, ILookahead<TSymbol> lookahead)
        {
            var production = new SymbolCollection<TSymbol>(new[]
            {
                Symbol<TSymbol>.Create(root),
                Symbol<TSymbol>.End()
            });

            return new Item<TSymbol>(root, production, lookahead, position: 0)
            {
                IsRoot = true
            };
        }

        public static Item<TSymbol> FromRule(ProductionRule<TSymbol> productionRule, ILookahead<TSymbol> lookahead)
        {
            if (productionRule == null)
                throw new ArgumentNullException(nameof(productionRule));

            var forSymbol = productionRule.ForSymbol;
            var production = new SymbolCollection<TSymbol>(
                productionRule.Production.Select(Symbol<TSymbol>.Create).ToArray());

            return new Item<TSymbol>(forSymbol, production, lookahead, position: 0);
        }

        public Option<Symbol<TSymbol>> GetSymbolAhead(int idxFromPosition = 0)
        {
            var symbolAheadPosition = Position + idxFromPosition;

            return Production.ElementAtOrNone(symbolAheadPosition)
                .Else(Lookahead[symbolAheadPosition - Production.Count]);
        }

        public Option<Item<TSymbol>> StepForward() =>
            HasSymbolAhead()
                ? (this with { Position = Position + 1 }).Some()
                : Option.None<Item<TSymbol>>();

        public bool HasSymbolAhead() => Position < Count;

        public bool HasProductionSymbolAhead() => Position < Production.Count;

        public override string ToString()
        {
            var forSymbol = ForSymbol;
            var formattedProductionsAndLookaheads = GetFormattedProductionsAndLookaheads();

            return $"{forSymbol} -> {formattedProductionsAndLookaheads}";
        }

        private string GetFormattedProductionsAndLookaheads()
        {
            var formattedProductions = string.Join(
                ", ",
                Production.Select((symbol, idx) => idx == Position
                    ? $"*{symbol}"
                    : symbol.ToString()));

            var lookaheadPosition = Position - Production.Count;
            var formattedLookaheads = string.Join(
                ", ",
                Lookahead.Select((symbol, idx) => idx == lookaheadPosition
                    ? $"*{symbol}"
                    : symbol.ToString()));
            var lastPos = Position == Count ? "*" : string.Empty;

            return Lookahead.Count > 0
                ? $"{formattedProductions}; {formattedLookaheads}{lastPos}"
                : $"{formattedProductions}{lastPos}";
        }
    }
}