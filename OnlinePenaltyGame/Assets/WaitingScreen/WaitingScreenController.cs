using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class WaitingScreenController : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI countdownText;
    [SerializeField] Image player2Image;
    float countdownTime = 5;
    PhotonView photonView;
    bool timeExpired = false;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            player2Image.gameObject.SetActive(true);
            photonView.RPC("PunRPC_CountdownTimer", RpcTarget.All);
        }
        else
        {
            Debug.Log("Room does not have 2 players yet");
        }
    }

    [PunRPC]
    IEnumerator PunRPC_CountdownTimer()
    {
        while (!timeExpired)
        {
            if (countdownTime <= 0)
            {
                timeExpired = true;
                PhotonNetwork.LoadLevel("Game");
                yield break;
            }

            countdownText.gameObject.SetActive(true);
            countdownText.text = countdownTime.ToString();
            yield return new WaitForSeconds(1);
            countdownTime--;
        }
    }
}
