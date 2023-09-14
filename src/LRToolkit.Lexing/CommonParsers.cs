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
    
    public LexemParser<TToken> Is<TToken>(TToken token)
    {
        return input =>
        {
            var match = _regex.Match(input.Text, input.Position);  
            return GetLexem(match, token);
        };
    }

    private static Option<Lexem<TToken>> GetLexem<TToken>(Match match, TToken token)
        => match.Success ? MakeLexem(token, match) : Option.None<Lexem<TToken>>();

    private static Option<Lexem<TToken>> MakeLexem<TToken>(TToken token, Match match)
    {
        var (value, position) = (match.Value, match.Index);
        var lexem = new Lexem<TToken>(token, value, position);

        return lexem.Some();
    }
}