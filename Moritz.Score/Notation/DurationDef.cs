
using System.Diagnostics;

namespace Moritz.Score.Notation
{
    public abstract class DurationDef
    {
        protected DurationDef(int msDuration)
        {
            _msDuration = msDuration;
        }

        public abstract IUniqueDef DeepClone();

        public virtual int MsDuration { get { return _msDuration; } set { _msDuration = value; } }
        protected int _msDuration = 0;
    }
}
