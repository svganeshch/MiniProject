using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterRigController : MonoBehaviour
{
    public static CharacterRigController Instance;

    public Rig spineBendRig;
    public GameObject weaponTarget;

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void Update()
    {

    }

    public virtual void SetRigWeight(float weight) { }
    public virtual void SetRigTarget() { }
}
