using UnityEngine;
using System;

public class TentacleAnimator : MonoBehaviour
{
    private Animator animator;

    // События для связи с "режиссером" (TaskZone)
    public Action OnImpactAction;
    public Action OnAnimationCompleteAction;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAttackAnimation(TaskType taskType)
    {
        // Выбираем имя состояния анимации для проигрывания
        string stateName = GetAnimationStateName(taskType);

        if (!string.IsNullOrEmpty(stateName))
        {
            // animator.Play() напрямую запускает нужную анимацию
            animator.Play(stateName);
        }
        else
        {
            Debug.LogError($"Для типа задачи {taskType} не найдено имя анимации!", this);
        }
    }

    // Вспомогательный метод, чтобы связать тип задачи с именем состояния в аниматоре
    private string GetAnimationStateName(TaskType taskType)
    {
        switch (taskType)
        {
            case TaskType.FloorHole:
                return "StrikeFloor";
            case TaskType.Fire:
                return "SpitFire";
            case TaskType.Gun:
                return "GrabCannon";
            case TaskType.SideHole:
                return "StrikeSide";
            // добавить
            default:
                return null;
        }
    }

    // Вызывается в кадре удара/плевка/хватки
    public void OnImpactMoment()
    {
        // Сообщаем, что пора создавать интерактивную задачу
        OnImpactAction?.Invoke();
    }

    // Вызывается в ПОСЛЕДНЕМ кадре ЛЮБОЙ из 4 анимаций
    public void OnAnimationComplete()
    {
        // Сообщаем, что сценка окончена
        OnAnimationCompleteAction?.Invoke();
        // Щупальце самоуничтожается после завершения своей роли
        Destroy(gameObject);
    }
}