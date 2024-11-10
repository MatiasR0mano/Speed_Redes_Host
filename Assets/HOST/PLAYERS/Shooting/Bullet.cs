using Fusion;
using Fusion.Addons.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkRigidbody3D))]
public class Bullet : NetworkBehaviour
{
    [SerializeField] byte _dmg = 25;
    TickTimer _lifeTimer = TickTimer.None;

    public override void Spawned()
    {
        GetComponent<NetworkRigidbody3D>().Rigidbody.AddForce(transform.forward * 10, ForceMode.VelocityChange);

        if (HasStateAuthority)
        {
            _lifeTimer = TickTimer.CreateFromSeconds(Runner, 2);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!_lifeTimer.Expired(Runner)) return;
        
        DespawnObject();
    }

    void DespawnObject()
    {
        Runner.Despawn(Object);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Object || !HasStateAuthority) return;

        if (other.TryGetComponent(out LifeHandler player))
        {
            player.TakeDamage(_dmg);
        }

        DespawnObject();
    }
}
