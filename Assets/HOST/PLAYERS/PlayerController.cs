using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public NetworkPlayer player;


    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData inputs)) return;

        player.Jump_model(inputs.jumpInput, inputs.double_jumpInput);
        player.UpdateJumpState(inputs.jumpInput);
        player.Jump2D(inputs.jumpInput);

        player.Move2D(inputs.movementInput);

        player.Rotation_AIM(player.aim, inputs.pos);

        if (player.skill != null && inputs.isFirePressed) player.Habilidad_Skill();
    }
}
