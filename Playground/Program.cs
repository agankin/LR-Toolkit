using LRBee.GrammarDefinition;
using LRBee.Parsing;

using static LRBee.GrammarDefinition.GrammarUtils;
using static Symbol;

var grammar = new GrammarBuilder<Symbol>(S)
{
    [S] = Prod(A, A),
    [A] = Prod(a, A),
    [A] = Prod(b)
}.Build();

var parserOrError = ParserBuilder<Symbol>.Build(grammar);
Console.WriteLine(parserOrError);

public enum Symbol
{
    S,
    A,
    a,
    b
}