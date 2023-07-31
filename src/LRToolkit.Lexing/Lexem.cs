namespace LRToolkit.Lexing
{
    public readonly record struct Lexem<TToken>
    {
        public Lexem(TToken token, string value, int position)
        {
            Token = token;
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Position = position >= 0
                ? position 
                : throw new ArgumentException($"{nameof(position)} arg cannot be less than zero.", nameof(position));
        }

        public TToken Token { get; }

        public string Value { get; }

        public int Position { get; }
    }
}