using Fusion;

public class Pinchos : NetworkBehaviour, Iinteract
{
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_interact([RpcTarget] PlayerRef player_rpc, NetworkPlayer player) => player.TakeDamage();
}
