using LRToolkit.Parsing;

namespace Playground;

public static class ParserObserver
{
    public static ParserTransitionsObserver<TSymbol> Configure<TSymbol>(ParserTransitionsObserver<TSymbol> observer)
        where TSymbol : notnull =>
        observer.OnShift((parsingState, symbolAhead, transitingToId) =>
        {
            var (parsingStack, _) = parsingState;

            Console.WriteLine($"[{parsingStack.Format()}] + {symbolAhead} --- SHIFT ---> State {transitingToId}");
        })
        .OnReduce((parsingState, symbolAhead, rule, transitingToId) =>
        {
            var (parsingStack, _) = parsingState;
            var reducedToSymbol = rule.ForSymbol;

            Console.WriteLine($"[{parsingStack.Format()}] + {symbolAhead} --- REDUCE {rule} ---> State {transitingToId}");
        })
        .OnAccept((parsingState, transitingToId) =>
        {
            var (parsingStack, _) = parsingState;
            
            Console.WriteLine($"[{parsingStack.Format()}] --- ACCEPT ---> State {transitingToId}");
        });

    private static string Format<TSymbol>(this IEnumerable<Symbol<TSymbol>> symbols) where TSymbol : notnull =>
        string.Join(", ", symbols);
}