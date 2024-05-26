using PureMonads;

namespace LRToolkit.Lexing;

public delegate Option<Lexem<TSymbol>> LexemParser<TSymbol>(TextInput input);