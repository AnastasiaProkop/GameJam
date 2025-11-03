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

    private Dictionary<TaskType, PrePlacedTask> taskRegistry;
    public bool IsOccupied => TaskList.Count >= taskRegistry.Count;
    public bool IsTaskActive(TaskType type) => TaskList.Any(t => t.taskType == type);
    private bool isSpawning = false; // Флаг, чтобы не запускать две анимации одновременно

    void Awake()
    {
        TaskList = new List<ShipTask>();

        taskRegistry = new Dictionary<TaskType, PrePlacedTask>();
        foreach (var placement in prePlacedTasks)
        {
            if (placement.taskObject != null && !taskRegistry.ContainsKey(placement.taskType))
            {
                taskRegistry.Add(placement.taskType, placement);
            }
        }
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
        var availableTypes = taskRegistry.Keys.Where(type => !IsTaskActive(type)).ToList();
        if (targetCannon != null && !targetCannon.IsTaskActive)
        {
            availableTypes.Add(TaskType.Gun);
        }

        if (availableTypes.Count > 0)
        {
            TaskType chosenType = availableTypes[UnityEngine.Random.Range(0, availableTypes.Count)];
            StartCoroutine(SpawnTaskSequence(manager, chosenType));
        }
    }

    private IEnumerator SpawnTaskSequence(ShipManager manager, TaskType taskType)
    {
        isSpawning = true;

        ShipTask taskInfoProvider = GetTaskObjectByType(taskType);
        if (taskInfoProvider == null)
        {
            Debug.LogError($"Не удалось найти информацию о задаче типа {taskType}");
            isSpawning = false;
            yield break;
        }
        
        // Определяем, где появится щупальце
        Transform spawnPoint = GetAnchorForTaskType(taskType);
        GameObject tentacleObj = Instantiate(tentacleAnimatorPrefab, spawnPoint.position, spawnPoint.rotation);
        TentacleAnimator tentacle = tentacleObj.GetComponent<TentacleAnimator>();
        tentacle.PlayAttackAnimation(taskType);
        yield return new WaitForSeconds(taskInfoProvider.taskCreationTiming);

        ActivatePrePlacedTask(manager, taskType);
        float remainingAnimationTime = taskInfoProvider.appearanceAnimationDuration - taskInfoProvider.taskCreationTiming;
        if (remainingAnimationTime > 0)
        {
            yield return new WaitForSeconds(remainingAnimationTime);
        }

        if (tentacleObj != null)
        {
            Destroy(tentacleObj);
        }

        isSpawning = false;
    }

    private ShipTask GetTaskObjectByType(TaskType type)
    {
        if (type == TaskType.Gun && targetCannon != null)
        {
            return targetCannon.GetComponent<ShipTask>();
        }
        if (taskRegistry.TryGetValue(type, out PrePlacedTask placement))
        {
            return placement.taskObject;
        }
        return null;
    }

    private void ActivatePrePlacedTask(ShipManager manager, TaskType taskType)
    {
        if (taskType == TaskType.Gun)
        {
            targetCannon?.ActivateTask(manager, this);
            AddTask(targetCannon.GetComponent<ShipTask>());
        }
        else if (taskRegistry.TryGetValue(taskType, out PrePlacedTask placement))
        {
            placement.taskObject.ActivateTask(manager, this);
            AddTask(placement.taskObject);
        }
    }

    public void StartWork(TaskType task)
    {
        TaskList.Find(shipTask => shipTask.taskType == task)?.StartWork();
    }

    public void StopWork(TaskType task)
    {
        TaskList.Find(shipTask => shipTask.taskType == task)?.StopWork();
    }

    private Transform GetAnchorForTaskType(TaskType type)
    {
        if (type == TaskType.Gun)
        {
            return targetCannon.animationAnchor; 
        }
        if (taskRegistry.TryGetValue(type, out PrePlacedTask placement))
        {
            return placement.animationAnchor != null ? placement.animationAnchor : transform;
        }
        return transform; 
    }

    public bool IsFull()
    {
        int activeCount = TaskList.Count;
        int possibleCount = taskRegistry.Count + (targetCannon != null ? 1 : 0);
        return activeCount >= possibleCount;
    }

}
