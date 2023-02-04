namespace LRBee.Lexing
{
    public readonly record struct TextInput
    {
        private const int StartPos = 0;

        public TextInput(string text) : this(text, StartPos) { }

        public TextInput(string text, int position) =>
            (Text, Position) = Validate(text, position);

        public string Text { get; init; }
        
        public int Position { get; init; }

        private static (string text, int position) Validate(string text, int position)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(Text));
            }

            if (position < 0)
            {
                throw new ArgumentException(
                    $"{nameof(Position)} cannot be less than zero.",
                    nameof(Position));
            }

            return (text, position);
        }
    }
}