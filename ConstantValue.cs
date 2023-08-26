using System;
using UnityEngine;

namespace Arr.GCS
{
    [Serializable]
    public class ConstantValue
    {
        [SerializeField] private string value;
        
        private ConstantValue(string value)
        {
            this.value = value;
        }
        
        public static implicit operator string(ConstantValue c) => c.value;
    }
}