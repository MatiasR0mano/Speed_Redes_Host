using Fusion;
using UnityEngine;

public class Pinchos : NetworkBehaviour, Iinteract
{
    public void interact(NetworkPlayer player) => player.TakeDamage();
}
