using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour
{
    public NavMeshAgent _agent => GetComponent<NavMeshAgent>();
    public Transform _target => GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

    private void Update()
    {
        _agent.destination = _target.position;
    }
}
