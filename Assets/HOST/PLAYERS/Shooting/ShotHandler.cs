using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotHandler : NetworkBehaviour
{
    [SerializeField] NetworkPrefabRef _bulletPrefab;
    [SerializeField] Transform _bulletSpawnTransform;

    public void Fire()
    {
        if (!HasStateAuthority) return;

        #region Spawn Bullet
        Runner.Spawn(_bulletPrefab, _bulletSpawnTransform.position, _bulletSpawnTransform.rotation);
        #endregion
    }
}
