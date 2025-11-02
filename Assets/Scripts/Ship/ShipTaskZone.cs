using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class ShipTaskZone : MonoBehaviour
{
    [Header("Настройка Сценки")]
    [Tooltip("Префаб анимированного щупальца")]
    public GameObject tentacleAnimatorPrefab;

    [System.Serializable]
    public class PrePlacedTask
    {
        public TaskType taskType;
        public ShipTask taskObject; 
        public Transform animationAnchor;
    }
    [Tooltip("Заполните этот список всеми задачами, которые могут появиться в этой зоне")]
    public List<PrePlacedTask> prePlacedTasks;

    [Tooltip("Ссылка на пушку, которую может захватить щупальце")]
    public Cannon targetCannon; 

    public List<ShipTask> TaskList { get; private set; }
    //[SerializeField, Min(1)] private int MaxTaskQuantity = 2;

    private Dictionary<TaskType, ShipTask> taskRegistry;
    public bool IsOccupied => TaskList.Count >= taskRegistry.Count;
    public bool IsTaskActive(TaskType type) => TaskList.Any(t => t.taskType == type);
    private bool isSpawning = false; // Флаг, чтобы не запускать две анимации одновременно

    void Awake()
    {
        TaskList = new List<ShipTask>();

        taskRegistry = new Dictionary<TaskType, ShipTask>();
        // Преобразуем удобный список из инспектора в быстрый словарь
        foreach (var placement in prePlacedTasks)
        {
            if (placement.taskObject != null && !taskRegistry.ContainsKey(placement.taskType))
            {
                taskRegistry.Add(placement.taskType, placement.taskObject);
                // Убедимся, что все задачи выключены при старте
                placement.taskObject.gameObject.SetActive(false);
            }
        }
        // Добавляем пушку в нашу картотеку, если она есть
        if (targetCannon != null)
        {
            taskRegistry.Add(TaskType.Gun, targetCannon.GetComponent<ShipTask>());
        }
    }

    public bool HasTaskOfType(TaskType type)
    {
        return TaskList.Any(task => task.taskType == type);
    }

    public void AddTask(ShipTask task)
    {
        if (IsOccupied)
        {
            Debug.Log($"Нет места для новой задачи");
            return;
        }
        TaskList.Add(task);
    }

    public void ClearTask(ShipTask task)
    {
        TaskList.Remove(task);
    }

    public void TrySpawnNewTask(ShipManager manager)
    {
        if (isSpawning || IsFull()) return;

        // Находим все задачи, которые еще не активны
        var availablePlacements = prePlacedTasks.Where(p => !IsTaskActive(p.taskType)).ToList();

        if (availablePlacements.Count > 0)
        {
            // Выбираем случайную из ДОСТУПНЫХ заготовок
            PrePlacedTask chosenPlacement = availablePlacements[UnityEngine.Random.Range(0, availablePlacements.Count)];
            // Запускаем корутину, передавая в нее всю информацию о задаче
            StartCoroutine(SpawnTaskSequence(manager, chosenPlacement));
        }
    }

    private IEnumerator SpawnTaskSequence(ShipManager manager, PrePlacedTask placement)
    {
        isSpawning = true;

        // 1. Создаем щупальце-актёра
        Transform spawnPoint = placement.animationAnchor != null ? placement.animationAnchor : this.transform;
        GameObject tentacleObj = Instantiate(tentacleAnimatorPrefab, spawnPoint.position, spawnPoint.rotation);
        TentacleAnimator tentacle = tentacleObj.GetComponent<TentacleAnimator>();

        // Флаг, который мы будем ждать. Он станет 'true', когда анимация закончится.
        bool animationFinished = false;

        Action onImpact = () => { ActivatePrePlacedTask(manager, placement.taskType); };
        Action onComplete = () => { animationFinished = true; };

        try
        {
            // Подписываемся на события
            tentacle.OnImpactAction += onImpact;
            tentacle.OnAnimationCompleteAction += onComplete;

            // Запускаем нужную анимацию
            tentacle.PlayAttackAnimation(placement.taskType);
            
            // Ждем, пока флаг animationFinished не станет true.
            yield return new WaitUntil(() => animationFinished);
        }
        finally
        {
            // Гарантированно отписываемся от событий, даже если что-то пошло не так
            if (tentacle != null)
            {
                tentacle.OnImpactAction -= onImpact;
                tentacle.OnAnimationCompleteAction -= onComplete;
            }
            isSpawning = false;
        }

    }

    // private void CreateInteractableTask(ShipManager manager, TaskType taskType)
    // {
    //     // Особый случай с пушкой
    //     if (taskType == TaskType.Gun && targetCannon != null)
    //     {
    //         targetCannon.ActivateTask(manager, this); // Активируем задачу на самой пушке
    //         AddTask(targetCannon.GetComponent<ShipTask>());
    //     }
    //     else // Обычные задачи (дыра, пожар и т.д.)
    //     {
    //         GameObject taskPrefab = allowedTaskPrefabs.First(p => p.GetComponent<ShipTask>().taskType == taskType);
    //         if (taskPrefab != null)
    //         {
    //             GameObject taskObject = Instantiate(taskPrefab, transform.position, Quaternion.identity, transform);
    //             ShipTask newTask = taskObject.GetComponent<ShipTask>();
    //             newTask.Initialize(manager, this);
    //             AddTask(newTask);
    //         }
    //     }
    // }

    private void ActivatePrePlacedTask(ShipManager manager, TaskType taskType)
    {
        // Находим нужную задачу в словаре
        if (taskRegistry.TryGetValue(taskType, out ShipTask taskToActivate))
        {
            if (taskType == TaskType.Gun)
            {
                GetComponent<Cannon>()?.ActivateTask(manager, this);
            }
            else
            {
                // Просто включаем объект
                taskToActivate.gameObject.SetActive(true);
                taskToActivate.Initialize(manager, this);
            }
            
            AddTask(taskToActivate);
        }
    }

    // Вспомогательный метод IsFull()
    private bool IsFull()
    {
        // Считаем количество активных задач из списка заранее размещенных
        int activePrePlaced = prePlacedTasks.Count(p => IsTaskActive(p.taskType));
        // Добавляем пушку, если она активна
        int totalActive = activePrePlaced + (targetCannon != null && targetCannon.IsTaskActive ? 1 : 0);
        // Сравниваем с общим количеством возможных задач
        int totalPossible = prePlacedTasks.Count + (targetCannon != null ? 1 : 0);
        
        return totalActive >= totalPossible;
    }

}
