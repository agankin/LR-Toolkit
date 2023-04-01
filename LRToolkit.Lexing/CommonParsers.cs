using System.Text.RegularExpressions;
using Optional;

namespace LRBee.Lexing
{
    public static class CommonParsers
    {
        public static TokenParser<TToken> Regex<TToken>(string regexPattern, TToken token)
        {
            var regex = new Regex(regexPattern);

            return input => regex.Match(input.Text, input.Position).GetLexem(token);
        }

        private static Option<Lexem<TToken>> GetLexem<TToken>(this Match match, TToken token)
            => match.Success
                ? MakeLexem(token, match)
                : Option.None<Lexem<TToken>>();

        private static Option<Lexem<TToken>> MakeLexem<TToken>(TToken token, Match match)
        {
            var (value, position) = (match.Value, match.Index);
            var lexem = new Lexem<TToken>(token, value, position);

            return Option.Some(lexem);
        }
    }
}