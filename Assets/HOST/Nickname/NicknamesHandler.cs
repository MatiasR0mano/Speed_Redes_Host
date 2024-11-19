using System;
using System.Collections.Generic;
using UnityEngine;

public class NicknamesHandler : MonoBehaviour
{
    public static NicknamesHandler Instance { get; private set; }
    [SerializeField] private NicknameItem _nicknameItemPrefab;
    private List<NicknameItem> _allNicknames;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _allNicknames = new List<NicknameItem>();
    }


    public NicknameItem CreateNewNicknameItem(ShowNickname owner)
    {
        //1- Se le cree un Nickname Item
        var newNickname = Instantiate(_nicknameItemPrefab, transform)
                            .Initialize(owner.transform);

        //2- Se agregue a la lista
        _allNicknames.Add(newNickname);

        //3- Suscribirse al evento de muerte del jugador para que cuando se ejecute, se destruya el nickname que le pertenece y se lo saque de la lista
        owner.OnDespawn += () =>
        {
            Destroy(newNickname.gameObject);
            _allNicknames.Remove(newNickname);
        };

        //4- Devolver el nickname creado
        return newNickname;
    }

    //En un tipo de Update ejecutar el metodo de cada nickname para que se posicionen en base a su jugador
    private void LateUpdate()
    {
        foreach (var nicknameItem in _allNicknames) nicknameItem.UpdatePosition();
    }
}
