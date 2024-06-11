using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIDetector : MonoBehaviour
{
    public LayerMask rugLayer;

    public NavMeshAgent agent;

    private float startingSpeed = 0.0f;
    private float startingAcceleration = 0.0f;

    public float slowInSpeed = 5f;
    public float slowInAcceleration = 1.5f;

    [SerializeField] private CommonAIBase aiBase;

    [SerializeField] private AIStatConfiguration rugStat;
    [SerializeField] private AIStatConfiguration rugStat2;

    protected Dictionary<AIStat, float> DecayRates = new Dictionary<AIStat, float>();

    private void Start()
    {
        aiBase = GetComponent<CommonAIBase>();
        startingAcceleration = agent.acceleration;
        startingSpeed = agent.speed;
    }

    // Check if an object is on top of the rug
    public bool IsOnRug(GameObject obj)
    {
        RaycastHit hit;
        if (Physics.Raycast(obj.transform.position, Vector3.down, out hit, Mathf.Infinity, rugLayer))
        {

            Debug.Log("On Rug");
            // You can optionally check the hit.collider to identify the rug
            return true;
        }
        return false;
    }

    private void Update()
    {
        if (IsOnRug(gameObject))
        {
            agent.acceleration = slowInAcceleration;
            agent.speed = slowInSpeed;

            // Update stats when on the rug
            aiBase.UpdateStatsOnRug(rugStat);
            aiBase.UpdateStatsOnRug(rugStat2);
        }
        else
        {
            agent.acceleration = startingAcceleration;
            agent.speed = startingSpeed;
        }
    }
}
