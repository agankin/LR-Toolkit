using System.Text.RegularExpressions;
using Optional;

namespace LRToolkit.Lexing;

public class RegexParser
{
    private readonly Regex _regex;

    public RegexParser(Regex regex) => _regex = regex;

    public static RegexParser Regex(string regexPattern)
    {
        var regex = new Regex(regexPattern);
        return new(regex);
    }
    
    public LexemParser<TSymbol> Is<TSymbol>(TSymbol symbol)
    {
        return input =>
        {
            var match = _regex.Match(input.Text, input.Position);  
            return GetLexem(match, symbol);
        };
    }

    private static Option<Lexem<TSymbol>> GetLexem<TSymbol>(Match match, TSymbol symbol)
        => match.Success ? MakeLexem(symbol, match) : Option.None<Lexem<TSymbol>>();

    private static Option<Lexem<TSymbol>> MakeLexem<TSymbol>(TSymbol symbol, Match match)
    {
        var (value, position) = (match.Value, match.Index);
        var lexem = new Lexem<TSymbol>(symbol, value, position);

        return lexem.Some();
    }
}