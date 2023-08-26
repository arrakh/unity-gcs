using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Arr.GCS.Editor
{
    public static class ConstantsGenerator
    {
        static readonly Dictionary<string, string> _constants = new Dictionary<string, string>();
        static string[] _keys = new string[0];

        public static void GenerateScript(ConstantsDatabase db)
        {
            var content = string.Empty;

            foreach (var group in db.constantsGroups)
                content += GetGroupFormat(group) + "\n";

            var finalScript = GetScriptFormat(content);

            var guids = AssetDatabase.FindAssets("l:Constants");
            if (guids.Length > 1) throw new Exception("There are more than one file labelled Constant!");
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            File.WriteAllText(path, finalScript);
            EditorUtility.SetDirty(AssetDatabase.LoadAssetAtPath<TextAsset>(path));
            EditorUtility.RequestScriptReload();
            CompilationPipeline.RequestScriptCompilation();
            
            GenerateKeys();
        }

        private static string GetScriptFormat(string content)
        {
            return  "namespace Arr.GCS\n" +
                    "{\n" +
                    Indent(1) + "public static class Constants\n" +
                    Indent(1) + "{\n" +
                   $"{content}\n" +
                    Indent(1) + "}\n" +
                    "}";
        }

        private static string GetGroupFormat(ConstantsGroupData data, int level = 2)
        {
            if (String.IsNullOrEmpty(data.groupName)) throw new Exception($"Group Name for {data.name} is EMPTY!");
            
            level++;

            var text = "\n" +
                       Indent(level) +  "[GeneratedConstant]\n" +
                       Indent(level) + $"public static class {data.groupName}\n" +
                       Indent(level) +  "{\n";

            foreach (var c in data.constants)
            {
                var n = c.Replace("-", "_").ToUpperInvariant();
                text += Indent(level + 1) + $"public const string {n} = \"{data.groupName.ToLowerInvariant()}-{c.ToLowerInvariant()}\";\n";
            }

            text += Indent(level) + "}";
            

            return text;
        }

        private static string Indent(int level)
        {
            var indent = string.Empty;
            for (int i = 0; i < level; i++)
                indent += "    ";
            return indent;
        }

        public static string[] GetKeys()
        {
            if (_constants.Count == 0)
                GenerateKeys();
            return _keys;
        }

        public static string GetEvent(string key) => _constants[key];

        public static int GetEventIndex(string eventConstant)
        {
            foreach (KeyValuePair<string, string> pair in _constants)
            {
                if (!string.Equals(eventConstant, pair.Value, StringComparison.OrdinalIgnoreCase)) continue;

                for (int i = 0; i < _keys.Length; i++)
                {
                    if (string.Equals(_keys[i], pair.Key, StringComparison.OrdinalIgnoreCase))
                        return i;
                }

                return 0;
            }

            return 0;
        }

        static void GenerateKeys()
        {
            IEnumerable<Type> types =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                let att = t.GetCustomAttribute<GeneratedConstantAttribute>()
                where att != null
                select t;

            _constants.Clear();
            _constants["None"] = string.Empty;

            foreach (Type type in types)
            {
                string key = GetDeclaringTypesName(type);
                key += type.Name;
                
                foreach (FieldInfo field in type.GetFields())
                {
                    string id = string.IsNullOrWhiteSpace(key) ? field.Name : $"{key}/{field.Name}";
                    _constants[id] = field.GetValue(null).ToString();
                }
            }

            _keys = _constants.Keys.OrderByDescending(s => string.IsNullOrWhiteSpace(_constants[s])).ThenByDescending(s => s.Contains('/'))
                .ThenBy(s => s).ToArray();
        }

        static string GetDeclaringTypesName(Type type)
        {
            if (type.DeclaringType == null) return string.Empty;

            string s = string.Empty;

            Type k = type.DeclaringType;

            List<string> lst = new List<string>();

            //find the root
            while (true)
            {
                lst.Add(k.Name);
                k = k.DeclaringType;
                if (k == null) break;
                if (k.DeclaringType != null) continue;
                lst.Add(k.Name);
                break;
            }

            for (int i = lst.Count - 1; i >= 1; i--)
                s += $"{lst[i]}/";

            return s;
        }
    }
}