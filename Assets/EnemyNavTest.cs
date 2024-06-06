using UnityEngine;
using UnityEngine.AI;

public class EnemyNavTest : MonoBehaviour
{
    public GameObject playa;
    NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponentInChildren<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(playa.transform.position);
    }
}
