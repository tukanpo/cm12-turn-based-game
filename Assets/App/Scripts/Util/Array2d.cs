using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace App.Util
{
    public class Array2d<T> : IEnumerable<T> where T : class
    {
        public int SizeX { get; }

        public int SizeY { get; }

        T[,] _fields;

        public T this [int x, int y]
        {
            get => _fields[x, y];
            set => _fields[x, y] = value;
        }

        protected Array2d(int sizeX, int sizeY)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            Initialize(SizeX, SizeY);
        }

        protected virtual T CreateInstance(int x, int y)
        {
            return Activator.CreateInstance(typeof(T), new object[]{x, y}) as T;
        }

        void Initialize(int sizeX, int sizeY)
        {
            _fields = new T[sizeX, sizeY];

            for (var x = 0; x < sizeX; x++)
            {
                for (var y = 0; y < sizeY; y++)
                {
                    _fields[x, y] = CreateInstance(x, y);
                }
            }
        }

        #region Implementation of IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            return _fields.Cast<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public bool IsEdge(int posX, int posY)
        {
            return (posX <= 0 || posX >= SizeX - 1 || posY <= 0 || posY >= SizeY - 1);
        }
    
        public bool IsOutOfRange(int posX, int posY)
        {
            return posX < 0 || posX > SizeX - 1 || posY < 0 || posY > SizeY - 1;
        }
    }
}
