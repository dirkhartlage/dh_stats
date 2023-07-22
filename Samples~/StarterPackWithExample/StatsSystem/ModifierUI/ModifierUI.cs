using dh_stats;
using dh_stats.Modifiers;
using UnityEngine;

namespace StatsSystem.ModifierUI
{
    public sealed class ModifierUI : MonoBehaviour
    {
        [SerializeField]
        private StatsSystemComponent statsSystemComponent;

        [SerializeField]
        private GameObject modifierUIElementPrefab;

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            RectTransform rectTransform = modifierUIElementPrefab.transform as RectTransform;

            if (rectTransform != null)
            {
                var transform1 = transform;
                Vector3 center = transform1.position;
                Vector3 scale = transform1.lossyScale;

                float height = rectTransform.rect.height * scale.y;
                Vector3 heightOffset = new Vector3(0, height, 0);

                Vector3 top = center + heightOffset;
                Vector3 bottom = center;

                Gizmos.color = Color.green;

                // Draw vertical line
                Gizmos.DrawLine(top, bottom);

                // Draw horizontal lines
                const int horizontalLineLength = 3000;
                Gizmos.DrawLine(top, top + Vector3.right * horizontalLineLength * scale.x);
                Gizmos.DrawLine(bottom, bottom + Vector3.right * horizontalLineLength * scale.x);
            }
        }
        #endif // UNITY_EDITOR

        private void Start()
        {
            // init
            foreach (var stat in statsSystemComponent.AvailableBoolStats)
                foreach (Modifier<bool> modifier in stat.Value.Modifiers)
                    AddModifier(modifier);

            foreach (var stat in statsSystemComponent.AvailableFloatStats)
                foreach (Modifier<float> modifier in stat.Value.Modifiers)
                    AddModifier(modifier);

            foreach (var stat in statsSystemComponent.AvailableIntStats)
                foreach (Modifier<int> modifier in stat.Value.Modifiers)
                    AddModifier(modifier);

            statsSystemComponent.OnBoolModifierAdded += AddBoolModifier;
            statsSystemComponent.OnFloatModifierAdded += AddFloatModifier;
            statsSystemComponent.OnIntModifierAdded += AddIntModifier;
        }

        // private void OnDisable()
        // {
        //     statsSystemComponent.OnBoolModifierAdded += AddBoolModifier;
        //     statsSystemComponent.OnFloatModifierAdded += AddFloatModifier;
        //     statsSystemComponent.OnIntModifierAdded += AddIntModifier;
        // }

        private void AddIntModifier(Modifier<int> modifier) => AddModifier(modifier);

        private void AddFloatModifier(Modifier<float> modifier) => AddModifier(modifier);

        private void AddBoolModifier(Modifier<bool> modifier) => AddModifier(modifier);

        private void AddModifier<T>(Modifier<T> modifier) where T : struct
        {
            GameObject g = Instantiate(modifierUIElementPrefab, transform);
            var uiElement = g.GetComponent<ModifierUIElement>();
            uiElement.Init(modifier);
        }
    }
}