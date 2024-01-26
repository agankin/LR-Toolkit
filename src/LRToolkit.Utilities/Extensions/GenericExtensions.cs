namespace LRToolkit.Utilities;

public static class GenericExtensions
{
    public static TResult Pipe<TTarget, TResult>(this TTarget target, Func<TTarget, TResult> transform) => transform(target);
}