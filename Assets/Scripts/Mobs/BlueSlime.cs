using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlueSlime : Slime
{
    protected override void Start()
    {
        base.Start();

        _jumpCooldownMaxTime = .2f;
        _jumpCooldownTime = _jumpCooldownTime = Random.Range(-2 *_jumpCooldownMaxTime, 0);
    }

    protected override void Update()
    {
        base.Update();
    }

}
