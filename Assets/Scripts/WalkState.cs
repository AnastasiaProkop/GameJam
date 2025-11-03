using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class WalkState : State
{
    public WalkState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        if (player.targetPos.x >= player.transform.position.x)
        {
            player.skeletonAnimation.AnimationState.SetAnimation(0, "go_front", true);
        }
        else
        {
            player.skeletonAnimation.AnimationState.SetAnimation(0, "go_back", true);
        }
        Debug.Log("Walk");
        player.navMeshAgent.SetDestination(player.targetPos);
    }

    public override void Exit()
    {
        base.Exit();
       // player.skeletonAnimation.AnimationState.SetAnimation(0, "go_back", false);
       // player.skeletonAnimation.AnimationState.SetAnimation(0, "go_front", false);
        
    }
    public override void Update()
    {

        if (!player.navMeshAgent.pathPending)
        {
            if (player.navMeshAgent.remainingDistance <= player.navMeshAgent.stoppingDistance)
            {
                if (!player.navMeshAgent.hasPath || player.navMeshAgent.velocity.sqrMagnitude == 0f)
                {
    
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
