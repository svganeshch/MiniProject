using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAttack : MonoBehaviour
{
    private Character characterCausingDamage;
    private Character characterTakingDamage;
    private HealthManager healthManager;
    private Character hitCharacter;

    [HideInInspector] public float weaponDamage;
    private LayerMask damageLayerMask;

    [SerializeField] protected Collider weaponCollider;
    [SerializeField] private HashSet<GameObject> hasDealtDamage;

    private void Awake()
    {
        if (weaponCollider == null)
        {
            weaponCollider = GetComponentInChildren<Collider>();
        }
        characterCausingDamage = GetComponentInParent<Character>();
    }

    private void Start()
    {
        hasDealtDamage = new HashSet<GameObject>();
        damageLayerMask = LayerMaskManager.Instance.damagableLayerMask;

        weaponCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider collidedWith)
    {
        if (collidedWith.gameObject == null) return;
        //Debug.Log("collided with : " + collidedWith.name + " layer : " + (1 << collidedWith.gameObject.layer) + " damageMask : " + damageLayerMask.value);

        if ((1 << collidedWith.gameObject.layer) != damageLayerMask.value) return;
        //Debug.Log("collided with a damagable layer : " + collidedWith.name);

        hitCharacter = GetHitCharacter(collidedWith);
        //hitCharacter = collidedWith.gameObject.GetComponentInParent<Character>();

        if (hitCharacter == null || hitCharacter == characterCausingDamage)
            return;

        healthManager = hitCharacter.healthManager;
        //if (healthManager == null || hitCharacter != characterTakingDamage)
        //{
        //    characterTakingDamage = hitCharacter;
        //    healthManager = characterTakingDamage.healthManager;
        //}

        //if (hasDealtDamage.Add(hitCharacter.gameObject))
        //{
        healthManager.TakeDamage(weaponDamage, characterCausingDamage, hitCharacter);
        //}
        //else
        //{
        //    Debug.Log("Already hit : " + hitCharacter.name);
        //}
    }

    private Character GetHitCharacter(Collider collidedWith)
    {
        if (hitCharacter != null && collidedWith.transform.root == hitCharacter.transform)
            return hitCharacter;

        return collidedWith.transform.GetComponentInParent<Character>();
    }

    public void StartDealDamage()
    {
        weaponCollider.enabled = true;

        //Debug.Log(characterCausingDamage.name + " damage enabled : " + characterCausingDamage.animator.GetCurrentAnimatorClipInfo(3)[0].clip.name);
    }

    public void StopDealDamage()
    {
        weaponCollider.enabled = false;
        hasDealtDamage.Clear();
        //Debug.Log(characterCausingDamage.name + " damage disabled " + characterCausingDamage.animator.GetCurrentAnimatorClipInfo(3)[0].clip.name);
    }
}
