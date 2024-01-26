using DFAutomaton.Utils;
using LRToolkit.Lexing;
using LRToolkit.Parsing;
using LRToolkit.Utilities;
using Optional.Unsafe;
using Playground;

var lexer = LispLexer.Create();

var grammar = LispGrammar.Create();
var lr1Behavior = new LR1ParserBuilderBehavior<LispSymbol>(grammar);
var parserOrError = ParserBuilder<LispSymbol>.Build(grammar, lr1Behavior, ParserObserver.Configure);

parserOrError.Map(parser =>
{
    var formattedGraph = StatesGraphFormatter<Symbol<LispSymbol>, ParsingState<LispSymbol>>.Format(parser.StartState);
    Console.WriteLine(formattedGraph);

    Console.Write("Expr> ");
    var text = Console.ReadLine() ?? string.Empty;
    var textInput = new TextInput(text);

    var lexems = lexer.GetLexems(textInput).Select(lexemOption => lexemOption.ValueOrFailure())
        .Where(lexem => lexem.Symbol != LispSymbol.WHITESPACE);
    foreach (var lexem in lexems)
        Console.WriteLine(lexem);

    Console.WriteLine("Run Parser:");
    parser.Run(lexems).Match(
        startSymbol =>
        {
            
        },
        error =>
        {
            Console.WriteLine($"ERROR: {error.Type}");
            Console.WriteLine($"    FROM STATE: {error.State}");
            Console.WriteLine($"    BY TRANSITION: {error.Transition}");
        });

    return new Nothing();
})
.Or(error =>
{
    Console.WriteLine($"Parser building error: {error}");
    return new Nothing();
});

Console.Write("To exit press any key...");
Console.ReadKey(true);