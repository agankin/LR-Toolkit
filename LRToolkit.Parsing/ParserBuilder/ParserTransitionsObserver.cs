using DFAutomaton;

namespace LRToolkit.Parsing
{
    public delegate void ShiftListener<TSymbol>(
        ParsingState<TSymbol> parsingState,
        Symbol<TSymbol> symbolAhead,
        IState<Symbol<TSymbol>, ParsingState<TSymbol>> toState)
        where TSymbol : notnull;

    public delegate void ReduceListener<TSymbol>(
        ParsingState<TSymbol> parsingState,
        Symbol<TSymbol> symbolAhead,
        Item<TSymbol> reducedItem,
        IState<Symbol<TSymbol>, ParsingState<TSymbol>> toState)
        where TSymbol : notnull;

    public delegate void GoToAfterReduceListener<TSymbol>(
        ParsingState<TSymbol> parsingState,
        Symbol<TSymbol> reducedToSymbol,
        IState<Symbol<TSymbol>, ParsingState<TSymbol>> toState)
        where TSymbol : notnull;

    public delegate void AcceptListener<TSymbol>(
        ParsingState<TSymbol> parsingState,
        IState<Symbol<TSymbol>, ParsingState<TSymbol>> toState)
        where TSymbol : notnull;

    public record ParserTransitionsObserver<TSymbol>(
        ShiftListener<TSymbol> ShiftListener,
        ReduceListener<TSymbol> ReduceListener,
        GoToAfterReduceListener<TSymbol> GoToAfterReduceListener,
        AcceptListener<TSymbol> AcceptListener)
        where TSymbol : notnull
    {
        public static ParserTransitionsObserver<TSymbol> Create() =>
            new ParserTransitionsObserver<TSymbol>(
                ShiftListener: (_, _, _) => { },
                ReduceListener: (_, _, _, _) => { },
                GoToAfterReduceListener: (_, _, _) => { },
                AcceptListener: (_, _) => { });

        public ParserTransitionsObserver<TSymbol> OnShift(ShiftListener<TSymbol> onShift) =>
            this with { ShiftListener = ShiftListener + onShift };

        public ParserTransitionsObserver<TSymbol> OnReduce(ReduceListener<TSymbol> onReduce) =>
            this with { ReduceListener = ReduceListener + onReduce };

        public ParserTransitionsObserver<TSymbol> OnGoToAfterReduce(GoToAfterReduceListener<TSymbol> onGoToAfterReduce)
        {
            return this with
            {
                GoToAfterReduceListener = GoToAfterReduceListener + onGoToAfterReduce
            };
        }

        public ParserTransitionsObserver<TSymbol> OnAccept(AcceptListener<TSymbol> onAccept) =>
            this with { AcceptListener = AcceptListener + onAccept };
    }
}