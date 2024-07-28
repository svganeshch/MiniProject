using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : HealthManager
{
    public override void Die()
    {
        base.Die();

        MenuManager.Instance.SetDeathMenu();
        character.characterSfxManager.PlayGameOverSound();
    }
}
