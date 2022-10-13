using System;
using System.Text;
using JetBrains.Annotations;

namespace fNbt {
    /// <summary> A tag containing a single byte. </summary>
    public sealed class NbtByte : NbtValueTag {
        /// <summary> Type of this tag (Byte). </summary>
        public override NbtTagType TagType {
            get { return NbtTagType.Byte; }
        }

        /// <summary> Value/payload of this tag (a single byte). </summary>
        public byte Value {
            get => _Value;
            set {
                _Value = value;
                OnPropertyChanged();
            }
        }
        private byte _Value;


        /// <summary> Creates an unnamed NbtByte tag with the default value of 0. </summary>
        public NbtByte() { }


        /// <summary> Creates an unnamed NbtByte tag with the given value. </summary>
        /// <param name="value"> Value to assign to this tag. </param>
        public NbtByte(byte value)
            : this(null, value) { }


        /// <summary> Creates an unnamed NbtByte tag with the given value. </summary>
        /// <param name="value"> Value to assign to this tag. </param>
        public NbtByte(bool value)
            : this(null, value ? (byte)1 : (byte)0) { }


        /// <summary> Creates an NbtByte tag with the given name and the default value of 0. </summary>
        /// <param name="tagName"> Name to assign to this tag. May be <c>null</c>. </param>
        public NbtByte([CanBeNull] string tagName)
            : this(tagName, 0) { }


        /// <summary> Creates an NbtByte tag with the given name and value. </summary>
        /// <param name="tagName"> Name to assign to this tag. May be <c>null</c>. </param>
        /// <param name="value"> Value to assign to this tag. </param>
        public NbtByte([CanBeNull] string tagName, byte value) {
            name = tagName;
            _Value = value;
        }

        /// <summary> Creates an NbtByte tag with the given name and value. </summary>
        /// <param name="tagName"> Name to assign to this tag. May be <c>null</c>. </param>
        /// <param name="value"> Value to assign to this tag. </param>
        public NbtByte([CanBeNull] string tagName, bool value) {
            name = tagName;
            _Value = value ? (byte)1 : (byte)0;
        }


        /// <summary> Creates a copy of given NbtByte tag. </summary>
        /// <param name="other"> Tag to copy. May not be <c>null</c>. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="other"/> is <c>null</c>. </exception>
        public NbtByte([NotNull] NbtByte other) {
            if (other == null) throw new ArgumentNullException(nameof(other));
            name = other.name;
            _Value = other.Value;
        }


        internal override bool ReadTag(NbtBinaryReader readStream) {
            if (readStream.Selector != null && !readStream.Selector(this)) {
                readStream.ReadByte();
                return false;
            }
            _Value = readStream.ReadByte();
            return true;
        }


        internal override void SkipTag(NbtBinaryReader readStream) {
            readStream.ReadByte();
        }


        internal override void WriteTag(NbtBinaryWriter writeStream) {
            writeStream.Write(NbtTagType.Byte);
            if (Name == null) throw new NbtFormatException("Name is null");
            writeStream.Write(Name);
            writeStream.Write(Value);
        }


        internal override void WriteData(NbtBinaryWriter writeStream) {
            writeStream.Write(Value);
        }


        /// <inheritdoc />
        public override object Clone() {
            return new NbtByte(this);
        }

        /// <inheritdoc />
        public override int CompareToValue(NbtValueTag other) {
            if (other is not NbtByte b)
                return 0;
            return this.Value.CompareTo(b.Value);
        }

        public override string ToString() {
            if (this.Name == null)
                return this.Value.ToString();
            return $"{this.Name}: {this.Value}";
        }
    }
}
