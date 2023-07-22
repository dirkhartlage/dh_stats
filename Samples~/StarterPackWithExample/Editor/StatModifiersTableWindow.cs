#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class StatModifiersTableWindow : EditorWindow
    {
        private const string ModifiersPath = "Assets/Stats/ModifiersData";

        private Vector2 _scrollPosition;

        private SortedDictionary<string, List<(string Id, string Operation, string Value, string Path)>>
                _targetStatDictionary;

        private Dictionary<string, bool> _duplicateIdTracker;

        [MenuItem("Window/StatModifiers Table")]
        public static void ShowWindow()
            => GetWindow(typeof(StatModifiersTableWindow), false, "StatModifiers Table");

        private void OnEnable()
            => GenerateTable();

        private void GenerateTable()
        {
            _targetStatDictionary = new SortedDictionary<string,
                    List<(string Id, string Operation, string Value, string Path)>>();
            _duplicateIdTracker = new Dictionary<string, bool>();

            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { ModifiersPath });

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

                var targetStatIdProperty = obj.GetType().GetProperty("TargetStatId");
                var idProperty = obj.GetType().GetProperty("Id");
                var operationProperty = obj.GetType().GetProperty("Operation");
                var baseValueProperty = obj.GetType().GetProperty("BaseValue");

                if (targetStatIdProperty != null && idProperty != null && operationProperty != null &&
                    baseValueProperty != null)
                {
                    string targetStatId = targetStatIdProperty.GetValue(obj, null)?.ToString();
                    string id = idProperty.GetValue(obj, null)?.ToString();
                    string operation = operationProperty.GetValue(obj, null)?.ToString();
                    string baseValue = baseValueProperty.GetValue(obj, null)?.ToString();

                    if (_targetStatDictionary.TryGetValue(targetStatId, out var valueList))
                        valueList.Add((id, operation, baseValue, path));
                    else
                        _targetStatDictionary[targetStatId] = new List<(string, string, string, string)>
                                { (id, operation, baseValue, path) };

                    _duplicateIdTracker[id] = _duplicateIdTracker.ContainsKey(id);
                }
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField($"Current ModifiersPath: {ModifiersPath}", EditorStyles.boldLabel);

            if (GUILayout.Button("Refresh"))
                GenerateTable();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("TargetStatId", EditorStyles.boldLabel, GUILayout.MaxWidth(150));
            EditorGUILayout.LabelField("Id", EditorStyles.boldLabel, GUILayout.MaxWidth(200));
            EditorGUILayout.LabelField("Operation", EditorStyles.boldLabel, GUILayout.MaxWidth(100));
            EditorGUILayout.LabelField("BaseValue", EditorStyles.boldLabel, GUILayout.MaxWidth(100));
            EditorGUILayout.LabelField("Path", EditorStyles.boldLabel, GUILayout.MaxWidth(400));
            EditorGUILayout.EndHorizontal();

            foreach (var targetStatId in _targetStatDictionary.Keys)
            {
                List<(string Id, string Operation, string Value, string Path)> elements =
                        _targetStatDictionary[targetStatId];

                foreach (var (id, operation, baseValue, path) in elements)
                {
                    if (string.IsNullOrEmpty(id))
                        GUI.color = Color.yellow;

                    else if (_duplicateIdTracker[id])
                        GUI.color = Color.red;

                    EditorGUILayout.BeginHorizontal("box");
                    EditorGUILayout.LabelField(targetStatId, GUILayout.MaxWidth(150));
                    EditorGUILayout.LabelField(id, GUILayout.MaxWidth(200));
                    EditorGUILayout.LabelField(operation, GUILayout.MaxWidth(100));
                    EditorGUILayout.LabelField(baseValue, GUILayout.MaxWidth(100));
                    EditorGUILayout.LabelField(path, GUILayout.MaxWidth(400));
                    EditorGUILayout.EndHorizontal();

                    GUI.color = Color.white;
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
#endif