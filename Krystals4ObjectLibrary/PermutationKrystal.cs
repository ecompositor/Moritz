using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using System.IO;

namespace Krystals4ObjectLibrary
{
    public sealed partial class PermutationKrystal : Krystal
    {
        #region constructors
        /// <summary>
        /// constructor for loading a permuted krystal from a file
        /// </summary>
        /// <param name="filepath"></param>
        public PermutationKrystal(string filepath)
            : base(filepath)
        {
            using(XmlReader r = XmlReader.Create(filepath))
            {
                K.ReadToXmlElementTag(r, "permutation"); // check that this is a permutation (the other checks have been done in base()
                for(int attr = 0; attr < r.AttributeCount; attr++)
                {
                    r.MoveToAttribute(attr);
                    switch(r.Name)
                    {
                        case "source":
                            this._sourceInputFilename = r.Value;
                            break;
                        case "axis":
                            this._axisInputFilename = r.Value;
                            break;
                        case "contour":
                            this._contourInputFilename = r.Value;
                            break;
                        case "pLevel":
                            this._permutationLevel = uint.Parse(r.Value);
                            break;
                        case "sortFirst":
                            this._sortFirst = bool.Parse(r.Value);
                            break;
                    }
                }
            }
            string sourceInputFilepath = K.KrystalsFolder + @"\" + _sourceInputFilename;
            string axisInputFilepath = K.KrystalsFolder + @"\" + _axisInputFilename;
            string contourInputFilepath = K.KrystalsFolder + @"\" + _contourInputFilename;

            _sourceInputKrystal = new PermutationSourceInputKrystal(sourceInputFilepath);
            _axisInputKrystal = new AxisInputKrystal(axisInputFilepath);
            _contourInputKrystal = new ContourInputKrystal(contourInputFilepath);

            _permutationNodeList = GetPermutationNodeList();
        }
        /// <summary>
        /// Constructor used when creating a new permuted krystal (which has no strands yet).
        /// </summary>
        /// <param name="sourcePath">The file path to the source krystal</param>
        /// <param name="axisPath">The file path to the axis input</param>
        /// <param name="contourPath">The file path to the contour input</param>
        /// <param name="level">The level at which permuting is done</param>
        /// <param name="sortFirst">Whether or not to sort the original into ascending order before permuting</param>
        public PermutationKrystal(string sourcePath, string axisPath, string contourPath, int permutationLevel, bool sortFirst)
            : base()
        {
            _sourceInputFilename = Path.GetFileName(sourcePath);
            _axisInputFilename = Path.GetFileName(axisPath);
            _contourInputFilename = Path.GetFileName(contourPath);

            _sourceInputKrystal = new PermutationSourceInputKrystal(sourcePath);
            _axisInputKrystal = new AxisInputKrystal(axisPath);
            _contourInputKrystal = new ContourInputKrystal(contourPath);

           _permutationLevel = (uint)permutationLevel;

            // Throws an exception on failure.
            CheckInputs(_sourceInputKrystal, _axisInputKrystal.Level, _contourInputKrystal.Level, _permutationLevel);
 
            _sortFirst = sortFirst;

            _permutationNodeList = GetPermutationNodeList();

        }
        #endregion
 
        private void CheckInputs(
            PermutationSourceInputKrystal sourceKrystal, 
            uint axisLevel,
            uint contourLevel,
            uint permutationLevel)
        {
            uint sourceLevel = sourceKrystal.Level;

            if(permutationLevel < 1)
            {
                throw new PermutationLevelException(
                    "Illegal inputs:\n" +
                    "The permutation level must be greater than zero.");
            }

            if(permutationLevel > sourceLevel)
            {
                throw new PermutationLevelException(
                    "Illegal inputs:\n" +
                    "The permutation level must be less than or equal to the level of the source input.");
            }

            if(permutationLevel <= axisLevel)
            {
                throw new PermutationLevelException(
                    "Illegal inputs:\n" +
                    "The permutation level must be greater than the level of the axis input.");
            }

            if(permutationLevel <= contourLevel)
            {
                throw new PermutationLevelException(
                    "Illegal inputs:\n" +
                    "The permutation level must be greater than the level of the contour input.");
            }

            if(!sourceKrystal.IsPermutableAtLevel(permutationLevel))
            {
                throw new ApplicationException(
                        "Cannot permute the source krystal.\n\n" +
                        "It has more than 7 elements at the requested level.");
            }
        }

        /// <summary>
        /// The permutation node list has as many nodes as there are strands in the output krystal.
        /// Its values are displayed on each strand in the tree view of the krystal.
        ///  
        /// Each node has member values which allow the output to be constructed from the input krystal.
        /// If the permutation level is less than the level of the source krystal, 
        ///     permutationNode.sourceMomentNumber is set to the moment number (index + 1) of the source strand
        ///                                                                 to be copied to this position.
        /// If the permutation level is equal to the level of the source krystal,
        ///     permutationNode.axis and
        ///     permutationNode.contour are set to the values used for permuting the strand.
        /// </summary>
        /// <returns></returns>
        private List<PermutationNode> GetPermutationNodeList()
        {
            List<PermutationNode> nodeList = new List<PermutationNode>();
            List<int> axisList = GetSourceAlignedValues(_sourceInputKrystal, _permutationLevel, _axisInputKrystal);
            List<int> contourList = GetSourceAlignedValues(_sourceInputKrystal, _permutationLevel, _contourInputKrystal);

            if(_permutationLevel < _sourceInputKrystal.Level)
            {
                List<OuterSuperStrand> superStrands = GetOuterSuperStrands(_sourceInputKrystal, _permutationLevel);
                // set all nodes' sourceMoments
                List<int> sourceMoments =
                    GetSourceMoments(superStrands, axisList, contourList, _permutationLevel, _sortFirst);
                int moment = 1;
                foreach(Strand strand in _sourceInputKrystal.Strands)
                {
                    nodeList.Add(new PermutationNode(sourceMoments[moment - 1]));
                    moment++;
                }
            }
            else // _permutationLevel == _sourceInputKrystal.Level
            {
                // permutationNode.sortFirst,
                // permutationNode.axis and
                // permutationNode.contour are set to the values used for permuting the strand.
                int moment = 0;
                foreach(Strand strand in _sourceInputKrystal.Strands)
                {
                    nodeList.Add(new PermutationNode(axisList[moment],contourList[moment]));
                    moment++;
                }
            }
            return nodeList;
        }

        private List<OuterSuperStrand> GetOuterSuperStrands(PermutationSourceInputKrystal source, uint permutationLevel)
        {
            List<OuterSuperStrand> outerSuperStrands = new List<OuterSuperStrand>();
            int originalMomentNumber = 1;
            OuterSuperStrand outerSuperStrand = new OuterSuperStrand(permutationLevel);
            foreach(Strand strand in source.Strands)
            {
                StrandObj strandObj = new StrandObj(strand, originalMomentNumber++);
                if(strand.Level > 1 && strand.Level <= permutationLevel)
                {
                    outerSuperStrands.Add(outerSuperStrand);
                    outerSuperStrand = new OuterSuperStrand(permutationLevel);
                }
                outerSuperStrand.StrandObjs.Add(strandObj);
            }
            outerSuperStrands.Add(outerSuperStrand);

            foreach(OuterSuperStrand gStrand in outerSuperStrands)
            {
                gStrand.SetInnerSuperStrands();
            }

            return outerSuperStrands;
        }

        /// <summary>
        /// Returns a list containing the original moment number (in the source krystal) of each strand in the
        /// permuted krystal. (The list is in order of strands in the permuted krystal.)
        /// </summary>
        private List<int> GetSourceMoments(
            List<OuterSuperStrand> outerSuperStrands,
            List<int> axisList, // has as many values as there are strands in the source file at permutationLevel and below            
            List<int> contourList, // has as many values as there are strands in the source file at permutationLevel and below
            uint permutationLevel,
            bool sortFirst)
        {
            Debug.Assert(axisList.Count == contourList.Count);
            Debug.Assert(axisList.Count == outerSuperStrands.Count);

            List<int> sourceMoments = new List<int>();

            for(int i = 0; i < outerSuperStrands.Count; i++ )
            {
                int density = outerSuperStrands[i].InnerSuperStrands.Count;
                if( density == 1 )
                    sourceMoments.Add(outerSuperStrands[i].StrandObjs[0].OriginalMomentNumber);
                else
                {
                    int[] contour = K.Contour(density, contourList[i], axisList[i]);
                    List<int> permutedSourceMoments = outerSuperStrands[i].PermutedSourceMoments(sortFirst, contour);
                    sourceMoments.AddRange(permutedSourceMoments);
                }
            }

            return sourceMoments;
        }

        /// <summary>
        /// Returns a list of values from the InputKrystal (axis or contour) aligned to the outerSuperStrands.
        /// The returned List has as many values as there are strands in the source file at permutationLevel and below.
        /// The permutationLevel must be less than or equal to the level of the source krystal, and greater than the
        /// level of the InputKrystal.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="krystal"></param>
        /// <returns></returns>
        private List<int> GetSourceAlignedValues(PermutationSourceInputKrystal source, uint permutationLevel, InputKrystal acKrystal)
        {
            Debug.Assert(permutationLevel <= source.Level && permutationLevel > acKrystal.Level);
            List<int> alignedValues = new List<int>();
            int acStrandIndex = 0;
            int acValueIndex = 0;
            int acValueLevel = (int)acKrystal.Level + 1;
            foreach(Strand strand in source.Strands)
            {
                if(permutationLevel == 1)
                {
                    AddValue(alignedValues, acKrystal, acStrandIndex, acValueIndex);
                    break;
                }
                else if(strand.Level <= permutationLevel)
                {
                    if(strand.Level == 1)
                    {
                        acStrandIndex = 0;
                        acValueIndex = 0;
                    }
                    else if(strand.Level == acValueLevel)
                    {
                        acValueIndex++;
                    }
                    else if(strand.Level < acValueLevel)
                    {
                        acStrandIndex++;
                        acValueIndex = 0;
                    }
                    AddValue(alignedValues, acKrystal, acStrandIndex, acValueIndex);
                }
                // else if(strand.Level > permutationLevel) do nothing
            }
            return alignedValues;
        }

        private void AddValue(List<int> alignedValues, InputKrystal acKrystal, int acStrandIndex, int acValueIndex)
        {
            Debug.Assert(acStrandIndex < acKrystal.Strands.Count);
            Debug.Assert(acValueIndex < acKrystal.Strands[acStrandIndex].Values.Count);

            alignedValues.Add((int)(acKrystal.Strands[acStrandIndex].Values[acValueIndex]));
        }

        #region public functions

        public void Permute()
        {
            this.Strands.Clear();
            if(_permutationLevel < _sourceInputKrystal.Level)
            {
                this.Strands.Clear();
                foreach(PermutationNode pn in _permutationNodeList)
                {
                    Debug.Assert(pn.SourceStrandNumber > 0 && pn.SourceStrandNumber <= _sourceInputKrystal.Strands.Count);

                    this.Strands.Add(_sourceInputKrystal.Strands[pn.SourceStrandNumber - 1]);
                }
            }
            else // _permutationLevel == _sourceInputKrystal.Level
            {
                for(int i = 0; i < _sourceInputKrystal.Strands.Count; i++)
                {
                    PermutationNode pn = this._permutationNodeList[i];
                    Strand strand = _sourceInputKrystal.Strands[i];
                    int density = strand.Values.Count;
                    int[] contour = K.Contour(density, pn.Contour, pn.Axis);
                    if(_sortFirst)
                        strand.Values.Sort();
                    Strand newStrand = new Strand(strand.Level);
                    for(int j = 0; j < density; j++)
                    {
                        newStrand.Values.Add(strand.Values[contour[j] - 1]);
                    }
                    this.Strands.Add(newStrand);
                }
            }
            this.Update(Strands);
        }

        /// <summary>
        /// Saves the krystal to a file.
        /// Throws an exception if an error occurs.
        /// The 'overwrite' argument is always ignored.
        /// </summary>
        public override void Save(bool overwrite)
        {
            string pathname;
            if(string.IsNullOrEmpty(_name) || overwrite == false) // this is a new or changed krystal, so generate a new name
            {
                if(_name != null && _name == "") // used by Krystals4
                    _name = base.GetNameOfEquivalentSavedKrystal("pk");
                if(string.IsNullOrEmpty(_name)) // null is used by Moritz
                {
                    int fileIndex = 1;
                    do
                    {
                        _name = String.Format("pk{0}({1})-{2}{3}",
                            _level, _maxValue, fileIndex, K.KrystalFilenameSuffix);
                        pathname = K.KrystalsFolder + @"\" + _name;
                        fileIndex++;
                    } while(File.Exists(pathname));
                }
                else pathname = K.KrystalsFolder + @"\" + _name;
            }
            else pathname = K.KrystalsFolder + @"\" + _name;

            if(MaxValueHasChanged())
            {
                File.Delete(pathname);
                _name = "";
                Save(false); // false means do not overwrite. This (recursive) call saves under a new name
            }
            else
            {
                XmlWriter w = base.BeginSaveKrystal(); // disposed of in EndSaveKrystal
                #region save heredity info
                w.WriteStartElement("permutation");
                w.WriteAttributeString("source", this._sourceInputFilename);
                w.WriteAttributeString("axis", this._axisInputFilename);
                w.WriteAttributeString("contour", this._contourInputFilename);
                w.WriteAttributeString("pLevel", this._permutationLevel.ToString());
                if(this._sortFirst == true)
                    w.WriteAttributeString("sortFirst", "true");
                else
                    w.WriteAttributeString("sortFirst", "false");
                w.WriteEndElement(); // permutation
                #endregion
                base.EndSaveKrystal(w); // saves the strands, closes the document, disposes of w
            }
        }

        private bool MaxValueHasChanged()
        {
            string[] segs = _name.Split('(', ')');
            uint max = uint.Parse(segs[1]);
            if(max == MaxValue)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Re-permutes this krystal (using the existing input krystals), then saves it, overwriting the existing file.
        /// All the krystals in the krystals folder are rebuilt, when one of them has been changed.
        /// </summary>
        public override void Rebuild()
        {
            this.Permute();
            Save(true); // true means overwrite
        }
        #endregion public functions
        #region Properties
        public string SourceInputFilename { get { return _sourceInputFilename; } }
        public string AxisInputFilename { get { return _axisInputFilename; } }
        public string ContourInputFilename { get { return _contourInputFilename; } }
        public PermutationSourceInputKrystal SourceInputKrystal
        {
            get { return _sourceInputKrystal; }
            //set { _sourceInputKrystal = value; }
        }
        public AxisInputKrystal AxisInputKrystal
        {
            get { return _axisInputKrystal; }
            //set { _axisInputKrystal = value; }
        }
        public ContourInputKrystal ContourInputKrystal
        {
            get { return _contourInputKrystal; }
            //set { _contourInputKrystal = value; }
        }
        public uint PermutationLevel
        {
            get { return _permutationLevel; }
            //set { _permutationLevel = value; }
        }
        public bool SortFirst
        {
            get { return _sortFirst; }
            //set { _sortFirst = value; }
        }

        public List<PermutationNode> PermutationNodeList
        {
            get { return _permutationNodeList; }
            set { _permutationNodeList = value; }
        }
        #endregion Properties

        #region private variables
        private string _sourceInputFilename;
        private string _axisInputFilename;
        private string _contourInputFilename;
        private uint _permutationLevel;
        private bool _sortFirst;
        private PermutationSourceInputKrystal _sourceInputKrystal;
        private AxisInputKrystal _axisInputKrystal;
        private ContourInputKrystal _contourInputKrystal;
        private List<PermutationNode> _permutationNodeList;
        #endregion private variables
    }
    /// <summary>
    /// The source InputKrystal for a permutation has this class
    /// </summary>
    public sealed class PermutationSourceInputKrystal : InputKrystal
    {
        public PermutationSourceInputKrystal(string filepath)
            : base(filepath)
        {
        }
    }
    /// <summary>
    /// This class contains permutation parameters, and is used to build the _permutationNodeList for a permutation.
    /// The _permutationNodeList is a list of permutationNodes parallel to the strands in the PermutationKrystal
    /// (there is one PermutationNode per strand).
    /// If the permutation level is less than the level of the source krystal, the sourceStrandNumber is used to copy
    /// the original strand from the source krystal (axis and contour values are used to find the sourceStrandNumber,
    /// but are ignored at the strand level).
    /// Otherwise, if the permutation level is equal to the source krystal's level, the sourceStrandNumber is zero, and
    /// the axis and contour parameters are used to find the permutation.
    /// </summary>
    public class PermutationNode : TreeNode
    {
        public PermutationNode(int sourceStrandNumber)
        {
            SourceStrandNumber = sourceStrandNumber;
        }
        public PermutationNode(int axis, int contour)
        {
            Axis = axis;
            Contour = contour;
        }
        public int SourceStrandNumber
        {
            get{ return _sourceStrandNumber;}
            set
            {
                _sourceStrandNumber = value;
                _axis = 0;
                _contour = 0;
            }
        }
        public int Axis
        {
            get{ return _axis;}
            set
            {
                _axis = value;
                _sourceStrandNumber = 0;
            }
        }

        public int Contour
        {
            get { return _contour; }
            set
            {
                _contour = value;
                _sourceStrandNumber = 0;
            }
        }

        private int _sourceStrandNumber = 0;
        private int _axis = 0;
        private int _contour = 0;
    }

    public class PermutationLevelException : ApplicationException
    {
        public PermutationLevelException(string errorMessage)
            : base(errorMessage)
        { }
    }

}

