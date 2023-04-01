namespace LRBee.Parsing
{
    internal abstract partial class Step<TSymbol> where TSymbol : notnull
    {
        private Step(
            Symbol<TSymbol> symbolAhead,
            ParserStateReducer<TSymbol> parserStateReducer,
            ItemSet<TSymbol> afterSymbolFullItemSet)
        {
            SymbolAhead = symbolAhead;
            ParserStateReducer = parserStateReducer;
            AfterSymbolFullItemSet = afterSymbolFullItemSet;
        }

        public Symbol<TSymbol> SymbolAhead { get; }

        public ParserStateReducer<TSymbol> ParserStateReducer { get; }

        public ItemSet<TSymbol> AfterSymbolFullItemSet { get; }

        public static Step<TSymbol> CreateShiftStep(
            Symbol<TSymbol> symbolAhead,
            ParserStateReducer<TSymbol> parserStateReducer,
            ItemSet<TSymbol> afterSymbolFullItemSet)
        {
            return new Shift(symbolAhead, parserStateReducer, afterSymbolFullItemSet);
        }

        public static Step<TSymbol> CreateReduceStep(
            Symbol<TSymbol> symbolAhead,
            ParserStateReducer<TSymbol> parserStateReducer,
            ItemSet<TSymbol> afterSymbolFullItemSet,
            Item<TSymbol> reducedItem)
        {
            return new Reduce(symbolAhead, parserStateReducer, afterSymbolFullItemSet, reducedItem);
        }

        public static Step<TSymbol> CreateAcceptStep(
            Symbol<TSymbol> symbolAhead,
            ParserStateReducer<TSymbol> parserStateReducer,
            ItemSet<TSymbol> afterSymbolFullItemSet)
        {
            return new Accept(symbolAhead, parserStateReducer, afterSymbolFullItemSet);
        }

        public abstract TResult Map<TResult>(
            Func<Shift, TResult> mapShift,
            Func<Reduce, TResult> mapReduce,
            Func<Accept, TResult> mapAccept);
    }
}