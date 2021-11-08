using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using KeepCoding;
using Rnd = UnityEngine.Random;
public class OccultManuscriptsScr : ModuleScript
{
    public Sprite[] Glyphs;
    public KMSelectable[] GlyphSelectables;

    internal int _solutionIndex;
    // Use this for initialization
    private void Start()
    {
        GlyphSelectables.Assign(
    onHighlight: (KMSelectable s) => s.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0, 0, 1),
    onHighlightEnded: (KMSelectable s) => s.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1),
    onInteract: (KMSelectable s) => PressGlyph(s)
    );
    }

    public override void OnActivate()
    {
        List<List<List<Sprite>>> glyphGrid = Enumerable.Range(0, 9)
    .Select(sbgdIx => Enumerable.Range(0, 4)
        .Select(rowIx => Enumerable.Range(0, 4)
            .Select(colIx => Glyphs[colIx + 4 * (sbgdIx % 3) + 12 * rowIx + 48 * (sbgdIx / 3)])
            .ToList())
        .ToList())
    .ToList();
        int gridIndex = Get<KMBombInfo>().GetSerialNumberNumbers().Last() - 1;
        if (gridIndex == -1)
            gridIndex = Get<KMBombInfo>().GetBatteryCount() % 9;
        List<List<Sprite>> chosenGrid = glyphGrid[gridIndex];
        List<int> pemutation = Enumerable.Range(0, 4).ToList().Shuffle();
        List<int> keyGlyphIndicesPlusSolution = new List<int>() { pemutation[0], pemutation[1] + 4, pemutation[2] + 8, pemutation[3] + 12 }.Shuffle();
        List<int> keyGlyphIndices = keyGlyphIndicesPlusSolution.Take(3).ToList();
        Debug.Log(keyGlyphIndices.Join());
        _solutionIndex = keyGlyphIndicesPlusSolution[3];
        List<Sprite> keyGlyphs = new List<Sprite>() { null, null, null };
        for (int i = 0; i < 3; i++)
            keyGlyphs[i] = chosenGrid[keyGlyphIndices[i] / 4][keyGlyphIndices[i] % 4];
        glyphGrid.RemoveAt(gridIndex);
        List<Sprite> junkGlyphOptions = glyphGrid.SelectMany(i => i.SelectMany(j => j)).ToList();
        List<Sprite> moduleGlyphs = junkGlyphOptions.Shuffle().Take(13).Concat(keyGlyphs).ToList().Shuffle();
        for (int i = 0; i < 16; i++)
            GlyphSelectables[i].GetComponent<SpriteRenderer>().sprite = moduleGlyphs[i];
        List<int> loggingIndices = moduleGlyphs.Select(i => int.Parse(i.name.Where(c => char.IsDigit(c)).Join("")) + 1).ToList();
        List<int> keyLoggingIndices = keyGlyphIndices.Select(i => i + 1).ToList();
        Log("The glyphs on the module are located at positions {0}, and {1} in the table in reading order.", loggingIndices.Take(15).Join(", "), loggingIndices[15]);
        Log("The chosen subgrid is {0}.", gridIndex + 1);
        Log("The key glyphs are located at positions {0}, and {1} in the subgrid. Therefore, you should press glyph {2}.", keyLoggingIndices.Take(2).Join(", "), keyLoggingIndices[2], _solutionIndex + 1);
    }
    private bool PressGlyph(KMSelectable glyph)
    {
        if (!IsSolved)
        {
            int index = Array.IndexOf(GlyphSelectables, glyph);
            glyph.AddInteractionPunch(0.5f);
            Log("You pressed glyph {0}.", index + 1);
            if (index == _solutionIndex)
            {
                Log("That was correct. Module solved.");
                PlaySound("Solve");
                Solve();
            }
            else
            {
                Log("That was incorrect. Strike!");
                Strike();
                OnActivate();
            }
        }
        return false;
    }
}