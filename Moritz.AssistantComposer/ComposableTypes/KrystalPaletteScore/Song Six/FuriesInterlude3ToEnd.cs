﻿using System.Collections.Generic;
using System.Diagnostics;
using System;

using Moritz.Score;
using Moritz.Score.Midi;
using Krystals4ObjectLibrary;

namespace Moritz.AssistantComposer
{
    /// <summary>
    /// The Algorithm for Song 6.
    /// This will develope as composition progresses...
    /// </summary>
    internal partial class SongSixAlgorithm : MidiCompositionAlgorithm
    {
        /// <summary>
        /// The arguments are all complete to the end of Verse 3
        /// </summary>
        private void GetFuriesInterlude3ToEnd(VoiceDef furies1, VoiceDef furies2, VoiceDef furies3, VoiceDef furies4,
            Clytemnestra clytemnestra, VoiceDef wind1, VoiceDef wind2, VoiceDef wind3, List<PaletteDef> palettes)
        {
            
            PaletteDef f2ChirpsPalette = palettes[11];
            PaletteDef f3ChirpsPalette = palettes[10];
            PaletteDef f4SongsPalette = palettes[9];

            Dictionary<string,int> msPositions = new Dictionary<string,int>();
            msPositions.Add("interlude3", wind3[40].MsPosition);
            msPositions.Add("verse4", clytemnestra[174].MsPosition);
            msPositions.Add("verse4Escaped", clytemnestra[236].MsPosition);
            msPositions.Add("interlude4", wind1[57].MsPosition);
            msPositions.Add("verse5", clytemnestra[269].MsPosition);
            msPositions.Add("verse5Calls", clytemnestra[288].MsPosition);
            msPositions.Add("postlude", clytemnestra[289].MsPosition);
            msPositions.Add("endOfPiece", wind1.EndMsPosition);

            int durationFromInterlude3StartToEnd = msPositions["endOfPiece"] - msPositions["interlude3"];

            VoiceDef f1Finale = GetFuries1Finale(palettes, msPositions, clytemnestra, wind3);

            //VoiceDef f2e = new VoiceDef(f2ChirpsPalette, krystal);
            //Debug.Assert(f2e.EndMsPosition <= durationFromInterlude3StartToEnd);
            //f2e.StartMsPosition = furies1[33].MsPosition + (furies1[33].MsDuration / 2);
            //Debug.Assert(f2e.EndMsPosition <= endOfPieceMsPosition);
            //furies2.InsertInRest(f2e);

            //VoiceDef f3e = new VoiceDef(f3ChirpsPalette, krystal);
            //Debug.Assert(f3e.EndMsPosition <= durationFromInterlude3StartToEnd);
            //f3e.StartMsPosition = wind3[42].MsPosition; // bar 63
            //Debug.Assert(f3e.EndMsPosition <= endOfPieceMsPosition);
            //furies3.InsertInRest(f3e);

            //VoiceDef f4e = new VoiceDef(f4SongsPalette, krystal);
            //Debug.Assert(f4e.EndMsPosition <= durationFromInterlude3StartToEnd);
            //f4e.StartMsPosition = wind3[42].MsPosition; // bar 63
            //Debug.Assert(f4e.EndMsPosition <= endOfPieceMsPosition);
            //furies4.InsertInRest(f4e);

            furies1.InsertInRest(f1Finale);

            AdjustFuries1FinaleAlignments(furies1, clytemnestra, wind3);
        }

        private VoiceDef GetFuries1Finale(List<PaletteDef> palettes, Dictionary<string, int> msPositions, Clytemnestra clytemnestra, VoiceDef wind3)
        {
            PaletteDef f1Interlude3Palette = palettes[12];
            PaletteDef f1Interlude4Palette = palettes[16];
            PaletteDef f1PostludePalette = palettes[20];

            //PermutationKrystal krystal = new PermutationKrystal("C://Moritz/krystals/krystals/pk4(12)-2.krys");
            ExpansionKrystal krystal = new ExpansionKrystal("C://Moritz/krystals/krystals/xk3(12.12.1)-1.krys");
            List<int> strandIndices = new List<int>();
            int index = 0;
            for(int i = 0; i < krystal.Strands.Count; ++i)
            {
                strandIndices.Add(index);
                index += krystal.Strands[i].Values.Count;
            }

            VoiceDef f1Interlude3Verse4e = GetF1Interlude3Verse4Escaped(f1Interlude3Palette, krystal, strandIndices, msPositions);
            VoiceDef f1Verse4eVerse5 = GetF1Verse4EscapedVerse5Calls(f1Interlude4Palette, krystal, strandIndices, msPositions);
            VoiceDef f1Postlude = GetF1Postlude(f1PostludePalette, krystal, strandIndices, msPositions);

            VoiceDef furies1Finale = f1Interlude3Verse4e;

            furies1Finale.AddRange(f1Verse4eVerse5);
            furies1Finale.AddRange(f1Postlude);

            if(furies1Finale[furies1Finale.Count - 1] is UniqueMidiRestDef)
            {
                furies1Finale.RemoveAt(furies1Finale.Count - 1);
            }

            if(furies1Finale[furies1Finale.Count - 1].MsPosition + furies1Finale[furies1Finale.Count - 1].MsDuration > msPositions["endOfPiece"])
            {
                furies1Finale.RemoveAt(furies1Finale.Count - 1);
            }

            AdjustFuriesFinalePitchWheelDeviations(furies1Finale);


            return furies1Finale;
        }

        private void AdjustFuries1FinaleAlignments(VoiceDef furies1Finale, Clytemnestra clytemnestra, VoiceDef wind3)
        {
            furies1Finale.AlignObjectAtIndex(25, 84, 102, clytemnestra[196].MsPosition);
            furies1Finale.AlignObjectAtIndex(102, 106, 140, clytemnestra[242].MsPosition);
            furies1Finale.AlignObjectAtIndex(106, 140, 163, wind3[61].MsPosition);
            furies1Finale.AlignObjectAtIndex(140, 163, 197, wind3[65].MsPosition);
            furies1Finale.AlignObjectAtIndex(163, 197, 214, clytemnestra[269].MsPosition);
        }

        /// <summary>
        /// This should work for furies 1-3, and maybe even 4!
        /// </summary>
        /// <param name="f13"></param>
        private void AdjustFuriesFinalePitchWheelDeviations(VoiceDef f13)
        {
            double furies1StartPwdValue = 5, furies1EndPwdValue = 28;
            double pwdfactor = Math.Pow(furies1EndPwdValue / furies1StartPwdValue, (double)1 / f13.Count); // f13.Count'th root of furies1EndPwdValue/furies1StartPwdValue -- the last pwd should be furies1EndPwdValue

            for(int i = 0; i < f13.Count; ++i)
            {
                f13[i].PitchWheelDeviation = (int)(furies1StartPwdValue * (Math.Pow(pwdfactor, i)));
            }
        }

        private VoiceDef GetF1Interlude3Verse4Escaped(PaletteDef f1Int3Palette, ExpansionKrystal krystal, List<int> strandIndices, Dictionary<string, int> msPositions)
        {
            VoiceDef f13 = new VoiceDef(f1Int3Palette, krystal);

            List<int> f1eStrandDurations = GetStrandDurations(f13, strandIndices);

            int extraTime = 1000;
            int diff = extraTime / f13.Count;
            for(int i = f13.Count - 1; i > 0; --i)
            {
                if(strandIndices.Contains(i))
                {
                    UniqueMidiRestDef umrd = new UniqueMidiRestDef(f13[i].MsPosition, f1eStrandDurations[strandIndices.IndexOf(i)] + extraTime);
                    extraTime -= diff;
                    f13.Insert(i, umrd);
                }
            }

            f13.StartMsPosition = msPositions["interlude3"];

            f13.RemoveBetweenMsPositions(msPositions["verse4Escaped"], int.MaxValue);

            if(f13[f13.Count - 1] is UniqueMidiRestDef)
            {
                f13[f13.Count - 1].MsDuration = msPositions["verse4Escaped"] - f13[f13.Count - 1].MsPosition;
            }

            return f13;
        }

        private VoiceDef GetF1Verse4EscapedVerse5Calls(PaletteDef f1Int4Palette, ExpansionKrystal krystal, List<int> strandIndices, Dictionary<string, int> msPositions)
        {
            VoiceDef f14 = new VoiceDef(f1Int4Palette, krystal);

            List<int> f1eStrandDurations = GetStrandDurations(f14, strandIndices);

            int extraTime = 500;
            int diff = extraTime / f14.Count;
            for(int i = f14.Count - 1; i > 0; --i)
            {
                if(strandIndices.Contains(i))
                {
                    UniqueMidiRestDef umrd = new UniqueMidiRestDef(f14[i].MsPosition, f1eStrandDurations[strandIndices.IndexOf(i)] + extraTime);
                    extraTime -= diff;
                    f14.Insert(i, umrd);
                }
            }

            f14.StartMsPosition = msPositions["verse4Escaped"];
            f14.RemoveBetweenMsPositions(msPositions["verse5Calls"], int.MaxValue);

            if(f14[f14.Count - 1] is UniqueMidiRestDef)
            {
                f14[f14.Count - 1].MsDuration = msPositions["postlude"] - f14[f14.Count - 1].MsPosition;
            }

            return f14;
        }


        private VoiceDef GetF1Postlude(PaletteDef f1PostludePalette, ExpansionKrystal krystal, List<int> strandIndices, Dictionary<string, int> msPositions)
        {
            VoiceDef f1p = new VoiceDef(f1PostludePalette, krystal);

            List<int> f1eStrandDurations = GetStrandDurations(f1p, strandIndices);

            for(int i = f1p.Count - 1; i > 0; --i)
            {
                if(strandIndices.Contains(i))
                {
                    UniqueMidiRestDef umrd = new UniqueMidiRestDef(f1p[i].MsPosition, f1eStrandDurations[strandIndices.IndexOf(i)] / 4);
                    f1p.Insert(i, umrd);
                }
            }

            f1p.StartMsPosition = msPositions["postlude"];
            f1p.RemoveBetweenMsPositions(msPositions["endOfPiece"], int.MaxValue);

            return f1p;
        }

        /// <summary>
        /// voiceDef contains the UniqueMidiChordDefs defined by a krystal, and nothing else.
        /// </summary>
        /// <param name="voiceDef"></param>
        /// <param name="strandIndices"></param>
        /// <returns></returns>
        private List<int> GetStrandDurations(VoiceDef voiceDef, List<int> strandIndices)
        {
            List<int> strandDurations = new List<int>();
            int duration;
            for(int i = 1; i < strandIndices.Count; ++i)
            {
                duration = 0;
                for(int j = strandIndices[i-1]; j < strandIndices[i]; ++j)
                {
                    duration += voiceDef[j].MsDuration;
                }
                strandDurations.Add(duration);
            }
            duration = 0;
            for(int i = strandIndices[strandIndices.Count - 1]; i < voiceDef.Count; ++i)
            {
                duration += voiceDef[i].MsDuration;
            }
            strandDurations.Add(duration);
            return strandDurations;
        }
    }
}
