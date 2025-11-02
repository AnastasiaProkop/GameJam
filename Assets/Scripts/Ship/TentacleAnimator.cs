using UnityEngine;

public class TentacleAnimator : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAttackAnimation(TaskType taskType)
    {
        string stateName = GetAnimationStateName(taskType);
        if (!string.IsNullOrEmpty(stateName))
        {
            animator.Play(stateName);
        }
    }

    private string GetAnimationStateName(TaskType taskType)
    {
        switch (taskType)
        {
            case TaskType.FloorHole: return "StrikeFloor";
            case TaskType.Fire: return "SpitFire";
            case TaskType.Gun: return "GrabCannon";
            case TaskType.SideHole: return "StrikeSide";
            default: return null;
        }
    }

}
