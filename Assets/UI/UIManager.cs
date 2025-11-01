using System.Collections.Generic;
using System.Linq;
using UI.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public UIDocument UIMainScreenDoc;
        public UIDocument[] children;
        public Popup helpPopup;
        private HealthBar healthBar;
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            if (UIMainScreenDoc != null && UIMainScreenDoc.rootVisualElement != null)
            {
                // getting health label
                healthBar = new HealthBar("main", UIMainScreenDoc); // TODO: generalize class
                //getting bottoms and assigning functions to them
                AssignActionToButton("help", GetHelpMenu);
                AssignActionToButton("DEBUG_LWR_H", LowerHealth);
                //getting popup
                FindPopupByName("help_popup");
            }
            else
            {
                Debug.LogError("UIDocument not found. Make sure there's a UIDocument in the scene.");
            }
        }

        void AssignActionToButton(string buttonName, System.Action action)
        {
            var button = UIMainScreenDoc.rootVisualElement.Q<Button>(buttonName);
            if (button != null)
            {
                button.clicked += action;
            }
            else
            {
                Debug.LogError($"Could not find '{buttonName}' in UI Document");
            }
        }

        void FindPopupByName(string popupName)
        {
            if (UIMainScreenDoc == null) return;
            children = UIMainScreenDoc.gameObject.GetComponentsInChildren<UIDocument>(); // TODO: optimize
            var popupUIDoc = children.FirstOrDefault(x => x.name == popupName);
            if (popupUIDoc != null)
            {
                helpPopup = new Popup(popupName, popupUIDoc);
            }
            else
            {
                Debug.LogError($"Could not find '{popupName}' because UI Document does not exist.");
            }
        }

        void Update()
        {
            healthBar?.Update();

        }
        public void LowerHealth()
        {
            healthBar?.DebugSimulateHealthChange();
        }

        public void GetHelpMenu()
        {
            if (helpPopup != null)
            {
                helpPopup.Toggle();
                helpPopup.SetPosition(UIMainScreenDoc, 0, 0);
            }
            else
            {
                Debug.LogWarning("Could not toggle popup. It was not loaded.");
            }
        }
    }

}
