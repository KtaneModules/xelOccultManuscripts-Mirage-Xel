using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using KeepCoding;
using Rnd = UnityEngine.Random;

public class OccultManuscriptsScr : ModuleScript {
    public Sprite[] Glyphs;
    public KMSelectable[] GlyphSelectables;

    internal int _solutionIndex;
    // Use this for initialization
	void Start () {
        GlyphSelectables.Assign(
    onHighlight: (KMSelectable s ) => s.GetComponent<SpriteRenderer>().color = new Color(0.8f,0,0,1),
    onHighlightEnded: (KMSelectable s) => s.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1)
    ); 

	}

    public override void OnActivate()
    {
        List<List<List<Sprite>>> glyphGrid = ChunkBy(Glyphs.ToList(), 16).Select(x => ChunkBy(x, 4)).ToList();
        int gridIndex = Get<KMBombInfo>().GetSerialNumberNumbers().Last() - 1;
        if (gridIndex == -1)       
            gridIndex = Get<KMBombInfo>().GetBatteryCount() % 9;
        List<List<Sprite>> chosenGrid = glyphGrid[gridIndex];
        List<int> pemutation = Enumerable.Range(0, 4).ToList().Shuffle();
        List<int> keyGlyphIndicesPlusSolution = new List<int>() { pemutation[0], pemutation[1] + 4, pemutation[2] + 8, pemutation[3] + 12}.Shuffle();
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
    }
    
    public static List<List<T>> ChunkBy<T> (List<T> source, int chunkSize)
    {
        return source
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / chunkSize)
            .Select(x => x.Select(v => v.Value).ToList())
            .ToList();
    }
}
