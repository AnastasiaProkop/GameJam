using UnityEngine;
using UnityEngine.Windows;

public abstract class State
{


    protected StateMachine stateMachine;
    protected string animBoolName;
    protected Animator animator;
    protected Player player;

    public State(Player player, StateMachine stateMachine, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
        //animator = player.animator;

    }
    public virtual void Enter()
    {
        //animator.SetBool(animBoolName, true);

    }

    public virtual void Update()
    {

    }
    public virtual void Exit()
    {
        //animator.SetBool(animBoolName, false);
    }

}
