namespace LRBee.GrammarDefinition
{
    public readonly struct Symbol<TSymbol> : IEquatable<Symbol<TSymbol>>
    {
        private readonly Key _key;

        public Symbol(TSymbol value, ProductionRule<TSymbol> production, int symbolPosition)
            : this()
        {
            Value = value;
            _key = new(production, symbolPosition);
        }

        public TSymbol Value { get; }

        public bool Equals(Symbol<TSymbol> other) =>
            _key == other._key;

        public override bool Equals(object? obj) =>
            (obj is Symbol<TSymbol> other) && Equals(other);

        public override int GetHashCode() => _key.GetHashCode();

        private readonly record struct Key(
            ProductionRule<TSymbol> Production,
            int SymbolPosition) { }
    }
}