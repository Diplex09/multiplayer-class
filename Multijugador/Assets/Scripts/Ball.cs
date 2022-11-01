using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    [SerializeField] private int _speed = 5;
    [SerializeField] private int _lifeTime = 10;
    [Networked] private TickTimer _life { get; set; }

    public void Init() {
        _life = TickTimer.CreateFromSeconds(Runner, 5);
    }

    public override void FixedUpdateNetwork() {
        if (_life.Expired(Runner)) {
            Runner.Despawn(Object);
        } else {
            transform.position += transform.forward * _speed * Runner.DeltaTime;
        }
    }
}
