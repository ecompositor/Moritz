using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;

using Moritz.Globals;

namespace Moritz.Score.Midi
{
    public class BasicMidiChordDef
    {
        public BasicMidiChordDef()
        {
        }

        public BasicMidiChordDef(int msDuration, byte? bank, byte? patch, bool hasChordOff, List<byte> pitches, List<byte> velocities)
        {
            MsDuration = msDuration;
            BankIndex = bank;
            PatchIndex = patch;
            HasChordOff = hasChordOff;
            Notes = pitches;
            Velocities = velocities;
        }
        public BasicMidiChordDef(XmlReader r)
        {
            // The reader is at the beginning of a "score:basicChord" element
            // inside a "score:basicChords" element inside a "score:midiChord" element.
            Debug.Assert(r.Name == "score:basicChord" && r.IsStartElement() && r.AttributeCount > 0);
            int nAttributes = r.AttributeCount;
            for(int i = 0; i < nAttributes; i++)
            {
                r.MoveToAttribute(i);
                switch(r.Name)
                {
                    //case "msPosition": // msPositions are not written to the <midiDefs> section of an SVG-MIDI file.
                    //    MsPosition = int.Parse(r.Value);
                    //    break;
                    case "msDuration":
                        MsDuration = int.Parse(r.Value);
                        break;
                    case "bank":
                        BankIndex = byte.Parse(r.Value);
                        break;
                    case "patch":
                        PatchIndex = byte.Parse(r.Value);
                        break;
                    case "hasChordOff":
                        // HasChordOff is true if this attribute is not present
                        byte val = byte.Parse(r.Value);
                        if(val == 0)
                            HasChordOff = false;
                        else
                            HasChordOff = true;
                        break;
                    case "notes":
                        Notes = M.StringToByteList(r.Value, ' ');
                        break;
                    case "velocities":
                        Velocities = M.StringToByteList(r.Value, ' ');
                        break;
                }
            }
        }

        public void WriteSVG(XmlWriter w)
        {
            w.WriteStartElement("score", "basicChord", null);

            //if(writeMsPosition) // positions are not written to the midiDefs section of an SVG-MIDI file
            //    w.WriteAttributeString("msPosition", MsPosition.ToString());
            w.WriteAttributeString("msDuration", MsDuration.ToString());
            if(BankIndex != null && BankIndex != M.DefaultBankIndex)
                w.WriteAttributeString("bank", BankIndex.ToString());
            if(PatchIndex != null)
                w.WriteAttributeString("patch", PatchIndex.ToString());
            if(HasChordOff == false)
                w.WriteAttributeString("hasChordOff", "0");
            if(Notes != null)
                w.WriteAttributeString("notes", M.ByteListToString(Notes));
            if(Velocities != null)
                w.WriteAttributeString("velocities", M.ByteListToString(Velocities));

            w.WriteEndElement();
        }

        //public int MsPosition = 0;
        public int MsDuration = 0;

        public List<byte> Notes = new List<byte>(); // A string of midiPitch numbers separated by spaces.
        public List<byte> Velocities = new List<byte>(); // A string of midi velocity values separated by spaces.
        public byte? BankIndex = null; // optional. If null, bank commands are not sent
        public byte? PatchIndex = null; // optional. If null, patch commands are not sent
        public bool HasChordOff = true;
    }
}
