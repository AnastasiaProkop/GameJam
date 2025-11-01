using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{

    StateMachine stateMachine;
    public IdleState idleState { get; private set; }
    public WalkState walkState { get; private set; }
    public PutOutFireState putOutFireState { get; private set; }
    public ShootState shootState { get; private set; }
    public FixFloorState fixFloorState { get; private set; }
    public FixSideState fixSideState { get; private set; }

    private Vector3 mousePos;
    public Vector3 targetPos { get; private set; }
    public float moveSpeed { get; private set; } = 5;
    public string currentTag { get; private set; }


    private void Awake()
    {
        targetPos = transform.position;
        stateMachine = new StateMachine();
        idleState = new IdleState(this, stateMachine, "IsIdle");
        walkState = new WalkState(this, stateMachine, "IsWalk");
        putOutFireState = new PutOutFireState(this, stateMachine, "IsPutOutFire");
        shootState = new ShootState(this, stateMachine, "IsShoot");
        fixFloorState = new FixFloorState(this, stateMachine, "IsFixFloor");
        fixSideState = new FixSideState(this, stateMachine, "IsFixSide");
    }
    private void Start()
    {
        stateMachine.Initialize(idleState);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Gun") || collision.collider.CompareTag("SideHole")) { 
        
            //currentTag = "Gun";
            targetPos = transform.position;
        }
        if (collision.collider.CompareTag("SideHole"))
        {
           /// currentTag = ""
        }
    }

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

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                if (hitInfo.collider.CompareTag("Gun") || hitInfo.collider.CompareTag("FloorHole") || hitInfo.collider.CompareTag("Fire") || hitInfo.collider.CompareTag("SideHole")) { 
                    targetPos = ray.GetPoint(distance);
                    currentTag = hitInfo.collider.tag;

                    if (Vector3.Distance(transform.position, targetPos) > 0.1f)
                    {
                        stateMachine.ChangeState(walkState);
                    }
                }
                
                
            }
        }
    }

    private void Update()
    {
        stateMachine.currentState.Update();

    }

}
