using UnityEngine;

public class Cannon : MonoBehaviour
{
    //[Tooltip("Ссылка на объект-подсветку")]
    //public GameObject highlightObject;
    
    private ShipTask shipTask;
    public bool IsTaskActive { get; private set; }

    void Awake()
    {
        shipTask = GetComponent<ShipTask>();
        shipTask.enabled = false; // Убедимся, что задача выключена
        //highlightObject.SetActive(false);
        IsTaskActive = false;
    }

    // Вызывается из TaskZone, когда щупальце хватает пушку
    public void ActivateTask(ShipManager manager, ShipTaskZone zone)
    {
        shipTask.enabled = true;
        shipTask.Initialize(manager, zone); // Важно инициализировать
        //highlightObject.SetActive(true);
        IsTaskActive = true;
    }


    public void DeactivateTask()
    {
        shipTask.enabled = false;
        //highlightObject.SetActive(false);
        IsTaskActive = false;
    }
}