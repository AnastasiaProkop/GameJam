using UnityEngine;
using System;

public class TentacleAnimator : MonoBehaviour
{
    private Animator animator;
    public Action OnImpactAction;
    public Action OnAnimationCompleteAction;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // TaskZone вызовет этот метод, чтобы запустить нужную анимацию
    public void PlayAttackAnimation(TaskType taskType)
    {
        switch (taskType)
        {
            case TaskType.FloorHole:
                animator.SetTrigger("OnStrikeFloor");
                break;
            case TaskType.Fire:
                animator.SetTrigger("OnSpitFire");
                break;
            case TaskType.Gun:
                animator.SetTrigger("OnGrabCannon");
                break;
            // добавить
        }
    }


    // Вызывается в кадре удара/плевка/хватки
    public void OnImpactMoment()
    {
        OnImpactAction?.Invoke();
    }

    // Вызывается в последнем кадре анимации ухода под воду
    public void OnRetreatComplete()
    {
        OnAnimationCompleteAction?.Invoke(); // Сообщаем, что сценка окончена
        Destroy(gameObject);
    }
}