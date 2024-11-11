using Fusion;
using UnityEngine;

public class LocalInputs : MonoBehaviour
{
    NetworkInputData _inputData;

    bool _isJumpPressed;
    bool _isFirePressed;
    public NetworkPlayer player;

    void Start()
    {
        _inputData = new NetworkInputData();
    }

    void Update()
    {
        _inputData.movementInput = Input.GetAxisRaw("Horizontal");
        _inputData.pos = Input.mousePosition;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _inputData.fire_shoot = true;
        }
        else
        {
            _inputData.fire_shoot = false;
        }


        if (Input.GetKey(KeyCode.W) && player.Grounded)
        {
            _inputData.jumpInput = true;
        }
        else
        {
            _inputData.jumpInput = false;
        }
        if (Input.GetKeyDown(KeyCode.W) && !player.Grounded && player.jumpState == JumpState.InFlight)
        {
            _inputData.double_jumpInput = true;
        }
        if (!Input.GetKeyDown(KeyCode.W) && player.stop_double_jump)
        {
            _inputData.double_jumpInput = false;

        }
        //else
        //{
        //    _inputData.double_jumpInput = false;
        //}
    }

    public NetworkInputData GetLocalInputs()
    {
        //_inputData.fire_shoot = _isFirePressed;
        //_isFirePressed = false;

        //_inputData.jumpInput = false;
        return _inputData;
    }

}
