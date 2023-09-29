using LRToolkit.GrammarDefinition;

namespace Playground;

using static GrammarUtils;
using static LispToken;

public static class LispGrammar
{
    public static Grammar<LispToken> Create()
    {
        var grammarBuilder = new GrammarBuilder<LispToken>(START)
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