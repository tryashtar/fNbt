using System;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;

namespace fNbt {
    public class ReadOnlyNbtTag : IReadableNbt {
        private readonly NbtTag Tag;
        public ReadOnlyNbtTag(NbtTag tag)
        {
            Tag = tag;
        }

        public NbtTagType TagType => Tag.TagType;
        public bool IsList => Tag.IsList;
        public NbtTagType ListType => Tag.ListType;
    }
}
