using UnityEngine;
using System.Collections.Generic;

public class ShipTaskZone : MonoBehaviour
{
    private readonly List<ShipTask> TaskList = new();
    [SerializeField, Min(1)] private int MaxTaskQuantity = 2;

    public bool IsOccupied => TaskList.Count >= MaxTaskQuantity;


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

}