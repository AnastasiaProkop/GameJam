using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ShipManager : MonoBehaviour
{
    public enum ShipState { Normal, Madness }
    public ShipState CurrentState { get; private set; }


    [Header("Настройки здоровья")]
    [Tooltip("Максимальное здоровье корабля")]
    public float maxHealth = 100f;
    public float CurrentHealth { get; private set; }



    [Header("Настройки Безумия (Madness)")]
    [Tooltip("Максимальное значение полосы Безумия. При достижении этого значения корабль переходит в состояние Madness.")]
    public float maxMadnessValue = 100f;
    [Tooltip("Базовая скорость, с которой Безумие растет само по себе, даже без задач")]
    public float madnessBaseIncreaseRate = 1.0f;
    [Tooltip("Насколько быстро полоса Безумия убывает в состоянии Madness (единиц в секунду).")]
    public float madnessDecayRate = 5.0f;

    // Текущее значение полосы Безумия
    public float CurrentMadness { get; private set; }

    [Header("Настройки Задач")]
    [Tooltip("Список всех зон, где могут появляться задачи")]
    public List<ShipTaskZone> shipTaskZones; 
    [Tooltip("Префаб задачи 'Пробоина'")]
    public GameObject breachTaskPrefab;

    [Tooltip("Как часто (в секундах) игра пытается создать новую задачу")]
    public float taskSpawnInterval = 15f;
    private float taskSpawnTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentHealth = maxHealth;
        CurrentState = ShipState.Normal;
        CurrentMadness = 0f; // Начинаем с нуля
        taskSpawnTimer = taskSpawnInterval;
    }

    // Update is called once per frame
    void Update()
    {
        //Пытаемся создать задачу
        HandleTaskSpawning();

        //Обновляем состояние
        switch (CurrentState)
        {
            case ShipState.Normal:
                UpdateNormalState();
                break;
            case ShipState.Madness:
                UpdateMadnessState();
                break;
        }

        //Debug.Log($"Состояние: {CurrentState}, Безумие: {CurrentMadness}/{maxMadnessValue}, Здоровье: {CurrentHealth}");
    }

    private void HandleTaskSpawning()
    {
        taskSpawnTimer -= Time.deltaTime;
        if (taskSpawnTimer <= 0)
        {
            SpawnNewTask();
            taskSpawnTimer = taskSpawnInterval;
        }
    }

    private void SpawnNewTask()
    {
        // Ищем все свободные зоны
        List<ShipTaskZone> availableZones = shipTaskZones.Where(zone => !zone.IsOccupied).ToList();
        
        
        if (availableZones.Count > 0)
        {
            // Выбираем случайную свободную зону
            ShipTaskZone randomZone = availableZones[Random.Range(0, availableZones.Count)];
            
            // Создаем экземпляр задачи и размещаем его в зоне
            // В будущем здесь можно будет выбирать тип задачи
            GameObject taskObject = Instantiate(breachTaskPrefab, randomZone.transform.position, Quaternion.identity, randomZone.transform);
            ShipTask newTask = taskObject.GetComponent<ShipTask>();
            
            // Передаем задаче ссылки на себя и на зону
            newTask.Initialize(this, randomZone);
            randomZone.AddTask(newTask);
            
            Debug.Log($"Новая задача создана в зоне: {randomZone.name}");
        }
        Debug.Log($"Нет места для новой задачи");
    }

    private void UpdateNormalState()
    {
        float totalIncreaseRate = madnessBaseIncreaseRate;

        // Проходим по всем активным задачам и суммируем их индивидуальное влияние
        foreach (var zone in shipTaskZones)
        {
            foreach (var task in zone.TaskList)
            {
                //доделать после Насти (возможность в ShipTaskZone размещать несколько задач)
                totalIncreaseRate += task.GetCurrentMadnessRate();
            }
        }

        //CurrentMadness += totalIncreaseRate * Time.deltaTime;
        IncreaseMadness(totalIncreaseRate * Time.deltaTime);

        if (CurrentMadness >= maxMadnessValue)
        {
            CurrentMadness = maxMadnessValue;
            CurrentState = ShipState.Madness;
            Debug.Log("КОРАБЛЬ ОХВАЧЕН БЕЗУМИЕМ!");
        }
    }
    
    private void UpdateMadnessState()
    {
        // Полоса безумия уменьшается
        // CurrentMadness = Mathf.Max(0, CurrentMadness - madnessDecayRate * Time.deltaTime);
        DecreaseMadness(madnessDecayRate * Time.deltaTime);

        // Проверяем, не вернулись ли мы в нормальное состояние
        if (CurrentMadness <= 0)
        {
            CurrentMadness = 0;
            CurrentState = ShipState.Normal;
            Debug.Log("Безумие отступило. Корабль в обычном состоянии.");
        }

        // Проверяем здоровье корабля
        if (CurrentHealth <= 0)
        {
            Debug.Log("ИГРА ОКОНЧЕНА! Корабль уничтожен.");
            // Логика поражения
        }


    }

    // Публичный метод, который задачи будут вызывать для нанесения урона
    public void TakeDamage(float damage)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
    }

    public void IncreaseMadness(float amount)
    {
        if (CurrentState == ShipState.Normal)
        {
            CurrentMadness = Mathf.Min(maxMadnessValue, CurrentMadness + amount);
        }
    }

    public void DecreaseMadness(float amount)
    {
        CurrentMadness = Mathf.Max(0, CurrentMadness - amount);
    }

    public int GetActiveTaskCount()
    {
        // Возвращаем количество активных задач
        return shipTaskZones.Count(zone => zone.IsOccupied);
       // return 1;
    }


}
