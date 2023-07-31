using DFAutomaton;

namespace LRToolkit.Parsing;

public delegate void ShiftListener<TSymbol>(
    ParsingState<TSymbol> parsingState,
    Symbol<TSymbol> symbol,
    long toStateId)
    where TSymbol : notnull;

public delegate void ReduceListener<TSymbol>(
    ParsingState<TSymbol> parsingState,
    Symbol<TSymbol> symbol,
    Item<TSymbol> reducedItem,
    long toStateId)
    where TSymbol : notnull;

public delegate void AcceptListener<TSymbol>(
    ParsingState<TSymbol> parsingState,
    long toStateId)
    where TSymbol : notnull;

public record ParserTransitionsObserver<TSymbol>(
    ShiftListener<TSymbol> ShiftListener,
    ReduceListener<TSymbol> ReduceListener,
    AcceptListener<TSymbol> AcceptListener)
    where TSymbol : notnull
{
    public static ParserTransitionsObserver<TSymbol> Create() =>
        new ParserTransitionsObserver<TSymbol>(
            ShiftListener: (_, _, _) => { },
            ReduceListener: (_, _, _, _) => { },
            AcceptListener: (_, _) => { });

    public ParserTransitionsObserver<TSymbol> OnShift(ShiftListener<TSymbol> onShift) =>
        this with { ShiftListener = ShiftListener + onShift };

    public ParserTransitionsObserver<TSymbol> OnReduce(ReduceListener<TSymbol> onReduce) =>
        this with { ReduceListener = ReduceListener + onReduce };

    public ParserTransitionsObserver<TSymbol> OnAccept(AcceptListener<TSymbol> onAccept) =>
        this with { AcceptListener = AcceptListener + onAccept };
}