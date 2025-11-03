using UnityEngine;

public class ShootState : State
{
    public ShootState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        player.currentZone = player.nextZone;

        player.shipManager.StartWorkInZone(
            TaskType.Gun, player.currentZone - 1);
        base.Enter();
    }

    public override void Exit()
    {
        player.shipManager.StopWorkInZone(
            TaskType.Gun, player.currentZone - 1);
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }
}
