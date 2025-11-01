using UnityEngine;

public class CrewMember : MonoBehaviour
{
    // Состояния матроса
    public enum CrewState { Idle, Moving, Working }
    public CrewState CurrentState { get; private set; }

    [Header("Параметры")]
    [Tooltip("Скорость передвижения матроса")]
    public float moveSpeed = 5f;

    // Ссылки на компоненты и цели
    private ShipTask assignedTask;
    private Vector3 targetPosition;
    private Animator animator;

    void Awake()
    {
        // Получаем компонент Animator при старте
        animator = GetComponent<Animator>();
        CurrentState = CrewState.Idle;
    }

    void Update()
    {
        // Если матрос в состоянии движения, он должен двигаться
        if (CurrentState == CrewState.Moving)
        {
            MoveToTarget();
        }
    }

    /// Главный метод для назначения задачи матросу.
    public void AssignToTask(ShipTask newTask)
    {
        // 1. Если матрос уже работал над старой задачей, он прекращает работу
        if (assignedTask != null)
        {
            assignedTask.StopWork();
        }

        // 2. Назначаем новую задачу
        assignedTask = newTask;

        // 3. Если новой задачи нет (например, игрок отменил действие), матрос становится свободным
        if (assignedTask == null)
        {
            CurrentState = CrewState.Idle;
            animator.SetBool("IsRunning", false);
            return;
        }

        // 4. Если задача есть, задаем цель и начинаем движение
        targetPosition = assignedTask.transform.position;
        CurrentState = CrewState.Moving;
        animator.SetBool("IsRunning", true); // Включаем анимацию бега
    }

    private void MoveToTarget()
    {
        // Проверяем, достаточно ли мы близко к цели
        if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // Двигаемся к цели
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            
            // Опционально: поворачиваем спрайт в сторону движения
            if (targetPosition.x > transform.position.x)
                transform.localScale = new Vector3(1, 1, 1);
            else if (targetPosition.x < transform.position.x)
                transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            // Мы прибыли в точку назначения
            ArrivedAtTask();
        }
    }

    private void ArrivedAtTask()
    {
        // Прибыв на место, матрос переходит в состояние работы
        CurrentState = CrewState.Working;
        transform.position = targetPosition; // "Прилипаем" к точке, чтобы избежать мелких смещений
        animator.SetBool("IsRunning", false); // Выключаем анимацию бега

        // Сообщаем задаче, что мы начали над ней работать
        if (assignedTask != null)
        {
            // Проверяем, существует ли задача до сих пор в этой зоне
            if (assignedTask.gameObject.activeInHierarchy)
            {
                assignedTask.StartWork();
                Debug.Log("Матрос прибыл и начал работать.");
            }
        }
    }
}