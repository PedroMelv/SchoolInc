using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SidewalkInfo
{
    public SidewalkPoint point;
    public GameObject school;
    public SidewalkType type;
}

public enum SidewalkType
{
    DEFAULT,
    CROSSWALK
}

public class SidewalkPoint : MonoBehaviour
{
    [SerializeField] private List<SidewalkInfo> connected = new List<SidewalkInfo>();
    public List<SidewalkInfo> Connected => connected;

    public bool Contains(SidewalkPoint point)
    {
        bool result = false;

        for (int i = 0; i < connected.Count; i++)
        {
            if (connected[i].point == point)
            {
                result = true;
                break;
            }
        }

        return result;
    }
    public void Add(SidewalkPoint point)
    {
        SidewalkInfo info = new SidewalkInfo();
        info.point = point;
        info.type = default;

        connected.Add(info);
    }
    public bool TryGet(SidewalkPoint point, out SidewalkInfo info)
    {
        info = null;

        for (int i = 0; i < connected.Count; i++)
        {
            if (connected[i].point == point)
            {
                info = connected[i];
                return true;
            }
        }
        return false;
    }

    public int AvailableSidewalks(List<SidewalkPoint> blacklist)
    {
        int amount = 0;

        for (int i = 0; i < connected.Count; i++)
        {
            if (!blacklist.Contains(connected[i].point))
                amount++;
        }

        return amount;
    }

    public SidewalkPoint GetNextSidewalk(Vector3 targetPosition, List<SidewalkPoint> blacklist)
    {
        float distance = float.MaxValue;

        SidewalkPoint closest = null;

        for (int i = 0; i < connected.Count; i++)
        {
            float currentDistance = Vector3.Distance(connected[i].point.transform.position, targetPosition);

            if (currentDistance < distance && !blacklist.Contains(connected[i].point))
            {
                distance = currentDistance;
                closest = connected[i].point;
            }
        }

        return closest;
    }

    public SidewalkInfo GetRandomSidewalk()
    {
        return connected[Random.Range(0, connected.Count)];
    }

    private void OnValidate()
    {
        for (int i = 0; i < connected.Count; i++) 
        {
            if (connected[i].point.connected.Count == 0 || !connected[i].point.Contains(this))
                connected[i].point.Add(this);
            else if(connected[i].point.Contains(this) && connected[i].point.TryGet(this, out SidewalkInfo info))
            {
                info.type = connected[i].type;
            }
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < connected.Count; i++)
        {
            if (connected[i] == null || connected[i].point == null)
                continue;

            switch (connected[i].type)
            {
                case SidewalkType.DEFAULT:
                    Gizmos.color = Color.red;
                    break;
                case SidewalkType.CROSSWALK:
                    Gizmos.color = Color.yellow;
                    break;
            }

            Gizmos.DrawLine(transform.position, connected[i].point.transform.position);
        }
    }
}
