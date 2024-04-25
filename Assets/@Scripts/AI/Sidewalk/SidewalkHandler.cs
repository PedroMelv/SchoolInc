using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SidewalkHandler : StaticInstance<SidewalkHandler>
{
    [SerializeField] private SidewalkPoint[] _sidewalks = new SidewalkPoint[0];


    private void Start()
    {
        _sidewalks = GetComponentsInChildren<SidewalkPoint>();
    }


    public static Vector3 GetRandomPointLerp()
    {
        if(Instance._sidewalks.Length == 0)
        {
            Instance._sidewalks = Instance.GetComponentsInChildren<SidewalkPoint>();
        }

        SidewalkPoint startPos = Instance._sidewalks[Random.Range(0, Instance._sidewalks.Length)];
        SidewalkInfo endPos = startPos.GetRandomSidewalk();

        return Vector3.Lerp(startPos.transform.position, endPos.point.transform.position, Random.value);
    }

    public static void GetPath(Vector3 start, Vector3 target, out Queue<SidewalkPoint> path)
    {
        List<SidewalkPoint> blacklist = new List<SidewalkPoint>();
        Stack<SidewalkPoint> stackPath = new Stack<SidewalkPoint>();
        
        SidewalkPoint targetPoint = GetClosest(target);
        SidewalkPoint startPoint = GetClosest(start);
        SidewalkPoint currentPoint = startPoint;

        stackPath.Push(currentPoint);

        int iterations = 0;
        int maxIterations = 1000;

        while (currentPoint != targetPoint && iterations < maxIterations)
        {
            iterations++;

            if (blacklist.Count == Instance._sidewalks.Length)
            {
                Debug.LogError("Blacklist got every sidewalks");
                break;
            }

            int amountOfSidewalks = currentPoint.AvailableSidewalks(blacklist);

            SidewalkPoint nextPoint = currentPoint.GetNextSidewalk(target, blacklist);

            if (amountOfSidewalks == 0)
            {
                Debug.Log("Dequeuing, " + stackPath.Count , stackPath.Peek());
                blacklist.Add(stackPath.Pop());
                currentPoint = stackPath.Peek();
                blacklist.Remove(currentPoint);
                
                continue;
            }

            blacklist.Add(currentPoint);
            currentPoint = nextPoint;

            stackPath.Push(currentPoint);
        }

        path = new Queue<SidewalkPoint>(stackPath.Reverse());
    }

    public static void GetPath(Vector3 start, out Queue<SidewalkPoint> path)
    {
        GetPath(start, GetRandomPoint(), out path);
    }

    public static Vector3 GetRandomPoint()
    {
        return Instance._sidewalks[Random.Range(0, Instance._sidewalks.Length)].transform.position;
    }

    public static SidewalkPoint GetClosest(Vector3 target)
    {
        SidewalkPoint closest = null;
        float distance = float.MaxValue;

        for (int i = 0; i < Instance._sidewalks.Length; i++)
        {
            float currentDistance = Vector3.Distance(Instance._sidewalks[i].transform.position, target);

            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = Instance._sidewalks[i];
            }
        }

        return closest;
    }   
}
