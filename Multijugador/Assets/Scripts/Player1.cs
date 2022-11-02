using System.IO;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using TMPro;

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

    public static void OnBallSpawned(Changed<Player1> changed) {
        changed.Behaviour._material.color = Color.white;
    }
    private Material _material;
    public Material Material {
        get {
            if (_material == null) {
                _material = GetComponentInChildren<MeshRenderer>().material;
            }
            return _material;
        }
        set => _material = value;
    }

    public override void Render()
    {
        if (Material == null) {
            Material.color = Color.Lerp(Material.color, Color.yellow, Time.deltaTime);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R) && Object.HasInputAuthority) {
            RPC_SendMessage("Hello World!");
        }
    }

    private TMP_Text _message;
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    void RPC_SendMessage(string message, RpcInfo info = default) {
        if (info.IsInvokeLocal) {
            message = "You said Hello!\n";
        } else {
            message = "Somebody said: Hello!\n";
        }

        if (_message ==  null) {
            _message = GameObject.Find("HelloText").GetComponent<TMP_Text>();
        }
        _message.text += message;
    }

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
                    Spawned = !Spawned;
                } 
                else if ((inputData.Buttons & NetworkInputData.MOUSEBUTTON2) !=0) {
                    _delay = TickTimer.CreateFromSeconds(Runner, 0.3f);
                    Runner.Spawn(_physxBallPrefab, transform.position + transform.forward, Quaternion.LookRotation(_forward), Object.InputAuthority, (runner, o) => {
                    o.GetComponent<PhysxBall>().Init(_forward);
                    });
                    Spawned = !Spawned;
                }
            }
        }
    }
}
