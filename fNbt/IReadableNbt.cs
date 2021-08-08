using System;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;

namespace fNbt
{
    public interface IReadableNbt
    {
        NbtTagType TagType { get; }
        bool IsList { get; }
        NbtTagType ListType { get; }
    }
}
