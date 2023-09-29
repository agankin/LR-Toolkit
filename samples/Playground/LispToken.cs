namespace Playground;

public enum LispToken
{
    START = 1,

    LIST,

    OPEN,
    CLOSE,
    
    ELEMENTS,
    ELEMENT,
    ATOM,

    WHITESPACE
}