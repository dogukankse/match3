using System;
using UnityEngine;

namespace Models
{
    [Serializable]
    public sealed class MinMax<T>
    {
        public T MIN => _min;
        public T MAX => _max;

        [SerializeField]private T _min;
        [SerializeField]private T _max;

        public MinMax(T min, T max)
        {
            _min = min;
            _max = max;
        }
    }
}