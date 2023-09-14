using Optional;

namespace LRToolkit.Lexing;

public delegate Option<Lexem<TToken>> LexemParser<TToken>(TextInput input);