using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private Text moneyText;
    /*[SerializeField] private BettingManager betManager;*/
    private PlayManager play;
    [SerializeField] private int turn;
    [SerializeField] private int sequence;
    [SerializeField] private bool started;
    [SerializeField] private Button[] betBtns;
    [SerializeField] private TextMeshProUGUI startBtnText;
    [SerializeField] private  int livePlayer;
    [SerializeField] private bool canCheck;
    [SerializeField] private bool canPush;
    /* [SerializeField] private Bet bet;*/

    // Start is called before the first frame update
    void Start()
    {
        play = GetComponent<PlayManager>();
        started = play.getStarted();
        /*betManager = GetComponent<BettingManager>();*/
        sequence = 0;
        livePlayer = 0;
    }

    public void StartBtnSet()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            startBtnText.text = "준비 하기";
        }
        else
        {
            startBtnText.text = "시작 하기";
        }
    }

    public void CallBtnDown()
    {
        if (canPush)
        {
            canPush = false;
            /* if(sequence == PhotonNetwork.LocalPlayer.ActorNumber-1)*/
            photonView.RPC("RPCUpdateMoney", RpcTarget.MasterClient, 1, play.GetActNumber());
            photonView.RPC("RPCBtn", RpcTarget.All, 1, play.GetActNumber());
        }
    }

    public void HalfBtnDown()
    {
        if(canPush)
        /*if (sequence == PhotonNetwork.LocalPlayer.ActorNumber-1)*/
        {
            canPush = false;
            photonView.RPC("RPCUpdateMoney", RpcTarget.MasterClient, 2, play.GetActNumber());
            photonView.RPC("RPCBtn", RpcTarget.All, 2, play.GetActNumber());
        }
    }

    public void QuarterBtnDown()
    {
        if (canPush)
        /*if (sequence == PhotonNetwork.LocalPlayer.ActorNumber-1)*/
        {
            canPush = false;
            photonView.RPC("RPCUpdateMoney", RpcTarget.MasterClient, 3, play.GetActNumber());
            photonView.RPC("RPCBtn", RpcTarget.All, 3, play.GetActNumber());
        }
    }

    public void TtadangBtnDown()
    {
        if (canPush)
        /* if (sequence == PhotonNetwork.LocalPlayer.ActorNumber-1)*/
        {
            canPush = false;
            photonView.RPC("RPCUpdateMoney", RpcTarget.MasterClient, 4, play.GetActNumber());
            photonView.RPC("RPCBtn", RpcTarget.All, 4, play.GetActNumber());
        }
    }

    public void CheckBtnDown()
    {
        if (canPush)
        /* if (sequence == PhotonNetwork.LocalPlayer.ActorNumber-1)*/
        {
            canPush = false;
            photonView.RPC("RPCUpdateMoney", RpcTarget.MasterClient, 5, play.GetActNumber());
            photonView.RPC("RPCBtn", RpcTarget.All, 5, play.GetActNumber());
        }
    }

    public void PpingBtnDown()
    {
        if (canPush)
        /* if (sequence == PhotonNetwork.LocalPlayer.ActorNumber-1)*/
        {
            canPush = false;
            photonView.RPC("RPCUpdateMoney", RpcTarget.MasterClient, 6, play.GetActNumber());
            photonView.RPC("RPCBtn", RpcTarget.All, 6, play.GetActNumber());
        }
    }

    public void DieBtnDown()
    {
        if (canPush)
        /* if (sequence == PhotonNetwork.LocalPlayer.ActorNumber-1)*/
        {
            canPush = false;
            photonView.RPC("RPCDie", RpcTarget.MasterClient, play.GetActNumber());
            photonView.RPC("RPCBtn", RpcTarget.All, 0, play.GetActNumber());
        }
    }

    public void StartGame()
    {

        if (!PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPCReady", RpcTarget.MasterClient, play.GetActNumber());
            return;
        }
        play.SetCards();
        play.SetDeck();
        play.SetHand();
        livePlayer = play.readyPlayers();
        for (int i = 0; i < 3; i++)
            photonView.RPC("RpcDrawCard", RpcTarget.All);
        photonView.RPC("RPCSetMoney", RpcTarget.All,0);
        /*photonView.RPC("RpcSelect", RpcTarget.All);*/
        //photonView.RPC("RpcGene", RpcTarget.All);
        //started = true;
        //photonView.RPC("SetCardImage", RpcTarget.All);
    }

    [PunRPC]
    public void RPCDie(int player)
    {
        sequence++;
        UpdateDie(player);
        if (play.readyPlayers() == 1)
        {
            photonView.RPC("RPCDieEndGame", RpcTarget.MasterClient);
            sequence = 0;
        }
        
        else if (sequence >= livePlayer)
        {
            if (turn < 5)
            {
                photonView.RPC("RpcDrawCard", RpcTarget.All);
                sequence %= livePlayer;
                livePlayer = play.readyPlayers();
            }
            else
            {
                photonView.RPC("RPCEndGame", RpcTarget.MasterClient);
                sequence =0;
            }
            //photonView.RPC("RpcGene", RpcTarget.All);
            //photonView.RPC("RpcSetCardImage", RpcTarget.All);
        }
       
        /*moneyText.text = pot.ToString();*/
    }

    [PunRPC]
    public void RPCUpdateMoney(int btn, int player)
    {
        if (btn ==1 || btn==5 || btn == 6)
            sequence++;
        UpdateMoney(btn, player);
        if (sequence >= livePlayer)
        {
            if (turn < 5)
            {
                photonView.RPC("RpcDrawCard", RpcTarget.All);
                sequence %= livePlayer;
                livePlayer = play.readyPlayers();
            }
            else
            {
                photonView.RPC("RPCEndGame", RpcTarget.MasterClient); ;
                sequence = 0;
            }
        }
        /*moneyText.text = pot.ToString();*/
    }

    [PunRPC]
    public void RPCBtn(int btn, int player)
    {
        play.PushButton(btn, player);
    }

    [PunRPC]
    public void RPCReady(int player)
    {
        play.ImReady(player);
    }

    [PunRPC]
    public void RPCEndGame()
    {
        play.EndGame();
    }

    [PunRPC]
    public void RPCDieEndGame()
    {
        play.DieEndGame();
    }

    [PunRPC]
    public void RPCSetMoney(int btn)
    {
        play.UpdateMoney(btn);
    }

    public void UpdateMoney(int btn, int player)
    {
        play.UpdateMoney(player, btn);
    }

    public void UpdateDie(int player)
    {
        play.UpdateDie(player);
    }

    [PunRPC]
    public void RpcDrawCard()
    {

            photonView.RPC("RpcDrawCard", RpcTarget.MasterClient, play.GetActNumber());
    }

    [PunRPC]
    public void RpcDrawCard(int player)
    {
        if(play.ImLive(player))
            play.DrawCard(player);
    }
    public int GetSequence()
    {
        return sequence;
    }

    public void BackSequence()
    {
        sequence--;
    }

    public void CanPush(int player)
    {
        photonView.RPC("RPCCanPush", RpcTarget.All, player);
    }

    [PunRPC]
    public void RPCCanPush(int player)
    {
        if (play.GetActNumber() == player)
            canPush = true;
        else
            canPush = false;
    }

    /* [PunRPC]
     public void RpcGene()
     {
         play.RPCGene();
     }*/

    /* [PunRPC]
     public void RpcSelect()
     {
         play.OnSelect();
     }*/

    /*[PunRPC]
    public void RpcSetCardImage()
    {
        play.SetCardImage();
    }*/

    /*[PunRPC]
    public void RPCBet(Bet bet)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        betManager.none();
        switch (bet)
        {
            case Bet.call:
                RPCUpdateMoney(ante);
                break;
            case Bet.half:
                RPCUpdateMoney((int)pot / 2);
                break;
            case Bet.check:
                RPCUpdateMoney(0);
                break;
            case Bet.pping:
                RPCUpdateMoney(DefaultAnte);
                break;
            case Bet.quarter:
                RPCUpdateMoney((int)pot / 4);
                break;
            case Bet.ttadang:
                RPCUpdateMoney(ante * 2);
                break;
            case Bet.die:
                RPCUpdateMoney(0);
                break;

        }
    }*/

    void Update()
    {
        started = play.getStarted();
        StartBtnSet();
        if (!started)
        {
            for(int i =0; i < betBtns.Length; i++)
            {
                betBtns[i].interactable = false;
            }
        }
        else
        {
            turn = play.getTurn();
            for (int i = 0; i < betBtns.Length; i++)
            {
                betBtns[i].interactable = true;
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(sequence);
        }
        else
        {
            sequence = (int)stream.ReceiveNext();
        }
    }
}
