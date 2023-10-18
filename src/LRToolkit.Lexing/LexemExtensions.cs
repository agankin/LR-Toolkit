namespace LRToolkit.Lexing;

public static class LexemExtensions
{
    public static int GetLength<TSymbol>(this Lexem<TSymbol> lexem) => lexem.Value.Length;
}