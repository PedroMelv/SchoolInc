using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovingCarAI : MonoBehaviour, IParkable
{
    private enum CarState
    {
        STOPPED = 0,
        FORWARD = 1,
        REVERSE = -1
    }


    [Header("Movement")]
    public ParkingArea parkingAreaTest;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float groundOffset;
    [SerializeField] private float pointCheckDistance = 1;
    [SerializeField] private float lowerSpeedThreshold = 10f;
    [SerializeField] private CarState carState = CarState.FORWARD;

    private Vector3 movingDirection;
    private float rotationDirection;

    private float calculatedSpeed;
    private Vector3[] path;
    private int pathIndex;

    [Header("Sensors")]

    [SerializeField] private float frontCheckDistance = 5f;
    [SerializeField] private Vector3 frontCheckOffset;
    [SerializeField] private Vector3 frontCheckSize = new Vector3(1f, 1f, 1f);
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask frontDetectLayer;

    private float distanceToBorder = 1f;
    private Vector3 lastForward;
    private Vector3 borderPosition;

    private Rigidbody rb;

    [SerializeField] private Vector3 debugOffset;
    [field: SerializeField] public float width { get; set; }
    [field: SerializeField] public float length { get; set; }
    [field: SerializeField] public float height { get; set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        movingDirection = transform.forward;
        rotationDirection = 1f;


        GeneratePath(parkingAreaTest.GetParkSpot(this));
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(mouseRay, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                GeneratePath(hit.point);
            }
        }

        CalculateSpeed();

        HandleMovement();

        DetectBorder();

        FollowPath();
    }

    private void HandleMovement()
    {

        rb.velocity = transform.forward * (int)carState * calculatedSpeed;

        if (path == null || path.Length == 0 || pathIndex >= path.Length)
        {
            carState = CarState.STOPPED;
            return;
        }else if(pathIndex < path.Length)
        {
            carState = CarState.FORWARD;
        }


        Quaternion targetRotation = Quaternion.LookRotation(path[pathIndex] - transform.position);

        rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * (int)carState * Time.deltaTime) );
    }
    private void CalculateSpeed()
    {
        calculatedSpeed = moveSpeed * (distanceToBorder < lowerSpeedThreshold ? distanceToBorder / lowerSpeedThreshold : 1f);
        calculatedSpeed = Mathf.Clamp(calculatedSpeed, 1f, moveSpeed);
    }

    private void DetectFront()
    {
        
        bool somethingInFront = RotaryHeart.Lib.PhysicsExtension.Physics.BoxCast(transform.position + frontCheckOffset, frontCheckSize * 0.5f, transform.forward, out RaycastHit hit, transform.rotation, frontCheckDistance, frontDetectLayer, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Editor);

        if (carState == CarState.STOPPED) return;

        if (somethingInFront)
        {
            carState = CarState.REVERSE;
        }
        else
        {
            carState = CarState.FORWARD;
        }
    }


    private void GeneratePath(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        
        Vector3 startPos = transform.position;
        Vector3 endPos = targetPosition;

        if (NavMesh.SamplePosition(startPos, out NavMeshHit hit, 100f, NavMesh.AllAreas))
        {
            startPos = hit.position;
        }
        else
            return;

        if (NavMesh.SamplePosition(endPos, out hit, 100f, NavMesh.AllAreas))
        {
            endPos = hit.position;
        }
        else
            return;
        
        NavMesh.CalculatePath(startPos, endPos, NavMesh.AllAreas, path);

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            pathIndex = 0;

            this.path = path.corners;

            for (int i = 0; i < this.path.Length; i++)
            {
                this.path[i].y += height;
            }

            this.path[0] = transform.position;
        }

        
    }

    private void DetectBorder()
    {
        if (lastForward != transform.forward * (int)carState)
        {
            borderPosition = FindBorder();

            lastForward = transform.forward * (int)carState;
        }

        Debug.DrawLine(transform.position, borderPosition, Color.red);

        distanceToBorder = Vector3.Distance(transform.position, borderPosition);
    }

    private Vector3 FindBorder()
    {
        float distance = 100f;
        int checkAmount = 25;

        float lastDecrease = distance;

        for (int i = 0; i < checkAmount; i++)
        {
            bool hitting = Physics.Raycast(transform.position + Vector3.up * 2 + transform.forward * (int)carState * distance, Vector3.down, 50f, groundLayer);
            bool willHitNext = Physics.Raycast(transform.position + Vector3.up * 2 + transform.forward * (int)carState * (distance + 1f), Vector3.down, 50f, groundLayer);
            
            if (hitting == false && willHitNext == true) break;

            if (hitting)
            {
                distance += lastDecrease / 2f;

            }
            else
            {
                distance -= lastDecrease / 2f;
                lastDecrease = lastDecrease / 2f;
            }
        }

        return transform.position + Vector3.up * 2 + transform.forward * (int)carState * distance;
    }

    private void FollowPath()
    {
        if (path == null || path.Length == 0 || pathIndex >= path.Length)
        {
            return;
        }

        if (Vector3.Distance(transform.position, path[pathIndex]) < pointCheckDistance)
        {
            pathIndex++;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.matrix = Matrix4x4.TRS(transform.position + debugOffset, transform.rotation, Vector3.one);

        Gizmos.DrawWireCube(Vector3.zero , new Vector3(width, height, length));

        Gizmos.matrix = Matrix4x4.identity;

        if (path == null || path.Length == 0) return;

        for (int i = 0; i < path.Length; i++)
        {
            if(i == pathIndex)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.blue;
            }
            Gizmos.DrawWireSphere(path[i], pointCheckDistance);
        }

        Gizmos.color = Color.red;

        for (int i = 0; i < path.Length - 1; i++)
        {
            Gizmos.DrawLine(path[i], path[i + 1]);
        }
    }
}
