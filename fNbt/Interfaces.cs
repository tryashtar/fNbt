using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fNbt
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface INbtTag
    {
        string Name { get; set; }
        NbtTagType TagType { get; }
        INbtContainer Parent { get; }
    }

    public interface INbtContainer : INbtTag, IReadOnlyCollection<INbtTag>
    {
        bool CanAdd(NbtTagType type);
        void Add(NbtTag tag);
        void AddRange(IEnumerable<NbtTag> tags);
        void Insert(int index, NbtTag tag);
        void Clear();
        bool Contains(NbtTag tag);
        bool Remove(NbtTag tag);
        int IndexOf(NbtTag tag);
        INbtTag this[int tagIndex] { get; }
    }

    public interface INbtByte : INbtTag
    {
        byte Value { get; set; }
    }

    public interface INbtShort : INbtTag
    {
        short Value { get; set; }
    }

    public interface INbtInt : INbtTag
    {
        int Value { get; set; }
    }

    public interface INbtLong : INbtTag
    {
        long Value { get; set; }
    }

    public interface INbtFloat : INbtTag
    {
        float Value { get; set; }
    }

    public interface INbtDouble : INbtTag
    {
        double Value { get; set; }
    }

    public interface INbtString : INbtTag
    {
        string Value { get; set; }
    }

    public interface INbtByteArray : INbtTag
    {
        byte[] Value { get; set; }
    }

    public interface INbtIntArray : INbtTag
    {
        int[] Value { get; set; }
    }

    public interface INbtLongArray : INbtTag
    {
        long[] Value { get; set; }
    }

    public interface INbtCompound : INbtContainer
    {
        IEnumerable<INbtTag> Tags { get; }
        bool Contains(string name);
        bool Remove(string name);
        int IndexOf(string name);
    }

    public interface INbtList : INbtContainer, IReadOnlyList<INbtTag>
    {
        NbtTagType ListType { get; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
