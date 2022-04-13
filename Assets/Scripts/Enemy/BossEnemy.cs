using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : Enemy
{
    protected override void Move(Vector3 targetDir)
    {
        targetDir = player.transform.position;
        base.Move(targetDir);
    }
}
