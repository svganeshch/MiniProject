using UnityEngine;

public class EnemyHudManager : HudManager
{
    protected override void Update()
    {
        LookCamera();
    }

    private void LookCamera()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - PlayerCamera.Instance.transform.position);
    }
}
