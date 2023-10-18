using Optional;

namespace LRToolkit.Lexing;

public delegate Option<Lexem<TSymbol>> LexemParser<TSymbol>(TextInput input);