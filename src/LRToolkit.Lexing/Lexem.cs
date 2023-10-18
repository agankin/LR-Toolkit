namespace LRToolkit.Lexing;

public readonly record struct Lexem<TSymbol>(TSymbol Symbol, string Value, int Position)
{
    public string Value { get; } = Value  ?? throw new ArgumentNullException(nameof(Value));

    public int Position { get; } = Position >= 0
        ? Position 
        : throw new ArgumentException($"{nameof(Position)} arg cannot be less than zero.", nameof(Position));
}