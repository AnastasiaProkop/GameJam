using UnityEngine;

public class FixFloorState : State
{
    public FixFloorState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        SoundManager.Instance.FixFloor();

        player.currentZone = player.nextZone;

        player.skeletonAnimation.AnimationState.SetAnimation(0, animBoolName, true);
        player.shipManager.StartWorkInZone(
            TaskType.FloorHole, player.currentZone - 1);
        base.Enter();
    }

    public override void Exit()
    {
        SoundManager.Instance.StopFixFloor();

        player.shipManager.StopWorkInZone(
            TaskType.FloorHole, player.currentZone - 1);
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (!player.TaskAvailable())
        {
            player.stateMachine.ChangeState(player.idleState);
        }
    }
}
