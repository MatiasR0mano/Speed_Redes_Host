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

        player.JumpWall2D(inputs.jump_wall);

        if (!player.takedamage && !player._inWall) player.Move2D(inputs.movementInput);

        player.Rotation_AIM(player.aim, inputs.pos);

        if (player.skill != null && inputs.fire_shoot)
        {
            Debug.Log("desactivo");
            player.Habilidad_Skill(player.verdadero_aim.transform);
            player.skill = null;
        }
        Debug.Log(inputs.permito_irme_a_la_m);
        player.RPC_DisconnectPlayer(inputs.permito_irme_a_la_m);
    }


}
