using Fusion;
using UnityEngine;

public class LocalInputs : MonoBehaviour
{
    NetworkInputData _inputData;

    bool _isJumpPressed;
    bool _isFirePressed;
    bool doubleJumped = false;
    public NetworkPlayer player;

    void Start()
    {
        _inputData = new NetworkInputData();
    }

    void Update()
    {
        _inputData.movementInput = Input.GetAxis("Horizontal");
        _inputData.pos = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _inputData.fire_shoot = true; 
        }
        else _inputData.fire_shoot = false;

        if (Input.GetKey(KeyCode.W) && player.Grounded)
        { 
            _inputData.jumpInput = true;
            player.RPC_PlayXsoundClip(2);
        }
        else _inputData.jumpInput = false;

        if (Input.GetKey(KeyCode.W) && player._inWall)
        {
            _inputData.jump_wall = true;
            player.RPC_PlayXsoundClip(2);
        }  
        else _inputData.jump_wall = false;

        if (Input.GetKeyDown(KeyCode.W) && !player.Grounded && player.jumpState == JumpState.InFlight && !doubleJumped)
        {
            _inputData.double_jumpInput = true;
            doubleJumped = true;

            player.RPC_PlayXsoundClip(2);
        }

        //if (!Input.GetKeyDown(KeyCode.W) && player.stop_double_jump) _inputData.double_jumpInput = false;

        if (player.Grounded)
        {
            doubleJumped = false;
            _inputData.double_jumpInput = false;
        }
    }

    public NetworkInputData GetLocalInputs()
    {
        return _inputData;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_MEVOY()
    {
        _inputData.permito_irme_a_la_m = true;
        Debug.Log(_inputData.permito_irme_a_la_m);
    }
    private void OnApplicationQuit()
    {
        RPC_MEVOY();
    }
}
