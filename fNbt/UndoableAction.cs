using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fNbt
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class DescriptionHolder
    {
        private readonly string Format;
        private readonly object[] Objects;
        public DescriptionHolder(string format, params object[] objects)
        {
            Format = format;
            Objects = objects;
        }

        public static DescriptionHolder Simple(string format) => new DescriptionHolder(format);

        public string Convert(Func<object, string> converter)
        {
            return String.Format(Format, Objects.Select(converter));
        }
    }

    public class UndoableAction
    {
        public readonly DescriptionHolder Description;
        protected Action DoAction;
        protected Action UndoAction;
        public bool IsDone { get; protected set; } = false;
        public UndoableAction(DescriptionHolder holder, Action action, Action undo)
        {
            Description = holder;
            DoAction = action;
            UndoAction = undo;
        }

        public void Undo()
        {
            if (IsDone)
            {
                UndoAction();
                IsDone = false;
            }
        }

        public void Do()
        {
            if (!IsDone)
            {
                DoAction();
                IsDone = true;
            }
        }

        public void Add(UndoableAction other)
        {
            if (other.IsDone != this.IsDone)
                throw new ArgumentException($"Can't merge actions with mismatching done-ness: [{this.Description}: {this.IsDone}] and [{other.Description}: {other.IsDone}]");
            DoAction += other.DoAction;
            UndoAction = other.UndoAction + UndoAction;
        }

        public static UndoableAction Merge(DescriptionHolder holder, IEnumerable<UndoableAction> actions)
        {
            var first = actions.First();
            var result = new UndoableAction(holder, first.DoAction, first.UndoAction);
            result.IsDone = first.IsDone;
            foreach (var action in actions.Skip(1))
            {
                result.Add(action);
            }
            return result;
        }
    }

    public class UndoableAction<T> : UndoableAction
    {
        protected readonly Func<T> DoFunction;
        public UndoableAction(DescriptionHolder holder, Func<T> function, Action undo) : base(holder, () => function(), undo)
        {
            DoFunction = function;
        }

        public T Do()
        {
            if (!IsDone)
            {
                T result = DoFunction();
                IsDone = true;
                return result;
            }
            return default;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
