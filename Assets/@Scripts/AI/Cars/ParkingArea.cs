using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingArea : MonoBehaviour
{
    private List<IParkable> parkedObjects = new List<IParkable>();
    public Vector3 startParkPosition;
    [SerializeField] private Vector3 parkDirection;

    public Vector3 GetParkSpot(IParkable forMe)
    {
        Vector3 spot = transform.position + startParkPosition;

        for (int i = 0; i < parkedObjects.Count; i++)
        {
            IParkable parkable = parkedObjects[i];
            spot += parkDirection * parkable.length;
        }

        spot += parkDirection * forMe.length / 2;

        parkedObjects.Add(forMe);

        return spot;
    }

    public void RemoveFromParkspot(IParkable me)
    {
        parkedObjects.Remove(me);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + startParkPosition, new Vector3(1, 1, 1));

        Gizmos.DrawLine(transform.position + startParkPosition, transform.position + startParkPosition + parkDirection * 10);
    }
}
