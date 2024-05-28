using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Navigation_UnityNavMesh : BaseNavigation
{
    NavMeshAgent LinkedAgent;

    protected override void Initialise()
    {
        LinkedAgent = GetComponent<NavMeshAgent>();
    }

    protected override bool RequestPath()
    {
        LinkedAgent.speed = MaxMoveSpeed;
        LinkedAgent.angularSpeed = RotationSpeed;
        LinkedAgent.stoppingDistance = DestinationReachedThreshold;

        LinkedAgent.SetDestination(Destination);

        OnBeganPathFinding();

        return true;
    }

    protected override void Tick_Default()
    {

    }

    protected override void Tick_Pathfinding()
    {
        if (LinkedAgent == null)
            return;

        if (!LinkedAgent.pathPending)
        {
            Debug.Log($"Path status: {LinkedAgent.pathStatus}");
            if (LinkedAgent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                OnPathFound();
            }
            else if (LinkedAgent.pathStatus == NavMeshPathStatus.PathPartial)
            {
                Debug.LogWarning("Path is partial, but we'll attempt to follow it.");
                OnPathFound();
            }
            else if (LinkedAgent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                Debug.LogError("Failed to find path - PathInvalid");
                OnFailedToFindPath();
            }
        }
    }

    protected override void Tick_PathFollowing()
    {
        if (LinkedAgent == null)
            return;

        bool atDestination = false;

        if (LinkedAgent.hasPath && !LinkedAgent.pathPending)
        {
            if (LinkedAgent.remainingDistance <= LinkedAgent.stoppingDistance)
            {
                atDestination = true;
            }
        }
        else if (!LinkedAgent.hasPath)
        {
            Vector3 vecToDestination = Destination - transform.position;
            float heightDelta = Mathf.Abs(vecToDestination.y);
            vecToDestination.y = 0f;

            atDestination = heightDelta < LinkedAgent.height &&
                            vecToDestination.magnitude <= DestinationReachedThreshold;
        }

        if (atDestination)
        {
            OnReachedDestination();
        }
        else if (LinkedAgent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            OnFailedToFindPath();
        }
        else if (LinkedAgent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            // Handle partial path by moving closer manually if near destination
            Vector3 vecToDestination = Destination - transform.position;
            if (vecToDestination.magnitude <= DestinationReachedThreshold * 2)
            {
                LinkedAgent.Move(vecToDestination.normalized * (LinkedAgent.speed * Time.deltaTime));
            }
            else if (DEBUG_ShowHeading)
            {
                Debug.DrawLine(transform.position + Vector3.up, LinkedAgent.steeringTarget, Color.green);
            }
        }
    }

    protected override void Tick_Animation()
    {
        float forwardsSpeed = Vector3.Dot(LinkedAgent.velocity, transform.forward) / LinkedAgent.speed;
        float sidewaysSpeed = Vector3.Dot(LinkedAgent.velocity, transform.right) / LinkedAgent.speed;

        AnimController.SetFloat("ForwardsSpeed", forwardsSpeed);
        AnimController.SetFloat("SidewaysSpeed", sidewaysSpeed);
    }

    public override void StopMovement()
    {
        LinkedAgent.ResetPath();
    }

    public override bool FindNearestPoint(Vector3 searchPos, float range, out Vector3 foundPos)
    {
        NavMeshHit hitResult;
        if (NavMesh.SamplePosition(searchPos, out hitResult, range, NavMesh.AllAreas))
        {
            foundPos = hitResult.position;
            return true;
        }

        foundPos = searchPos;

        return false;
    }

    public override bool SetDestination(Vector3 newDestination)
    {
        // Log the new destination being set
        Debug.Log($"Setting destination to {newDestination}");

        // Check if the new destination is on the nav mesh
        if (NavMesh.SamplePosition(newDestination, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            newDestination = hit.position;

            // location is already our destination?
            Vector3 destinationDelta = newDestination - Destination;
            destinationDelta.y = 0f;
            if (IsFindingOrFollowingPath && (destinationDelta.magnitude <= DestinationReachedThreshold))
                return true;

            // are we already near the destination
            destinationDelta = newDestination - transform.position;
            destinationDelta.y = 0f;
            if (destinationDelta.magnitude <= DestinationReachedThreshold)
                return true;

            Destination = newDestination;

            return RequestPath();
        }
        else
        {
            Debug.LogError("Requested destination is outside the NavMesh bounds.");
            return false;
        }
    }
}
