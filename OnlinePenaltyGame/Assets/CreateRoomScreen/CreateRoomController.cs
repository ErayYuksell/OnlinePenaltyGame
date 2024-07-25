using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public class CreateRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField createRoomInput;
    [SerializeField] TMP_InputField joinRoomInput;

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createRoomInput.text);
    }
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinRoomInput.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("WaitingScreen");
    }
}
