using UnityEngine;

public class FixFloorState : State
{
    public FixFloorState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        player.skeletonAnimation.AnimationState.SetAnimation(0, "water", true);
        base.Enter();
    }

    public override void Exit()
    {
       // player.skeletonAnimation.AnimationState.SetAnimation(0, "water", false);
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }
}
