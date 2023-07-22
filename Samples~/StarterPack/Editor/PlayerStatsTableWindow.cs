#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PlayerStatsTableWindow : EditorWindow
    {
        private const string StatsPath = "Assets/Stats/StatsData";

        private Vector2 _scrollPosition;

        private SortedDictionary<int, List<(string Id, string DisplayName, string BaseValue, string Path)>>
                _orderDictionary;

        private Dictionary<string, bool> _duplicateIdTracker;

        [MenuItem("Window/PlayerStats Table")]
        public static void ShowWindow()
            => GetWindow(typeof(PlayerStatsTableWindow), false, "PlayerStats Table");

        private void OnEnable()
            => GenerateTable();

        private void GenerateTable()
        {
            _orderDictionary =
                    new SortedDictionary<int, List<(string Id, string DisplayName, string BaseValue, string Path)>>();
            _duplicateIdTracker = new Dictionary<string, bool>();

            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { StatsPath });

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

                var idProperty = obj.GetType().GetProperty("Id");
                var displayNameProperty = obj.GetType().GetProperty("DisplayName");
                var orderProperty = obj.GetType().GetProperty("Order");
                var baseValueProperty = obj.GetType().GetProperty("BaseValue");

                if (idProperty != null && orderProperty != null && baseValueProperty != null &&
                    displayNameProperty != null)
                {
                    string id = idProperty.GetValue(obj, null)?.ToString();
                    string displayName = displayNameProperty.GetValue(obj, null)?.ToString();
                    int order = (int)orderProperty.GetValue(obj, null);
                    string baseValue = baseValueProperty.GetValue(obj, null)?.ToString();

                    if (_orderDictionary.TryGetValue(order, out var value))
                        value.Add((id, displayName, baseValue, path));
                    else
                        _orderDictionary[order] = new List<(string, string, string, string)>
                                { (id, displayName, baseValue, path) };

                    _duplicateIdTracker[id] = _duplicateIdTracker.ContainsKey(id);
                }
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField($"Current StatsPath: {StatsPath}", EditorStyles.boldLabel);

            if (GUILayout.Button("Refresh"))
            {
                GenerateTable();
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ID", EditorStyles.boldLabel, GUILayout.MaxWidth(180));
            EditorGUILayout.LabelField("Display Name", EditorStyles.boldLabel, GUILayout.MaxWidth(180));
            EditorGUILayout.LabelField("Order", EditorStyles.boldLabel, GUILayout.MaxWidth(100));
            EditorGUILayout.LabelField("BaseValue", EditorStyles.boldLabel, GUILayout.MaxWidth(100));
            EditorGUILayout.LabelField("Path", EditorStyles.boldLabel, GUILayout.MaxWidth(400));
            EditorGUILayout.EndHorizontal();

            foreach (var order in _orderDictionary.Keys)
            {
                List<(string Id, string DisplayName, string BaseValue, string Path)> elements = _orderDictionary[order];

                foreach (var (id, displayName, baseValue, path) in elements)
                {
                    if (_duplicateIdTracker[id])
                        GUI.color = Color.red;

                    EditorGUILayout.BeginHorizontal("box");
                    EditorGUILayout.LabelField(id, GUILayout.MaxWidth(180));
                    EditorGUILayout.LabelField(displayName, GUILayout.MaxWidth(180));
                    EditorGUILayout.LabelField(order.ToString(), GUILayout.MaxWidth(100));
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