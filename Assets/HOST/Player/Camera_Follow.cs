using UnityEngine;
using Cinemachine;
public class Camera_Follow : MonoBehaviour
{
    public CinemachineVirtualCamera _cam;
    //public void CameraCine(Player2 player) => _cam.Follow = player.transform;
    public void CameraCine(NetworkPlayer player) => _cam.Follow = player.transform;
}