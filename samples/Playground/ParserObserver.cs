using LRToolkit.Parsing;

namespace Playground;

public static class ParserObserver
{
    public static ParserTransitionsObserver<TSymbol> Configure<TSymbol>(ParserTransitionsObserver<TSymbol> observer)
        where TSymbol : notnull =>
        observer.OnShift((parsingState, symbolAhead, transitingToId) =>
        {
            var (parsingStack, _) = parsingState;

            Console.WriteLine($"[{Format(parsingStack)}] + {symbolAhead} --- SHIFT ---> State {transitingToId}");
        })
        .OnReduce((parsingState, symbolAhead, rule, transitingToId) =>
        {
            var (parsingStack, _) = parsingState;
            var reducedToSymbol = rule.ForSymbol;

            Console.WriteLine($"[{Format(parsingStack)}] + {symbolAhead} --- REDUCE {rule} ---> State {transitingToId}");
        })
        .OnAccept((parsingState, transitingToId) =>
        {
            var (parsingStack, _) = parsingState;
            
            Console.WriteLine($"[{Format(parsingStack)}] --- ACCEPT ---> State {transitingToId}");
        });

    private static string Format<TSymbol>(IEnumerable<ParsingTreeNode<TSymbol>> node) =>
        string.Join(", ", node.Select(symbol => symbol.Symbol));
}