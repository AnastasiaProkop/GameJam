using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    namespace Components
    {
        public class HealthBar
        {
            private readonly Label m_HealthLabel;
            private readonly VisualElement m_HealthBarMask;

            // ON BEST - INFO FROM PLAYER CONTROL
            private const int DEBUG_MAX_HEALTH = 100;
            private int debug_health = 100;
            private bool healthChanged = true;
            public void DebugSimulateHealthChange()
            {
                if (debug_health > 0)
                {
                    debug_health -= 10;
                }
                else
                {
                    debug_health = DEBUG_MAX_HEALTH;
                }
                healthChanged = true;
            }
            public HealthBar(string barName, UIDocument doc)
            {
                m_HealthLabel = doc.rootVisualElement.Q<Label>("health_label");
                m_HealthBarMask = doc.rootVisualElement.Q<VisualElement>("health_mask");
                if (m_HealthLabel == null)
                {
                    Debug.LogWarning("Could not find 'health_label' in UI Document");
                }
                if (m_HealthBarMask == null)
                {
                    Debug.LogWarning("Could not find 'health_label' in UI Document");
                }
            }
            public void LowerHealth()
            {
                DebugSimulateHealthChange();
                Debug.Log("Performing action!");
            }
            public void Update()
            {
                if (healthChanged && m_HealthLabel != null)
                {
                    m_HealthLabel.text = $"{debug_health}/{DEBUG_MAX_HEALTH}";
                    float healthRatio = (float)debug_health / DEBUG_MAX_HEALTH;
                    float healthPercent = Mathf.Lerp(0, 100, healthRatio);
                    m_HealthBarMask.style.width = Length.Percent(healthPercent);
                }
            }
        };

    }
}
