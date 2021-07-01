using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fNbt
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public abstract class NbtContainerTag : NbtTag, IList<NbtTag>
    {
        public bool IsReadOnly => false;

        public abstract int Count { get; }
        public abstract bool Contains(NbtTag item);
        public abstract int IndexOf(NbtTag item);
        public abstract void CopyTo(NbtTag[] array, int arrayIndex);
        public abstract IEnumerator<NbtTag> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public abstract bool CanAdd(NbtTagType type);
        public abstract void Add(NbtTag item);
        public abstract void Insert(int index, NbtTag item);
        public abstract bool Remove(NbtTag item);
        public abstract void RemoveAt(int index);
        public abstract void AddRange(IEnumerable<NbtTag> items);
        public abstract void Clear();

        public IEnumerable<NbtTag> GetAllTags()
        {
            foreach (var tag in this)
            {
                yield return tag;
                if (tag is NbtContainerTag container)
                {
                    foreach (var sub in container.GetAllTags())
                    {
                        yield return sub;
                    }
                }
            }
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
