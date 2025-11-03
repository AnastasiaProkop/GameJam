using UnityEngine;

public class Cannon : MonoBehaviour
{
    //[Tooltip("Ссылка на объект-подсветку")]
    //public GameObject highlightObject;
    public Transform animationAnchor;
    
    private ShipTask shipTask;
    public bool IsTaskActive { get; private set; }

    [Header("Эффекты Выстрела")]
    [Tooltip("Перетащите сюда префаб эффекта взрыва")]
    public GameObject explosionVFXPrefab;
    [Tooltip("Перетащите сюда дочерний объект 'MuzzlePoint'")]
    public Transform pointVFX;

    void Awake()
    {
        shipTask = GetComponent<ShipTask>();
        shipTask.enabled = false; // Убедимся, что задача выключена
        //highlightObject.SetActive(false);

        if (animationAnchor == null)
        {
            Debug.LogWarning("У пушки не назначен AnimationAnchor! Будет использоваться ее центр.", this);
            // В качестве запасного варианта используем transform самой пушки
            animationAnchor = this.transform;
        }
        IsTaskActive = false;
    }

    // Вызывается из TaskZone, когда щупальце хватает пушку
    public void ActivateTask(ShipManager manager, ShipTaskZone zone)
    {
        shipTask.enabled = true;
        shipTask.Initialize(manager, zone); // Важно инициализировать
        //highlightObject.SetActive(true);
        IsTaskActive = true;
        Debug.Log("Пушку схватили");
    }


    public void DeactivateTask()
    {
        shipTask.enabled = false;
        //highlightObject.SetActive(false);
        if (explosionVFXPrefab != null && pointVFX != null)
        {
            Instantiate(explosionVFXPrefab, pointVFX.position, pointVFX.rotation);
        }
        IsTaskActive = false;
    }
}