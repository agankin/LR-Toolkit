namespace LRToolkit.Parsing
{
    internal abstract partial class Step<TSymbol> where TSymbol : notnull
    {
        public class Reduce : Step<TSymbol>
        {
            public Reduce(
                Symbol<TSymbol> symbolAhead,
                ParserStateReducer<TSymbol> parserStateReducer,
                ItemSet<TSymbol> afterSymbolFullItemSet,
                Item<TSymbol> reducedItem)
                : base(symbolAhead, parserStateReducer, afterSymbolFullItemSet)
            {
                ReducedItem = reducedItem;
            }

            public Item<TSymbol> ReducedItem { get; }

            public override TResult Map<TResult>(
                Func<Shift, TResult> mapShift,
                Func<Reduce, TResult> mapReduce,
                Func<Accept, TResult> mapAccept)
            {
                return mapReduce(this);
            }
        }
    }
}