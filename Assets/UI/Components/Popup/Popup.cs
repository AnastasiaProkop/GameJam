using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    namespace Components
    {
        public class Popup
        {
            private readonly VisualElement popup;
            private readonly UIDocument doc;
            private bool isHelpVisible = false;
            private readonly System.Action onClose = null;

            public Popup(string popupName, UIDocument document, System.Action action = null)
            {
                doc = document;
                popup = doc.rootVisualElement.Q<VisualElement>(popupName);
                onClose = action;
            }

            public void Close()
            {
                if (popup != null)
                {
                    popup.style.display = DisplayStyle.None;
                    isHelpVisible = false;
                    onClose?.Invoke();
                }
            }

            public void SetPosition(UIDocument target, float xPercent, float yPercent)
            {
                if (target == null || doc == null) return;

                VisualElement targetRoot = target.rootVisualElement;
                VisualElement docRoot = doc.rootVisualElement;
                docRoot.style.position = Position.Absolute;
                docRoot.style.left = targetRoot.layout.x + (targetRoot.layout.width * xPercent / 100f);
                docRoot.style.top = targetRoot.layout.y + (targetRoot.layout.height * yPercent / 100f);
            }

            public void Open()
            {
                if (popup != null)
                {
                    popup.style.display = DisplayStyle.Flex;
                    isHelpVisible = true;
                }
            }

            public void Toggle()
            {
                if (isHelpVisible)
                    Close();
                else
                    Open();
            }
        }
    }
}