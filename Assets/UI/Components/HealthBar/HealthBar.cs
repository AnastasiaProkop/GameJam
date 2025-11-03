using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    namespace Components
    {
        public class HealthBar
        {
            private readonly VisualElement m_HealthBarMask;

            // ON BEST - INFO FROM PLAYER CONTROL
            private const int DEBUG_MAX_HEALTH = 100;
            private int debug_health = 100;
            private bool healthChanged = true;
            public void DebugSimulateHealthChange(int addValue = 0)
            {
                if (debug_health > 0 + addValue)
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
                m_HealthBarMask = doc.rootVisualElement.Q<VisualElement>(barName);
                if (m_HealthBarMask == null)
                {
                    Debug.LogWarning($"Could not find '{barName}' in UI Document");
                }
            }
            public void LowerHealth()
            {
                DebugSimulateHealthChange();
                Debug.Log("Performing action!");
            }
            
            public void Update(bool isWidth)
            {
                if (healthChanged)
                {
                    float healthRatio = (float)debug_health / DEBUG_MAX_HEALTH;
                    float healthPercent = Mathf.Lerp(0, 100, healthRatio);
                    var lengthToCrop = Length.Percent(healthPercent);
                    if (isWidth)
                    {
                        m_HealthBarMask.style.width = lengthToCrop;
                    } else
                    {
                        m_HealthBarMask.style.height = lengthToCrop;
                    }
                }
            }
        };

    }
}
