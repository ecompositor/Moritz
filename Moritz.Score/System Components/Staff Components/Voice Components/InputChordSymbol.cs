using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Moritz.Globals;
using Moritz.Score.Midi;
using Moritz.Score.Notation;

namespace Moritz.Score
{
    public class InputChordSymbol : ChordSymbol
    {
        public InputChordSymbol(Voice voice, UniqueInputChordDef umcd, int minimumCrotchetDurationMS, float fontSize)
            : base(voice, umcd.MsDuration, umcd.MsPosition, minimumCrotchetDurationMS, fontSize)
        {
            //_uniqueMidiChordDef = umcd;
            //MidiChordDef midiChordDef = umcd as MidiChordDef;
            //if(midiChordDef != null)
            //{
            //    SetHeads(midiChordDef.MidiHeadSymbols);

            //    if(midiChordDef.OrnamentNumberSymbol != 0)
            //    {
            //        AddOrnamentSymbol("~" + midiChordDef.OrnamentNumberSymbol.ToString());
            //    }

            //    if(midiChordDef.Lyric != null)
            //    {
            //        TextInfo textInfo = new TextInfo(midiChordDef.Lyric, "Arial", (float)(FontHeight / 2F), TextHorizAlign.center);
            //        Lyric lyric = new Lyric(this, textInfo);
            //        DrawObjects.Add(lyric);
            //    }
            //}
        }

        /// <summary>
        /// Writes this inputChord's content
        /// </summary>
        /// <param name="w"></param>
        protected override void WriteContent(SvgWriter w, string idNumber)
        {
            _uniqueInputChordDef.WriteSvg(w, idNumber);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("input chord  ");
            sb.Append(InfoString);
            return sb.ToString();
        }

        public UniqueInputChordDef UniqueInputChordDef { get { return _uniqueInputChordDef; } }
        protected UniqueInputChordDef _uniqueInputChordDef = null;
    }
}