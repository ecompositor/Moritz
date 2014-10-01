﻿
using System.Collections.Generic;

namespace Moritz.Symbols
{
    /// <summary>
    /// Public values are in viewbox pixel units.
    /// </summary>
    public class PageFormat
    {
        public PageFormat()
        {
        }

        public readonly int ViewBoxMagnification = 8;

        #region paper size
        public float Right { get { return RightVBPX; } }
        public float Bottom 
        { 
            get 
            { 
                int nGaps = (int)( BottomVBPX / Gap);
                return nGaps * Gap;
            }
        }
        public float ScreenRight { get { return RightVBPX / ViewBoxMagnification; } }
        public float ScreenBottom { get { return BottomVBPX / ViewBoxMagnification; } }

        public string PaperSize; // default
        public bool IsLandscape = false;
        public float RightVBPX = 0;
        public float BottomVBPX = 0;
        public readonly float HorizontalPixelsPerMillimeter = 3.4037F; // on my computer (December 2010).
        public readonly float VerticalPixelsPerMillimeter = 2.9464F; // on my computer (December 2010).
        #endregion

        #region title size and position
        public float Page1ScreenTitleY { get { return Page1TitleY / ViewBoxMagnification; } }
        public float Page1TitleHeight;
        public float Page1AuthorHeight;
        public float Page1TitleY;
        #endregion

        #region frame
        public float LeftScreenMarginPos { get { return LeftMarginPos / ViewBoxMagnification; } }
        public float FirstPageFrameHeight { get { return BottomMarginPos - TopMarginPage1; } }
        public float OtherPagesFrameHeight { get { return BottomMarginPos - TopMarginOtherPages; } }
        public float TopMarginPage1;
        public float TopMarginOtherPages;
        public float RightMarginPos;
        public float LeftMarginPos;
        public float BottomMarginPos;
        #endregion

        #region website links
        /// <summary>
        /// the text of the link to the "about" file
        /// </summary>
        public string AboutLinkText;
        /// <summary>
        /// the "about" file's URL.
        /// </summary>
        public string AboutLinkURL;
        #endregion

        #region notation
        /// <summary>
        /// All written scores set the ChordSymbolType to one of the values in M.ChordTypes.
        /// M.ChordTypes does not include "none", because it is used to populate ComboBoxes.
        /// The value "none" is used to signal that there is no written score. It is used by
        /// AudioButtonsControl inside palettes, just to play a sound.
        /// </summary>
        public string ChordSymbolType = "none";
        #region standard chord notation options
        /// <summary>
        /// This value is used to find the duration class of DurationSymbols
        /// </summary>
        public int MinimumCrotchetDuration;
        public bool BeamsCrossBarlines;
        #endregion
        public float StafflineStemStrokeWidth;
        public float Gap;
        /// <summary>
        /// The view box pixel distance between staves when they are not vertically justified.
        /// </summary>
        public float DefaultDistanceBetweenStaves;
        /// <summary>
        /// The view box pixel distance between systems when they are not vertically justified.
        /// </summary>
        public float DefaultDistanceBetweenSystems;
        public List<List<byte>> OutputVoiceIndicesPerStaff = null;
        public List<List<byte>> InputVoiceIndicesPerStaff = null;        
        public List<string> ClefsList = null;
        public List<int> StafflinesPerStaff = null;
        public List<int> StaffGroups = null;
        public List<string> LongStaffNames = null;
        public List<string> ShortStaffNames = null;

        public List<int> SystemStartBars = null;
        public int DefaultNumberOfBarsPerSystem { get { return 5; } }
        public float StaffNameFontHeight { get { return Gap * 2.5F; } }
        /// <summary>
        /// the normal font size on staves having Gap sized spaces (after experimenting with cLicht). 
        /// </summary>
        public float MusicFontHeight { get { return (Gap * 4) * 0.98F; } }
        public float CautionaryNoteheadsFontHeight { get { return MusicFontHeight * 0.8F; } }
        public float InputStavesSizeFactor { get { return 1.5F; } }
        public float BarlineStrokeWidth { get { return StafflineStemStrokeWidth * 2F; } }
        public float NoteheadExtenderStrokeWidth { get { return StafflineStemStrokeWidth * 3.4F; } }
        public float BeamThickness { get { return Gap * 0.42F; } }
        #endregion

    }
}
