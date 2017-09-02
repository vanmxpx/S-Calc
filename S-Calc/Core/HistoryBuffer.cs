using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S_Calc.Core
{
    [Serializable]
    public class HistoryBuffer<T> : IEnumerable<T>
        where T : CalculationHistory
    {
        /// <summary>
        /// Min=10, Max=65535, default=100
        /// </summary>
        ushort size;

        ushort cursorPosition;

        T[] array;

        int getIndexOfLastNotNullElement()
        {
            int index = array.Length - 1;
            foreach (var e in array.Reverse())
            {
                if (ReferenceEquals(e, null))
                {
                    index--;
                }
                else
                {
                    return index;
                }
            }
            return index;
        }

        public T First
        {
            get
            {
                return array[0];
            }
        }

        public T Last
        {
            get
            {
                return array[getIndexOfLastNotNullElement()];
            }
        }

        public bool IsUndoable
        {
            get
            {
                int tmp = getIndexOfLastNotNullElement();
                return tmp != -1 && cursorPosition != getIndexOfLastNotNullElement();
            }
        }

        public bool IsRedoable
        {
            get
            {
                return cursorPosition != 0;
            }
        }

        public ushort Size
        {
            get
            {
                return size;
            }
            set
            {
                if (value >= 10 && value <= ushort.MaxValue)
                {
                    size = value;
                    T[] tmp = new T[size];
                    Array.Copy(array, 0, tmp, 0, size);
                    array = tmp;
                    cursorPosition = 0;
                }
            }
        }

        public HistoryBuffer()
        {
            size = 100;
            cursorPosition = 0;
            array = new T[size];
        }

        public void Push(T element)
        {
            ShifOnPush();
            array[0] = element;
        }

        T ElementOnCursorPosition()
        {
            if (array[cursorPosition] != null)
            {
                return array[cursorPosition];
            }
            else
            {
                throw new InvalidOperationException("Element on current cursor position is NULL");
            }
        }

        public T Undo()
        {
            if (IsUndoable)
            {
                cursorPosition++;
                return ElementOnCursorPosition();
            }
            else
            {
                throw new InvalidOperationException("Undo option doesn't available.");
            }
        }

        public T Redo()
        {
            if (IsRedoable)
            {
                cursorPosition--;
                return ElementOnCursorPosition();
            }
            else
            {
                throw new InvalidOperationException("Redo option doesn't available.");
            }
        }

        public void ResetCursorPosition()
        {
            cursorPosition = 0;
        }

        [Obsolete("Current version doesn't use Pop methods")]
        public T Pop()
        {
            T tmp = (T)array[0].Clone();
            ShiftOnPop();
            return tmp;
        }

        /// <summary>
        /// Before
        /// </summary>
        void ShifOnPush()
        {
            int index = getIndexOfLastNotNullElement();
            if (index != -1)
            {
                if (index == size - 1)
                {
                    index--;
                }
                while (index >= 0)
                {
                    array[index + 1] = (T)array[index].Clone();
                    index--;
                }
                array[0] = null;
            }
        }

        [Obsolete("Current version doesn't use Pop methods")]
        void ShiftOnPop()
        {
            int index = 1;
            int edge = getIndexOfLastNotNullElement();
            while (index <= edge)
            {
                array[index - 1] = (T)array[index].Clone();
                index++;
            }
            array[edge] = null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T e in array)
            {
                if (!ReferenceEquals(e, null))
                {
                    yield return e;
                }
                else
                {
                    yield break;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
