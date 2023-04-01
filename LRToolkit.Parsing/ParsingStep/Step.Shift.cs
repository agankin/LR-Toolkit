namespace LRToolkit.Parsing
{
    internal abstract partial class Step<TSymbol> where TSymbol : notnull
    {
        public class Shift : Step<TSymbol>
        {
            public Shift(
                Symbol<TSymbol> symbolAhead,
                ParserStateReducer<TSymbol> parserStateReducer,
                ItemSet<TSymbol> afterSymbolFullItemSet)
                : base(symbolAhead, parserStateReducer, afterSymbolFullItemSet)
            {
            }

            public override TResult Map<TResult>(
                Func<Shift, TResult> mapShift,
                Func<Reduce, TResult> mapReduce,
                Func<Accept, TResult> mapAccept)
            {
                return mapShift(this);
            }
        }
    }
}