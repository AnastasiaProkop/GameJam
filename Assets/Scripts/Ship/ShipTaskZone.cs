using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ShipTaskZone : MonoBehaviour
{
    [Header("Настройка Сценки")]
    [Tooltip("Префаб анимированного щупальца")]
    public GameObject tentacleAnimatorPrefab;

    [Header("Настройка Зоны")]
    [Tooltip("Список префабов задач, которые МОГУТ появиться в этой зоне")]
    public List<GameObject> allowedTaskPrefabs;

    [Tooltip("Ссылка на пушку, которую может захватить щупальце")]
    public Cannon targetCannon; 

    public List<ShipTask> TaskList { get; private set; }
    //[SerializeField, Min(1)] private int MaxTaskQuantity = 2;

    public bool IsOccupied => TaskList.Count >= allowedTaskPrefabs.Count;
    private bool isSpawning = false; // Флаг, чтобы не запускать две анимации одновременно

    void Awake()
    {
        TaskList = new List<ShipTask>();
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
        if (isSpawning || IsOccupied) return;

        // Находим доступные для спавна задачи
        var availablePrefabs = allowedTaskPrefabs.Where(p => !HasTaskOfType(p.GetComponent<ShipTask>().taskType)).ToList();
        
        // Проверяем особый случай с пушкой
        bool canSpawnCannonTask = (targetCannon != null && !targetCannon.IsTaskActive);
        if(canSpawnCannonTask)
        {
            availablePrefabs.Add(targetCannon.gameObject);
        }

        if (availablePrefabs.Count > 0)
        {
            GameObject chosenPrefab = availablePrefabs[Random.Range(0, availablePrefabs.Count)];
            StartCoroutine(SpawnTaskSequence(manager, chosenPrefab.GetComponent<ShipTask>().taskType));
        }
    }

    private IEnumerator SpawnTaskSequence(ShipManager manager, TaskType taskType)
    {
        isSpawning = true;

        // 1. Создаем щупальце-актёра
        GameObject tentacleObj = Instantiate(tentacleAnimatorPrefab, transform.position, Quaternion.identity);
        TentacleAnimator tentacle = tentacleObj.GetComponent<TentacleAnimator>();

        // 2. Подписываемся на его события
        tentacle.OnImpactAction += () => {
            CreateInteractableTask(manager, taskType);
        };
        
        // 3. Запускаем анимацию
        tentacle.PlayAttackAnimation(taskType);
        
        yield return new WaitForSeconds(5f);

        isSpawning = false;
    }

    private void CreateInteractableTask(ShipManager manager, TaskType taskType)
    {
        // Особый случай с пушкой
        if (taskType == TaskType.Gun && targetCannon != null)
        {
            targetCannon.ActivateTask(manager, this); // Активируем задачу на самой пушке
            AddTask(targetCannon.GetComponent<ShipTask>());
        }
        else // Обычные задачи (дыра, пожар и т.д.)
        {
            GameObject taskPrefab = allowedTaskPrefabs.First(p => p.GetComponent<ShipTask>().taskType == taskType);
            if (taskPrefab != null)
            {
                GameObject taskObject = Instantiate(taskPrefab, transform.position, Quaternion.identity, transform);
                ShipTask newTask = taskObject.GetComponent<ShipTask>();
                newTask.Initialize(manager, this);
                AddTask(newTask);
            }
        }
    }

}
