using Fusion;
using UnityEngine;
public struct NetworkInputData : INetworkInput
{
    public float movementInput;
    public bool jumpInput;
    public bool double_jumpInput;
    public bool fire_shoot;


    public Vector3 pos;
    public NetworkBool isFirePressed;
    public NetworkButtons networkButtons;
}

enum MyButtons
{
    Jump = 0,
}
