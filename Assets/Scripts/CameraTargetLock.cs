using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetLock
{
    Character character;
    float lockOnRadius = 20f;
    float minimumViewableAngle = -50;
    float maximumViewableAngle = 50;
    List<Enemy> availableTargets = new List<Enemy>();

    public Enemy nearestTarget;

    public CameraTargetLock(Character character)
    {
        this.character = character;
    }

    public void FindLockOnTarget()
    {
        float shortestDistance = Mathf.Infinity;
        float shortestDistanceOfRightTarget = Mathf.Infinity;
        float sgortestDistanceOfLeftTarget = -Mathf.Infinity;

        Collider[] colliders = Physics.OverlapSphere(character.transform.position, lockOnRadius, character.enemyLayerMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            Enemy lockOnTarget = colliders[i].GetComponent<Enemy>();

            if (lockOnTarget != null)
            {
                Vector3 lockOnTargetDirection = lockOnTarget.transform.position - character.transform.position;
                float distanceFromTarget = Vector3.Distance(character.transform.position, lockOnTarget.transform.position);
                float viewableAngle = Vector3.Angle(lockOnTargetDirection, character.mainCameraTransform.forward);

                if (lockOnTarget.isDead)
                    continue;

                if (viewableAngle > minimumViewableAngle && viewableAngle < maximumViewableAngle)
                {
                    RaycastHit hit;

                    if (Physics.Linecast(character.targetLockCast.position, lockOnTarget.targetLock.transform.position, out hit, character.obstaclesLayerMask))
                    {
                        continue;
                    }
                    else
                    {
                        availableTargets.Add(lockOnTarget);
                        Debug.Log("available target : " + lockOnTarget.name);
                    }
                }
            }
        }

        for (int i = 0; i < availableTargets.Count; i++)
        {
            if (availableTargets[i] != null)
            {
                float distanceFromTarget = Vector3.Distance(character.transform.position, availableTargets[i].transform.position);
                Vector3 lockTargetDirection = availableTargets[i].transform.position - character.transform.position;

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestTarget = availableTargets[i];

                    Debug.Log("nearest target : " + nearestTarget.name);
                }
            }
            else
            {
                ClearLockOnTargets();
            }
        }
    }

    public void ClearLockOnTargets()
    {
        nearestTarget = null;
        availableTargets.Clear();
    }
}
