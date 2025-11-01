using UnityEngine;

public class TaskZone : MonoBehaviour
{
    public ShipTask CurrentTask { get; private set; }
    public bool IsOccupied => CurrentTask != null;

    public void AssignTask(ShipTask task)
    {
        CurrentTask = task;
    }

    public void ClearTask()
    {
        CurrentTask = null;
    }
}