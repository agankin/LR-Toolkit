namespace LRToolkit.Lexing;

public static class TextInputExtensions
{
    public static bool ReachedEnd(this TextInput input) =>
        input.Position >= input.Text.Length;

    public static TextInput Step(this TextInput input, int forwardCount) =>
        input with { Position = input.Position + forwardCount };
}