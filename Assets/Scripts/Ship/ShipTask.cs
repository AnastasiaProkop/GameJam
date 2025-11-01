using UnityEngine;

public class ShipTask : MonoBehaviour
{
    [Header("Параметры Задачи")]
    [Tooltip("Сколько времени (в секундах) нужно, чтобы выполнить задачу")]
    public float timeToComplete = 10f;
    [Tooltip("Какой урон в секунду наносит эта задача в состоянии 'Madness'")]
    public float damageInMadness = 1f; // Переименованная переменная

    public float CurrentProgress { get; private set; }
    private bool isBeingWorkedOn = false;

    private ShipManager shipManager;
    private ShipTaskZone parentZone;

    public void Initialize(ShipManager manager, ShipTaskZone zone)
    {
        shipManager = manager;
        parentZone = zone;
    }

    void Update()
    {
        if (isBeingWorkedOn)
        {
            CurrentProgress += Time.deltaTime;
            if (CurrentProgress >= timeToComplete)
            {
                Complete();
            }
        }
        // Обновленная проверка состояния
        else if (shipManager.CurrentState == ShipManager.ShipState.Madness)
        {
            shipManager.TakeDamage(damageInMadness * Time.deltaTime);
        }
    }

    public void StartWork()
    {
        isBeingWorkedOn = true;
    }

    public void StopWork()
    {
        isBeingWorkedOn = false;
    }

    private void Complete()
    {
        Debug.Log("Задача выполнена!");
        parentZone.ClearTask(this);
        Destroy(gameObject);
    }
}
