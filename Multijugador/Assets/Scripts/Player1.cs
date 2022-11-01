using System.IO;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Player1 : NetworkBehaviour
{
    [SerializeField] private NetworkCharacterControllerPrototype _characterControllerPrototype;
    [SerializeField] private int _movementSpeed = 5;
    [SerializeField] private Ball _ballPrefab;
    [SerializeField] private PhysxBall _physxBallPrefab;
    private Vector3 _forward;
    [Networked] private TickTimer _delay { get; set; }
    [Networked (OnChanged = nameof(OnBallSpawned))]
    public NetworkBool Spawned { get; set; }

    public void OnBallSpawned(Changed<Player1> changed) {
        changed.Behaviour.material.color = Color.white;
    }

    [SerializeField] private Material _material;

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData inputData)) {
            inputData.Direction.Normalize();
            _characterControllerPrototype.Move(inputData.Direction * Runner.DeltaTime * _movementSpeed);

            if (inputData.Direction.sqrMagnitude > 0) {
                _forward = inputData.Direction;
            }

            if (_delay.ExpiredOrNotRunning(Runner)) {
                if ((inputData.Buttons & NetworkInputData.MOUSEBUTTON1) !=0) {
                    _delay = TickTimer.CreateFromSeconds(Runner, 0.3f);
                    Runner.Spawn(_ballPrefab, transform.position + transform.forward, Quaternion.LookRotation(_forward), Object.InputAuthority, (runner, o) => {
                    o.GetComponent<Ball>().Init();
                    });
                } 
                else if ((inputData.Buttons & NetworkInputData.MOUSEBUTTON2) !=0) {
                    _delay = TickTimer.CreateFromSeconds(Runner, 0.3f);
                    Runner.Spawn(_physxBallPrefab, transform.position + transform.forward, Quaternion.LookRotation(_forward), Object.InputAuthority, (runner, o) => {
                    o.GetComponent<PhysxBall>().Init(_forward);
                    });
                }
            }
        }
    }
}
