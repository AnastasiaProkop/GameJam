using UnityEngine;

public enum TaskType
{
    Gun,
    FloorHole,
    SideHole,
    Fire
}

public class ShipTask : MonoBehaviour
{
    [Header("Тип Задачи")]
    public TaskType taskType;

    [Header("Параметры Задачи")]
    [Tooltip("Сколько времени (в секундах) нужно, чтобы выполнить задачу")]
    public float timeToComplete = 10f;
    
    [Tooltip("Какой урон в секунду наносит эта задача в состоянии 'Madness'")]
    public float baseDamageInMadness = 1f; 
    [Tooltip("Базовая скорость, с которой эта задача заполняет полосу Безумия (ед/сек)")]
    public float baseMadnessRate = 0.5f;

    [Header("Параметры Провала Задачи")]
    [Tooltip("Время в секундах, по истечении которого задача 'проваливается'")]
    public float failureTime = 20f;
    [Tooltip("Единовременный штраф к Безумию при провале в обычном состоянии")]
    public float madnessPenalty = 10f;
    [Tooltip("Единовременный урон кораблю при провале в состоянии Безумия")]
    public float damagePenalty = 10f;
    [Tooltip("На сколько увеличивается 'влияние' этой задачи на Безумие после провала")]
    public float baseMadnessRateIncreaseOnFailure = 0.25f;
    [Tooltip("На сколько увеличивается урон этой задачи по кораблю после провала")]
    public float baseDamageInMadnessIncreaseOnFailure = 0.5f;

    // Анимация
    [Header("Параметры Анимации")]
    [Tooltip("Примерная длительность анимации появления для этой задачи (в секундах)")]
    public float appearanceAnimationDuration = 4.0f;
    public float taskCreationTiming = 1.0f;
    
    // Приватные переменные для отслеживания состояния
    private float currentFailureTimer;
    private float currentbaseMadnessRate; // Текущее "влияние" задачи на безумие
    private float currentbaseDamageInMadness; // Текущее количество урона, наносимое в состоянии безумия
    
    public float CurrentProgress { get; private set; }
    private bool isBeingWorkedOn = false;
    private bool isFailed = false;

    private Animator taskAnimator;
    [Header("UI")]
    public GameObject taskUIPrefab;
    public Vector3 uiOffset; // Смещение, чтобы UI был над задачей
    private TaskUI currentTaskUI;

    private ShipManager shipManager; // ссылка на ShipManager, чтобы отслеживать состояние корабля(обычное/безумие)
    private ShipTaskZone parentZone; // ссылка на ShipTaskZone, в которой нахожится задача, чтобы после выполнения задачи отключать её

    public void Initialize(ShipManager manager, ShipTaskZone zone)
    {
        shipManager = manager;
        parentZone = zone;
    }

    void Awake()
    {
        taskAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        CurrentProgress = 0f;
        isBeingWorkedOn = false;
        isFailed = false;
        currentFailureTimer = failureTime;
        currentbaseMadnessRate = baseMadnessRate;
        currentbaseDamageInMadness = baseDamageInMadness;
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
            shipManager.TakeDamage(currentbaseDamageInMadness * Time.deltaTime);
        }

        if (currentTaskUI != null)
        {
            float progress = CurrentProgress / timeToComplete;
            float failureTime = currentFailureTimer / this.failureTime;
            currentTaskUI.UpdateBars(progress, failureTime);
        }
    }

    public void ActivateTask(ShipManager manager, ShipTaskZone zone)
    {
        Initialize(manager, zone);
        gameObject.SetActive(true);
        if (taskAnimator != null)
        {
            taskAnimator.SetBool("IsActive", true);
        }


        // Создаем UI над задачей
        if (taskUIPrefab != null && currentTaskUI == null)
        {
            GameObject uiObj = Instantiate(taskUIPrefab, transform.position + uiOffset, Quaternion.identity, transform);
            currentTaskUI = uiObj.GetComponent<TaskUI>();
        }
    }
    

    public void StartWork()
    {
        Debug.Log("Started fixing " + taskType.ToString());
        isBeingWorkedOn = true;
    }

    public void StopWork()
    {
        Debug.Log("Stopped fixing " + taskType.ToString());
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
            currentbaseMadnessRate += baseMadnessRateIncreaseOnFailure;
            Debug.Log($"Безумие увеличилось на {madnessPenalty}. Новое влияние задачи: {currentbaseMadnessRate}");
        }
        else // Состояние Madness
        {
            shipManager.TakeDamage(damagePenalty);
            currentbaseDamageInMadness += baseDamageInMadnessIncreaseOnFailure;
            Debug.Log($"Корабль получил {damagePenalty} урона!");
        }
    }

    private void Complete()
    {
        Debug.Log("Задача выполнена!");
        parentZone.ClearTask(this);

        if (taskType == TaskType.Gun)
        {
            GetComponent<Cannon>()?.DeactivateTask();
        }
        else
        {
            ResetTask();
        }
    }

    // Этот метод будет вызываться, когда задача выполнена и должна снова скрыться
    public void ResetTask()
    {
        // Сбрасываем все изменяемые состояния
        Start();
        gameObject.SetActive(false);
        if (currentTaskUI != null)
        {
            Destroy(currentTaskUI.gameObject);
        }
    }

    public float GetCurrentMadnessRate()
    {
        return currentbaseMadnessRate;
    }

    public float GetCurrentDamageInMadness()
    {
        return currentbaseDamageInMadness;
    }

    public bool TaskAvailable()
    {
        return !isBeingWorkedOn;
    }
}
