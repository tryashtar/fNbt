using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fNbt
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public abstract class NbtArrayTag : NbtTag, IEquatable<NbtArrayTag>
    {
        public abstract bool ValueEquals(NbtArrayTag other);
        public override bool Equals(object obj)
        {
            if (obj is not NbtValueTag tag)
                return false;
            return Equals(tag);
        }

        public bool Equals(NbtArrayTag other)
        {
            if (this == other)
                return true;
            if (other.Name != this.Name)
                return false;
            if (other.TagType != this.TagType)
                return false;
            return ValueEquals(other);
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
