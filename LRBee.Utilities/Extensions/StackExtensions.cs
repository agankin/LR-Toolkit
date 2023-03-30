using System.Collections.Immutable;

namespace LRBee.Utilities
{
    public static class StackExtensions
    {
        public static ImmutableStack<TValue> PopSkip<TValue>(this ImmutableStack<TValue> stack, int count)
        {
            if (count < 0)
                throw new InvalidOperationException("Number of skipped elements must be positive number.");

            return Enumerable.Range(0, count).Aggregate(stack, (stack, _) => stack.Pop());
        }

        public static (ImmutableStack<TValue>, IReadOnlyList<TValue>) Pop<TValue>(
            this ImmutableStack<TValue> stack,
            int count)
        {
            if (count <= 0)
                throw new InvalidOperationException("Number of popped elements must be greater zero.");

            return Enumerable.Range(0, count)
                .Aggregate(
                    (stack, ImmutableList<TValue>.Empty),
                    (state, idx) =>
                    {
                        var (curStack, curPopped) = state;

                        var nextStack = curStack.Pop(out var value);
                        var nextPopped = curPopped.Insert(0, value);

                        return (nextStack, nextPopped);
                    });
        }
    }
}