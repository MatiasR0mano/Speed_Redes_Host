using Fusion;
using Unity.Burst.Intrinsics;
using UnityEditor.Experimental.GraphView;

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
            player.Habilidad_Skill();
            player.skill = null;
            player.aim.SetActive(false);
        }
    }
}
