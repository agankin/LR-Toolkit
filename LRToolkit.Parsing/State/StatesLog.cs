using System.Collections.Immutable;

namespace LRBee.Parsing
{
    public readonly record struct StatesLog<TSymbol> where TSymbol : notnull
    {
        public static readonly StatesLog<TSymbol> Empty =
            new StatesLog<TSymbol>
            {
                States = ImmutableList<State<TSymbol>>.Empty,
                StatesSet = ImmutableHashSet<State<TSymbol>>.Empty
            };

        public State<TSymbol> this[int stepsBack] => States[stepsBack];

        public ImmutableList<State<TSymbol>> States { get; private init; }

        private ImmutableHashSet<State<TSymbol>> StatesSet { get; init; }

        public bool Contains(State<TSymbol> state) => StatesSet.Contains(state);

        public StatesLog<TSymbol> Push(State<TSymbol> state) =>
            this with
            {
                States = States.Insert(0, state),
                StatesSet = StatesSet.Add(state)
            };
    }
}