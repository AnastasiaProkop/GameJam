using Spine.Unity;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class Player : MonoBehaviour
{

    public StateMachine stateMachine {  get; private set; }
    public IdleState idleState { get; private set; }
    public WalkState walkState { get; private set; }
    public PutOutFireState putOutFireState { get; private set; }
    public ShootState shootState { get; private set; }
    public FixFloorState fixFloorState { get; private set; }
    public FixSideState fixSideState { get; private set; }

    private Vector3 mousePos;
    public Vector3 targetPos { get; private set; }

    private Vector3 potentialPos;

    [SerializeField] private GameObject dragedSailor;
    public float moveSpeed { get; private set; } = 5;
    public string currentTag { get; private set; }

    public NavMeshAgent navMeshAgent { get; private set; }

    [SerializeField] public SkeletonAnimation skeletonAnimation {  get; private set; }


    private void Awake()
    {
        targetPos = transform.position;
        stateMachine = new StateMachine();

        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();

        idleState = new IdleState(this, stateMachine, "idle");
        walkState = new WalkState(this, stateMachine, "");
        putOutFireState = new PutOutFireState(this, stateMachine, "IsPutOutFire");
        shootState = new ShootState(this, stateMachine, "IsShoot");
        fixFloorState = new FixFloorState(this, stateMachine, "water");
        fixSideState = new FixSideState(this, stateMachine, "bort");

        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        stateMachine.Initialize(idleState);

        dragedSailor.SetActive(false);

        navMeshAgent.updateRotation = false; 
        navMeshAgent.updateUpAxis = false;   
    }

    public Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    private void OnMouseDown()
    {
        dragedSailor.SetActive(true);
        mousePos = Input.mousePosition - GetMousePos();
    }



    private void OnMouseDrag()
    {
        potentialPos = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePos);

        Plane plane = new Plane(Vector3.up, dragedSailor.transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;

        if (plane.Raycast(ray, out distance))
        {            
            potentialPos = ray.GetPoint(distance);
            dragedSailor.transform.position = new Vector3(potentialPos.x, dragedSailor.transform.position.y, potentialPos.z);
            
        }


    }

    private void OnMouseUp()
    {
        dragedSailor.SetActive(false);

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
                        //navMeshAgent.SetDestination(targetPos);
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
