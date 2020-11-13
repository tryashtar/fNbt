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
        protected abstract void DoAdd(NbtTag item);
        protected abstract void DoInsert(int index, NbtTag item);
        protected abstract bool DoRemove(NbtTag item);
        protected abstract void DoRemoveAt(int index);
        protected abstract void DoAddRange(IEnumerable<NbtTag> items);
        protected abstract void DoClear();

        public void Add(NbtTag item)
        {
            PerformAction(new DescriptionHolder("Add {0} to {1}", this, item),
                () => DoAdd(item),
                () => DoRemove(item)
            );
        }
        public void AddRange(IEnumerable<NbtTag> items)
        {
            var list = items.ToList();
            PerformAction(new DescriptionHolder("Add {0} to {1}", this, items),
                () => DoAddRange(items),
                () => { foreach (var item in list) { DoRemove(item); } }
            );
        }
        public void Clear()
        {
            var list = this.ToList();
            PerformAction(new DescriptionHolder("Clear all tags from {0}", this),
                () => DoClear(),
                () => DoAddRange(list)
            );
        }
        public void Insert(int index, NbtTag item)
        {
            PerformAction(new DescriptionHolder("Insert {0} into {1} at index {2}", this, item, index),
                () => DoInsert(index, item),
                () => DoRemoveAt(index)
            );
        }
        public bool Remove(NbtTag item)
        {
            int index = IndexOf(item);
            if (index == -1)
                return false;
            return PerformAction(new DescriptionHolder("Remove {0} from {1}", item, this),
                () => DoRemove(item),
                () => { DoInsert(index, item); }
            );
        }
        public void RemoveAt(int index)
        {
            var item = this[index];
            PerformAction(new DescriptionHolder("Remove {0} from {1}", item, this),
                () => DoRemoveAt(index),
                () => DoInsert(index, item)
            );
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
