using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Bet
{
    die,
    call,
    half,
    check,
    pping,
    ttadang,
    quarter,
    none
}

public class BettingManager : MonoBehaviourPunCallbacks
{
    private ButtonManager buttonManager;
    

    public Bet bet;

    // Start is called before the first frame update
    void Start()
    {
        buttonManager = GetComponent<ButtonManager>();
        bet = Bet.none;
    }


    public void CallBtnDown()
    {
        photonView.RPC("Betting", RpcTarget.All, Bet.call);
    }

    public void HalfBtnDown()
    {
        photonView.RPC("Betting", RpcTarget.All, Bet.half);
    }

    public void QuarterBtnDown()
    {
        photonView.RPC("Betting", RpcTarget.All, Bet.quarter);
    }

    public void TtadangBtnDown()
    {
        photonView.RPC("Betting", RpcTarget.All, Bet.ttadang);
    }

    public void CheckBtnDown()
    {
        photonView.RPC("Betting", RpcTarget.All, Bet.check);
    }

    public void PpingBtnDown()
    {
        photonView.RPC("Betting", RpcTarget.All, Bet.pping);
    }

    public void DieBtnDown()
    {
        photonView.RPC("Betting", RpcTarget.All, Bet.die);
    }

    [PunRPC]
    public void Betting(Bet bet)
    {
        this.bet = bet;
    }

    public void none()
    {
        this.bet = Bet.none;
    }
}
