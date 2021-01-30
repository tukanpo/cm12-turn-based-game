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

        protected T[,] _fileds;

        public T this [int x, int y]
        {
            get { return _fileds[x, y]; }
            set { _fileds[x, y] = value; }
        }

        public Array2d(int sizeX, int sizeY)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            Initialize(SizeX, SizeY);
        }

        void Initialize(int sizeX, int sizeY)
        {
            _fileds = new T[sizeX, sizeY];

            for (var x = 0; x < sizeX; x++)
            {
                for (var y = 0; y < sizeY; y++)
                {
                    _fileds[x, y] = Activator.CreateInstance(typeof(T), new Object[]{x, y}) as T;
                }
            }
        }

        #region Implementation of IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            // MEMO: これ何だったか思い出せん
            // return _fileds.SelectMany(x => x).GetEnumerator();
            return _fileds.Cast<T>().GetEnumerator();
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
