using DFAutomaton.Utils;
using LRToolkit.GrammarDefinition;
using LRToolkit.Parsing;
using LRToolkit.Utilities;
using Playground;

using static LRToolkit.GrammarDefinition.GrammarUtils;
using static Symbol;

var grammar = new GrammarBuilder<Symbol>(S)
{
    [S] = Prod(A),
    [A] = Prod(a, A),
    [A] = Prod(a)
}.Build();

var lookaheadFactory = new OneLookaheadFactory<Symbol>(grammar);
var parserOrError = ParserBuilder<Symbol>.Build(grammar, lookaheadFactory, ParserObserver.Configure);

parserOrError.Map(parser =>
{
    var formattedGraph = StatesGraphFormatter<Symbol<Symbol>, ParsingState<Symbol>>.Format(parser.StartState);
    Console.WriteLine(formattedGraph);

    Console.WriteLine("Run Parser:");
    parser.Run(new[] { a, a })
        .MatchNone(error =>
        {
            Console.WriteLine($"ERROR: {error.Type}");
            Console.WriteLine($"    FROM STATE: {error.State}");
            Console.WriteLine($"    BY TRANSITION: {error.Transition}");
        });

    return VoidValue.Instance;
})
.Or(error =>
{
    Console.WriteLine($"Error: {error}");
    return VoidValue.Instance;
});

Console.Write("To exit press any key...");
Console.ReadKey(true);

public enum Symbol
{
    S,
    A,
    a,
    b
}