using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public float spawnRadius;
    public int spawnCount;

    public GameObject[] spawnPrefabs;
    public List<GameObject> spawnedObjs = new List<GameObject>();

    public int patrolPointsCount = 4;
    public List<Vector3> patrolPoints = new List<Vector3>();

    private void Start()
    {
        Spawn();
    }

    private void AssignPatrolPoints()
    {
        patrolPoints.Clear();

        for (int i = 0; i < patrolPointsCount; i++)
        {
            Vector3 randomPatrolPoint = Random.insideUnitCircle * spawnRadius;
            randomPatrolPoint = new Vector3(transform.position.x + randomPatrolPoint.x, transform.position.y, transform.position.z + randomPatrolPoint.y);

            patrolPoints.Add(randomPatrolPoint);
        }
    }

    private void Spawn()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            AssignPatrolPoints();

            Vector3 spawnPosition = patrolPoints[0];
            GameObject prefabToSpawn = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];
            GameObject spawnedObj = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

            Enemy spawnedEnemy = spawnedObj.GetComponent<Enemy>();
            spawnedEnemy.patrolPoints = patrolPoints;
            spawnedEnemy.attackProbability = Random.Range(0.5f, 1f);

            spawnedObjs.Add(spawnedObj);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
