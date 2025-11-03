using UnityEngine;
using UnityEngine.UI;

public class TaskUI : MonoBehaviour
{
    public Image failureBar;
    public Image progressBar;
    private Camera mainCamera;

    void Start() 
    { 
        mainCamera = Camera.main; 
    }

    // Заставляет UI всегда смотреть на камеру
    void LateUpdate()
    {
        if (mainCamera != null)
        {
            transform.forward = mainCamera.transform.forward;
        }
    }

    public void UpdateBars(float progressNormalized, float failureTimeNormalized)
    {
        if(progressBar != null) progressBar.fillAmount = progressNormalized;
        if(failureBar != null) failureBar.fillAmount = failureTimeNormalized;
    }
}