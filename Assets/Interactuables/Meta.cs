using Fusion;

public class Meta : NetworkBehaviour, Iinteract
{
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_interact([RpcTarget] PlayerRef player_rpc, NetworkPlayer player)
    {
        player.ganador = true;
        gameObject.SetActive(false);
        Gamemanager.instance.RPC_Win(Runner.LocalPlayer, player);
        Gamemanager.instance.RPC_Defeat(Runner.LocalPlayer, player);
        Gamemanager.instance.audSource1.PlayOneShot(Gamemanager.instance.clip2);
    }
}
