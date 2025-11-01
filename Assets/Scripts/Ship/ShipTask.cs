using UnityEngine;

public class ShipTask : MonoBehaviour
{
    [Header("Параметры Задачи")]
    [Tooltip("Сколько времени (в секундах) нужно, чтобы выполнить задачу")]
    public float timeToComplete = 10f;
    
    [Tooltip("Какой урон в секунду наносит эта задача в состоянии 'Madness'")]
    public float damageInMadness = 1f; 
    [Tooltip("Базовая скорость, с которой эта задача заполняет полосу Безумия (ед/сек)")]
    public float madnessRate = 0.5f;

    [Header("Параметры Провала Задачи")]
    [Tooltip("Время в секундах, по истечении которого задача 'проваливается'")]
    public float failureTime = 20f;
    [Tooltip("Единовременный штраф к Безумию при провале в обычном состоянии")]
    public float madnessPenalty = 10f;
    [Tooltip("Единовременный урон кораблю при провале в состоянии Безумия")]
    public float damagePenalty = 10f;
    [Tooltip("На сколько увеличивается 'влияние' этой задачи на Безумие после провала")]
    public float madnessRateIncreaseOnFailure = 0.25f;
    [Tooltip("На сколько увеличивается урон этой задачи по кораблю после провала")]
    public float damageInMadnessIncreaseOnFailure = 0.5f;

    // Приватные переменные для отслеживания состояния
    private float currentFailureTimer;
    private float currentMadnessRate; // Текущее "влияние" задачи на безумие
    private float currentDamageInMadness; // Текущее количество урона, наносимое в состоянии безумия
    private bool isFailed = false;

    public float CurrentProgress { get; private set; }
    private bool isBeingWorkedOn = false;

    private ShipManager shipManager;
    private ShipTaskZone parentZone;

    public void Initialize(ShipManager manager, ShipTaskZone zone)
    {
        shipManager = manager;
        parentZone = zone;
    }

    void Start()
    {
        currentFailureTimer = failureTime;
        currentMadnessRate = madnessRate;
        currentDamageInMadness = damageInMadness;
    }

    void Update()
    {
        // Таймер провала тикает только в том случае, если он еще не сработал
        if (!isFailed)
        {
            HandleFailureTimer();
        }

        if (isBeingWorkedOn)
        {
            CurrentProgress = Mathf.Min(timeToComplete, CurrentProgress + Time.deltaTime);
            if (CurrentProgress >= timeToComplete)
            {
                Complete();
            }
        }
        else if (shipManager.CurrentState == ShipManager.ShipState.Madness)
        {
            shipManager.TakeDamage(currentDamageInMadness * Time.deltaTime);
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

    private void HandleFailureTimer()
    {
        currentFailureTimer = Mathf.Max(0, currentFailureTimer - Time.deltaTime);

        if (currentFailureTimer <= 0)
        {
            // Таймер истек, вызываем логику провала
            TriggerFailure();
        }
    }

    private void TriggerFailure()
    {
        isFailed = true;

        Debug.Log($"Задача '{this.name}' провалена по таймеру!");

        // Проверяем текущее состояние корабля и применяем соответствующий штраф
        if (shipManager.CurrentState == ShipManager.ShipState.Normal)
        {
            shipManager.IncreaseMadness(madnessPenalty);
            currentMadnessRate += madnessRateIncreaseOnFailure;
            Debug.Log($"Безумие увеличилось на {madnessPenalty}. Новое влияние задачи: {currentMadnessRate}");
        }
        else // Состояние Madness
        {
            shipManager.TakeDamage(damagePenalty);
            currentDamageInMadness += damageInMadnessIncreaseOnFailure;
            Debug.Log($"Корабль получил {damagePenalty} урона!");
        }
    }

    private void Complete()
    {
        Debug.Log("Задача выполнена!");
        parentZone.ClearTask(this);
        Destroy(gameObject);
    }

    public float GetCurrentMadnessRate()
    {
        return currentMadnessRate;
    }

    public float GetCurrentDamageInMadness()
    {
        return currentDamageInMadness;
    }
}
