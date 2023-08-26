using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arr.GCS
{
    
    [CreateAssetMenu(menuName = "Constants Group", fileName = "ConstantsGroupData", order = 0)]
    public class ConstantsGroupData : ScriptableObject
    {
        public string groupName;
        public string[] constants;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(groupName)) groupName = name;
        }
    }
}