using System.Collections.Immutable;

namespace LRToolkit.Utilities;

public static class ImmutableStackExtensions
{
    public static ImmutableStack<TValue> PopSkip<TValue>(this ImmutableStack<TValue> stack, int count)
    {
        if (count < 0)
            throw new InvalidOperationException("Number of skipped elements must be greater or equal zero.");

        return Enumerable.Range(0, count).Aggregate(stack, (stack, _) => stack.Pop());
    }

    public static ImmutableStack<TValue> Pop<TValue>(this ImmutableStack<TValue> stack, int count, out IReadOnlyCollection<TValue> poppedValues)
    {
        if (count < 0)
            throw new InvalidOperationException("Number of popped elements must be greater or equal zero.");

        ImmutableStack<TValue> nextStack;
        (nextStack, poppedValues) = Enumerable.Range(0, count)
            .Aggregate(
                (Stack: stack, PoppedValues: ImmutableList<TValue>.Empty),
                (state, _) => (state.Stack.Pop(out var value), state.PoppedValues.Insert(0, value)));

        return nextStack;
    }

    public static ImmutableStack<TValue> Peek<TValue>(this ImmutableStack<TValue> stack, out TValue peekedValue)
    {
        peekedValue = stack.Peek();
        return stack;
    }
}