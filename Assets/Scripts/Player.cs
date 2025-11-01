using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{

    StateMachine stateMachine;
    public IdleState idleState { get; private set; }
    public WalkState walkState { get; private set; }

    private Vector3 mousePos;
    public Vector3 targetPos { get; private set; }
    public float moveSpeed { get; private set; } = 5;

    public Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    private void OnMouseDown()
    {
        mousePos = Input.mousePosition - GetMousePos();
    }

    /*private void OnMouseDrag()
    {
        targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePos);
    }*/

    private void OnMouseUp()
    {
        Plane plane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;

        if (plane.Raycast(ray, out distance))
        {
            targetPos = ray.GetPoint(distance);

            if (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                stateMachine.ChangeState(walkState);
            }
        }
    }
    private void Awake()
    {
        targetPos = transform.position;
        stateMachine = new StateMachine();
        idleState = new IdleState(this, stateMachine, "IsIdle");
        walkState = new WalkState(this, stateMachine, "IsWalk");
    }
    private void Start()
    {
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        stateMachine.currentState.Update();
        //transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * moveSpeed);

    }

}
