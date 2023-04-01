namespace LRBee.Lexing
{
    public readonly record struct TextInput(string Text, int Position = 0)
    {
        public required string Text { get; init; } = Text ?? throw new ArgumentNullException(nameof(Text));

        public required int Position { get; init; } = ValidatePosition(Position, Text);

        private static int ValidatePosition(int position, string text)
        {
            if (position < 0)
                throw new ArgumentException($"Value of {nameof(Position)} is less than zero.", nameof(Position));

            if (position > text.Length)
                throw new ArgumentException($"Value of {nameof(Position)} exceeds text symbols count.", nameof(Position));

            return position;
        }
    }
}