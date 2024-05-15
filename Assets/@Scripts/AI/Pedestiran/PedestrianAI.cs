using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianAI : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private float height = 1.15f;
    private Vector3 direction;
    private Vector3 offset;

    private bool pathCompleted = true;

    [SerializeField] private float stopDistance;
    [SerializeField] private LayerMask stopLayer;

    [Space]
    private Queue<SidewalkPoint> path = new Queue<SidewalkPoint>();

    private void Start()
    {
        transform.position = SidewalkHandler.GetRandomPointLerp();
        StartCoroutine(EGetPath());
    }

    private void Update()
    {
        if (pathCompleted || path.Count == 0) return;

        direction = (path.Peek().transform.position - (transform.position + offset));
        direction.y = 0f;
        direction.Normalize();

        if(!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, stopDistance, stopLayer))
        {
            transform.position += direction * speed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, height, transform.position.z);

            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.1f);
        }

        

        if (IsCloseToTarget())
        {
            path.Dequeue();
            if(path.Count == 0)
            {
                pathCompleted = true;

                SidewalkHandler.GetPath(transform.position, out Queue<SidewalkPoint> path);
                SetPath(path);

                return;
            }
        }
    }

    private IEnumerator EGetPath()
    {
        yield return new WaitForEndOfFrame();

        SidewalkHandler.GetPath(transform.position, out Queue<SidewalkPoint> path);
        SetPath(path);
    }

    public void SetPath(Queue<SidewalkPoint> path)
    {
        this.path = path;
        speed = Random.Range(2.5f, 5f);
        offset = new Vector3(Random.Range(-1f,1f), 0f, Random.Range(-1f, 1f));
        
        pathCompleted = false;
    }

    private bool IsCloseToTarget()
    {
        Vector3 positionA = path.Peek().transform.position;
        positionA.y = transform.position.y;
        Vector3 positionB = transform.position + offset;

        return Vector3.Distance(positionA, positionB) < .1f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * stopDistance);
        Gizmos.color = Color.red;
        if (this.path == null || this.path.Count == 0) return;

        List<SidewalkPoint> path = new List<SidewalkPoint>(this.path);

        Gizmos.color = Color.magenta;
        for (int i = 0; i < path.Count - 1; i++)
        {
            
            Gizmos.DrawLine(path[i].transform.position, path[i + 1].transform.position);
            Gizmos.DrawWireCube(path[i].transform.position, Vector3.one);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawCube(path[path.Count - 1].transform.position, Vector3.one * 2f);
    }
}
