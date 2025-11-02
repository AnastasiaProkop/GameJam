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
        Debug.Log("Walk");
    }

    public override void Exit()
    {
        base.Exit();
    }
    public override void Update()
    {
        if (!player.navMeshAgent.pathPending)
        {
            if (player.navMeshAgent.remainingDistance <= player.navMeshAgent.stoppingDistance)
            {
                if (!player.navMeshAgent.hasPath || player.navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    // Агент остановился — выбираем действие
                    if (player.currentTag == "Gun")
                        stateMachine.ChangeState(player.shootState);
                    else if (player.currentTag == "FloorHole")
                        stateMachine.ChangeState(player.fixFloorState);
                    else if (player.currentTag == "SideHole")
                        stateMachine.ChangeState(player.fixSideState);
                    else if (player.currentTag == "Fire")
                        stateMachine.ChangeState(player.putOutFireState);
                    else
                        stateMachine.ChangeState(player.idleState);
                }
            }
        }

        base.Update();
    }
}
