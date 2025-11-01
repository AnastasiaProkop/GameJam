using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class WalkState : State
{
    public WalkState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }
    public override void Update()
    {
        player.transform.position = Vector3.MoveTowards(player.transform.position, player.targetPos, Time.deltaTime * player.moveSpeed);
        
        if (Vector3.Distance(player.transform.position, player.targetPos) < 0.005)
       {
          stateMachine.ChangeState(player.idleState);
        }

        base.Update();
    }
}
