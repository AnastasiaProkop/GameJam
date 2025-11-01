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
        
        if (Vector3.Distance(player.transform.position, player.targetPos) < 0.0005)
        {
            if (player.currentTag == "Gun")
            {
                stateMachine.ChangeState(player.shootState);
            }
            if (player.currentTag == "FloorHole")
            {
                stateMachine.ChangeState(player.fixFloorState);
            }
            if (player.currentTag == "SideHole")
            {
                stateMachine.ChangeState(player.fixSideState);
            }
            if (player.currentTag == "Fire")
            {
                stateMachine.ChangeState(player.putOutFireState);
            }
            //stateMachine.ChangeState(player.idleState);
        }

        base.Update();
    }
}
