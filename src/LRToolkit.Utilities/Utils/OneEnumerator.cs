using System.Collections;

namespace LRToolkit.Utilities;

public class OneEnumerator<TElement> : IEnumerator<TElement>
{
    private readonly TElement _element;
    private EnumerationState _state = EnumerationState.Initial;

    public OneEnumerator(TElement element) => _element = element;

    public TElement Current => _state == EnumerationState.AtElement
        ? _element
        : throw new InvalidOperationException("Enumerator is not in a state of having current element.");

    object? IEnumerator.Current => Current;

    public bool MoveNext() =>
        (_state = _state switch
        {
            EnumerationState.Initial => EnumerationState.AtElement,
            EnumerationState.AtElement => EnumerationState.Finished,
            _ => EnumerationState.Finished
        }) == EnumerationState.AtElement;

    public void Reset() => _state = EnumerationState.Initial;

    public void Dispose() { }

    private enum EnumerationState
    {
        Initial = 1,

        AtElement,

        Finished
    }
}