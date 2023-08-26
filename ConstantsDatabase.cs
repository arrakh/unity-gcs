using System.Collections.Generic;
using UnityEngine;

namespace Arr.GCS
{
    [CreateAssetMenu(menuName = "Constants Database", fileName = "ConstantsDatabase", order = 0)]
    public class ConstantsDatabase : ScriptableObject
    {
        public List<ConstantsGroupData> constantsGroups;
    }
}