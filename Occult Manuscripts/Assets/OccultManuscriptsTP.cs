using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KeepCoding;
public class OccultManuscriptsTP : TPScript<OccultManuscriptsScr> {

    public override IEnumerator ForceSolve()
    {
        yield return null;
        Module.GlyphSelectables[Module._solutionIndex].OnInteract();
    }

    public override IEnumerator Process(string command)
    {
        int inputtedGlyph;

        if (!int.TryParse(command, out inputtedGlyph))
            yield break;
        yield return null;
        if (inputtedGlyph < 1 || inputtedGlyph > 16)
        {
            yield return SendToChatError("Input out of range!");
            yield break;
        }
        Module.GlyphSelectables[inputtedGlyph = 1].OnInteract();
    }
}
