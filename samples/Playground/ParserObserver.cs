using LRToolkit.Parsing;

namespace Playground;

public static class ParserObserver
{
    public static ParserTransitionsObserver<TSymbol> Configure<TSymbol>(ParserTransitionsObserver<TSymbol> observer)
        where TSymbol : notnull =>
        observer.OnShift((parsingState, symbolAhead, transitingToId) =>
        {
            var (parsedSymbols, parsedSymbolsItemSets) = parsingState;

            Console.WriteLine($"[{Format(parsedSymbols)}] + {symbolAhead} --- SHIFT ---> State {transitingToId}");
        })
        .OnReduce((parsingState, symbolAhead, rule, transitingToId) =>
        {
            var (parsedSymbols, parsedSymbolsItemSets) = parsingState;
            var reducedToSymbol = rule.ForSymbol;

            Console.WriteLine($"[{Format(parsedSymbols)}] + {symbolAhead} --- REDUCE {rule} ---> State {transitingToId}");
        })
        .OnAccept((parsingState, transitingToId) =>
        {
            var (parsedSymbols, parsedSymbolsItemSets) = parsingState;
            var parsedSymbolsDescription = string.Join(", ", parsedSymbols);
            
            Console.WriteLine($"[{Format(parsedSymbols)}] --- ACCEPT ---> State {transitingToId}");
        });

    private static string Format<TSymbol>(IEnumerable<ParsedSymbol<TSymbol>> parsedSymbols) =>
        string.Join(", ", parsedSymbols.Select(symbol => symbol.Symbol));
}