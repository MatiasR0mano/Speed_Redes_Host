using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeHandler : NetworkBehaviour
{
    byte _currentLife;

    const byte MAX_LIFE = 100;

    public override void Spawned()
    {
        _currentLife = MAX_LIFE;
    }

    public void TakeDamage(byte dmg)
    {
        if (_currentLife < dmg)
        {
            dmg = _currentLife;
        }
        _currentLife -= dmg;

        if (_currentLife == 0)
        {
            DisconnectPlayer();
        }
    }

    void DisconnectPlayer()
    {
        if (!Object.HasInputAuthority)
        {
            Runner.Disconnect(Object.InputAuthority);
        }

        Runner.Despawn(Object);
    }
}
