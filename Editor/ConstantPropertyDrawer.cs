using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Arr.GCS.Editor
{
    [CustomPropertyDrawer(typeof(ConstantValue))]
    public class ConstantPropertyDrawer : PropertyDrawer
    {
        private ConstantFilterAttribute[] filters = null;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var selectedEvent = property.FindPropertyRelative("value");

            filters ??= property.GetAttributes<ConstantFilterAttribute>();

            string[] constants = ConstantsGenerator.GetKeys();
            
            var filteredConstants = constants.Where(KeyFilter).ToArray();

            int constantsIndex = ConstantsGenerator.GetEventIndex(selectedEvent.stringValue);

            var currentFilterIndex = Array.IndexOf(filteredConstants, constants[constantsIndex]);
            if (currentFilterIndex == -1) currentFilterIndex = 0;
            var filteredIndex = EditorGUI.Popup(position, label.text, currentFilterIndex, filteredConstants);

            if (filteredIndex != currentFilterIndex)
            {
                selectedEvent.stringValue = ConstantsGenerator.GetEvent(filteredConstants[filteredIndex]);
                selectedEvent.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }

        private bool KeyFilter(string key)
        {
            if (filters == null || filters.Length <= 0) return true;
            
            foreach (var filter in filters)
                if (key.StartsWith(filter.onlyShowPath, StringComparison.InvariantCulture)) return true;

            return false;
        }
    }
}