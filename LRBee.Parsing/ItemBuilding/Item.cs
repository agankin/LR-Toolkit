using LRBee.GrammarDefinition;
using Optional;

namespace LRBee.Parsing.ItemBuilding
{
    public readonly record struct Item<TSymbol>(ProductionRule<TSymbol> ProductionRule, int Position)
    {
        public static Item<TSymbol> Init(ProductionRule<TSymbol> productionRule) =>
            new Item<TSymbol>(productionRule, Position: 0);

        public Option<TSymbol> GetNextSymbol() => HasPosition(Position)
            ? Option.Some(ProductionRule[Position])
            : Option.None<TSymbol>();

        public Option<Item<TSymbol>> StepPosition() => HasPosition(Position)
            ? Option.Some(this with { Position = Position + 1 })
            : Option.None<Item<TSymbol>>();

        private bool HasPosition(int position) => position < ProductionRule.Count;
    }
}