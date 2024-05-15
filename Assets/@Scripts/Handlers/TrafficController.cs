using System.Collections;
using UnityEngine;

public class TrafficController : MonoBehaviour
{
    [SerializeField] private Transform[] streetPoints;   
    [SerializeField]private GameObject[] simpleCars;
    private PooledObject[] pooledCars;

    private float spawnRate = 0f;

    private void Start()
    {
        pooledCars = new PooledObject[simpleCars.Length];
        for (int i = 0; i < simpleCars.Length; i++)
        {
            pooledCars[i] = PooledObject.CreateOrFind(simpleCars[i]);
        }
    }

    private void Update()
    {
        if(spawnRate <= 0)
        {
            SpawnCar();
            spawnRate = Random.Range(1f, 8f);
        }
        else
        {
            spawnRate -= Time.deltaTime;
        }
    }

    private void SpawnCar()
    {
        int randomCar = Random.Range(0, pooledCars.Length);
        Transform streetPoint = streetPoints[Random.Range(0, streetPoints.Length)];

        Transform inversePoint = streetPoints[0];

        for (int i = 0; i < streetPoints.Length; i++)
        {
            if (streetPoints[i] != streetPoint) inversePoint = streetPoints[i];
        }

        GameObject carSpawned = pooledCars[randomCar].Instantiate(streetPoint.position, streetPoint.rotation);

        StartCoroutine(CheckCar(carSpawned, inversePoint, pooledCars[randomCar]));
    }

    private IEnumerator CheckCar(GameObject car, Transform inversePoint, PooledObject pool)
    {
        while(Vector3.Distance(car.transform.position, inversePoint.position) > 25f)
        {
            yield return new WaitForSeconds(0.1f);
        }

        pool.Return(car);
    }
}
