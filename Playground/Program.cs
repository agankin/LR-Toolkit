using DFAutomaton.Utils;
using LRToolkit.GrammarDefinition;
using LRToolkit.Parsing;
using LRToolkit.Utilities;
using Playground;

using static LRToolkit.GrammarDefinition.GrammarUtils;
using static Symbol;

var grammar = new GrammarBuilder<Symbol>(S)
{
    [S] = Prod(A, A),
    [A] = Prod(a, A),
    [A] = Prod(b)
}.Build();

var lookaheadFactory = new OneLookaheadFactory<Symbol>(grammar);
var parserOrError = ParserBuilder<Symbol>.Build(grammar, lookaheadFactory, ParserObserver.Configure);

parserOrError.Map(parser =>
{
    var formattedGraph = StatesGraphFormatter<Symbol<Symbol>, ParsingState<Symbol>>.Format(parser.StartState);
    Console.WriteLine(formattedGraph);

    Console.WriteLine("Run Parser:");
    parser.Run(new[] { a, b, a, b })
       .MatchNone(error => Console.WriteLine($"{error.State} {error.Transition} -> Error: {error.Type}"));

    return new VoidValue();
})
.Or(error =>
{
    Console.WriteLine($"Error: {error}");
    return new VoidValue();
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