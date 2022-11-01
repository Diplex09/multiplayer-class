using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PhysxBall : NetworkBehaviour
{
    [SerializeField] private int _lifeTime = 10;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private int _force;
    [Networked] private TickTimer _life { get; set; }

    public void Init(Vector3 playerForward) {
        _life = TickTimer.CreateFromSeconds(Runner, 5);
        _rigidbody.velocity = playerForward * _force;
        //_rigidbody.AddForce(playerForward * 1000);
    }

    public override void FixedUpdateNetwork() {
        if (_life.Expired(Runner)) {
            Runner.Despawn(Object);
        }
    }
}
