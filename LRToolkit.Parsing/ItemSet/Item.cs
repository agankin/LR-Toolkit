using LRToolkit.GrammarDefinition;
using Optional;

namespace LRToolkit.Parsing
{
    public record Item<TSymbol> where TSymbol : notnull
    {
        private Item(TSymbol forSymbol, SymbolCollection<TSymbol> production, int position = 0)
        {
            ForSymbol = forSymbol;
            Production = production;
            Position = position;
        }

        public TSymbol ForSymbol { get; init; }

        public SymbolCollection<TSymbol> Production { get; init; }

        public int Position { get; init; }

        public bool IsRoot { get; init; }

        public static Item<TSymbol> ForRoot(TSymbol root)
        {
            var production = new SymbolCollection<TSymbol>(new[]
            {
                Symbol<TSymbol>.Create(root),
                Symbol<TSymbol>.End()
            });

            return new Item<TSymbol>(root, production, position: 0)
            {
                IsRoot = true
            };
        }

        public static Item<TSymbol> FromRule(ProductionRule<TSymbol> productionRule)
        {
            if (productionRule == null)
                throw new ArgumentNullException(nameof(productionRule));

            var forSymbol = productionRule.ForSymbol;
            var production = new SymbolCollection<TSymbol>(
                productionRule.Production.Select(Symbol<TSymbol>.Create).ToArray());

            return new Item<TSymbol>(forSymbol, production, position: 0);
        }

        public Option<Symbol<TSymbol>> GetSymbolAhead() =>
            ForHasSymbolsAhead(() => Production[Position]);

        public Option<Item<TSymbol>> StepForward() =>
            ForHasSymbolsAhead(() => this with { Position = Position + 1 });

        public bool HasSymbolsAhead() => Position < Production.Count;

        public override string ToString()
        {
            var forSymbol = ForSymbol;
            var productionSymbols = string.Join(
                ", ",
                Production.Select((symbol, idx) => idx == Position
                    ? $"*{symbol}"
                    : symbol.ToString() ?? string.Empty));
            var lastPos = Position == Production.Count ? "*" : string.Empty;

            return $"{forSymbol} -> {productionSymbols}{lastPos}";
        }

        private Option<TResult> ForHasSymbolsAhead<TResult>(Func<TResult> getValue) => HasSymbolsAhead()
            ? getValue().Some()
            : Option.None<TResult>();
    }
}