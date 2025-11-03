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
        public UIDocument startUIDoc;
        public Popup pausePopup;
        public Popup startPopup;
        private HealthBar healthBar;
        private HealthBar insanityBar;
        private Label coinLabel;
        private int numberOfCoins = 0;
        private bool paused = false;
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            Pause(true);
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
                FindPopups();
                pausePopup.AssignActionToButton("resume", GetPauseMenu);
                pausePopup.AssignActionToButton("exit", Exit);
                startPopup.AssignActionToButton("play", StartGame);
                startPopup.Open();
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

        void FindPopups()
        {
            if (popupUIDoc != null)
            {
                pausePopup = new Popup("pause_popup", popupUIDoc);
                startPopup = new Popup("start_popup", startUIDoc);
            }
            else
            {
                Debug.LogError($"Could not find popups because UI Document does not exist.");
            }
        }

        void Update()
        {
            healthBar?.Update(false);
            insanityBar?.Update(true);
            if (!paused && Input.GetKeyDown(KeyCode.Space))
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

        public void StartGame()
        {
            if (startUIDoc != null)
            {
                startPopup.Close();
                Pause(false);
            }
            else
            {
                Debug.LogWarning("Could not toggle popup. It was not loaded.");
            }
        }

        public void GetPauseMenu()
        {
            if (pausePopup != null)
            {
                bool isOpen = pausePopup.Toggle();
                Pause(isOpen);
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

        public void Pause(bool shouldPause)
        {
            if (shouldPause)
            {
                paused = true;
                Time.timeScale = 0;
            }
            else
            {
                paused = false;
                Time.timeScale = 1;
            }
        }
    }

}
