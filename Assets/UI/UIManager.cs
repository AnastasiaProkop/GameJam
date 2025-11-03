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
        public UIDocument popupUIDoc;
        // public UIDocument loseGameUIDoc;
        // public UIDocument startUIDoc;
        private UIDocument[] children;
        public Popup pausePopup;
        private HealthBar healthBar;
        private HealthBar insanityBar;
        private Label coinLabel;

        private int numberOfCoins = 0;
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            if (UIMainScreenDoc != null && UIMainScreenDoc.rootVisualElement != null)
            {
                // getting health label
                healthBar = new HealthBar("healthbar_mask", UIMainScreenDoc); // TODO: generalize class
                insanityBar = new HealthBar("insanity_mask", UIMainScreenDoc); // TODO: generalize class
                coinLabel = UIMainScreenDoc.rootVisualElement.Q<Label>("coins_num");
                //getting bottoms and assigning functions to them
                // AssignActionToButton("pause", GetPauseMenu);
                AssignActionToButton("DEBUG_LWR_H", LowerHealth);
                //getting popup
                FindPopupByName("pause_popup");
                pausePopup.AssignActionToButton("resume", GetPauseMenu);
                pausePopup.AssignActionToButton("exit", Exit);
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
            if (popupUIDoc != null)
            {
                pausePopup = new Popup(popupName, popupUIDoc);
            }
            else
            {
                Debug.LogError($"Could not find '{popupName}' because UI Document does not exist.");
            }
        }

        void Update()
        {
            healthBar?.Update(false);
            insanityBar?.Update(true);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GetPauseMenu();
            }
        }
        public void LowerHealth()
        {
            healthBar?.DebugSimulateHealthChange();
            insanityBar?.DebugSimulateHealthChange();
            coinLabel.text = (numberOfCoins++).ToString();
        }

        public void GetPauseMenu()
        {
            if (pausePopup != null)
            {
                bool isOpen = pausePopup.Toggle();
                if (isOpen)
                {
                    Time.timeScale = 0;
                }
                else
                {
                    Time.timeScale = 1;
                }
            }
            else
            {
                Debug.LogWarning("Could not toggle popup. It was not loaded.");
            }
        }
        
        public void Exit()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }

}
