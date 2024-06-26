using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponEquipment : WeaponEquipment
{
    private Player player;

    private Material[] scarfMaterials;

    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
        scarfMaterials = player.playerScarf.GetComponent<SkinnedMeshRenderer>().materials;
    }

    public override void DrawWeapon()
    {
        base.DrawWeapon();

        HandleScarfChange();
    }

    public override void HolsterWeapon()
    {
        base.HolsterWeapon();

        currentWeaponSlot = 0;
        HandleScarfChange();
    }

    private void HandleScarfChange()
    {
        Color baseColor;
        Color shineColor;

        switch (currentWeaponSlot)
        {
            case 1:
                baseColor = player.katanaBaseColor;
                shineColor = player.katanaColor;
                break;
            case 2:
                baseColor = player.greatSwordBaseColor;
                shineColor = player.greatSwordColor;
                break;
            case 3:
                baseColor = player.daggerBaseColor;
                shineColor = player.daggerColor;
                break;
            default:
                baseColor = player.unarmedBaseColor;
                shineColor = player.unarmedColor;
                break;
        }

        foreach (Material scarfMaterial in scarfMaterials)
        {
            scarfMaterial.SetColor("_BaseColor", baseColor);
            scarfMaterial.SetColor("_ShineColor", shineColor);
        }
    }
}
