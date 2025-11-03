using Spine.Unity;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public GameObject ship;
    public ShipManager shipManager { get; private set; }
    public int currentZone { get; set; } = 0;
    public int nextZone { get; private set; } = 0;

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
        idleState = new IdleState(this, stateMachine, "idle");
        walkState = new WalkState(this, stateMachine, "");
        putOutFireState = new PutOutFireState(this, stateMachine, "IsPutOutFire");
        shootState = new ShootState(this, stateMachine, "IsShoot");
        fixFloorState = new FixFloorState(this, stateMachine, "water");
        fixSideState = new FixSideState(this, stateMachine, "bort");

        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();

        shipManager = ship.GetComponent<ShipManager>();
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

                    // Находим номер зоны в конце названия
                    nextZone = int.Parse(hitInfo.collider.transform.parent.name[^1..]);

                    if (!TaskAvailable()) return;

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

    private bool TaskAvailable()
    {
        TaskType? type =
            currentTag == "Gun"       ? TaskType.Gun       :
            currentTag == "FloorHole" ? TaskType.FloorHole :
            currentTag == "SideHole"  ? TaskType.SideHole  :
            currentTag == "Fire"      ? TaskType.Fire      :
                                        null               ;
        if (type == null) return true;

        return shipManager.TaskAvailableInZone((TaskType)type, nextZone - 1);
    }
}
