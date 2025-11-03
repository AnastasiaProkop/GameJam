using UnityEngine;

public class FixSideState : State
{
    public FixSideState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        player.currentZone = player.nextZone;

        player.animatorEffects.gameObject.SetActive(true);

        player.skeletonAnimation.AnimationState.SetAnimation(0, animBoolName, true);
        player.shipManager.StartWorkInZone(
            TaskType.SideHole, player.currentZone - 1);
        base.Enter();
    }

    public override void Exit()
    {
        player.animatorEffects.gameObject.SetActive(false);
        //player.skeletonAnimation.AnimationState.SetAnimation(0, "bort", false);
        player.shipManager.StopWorkInZone(
            TaskType.SideHole, player.currentZone - 1);
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (!player.TaskAvailable())
        {
            player.stateMachine.ChangeState(player.idleState);
            player.ClearCurrentTask(); 
        }
    }

 
}
