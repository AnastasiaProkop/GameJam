using UnityEngine;

public class FixSideState : State
{
    public FixSideState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        player.skeletonAnimation.AnimationState.SetAnimation(0, "bort", true);
        base.Enter();
    }

    public override void Exit()
    {
        //player.skeletonAnimation.AnimationState.SetAnimation(0, "bort", false);
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }
}
