using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPun, IPunObservable
{
    PlayManager play;
    private int player;
    // Start is called before the first frame update
    void Awake()
    {
        if (photonView.IsMine)
        {
            player = PhotonNetwork.LocalPlayer.ActorNumber;
            play = FindObjectOfType<PlayManager>();
            //play.OnConnected(player);
        }
    }

    private void OnDestroy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            play = FindObjectOfType<PlayManager>();
            play.OnDisConnected(player);

        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (photonView.IsMine)
        {
            stream.SendNext(player);
        }
        else
        {
            player = (int)stream.ReceiveNext();
        }
    }
}
