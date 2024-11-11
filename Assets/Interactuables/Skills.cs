using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public abstract class Skills : NetworkBehaviour, Iinteract
{
    float _time_delete = 3.5f;
    public float count = 0f;
    [SerializeField] Skill_N1 _skill_prefab;
    public bool habil;
    public NetworkRigidbody2D _net_rb2D;
    public abstract void Habilidad(NetworkPlayer player);

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_interact([RpcTarget] PlayerRef player_rpc,NetworkPlayer player)
    {
        if (!habil)
        {
            player.skill = _skill_prefab;
            Runner.Despawn(Object);
            player.aim.SetActive(true);
        }
        else player.TakeDamage();
    }

    public float Time_Delete { get { return _time_delete; } }
}
