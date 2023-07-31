namespace LRToolkit.Lexing;

public static class LexemExtensions
{
    public static int GetLength<TToken>(this Lexem<TToken> lexem) => lexem.Value.Length;
}