using Fusion;

public class Meta : NetworkBehaviour, Iinteract
{
    public void interact(NetworkPlayer player)
    {
        player.ganador = true;
        gameObject.SetActive(false);
        //Gamemanager.instance.RPC_Win(Runner.LocalPlayer, player);
        //Gamemanager.instance.RPC_Defeat(Runner.LocalPlayer, player);
    }
}
