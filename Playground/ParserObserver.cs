using LRToolkit.Parsing;

namespace Playground
{
    public static class ParserObserver
    {
        public static ParserTransitionsObserver<TSymbol> Configure<TSymbol>(ParserTransitionsObserver<TSymbol> observer)
            where TSymbol : notnull =>
            observer.OnShift((parsingState, symbolAhead, transitingTo) =>
            {
                var (parsedSymbols, parsedSymbolsItemSets) = parsingState;

                Console.WriteLine($"[{Format(parsedSymbols)}] {symbolAhead} -> SHIFT -> State {transitingTo.Id}");
            })
            .OnReduce((parsingState, symbolAhead, rule, transitingTo) =>
            {
                var (parsedSymbols, parsedSymbolsItemSets) = parsingState;
                var reducedToSymbol = rule.ForSymbol;

                Console.WriteLine($"[{Format(parsedSymbols)}] {symbolAhead} -> REDUCE {rule} -> State {transitingTo.Id}");
            })
            .OnGoToAfterReduce((parsingState, symbolAhead, transitingTo) =>
            {
                var (parsedSymbols, parsedSymbolsItemSets) = parsingState;
                var parsedSymbolsDescription = string.Join(", ", parsedSymbols);
                
                Console.WriteLine($"[{Format(parsedSymbols)}] {symbolAhead} -> GO TO AFTER REDUCE -> State {transitingTo.Id}");
            })
            .OnAccept((parsingState, transitingTo) =>
            {
                var (parsedSymbols, parsedSymbolsItemSets) = parsingState;
                var parsedSymbolsDescription = string.Join(", ", parsedSymbols);
                
                Console.WriteLine($"[{Format(parsedSymbols)}] -> ACCEPT -> State {transitingTo.Id}");
            });

        private static string Format<TSymbol>(IEnumerable<ParsedSymbol<TSymbol>> parsedSymbols) =>
            string.Join(", ", parsedSymbols.Select(symbol => symbol.Symbol));
    }
}