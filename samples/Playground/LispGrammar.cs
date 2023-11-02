using LRToolkit.GrammarDefinition;

namespace Playground;

using static GrammarUtils;
using static LispSymbol;

public static class LispGrammar
{
    public static Grammar<LispSymbol> Create()
    {
        var grammarBuilder = new GrammarBuilder<LispSymbol>(START)
        {
            [START] = _( ELEMENT ),

            [ELEMENT] = _( LIST ),
            [ELEMENT] = _( ATOM ),

            [LIST] = _( OPEN, ELEMENTS, CLOSE ),
            
            [ELEMENTS] = _( ELEMENT, ELEMENTS ),
            [ELEMENTS] = _( ELEMENT ),
        };

        return grammarBuilder.Build();
    }
}