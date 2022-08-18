using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace fNbt {
    /// <summary> A tag containing a set of other named tags. Order is not guaranteed. </summary>
    public sealed class NbtCompound : NbtContainerTag {
        /// <summary> Type of this tag (Compound). </summary>
        public override NbtTagType TagType {
            get { return NbtTagType.Compound; }
        }

        readonly OrderedDictionary<string, NbtTag> tags = new OrderedDictionary<string, NbtTag>();


        /// <summary> Creates an empty unnamed NbtByte tag. </summary>
        public NbtCompound() {}


        /// <summary> Creates an empty NbtByte tag with the given name. </summary>
        /// <param name="tagName"> Name to assign to this tag. May be <c>null</c>. </param>
        public NbtCompound([CanBeNull] string tagName) {
            name = tagName;
        }


        /// <summary> Creates an unnamed NbtByte tag, containing the given tags. </summary>
        /// <param name="tags"> Collection of tags to assign to this tag's Value. May not be null </param>
        /// <exception cref="ArgumentNullException"> <paramref name="tags"/> is <c>null</c>, or one of the tags is <c>null</c>. </exception>
        /// <exception cref="ArgumentException"> If some of the given tags were not named, or two tags with the same name were given. </exception>
        public NbtCompound([NotNull] IEnumerable<NbtTag> tags)
            : this(null, tags) {}


        /// <summary> Creates an NbtByte tag with the given name, containing the given tags. </summary>
        /// <param name="tagName"> Name to assign to this tag. May be <c>null</c>. </param>
        /// <param name="tags"> Collection of tags to assign to this tag's Value. May not be null </param>
        /// <exception cref="ArgumentNullException"> <paramref name="tags"/> is <c>null</c>, or one of the tags is <c>null</c>. </exception>
        /// <exception cref="ArgumentException"> If some of the given tags were not named, or two tags with the same name were given. </exception>
        public NbtCompound([CanBeNull] string tagName, [NotNull] IEnumerable<NbtTag> tags) {
            if (tags == null) throw new ArgumentNullException("tags");
            name = tagName;
            foreach (NbtTag tag in tags) {
                Add(tag);
            }
        }


        /// <summary> Creates a deep copy of given NbtCompound. </summary>
        /// <param name="other"> An existing NbtCompound to copy. May not be <c>null</c>. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="other"/> is <c>null</c>. </exception>
        public NbtCompound([NotNull] NbtCompound other) {
            if (other == null) throw new ArgumentNullException("other");
            name = other.name;
            foreach (NbtTag tag in other.tags.Values) {
                Add((NbtTag)tag.Clone());
            }
        }


        /// <summary> Gets or sets the tag with the specified name. May return <c>null</c>. </summary>
        /// <returns> The tag with the specified key. Null if tag with the given name was not found. </returns>
        /// <param name="tagName"> The name of the tag to get or set. Must match tag's actual name. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="tagName"/> is <c>null</c>; or if trying to assign null value. </exception>
        /// <exception cref="ArgumentException"> <paramref name="tagName"/> does not match the given tag's actual name;
        /// or given tag already has a Parent. </exception>
        public override NbtTag this[[NotNull] string tagName] {
            [CanBeNull]
            get { return Get<NbtTag>(tagName); }
            set {
                if (tagName == null) {
                    throw new ArgumentNullException("tagName");
                } else if (value == null) {
                    throw new ArgumentNullException("value");
                } else if (value.Parent != null) {
                    throw new ArgumentException("A tag may only be added to one compound/list at a time.");
                } else if (value == this) {
                    throw new ArgumentException("Cannot add tag to itself");
                } else if (value.Name != tagName) {
                    if (value.Name == null)
                        value.Name = tagName;
                    else
                        throw new ArgumentException("Given tag name must match tag's actual name.");
                }
                tags[tagName] = value;
                value.Parent = this;
            }
        }

        /// <summary> Gets or sets the tag at the specified index. </summary>
        /// <returns> The tag at the specified index. </returns>
        /// <param name="tagIndex"> The zero-based index of the tag to get or set. </param>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="tagIndex"/> is not a valid index in the NbtList. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="value"/> is <c>null</c>. </exception>
        [NotNull]
        public override NbtTag this[int tagIndex] {
            get { return tags[tagIndex]; }
            set { tags.RemoveAt(tagIndex); this[value.Name] = value; }
        }

        /// <summary> Gets the tag with the specified name. May return <c>null</c>. </summary>
        /// <param name="tagName"> The name of the tag to get. </param>
        /// <typeparam name="T"> Type to cast the result to. Must derive from NbtTag. </typeparam>
        /// <returns> The tag with the specified key. Null if tag with the given name was not found. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="tagName"/> is <c>null</c>. </exception>
        /// <exception cref="InvalidCastException"> If tag could not be cast to the desired tag. </exception>
        [CanBeNull]
        public T Get<T>([NotNull] string tagName) where T : NbtTag {
            if (tagName == null) throw new ArgumentNullException("tagName");
            if (tags.ContainsKey(tagName)) {
                return (T)tags[tagName];
            }
            return null;
        }


        /// <summary> Gets the tag with the specified name. May return <c>null</c>. </summary>
        /// <param name="tagName"> The name of the tag to get. </param>
        /// <returns> The tag with the specified key. Null if tag with the given name was not found. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="tagName"/> is <c>null</c>. </exception>
        /// <exception cref="InvalidCastException"> If tag could not be cast to the desired tag. </exception>
        [CanBeNull]
        public NbtTag Get([NotNull] string tagName) {
            if (tagName == null) throw new ArgumentNullException("tagName");
            if (tags.ContainsKey(tagName)) {
                return tags[tagName];
            }
            return null;
        }


        /// <summary> Gets the tag with the specified name. </summary>
        /// <param name="tagName"> The name of the tag to get. </param>
        /// <param name="result"> When this method returns, contains the tag associated with the specified name, if the tag is found;
        /// otherwise, null. This parameter is passed uninitialized. </param>
        /// <typeparam name="T"> Type to cast the result to. Must derive from NbtTag. </typeparam>
        /// <returns> true if the NbtCompound contains a tag with the specified name; otherwise, false. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="tagName"/> is <c>null</c>. </exception>
        /// <exception cref="InvalidCastException"> If tag could not be cast to the desired tag. </exception>
        public bool TryGet<T>([NotNull] string tagName, out T result) where T : NbtTag {
            if (tagName == null) throw new ArgumentNullException("tagName");
            if (tags.ContainsKey(tagName)) {
                result = (T)tags[tagName];
                return true;
            } else {
                result = null;
                return false;
            }
        }


        /// <summary> Gets the tag with the specified name. </summary>
        /// <param name="tagName"> The name of the tag to get. </param>
        /// <param name="result"> When this method returns, contains the tag associated with the specified name, if the tag is found;
        /// otherwise, null. This parameter is passed uninitialized. </param>
        /// <returns> true if the NbtCompound contains a tag with the specified name; otherwise, false. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="tagName"/> is <c>null</c>. </exception>
        /// <exception cref="InvalidCastException"> If tag could not be cast to the desired tag. </exception>
        public bool TryGet([NotNull] string tagName, out NbtTag result) {
            if (tagName == null) throw new ArgumentNullException("tagName");
            if (tags.ContainsKey(tagName)) {
                result = tags[tagName];
                return true;
            } else {
                result = null;
                return false;
            }
        }

        /// <summary> Whether a tag of the specified type can be added to this NbtCompound (always true). </summary>
        /// <param name="type"> The type to check. </param>
        /// <returns> Whether the type is valid in this NbtCompound. </returns>
        public override bool CanAdd(NbtTagType type) => true;

        /// <summary> Adds all tags from the specified collection to this NbtCompound. </summary>
        /// <param name="newTags"> The collection whose elements should be added to this NbtCompound. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="newTags"/> is <c>null</c>, or one of the tags in newTags is <c>null</c>. </exception>
        /// <exception cref="ArgumentException"> If one of the given tags was unnamed,
        /// or if a tag with the given name already exists in this NbtCompound. </exception>
        protected override void DoAddRange([NotNull] IEnumerable<NbtTag> newTags) {
            if (newTags == null) throw new ArgumentNullException("newTags");
            foreach (NbtTag tag in newTags) {
                DoAdd(tag);
            }
        }


        /// <summary> Determines whether this NbtCompound contains a tag with a specific name. </summary>
        /// <param name="tagName"> Tag name to search for. May not be <c>null</c>. </param>
        /// <returns> true if a tag with given name was found; otherwise, false. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="tagName"/> is <c>null</c>. </exception>
        [Pure]
        public bool Contains([NotNull] string tagName) {
            if (tagName == null) throw new ArgumentNullException("tagName");
            return tags.ContainsKey(tagName);
        }

        public void Sort(IComparer<NbtTag> sorter, bool recursive)
        {
            if (recursive)
            {
                var restore_sort = (NbtCompound)this.Clone();
                PerformAction(new DescriptionHolder("Sort {0}", this),
                     () => DoSort(sorter, true),
                     () => DoUnsortRecursive(restore_sort)
                 );
            }
            else
            {
                var restore_sort = this.Tags.ToList();
                PerformAction(new DescriptionHolder("Sort {0}", this),
                     () => DoSort(sorter, false),
                     () => DoUnsortRoot(restore_sort)
                 );
            }
        }

        private void DoUnsortRecursive(NbtCompound reference)
        {
            var order = Tags.OrderBy(x => reference.IndexOf(x.Name)).ToList();
            foreach (var tag in order)
            {
                if (tag is NbtCompound sub)
                    sub.DoUnsortRecursive((NbtCompound)reference[tag.Name]);
                else if (tag is NbtList list)
                    UnsortListChildren(list, (NbtList)reference[tag.Name]);
            }
            DoUnsortRoot(order);
        }

        private static void UnsortListChildren(NbtList list, NbtList reference)
        {
            if (list.ListType == NbtTagType.Compound)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    ((NbtCompound)list[i]).DoUnsortRecursive((NbtCompound)reference[i]);
                }
            }
            else if (list.ListType == NbtTagType.List)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    UnsortListChildren((NbtList)list[i], (NbtList)reference[i]);
                }
            }
        }

        private void DoUnsortRoot(List<NbtTag> order)
        {
            DoClear();
            DoAddRange(order);
        }

        private void DoSort(IComparer<NbtTag> sorter, bool recursive)
        {
            var tags = Tags.OrderBy(x => x, sorter).ToList();
            if (recursive)
            {
                foreach (var tag in tags)
                {
                    if (tag is NbtCompound sub)
                        sub.DoSort(sorter, true);
                    else if (tag is NbtList list)
                        SortListChildren(list, sorter);
                }
            }
            DoClear();
            DoAddRange(tags);
            RaiseChanged(this);
        }

        private static void SortListChildren(NbtList list, IComparer<NbtTag> sorter)
        {
            if (list.ListType == NbtTagType.Compound)
            {
                foreach (NbtCompound item in list)
                {
                    item.DoSort(sorter, true);
                }
            }
            else if (list.ListType == NbtTagType.List)
            {
                foreach (NbtList item in list)
                {
                    SortListChildren(item, sorter);
                }
            }
        }


        /// <summary> Removes the tag with the specified name from this NbtCompound. </summary>
        /// <param name="tagName"> The name of the tag to remove. </param>
        /// <returns> true if the tag is successfully found and removed; otherwise, false.
        /// This method returns false if name is not found in the NbtCompound. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="tagName"/> is <c>null</c>. </exception>
        public bool Remove(string tagName)
        {
            int index = IndexOf(tagName);
            if (index == -1)
                return false;
            var tag = tags[index];
            return PerformAction(new DescriptionHolder("Remove {0} from {1}", tag, this),
                () => DoRemove(tagName),
                () => { DoInsert(index, tag); }
            );
        }

        private bool DoRemove([NotNull] string tagName) {
            if (tagName == null) throw new ArgumentNullException("tagName");
            if (!tags.ContainsKey(tagName)) {
                return false;
            }
            var tag = tags[tagName];
            tags.Remove(tagName);
            tag.Parent = null;
            return true;
        }

        /// <summary> Removes a tag at the specified index from this NbtCompound. </summary>
        /// <param name="index"> The zero-based index of the item to remove. </param>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="index"/> is not a valid index in the NbtCompound. </exception>
        protected override void DoRemoveAt(int index) {
            var tag = tags[index];
            tag.Parent = null;
            tags.RemoveAt(index);
        }


        internal void RenameTag([NotNull] string oldName, [NotNull] string newName) {
            Debug.Assert(oldName != null);
            Debug.Assert(newName != null);
            Debug.Assert(newName != oldName);
            if (tags.ContainsKey(newName)) {
                throw new ArgumentException("Cannot rename: a tag with the name already exists in this compound.");
            }
            if (!tags.ContainsKey(oldName)) {
                throw new ArgumentException("Cannot rename: no tag found to rename.");
            }
            var tag = tags[oldName];
            var index = IndexOf(tag);
            tags.Remove(oldName);
            tags.Insert(index, newName, tag);
        }

        /// <summary>
        /// Returns the index of the provided tag
        /// </summary>
        /// <param name="tag">The tag to search for</param>
        /// <returns>The index of the provided tag in this compound, or -1 if it does not contain it</returns>
        public override int IndexOf(NbtTag tag) {
            for (int i = 0; i < tags.Count; i++) {
                if (tags[i] == tag)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of the provided tag, by name
        /// </summary>
        /// <param name="name">The name to search for</param>
        /// <returns>The index of a provided tag in this compound with this name, or -1 if it does not contain it</returns>
        public int IndexOf(string name) {
            for (int i = 0; i < tags.Count; i++) {
                if (tags[i].Name == name)
                    return i;
            }
            return -1;
        }

        /// <summary> Gets a collection containing all tag names in this NbtCompound. </summary>
        [NotNull]
        public IEnumerable<string> Names {
            get { return tags.Keys; }
        }

        /// <summary> Gets a collection containing all tags in this NbtCompound. </summary>
        [NotNull]
        public IEnumerable<NbtTag> Tags {
            get { return tags.Values; }
        }

        #region Reading / Writing

        internal static NbtTag CreateTag(NbtTagType type) {
            switch (type) {
                case NbtTagType.Byte:
                    return  new NbtByte();
                case NbtTagType.Short:
                    return new NbtShort();
                case NbtTagType.Int:
                    return new NbtInt();
                case NbtTagType.Long:
                    return new NbtLong();
                case NbtTagType.Float:
                    return new NbtFloat();
                case NbtTagType.Double:
                    return new NbtDouble();
                case NbtTagType.ByteArray:
                    return new NbtByteArray();
                case NbtTagType.String:
                    return new NbtString();
                case NbtTagType.List:
                    return new NbtList();
                case NbtTagType.Compound:
                    return new NbtCompound();
                case NbtTagType.IntArray:
                    return new NbtIntArray();
                case NbtTagType.LongArray:
                    return new NbtLongArray();
                default:
                    throw new NbtFormatException("Unsupported tag type found: " + type);
            }
        }

        internal override bool ReadTag(NbtBinaryReader readStream) {
            if (Parent != null && readStream.Selector != null && !readStream.Selector(this)) {
                SkipTag(readStream);
                return false;
            }

            while (true) {
                NbtTagType nextTag = readStream.ReadTagType();
                if (nextTag == NbtTagType.End)
                    return true;

                NbtTag newTag = CreateTag(nextTag);
                newTag.Parent = this;
                newTag.Name = readStream.ReadString();
                if (newTag.ReadTag(readStream)) {
                    // ReSharper disable AssignNullToNotNullAttribute
                    // newTag.Name is never null
                    tags.Add(newTag.Name, newTag);
                    // ReSharper restore AssignNullToNotNullAttribute
                }
            }
        }


        internal override void SkipTag(NbtBinaryReader readStream) {
            while (true) {
                NbtTagType nextTag = readStream.ReadTagType();
                NbtTag newTag;
                switch (nextTag) {
                    case NbtTagType.End:
                        return;

                    case NbtTagType.Byte:
                        newTag = new NbtByte();
                        break;

                    case NbtTagType.Short:
                        newTag = new NbtShort();
                        break;

                    case NbtTagType.Int:
                        newTag = new NbtInt();
                        break;

                    case NbtTagType.Long:
                        newTag = new NbtLong();
                        break;

                    case NbtTagType.Float:
                        newTag = new NbtFloat();
                        break;

                    case NbtTagType.Double:
                        newTag = new NbtDouble();
                        break;

                    case NbtTagType.ByteArray:
                        newTag = new NbtByteArray();
                        break;

                    case NbtTagType.String:
                        newTag = new NbtString();
                        break;

                    case NbtTagType.List:
                        newTag = new NbtList();
                        break;

                    case NbtTagType.Compound:
                        newTag = new NbtCompound();
                        break;

                    case NbtTagType.IntArray:
                        newTag = new NbtIntArray();
                        break;
                    
                    case NbtTagType.LongArray:
                        newTag = new NbtLongArray();
                        break;

                    default:
                        throw new NbtFormatException("Unsupported tag type found in NBT_Compound: " + nextTag);
                }
                readStream.SkipString();
                newTag.SkipTag(readStream);
            }
        }


        internal override void WriteTag(NbtBinaryWriter writeStream) {
            writeStream.Write(NbtTagType.Compound);
            if (Name == null) throw new NbtFormatException("Name is null");
            writeStream.Write(Name);
            WriteData(writeStream);
        }


        internal override void WriteData(NbtBinaryWriter writeStream) {
            foreach (NbtTag tag in tags.Values) {
                tag.WriteTag(writeStream);
            }
            writeStream.Write(NbtTagType.End);
        }

        #endregion


        #region Implementation of IEnumerable<NbtTag>

        /// <summary> Returns an enumerator that iterates through all tags in this NbtCompound. </summary>
        /// <returns> An IEnumerator&gt;NbtTag&lt; that can be used to iterate through the collection. </returns>
        public override IEnumerator<NbtTag> GetEnumerator() {
            return tags.Values.GetEnumerator();
        }

        #endregion


        #region Implementation of ICollection<NbtTag>

        /// <summary> Adds a tag to this NbtCompound. </summary>
        /// <param name="newTag"> The object to add to this NbtCompound. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="newTag"/> is <c>null</c>. </exception>
        /// <exception cref="ArgumentException"> If the given tag is unnamed;
        /// or if a tag with the given name already exists in this NbtCompound. </exception>
        protected override void DoAdd([NotNull] NbtTag newTag) {
            if (newTag == null) {
                throw new ArgumentNullException("newTag");
            } else if (newTag == this) {
                throw new ArgumentException("Cannot add tag to self");
            } else if (newTag.Name == null) {
                throw new ArgumentException("Only named tags are allowed in compound tags.");
            } else if (newTag.Parent != null) {
                throw new ArgumentException("A tag may only be added to one compound/list at a time.");
            }
            tags.Add(newTag.Name, newTag);
            newTag.Parent = this;
        }

        /// <summary> Inserts a tag into this NbtCompound. </summary>
        /// <param name="index"> The index to which the tag should be added. </param>
        /// <param name="newTag"> The object to add to this NbtCompound. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="newTag"/> is <c>null</c>. </exception>
        /// <exception cref="ArgumentException"> If the given tag is unnamed;
        /// or if a tag with the given name already exists in this NbtCompound. </exception>
        protected override void DoInsert(int index, [NotNull] NbtTag newTag)
        {
            if (newTag == null) {
                throw new ArgumentNullException("newTag");
            } else if (newTag == this) {
                throw new ArgumentException("Cannot add tag to self");
            } else if (newTag.Name == null) {
                throw new ArgumentException("Only named tags are allowed in compound tags.");
            } else if (newTag.Parent != null) {
                throw new ArgumentException("A tag may only be added to one compound/list at a time.");
            }
            tags.Insert(index, newTag.Name, newTag);
            newTag.Parent = this;
        }


        /// <summary> Removes all tags from this NbtCompound. </summary>
        protected override void DoClear() {
            foreach (NbtTag tag in tags.Values) {
                tag.Parent = null;
            }
            tags.Clear();
        }


        /// <summary> Determines whether this NbtCompound contains a specific NbtTag.
        /// Looks for exact object matches, not name matches. </summary>
        /// <returns> true if tag is found; otherwise, false. </returns>
        /// <param name="tag"> The object to locate in this NbtCompound. May not be <c>null</c>. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="tag"/> is <c>null</c>. </exception>
        [Pure]
        public override bool Contains([NotNull] NbtTag tag) {
            if (tag == null) throw new ArgumentNullException("tag");
            if (tag.Name == null) return false;
            if (tags.ContainsKey(tag.Name)) {
                var existing = tags[tag.Name];
                return existing == tag;
            }
            return false;
        }


        /// <summary> Copies the tags of the NbtCompound to an array, starting at a particular array index. </summary>
        /// <param name="array"> The one-dimensional array that is the destination of the tag copied from NbtCompound.
        /// The array must have zero-based indexing. </param>
        /// <param name="arrayIndex"> The zero-based index in array at which copying begins. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="array"/> is <c>null</c>. </exception>
        /// <exception cref="ArgumentOutOfRangeException"> arrayIndex is less than 0. </exception>
        /// <exception cref="ArgumentException"> Given array is multidimensional; arrayIndex is equal to or greater than the length of array;
        /// the number of tags in this NbtCompound is greater than the available space from arrayIndex to the end of the destination array;
        /// or type NbtTag cannot be cast automatically to the type of the destination array. </exception>
        public override void CopyTo(NbtTag[] array, int arrayIndex) {
            tags.Values.CopyTo(array, arrayIndex);
        }


        /// <summary> Removes the first occurrence of a specific NbtTag from the NbtCompound.
        /// Looks for exact object matches, not name matches. </summary>
        /// <returns> true if tag was successfully removed from the NbtCompound; otherwise, false.
        /// This method also returns false if tag is not found. </returns>
        /// <param name="tag"> The tag to remove from the NbtCompound. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="tag"/> is <c>null</c>. </exception>
        /// <exception cref="ArgumentException"> If the given tag is unnamed </exception>
        protected override bool DoRemove([NotNull] NbtTag tag) {
            if (tag == null) throw new ArgumentNullException("tag");
            if (tag.Name == null) throw new ArgumentException("Trying to remove an unnamed tag.");
            if (tags.ContainsKey(tag.Name)) {
                var maybeItem = tags[tag.Name];
                if (maybeItem == tag) {
                    tags.Remove(tag.Name);
                    tag.Parent = null;
                    return true;
                }
            }
            return false;
        }


        /// <summary> Gets the number of tags contained in the NbtCompound. </summary>
        /// <returns> The number of tags contained in the NbtCompound. </returns>
        public override int Count {
            get { return tags.Count; }
        }

        #endregion

        /// <inheritdoc />
        public override object Clone() {
            return new NbtCompound(this);
        }


        internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel) {
            for (int i = 0; i < indentLevel; i++) {
                sb.Append(indentString);
            }
            sb.Append("TAG_Compound");
            if (!String.IsNullOrEmpty(Name)) {
                sb.AppendFormat("(\"{0}\")", Name);
            }
            sb.AppendFormat(": {0} entries {{", tags.Count);

            if (Count > 0) {
                sb.Append('\n');
                foreach (NbtTag tag in tags.Values) {
                    tag.PrettyPrint(sb, indentString, indentLevel + 1);
                    sb.Append('\n');
                }
                for (int i = 0; i < indentLevel; i++) {
                    sb.Append(indentString);
                }
            }
            sb.Append('}');
        }
    }
}
