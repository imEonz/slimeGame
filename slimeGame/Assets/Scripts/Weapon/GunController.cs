using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class GunController : WeaponController
{
    protected override void Shot()
    {
        if (!CanFire()) return;
        TeleportAroundPlayer();
        base.Shot();
    }
    public void TeleportAroundPlayer()
    {
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        Vector2 offset = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * orbitRadius;

        transform.position = (Vector2)player.position + offset;
    }
}
