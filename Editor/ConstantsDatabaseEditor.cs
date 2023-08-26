using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Arr.GCS.Editor
{
    [CustomEditor(typeof(ConstantsDatabase))]
    public class ConstantsDatabaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Generate Constants From This") && target is ConstantsDatabase db1)
                ConstantsGenerator.GenerateScript(db1);

            if (GUILayout.Button("Populate Groups") && target is ConstantsDatabase db2)
            {
                db2.constantsGroups = new List<ConstantsGroupData>();
                string[] guids = AssetDatabase.FindAssets($"t:{typeof(ConstantsGroupData).FullName}");

                for (var i = 0; i < guids.Length; i++)
                {
                    string guid = guids[i];
                    string path = AssetDatabase.GUIDToAssetPath(guid);

                    var asset = AssetDatabase.LoadAssetAtPath<ConstantsGroupData>(path);
                    db2.constantsGroups.Add(asset);
                }
            }

            base.OnInspectorGUI();
        }
    }
}