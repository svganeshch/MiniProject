using System.Collections;
using UnityEngine;

public class EnemyRigController : CharacterRigController
{
    Enemy enemy;

    float timePassed;
    float rigWeightChangeDuration = 0.5f;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<Enemy>();
    }

    public override void SetRigWeight(float weight)
    {
        base.SetRigWeight(weight);

        StartCoroutine(ChangeRigWeight(weight));
    }

    public override void SetRigTarget()
    {
        base.SetRigTarget();

        weaponTarget.transform.SetParent(enemy.currentTarget.targetLockCast);
        weaponTarget.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    private IEnumerator ChangeRigWeight(float weight)
    {
        timePassed = 0f;
        float currentRigWeight = spineBendRig.weight;

        while (timePassed < rigWeightChangeDuration)
        {
            spineBendRig.weight += Mathf.Lerp(currentRigWeight, weight, timePassed / rigWeightChangeDuration);
            timePassed += Time.deltaTime;

            yield return null;
        }

        spineBendRig.weight = weight;
    }
}