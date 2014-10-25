using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

using Krystals4ObjectLibrary;
using Moritz.Spec;
using Moritz.Symbols;
using Moritz.Palettes;
using Moritz.Algorithm;
using Moritz.Algorithm.PaletteDemo;
using Moritz.Algorithm.Study2c3_1;
using Moritz.Algorithm.SongSix;
using Moritz.Algorithm.Study3Sketch1;
using Moritz.Algorithm.Study3Sketch2;
using Moritz.Xml;

namespace Moritz.Composer
{
    public partial class ComposableSvgScore : SvgScore
    {
        /// <summary>
        /// The argument (title) will be printed as the title of the score on its first page.
        /// The title is the root name of an .mkss file containing the settings for a particular score of an algorithm.
        /// The .mkss file should be in a subfolder of the assistant performer's 'scores' folder. (This ensures that the CLicht
        /// font loads correctly.)
        /// The Assistant Composer creates the score in the same folder as its .mkss file. The folder can have any name.
        /// 
        /// Note that
        ///     1. Two scores can't have the same title but different algorithms.
        ///     2. Different scores that use the same algorithm can have the same title if they are created in different folders.
        ///        (Different .mkss files can have the same name if they are in different folders.)
        ///     3. More than one .mkss file can be put in the same folder. In this case, the diffent scores will all appear in
        ///        the Assistant Composer's 'scores' pop-up menu.
        /// 
        /// To add a new score and/or algorithm, simply add a new case to the switch below.
        /// </summary>
        public static CompositionAlgorithm Algorithm(string title)
        {
            CompositionAlgorithm algorithm = null;
            switch(title)
            {
                case "Study 2c3.1":
                    algorithm = new Study2c3_1Algorithm();
                    break;
                case "Song Six":
                    algorithm = new SongSixAlgorithm();
                    break;
                case "Study 3 sketch 1":
                    algorithm = new Study3Sketch1Algorithm();
                    break;
                case "Study 3 sketch 2":
                    algorithm = new Study3Sketch2Algorithm();
                    break;
                case "Study 3":
                    algorithm = new Study3Sketch2Algorithm();
                    break;
                case "paletteDemo":
                    algorithm = new PaletteDemoAlgorithm();
                    break;
                default:
                    MessageBox.Show("Error in ComposableScoreAlgorithm.cs:\n\n" +
                                    "Score title not found in switch in ComposableSvgScore.Algorithm(...).\n" +
                                    "(Add a new case statement.)",
                                    "Program Error");
                    break;
            }
            return algorithm;
        }

    }
}
