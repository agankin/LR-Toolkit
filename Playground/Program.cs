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

var parserOrError = ParserBuilder<Symbol>.Build(grammar, ParserObserver.Configure);

parserOrError.Map(parser =>
{
    var formattedGraph = StatesGraphFormatter<Symbol<Symbol>, ParsingState<Symbol>>.Format(parser.StartState);
    Console.WriteLine(formattedGraph);

    Console.WriteLine("Run Parser:");
    parser.Run(new[] { a, b, a, b }).MatchNone(error => Console.WriteLine($"Error: {error.Type}"));

    return new VoidValue();
});

public enum Symbol
{
    S,
    A,
    a,
    b
}