using Fusion.Addons.Physics;
using Fusion;
using UnityEngine;

public class Skill_N1 : Skills
{
    [SerializeField] private float _lifeTime;
    private TickTimer _lifeTimer;
    public override void Habilidad(NetworkPlayer _player)
    {
        habil = true;
    }

    public override void Spawned()
    {
        _lifeTimer = TickTimer.CreateFromSeconds(Runner, _lifeTime);
        _net_rb2D.Rigidbody.velocity = new Vector2(transform.up.x * 25f, transform.up.y * 25f);
    }
    public override void FixedUpdateNetwork()
    {
        if (!_lifeTimer.Expired(Runner)) return;
        if (habil)
        {
            count = count < Time_Delete ? count += Runner.DeltaTime : count;
            if (count >= Time_Delete) Runner.Despawn(Object);
        }
    }
}