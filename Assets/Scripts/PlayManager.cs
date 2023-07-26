using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayManager : MonoBehaviourPunCallbacks, IPunObservable
{
    private List<int> cards;
    private Stack<int> deck;
    [SerializeField] private bool started;
    [SerializeField]private Button startBtn;
    [SerializeField] private Sprite[] cardSprites;
    [SerializeField] private Image[] localCards;
    [SerializeField] private Image[] RemoteCards;
    private int[][] playerHands;
    [SerializeField] private int turn;
    [SerializeField] private int geneCount;
	[SerializeField] private int[] playerTopMark;
    [SerializeField] private int[] playerTopNum;
    [SerializeField] private Text geneText;
    [SerializeField] private int[] genealogy;
    int[] genealogy_ = { -1, -1, -1, -1, -1};
    [SerializeField] private int startDrawCount;
    [SerializeField] private GameObject firstSelect;
    [SerializeField] private Image[] selectHandImages;
    [SerializeField] private int selectCount;
    [SerializeField] private TextMeshProUGUI selectText;
    [SerializeField] private bool selected;
    [SerializeField] private int[] haveMoney;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private int ante;
    [SerializeField] private int defaultAnte;
    [SerializeField] private int pot;
    [SerializeField] private int setMoneyInt;
    [SerializeField] private Text potText;
    [SerializeField] private GameObject winnerPanel;
    [SerializeField] private Image[] winnerCards;
    [SerializeField]private int winGene;
    [SerializeField] private int winPlayer;
    [SerializeField] private bool[] playerReady;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private TextMeshProUGUI winGeneText;
    //[SerializeField] private int readyPeople;
    [SerializeField] private int rate;
    [SerializeField] private Button[] betBtns;

  
    [SerializeField] private TextMeshProUGUI whatPlayerText;
    [SerializeField] private GameObject readyPanel;
    [SerializeField] private TextMeshProUGUI[] onReadyPlayerText;
    [SerializeField] private int actorNumber;
    [SerializeField] private int player1ActorNumber;
    [SerializeField] private int player2ActorNumber;
    [SerializeField] private int player3ActorNumber;
    [SerializeField] private int player4ActorNumber;
    [SerializeField] private int player5ActorNumber;
    [SerializeField] private string player1Name;
    [SerializeField] private string player2Name;
    [SerializeField] private string player3Name;
    [SerializeField] private string player4Name;
    [SerializeField] private string player5Name;
    [SerializeField] private int player1Image;
    [SerializeField] private int player2Image;
    [SerializeField] private int player3Image;
    [SerializeField] private int player4Image;
    [SerializeField] private int player5Image;

    [SerializeField] private TextMeshProUGUI playerNameText1;
    [SerializeField] private TextMeshProUGUI playerNameText2;
    [SerializeField] private TextMeshProUGUI playerNameText3;
    [SerializeField] private TextMeshProUGUI playerNameText4;

    [SerializeField] private Image[] images;
    [SerializeField] private Sprite[] sprites;

    [SerializeField] private TextMeshProUGUI playerMoneyText1;
    [SerializeField] private TextMeshProUGUI playerMoneyText2;
    [SerializeField] private TextMeshProUGUI playerMoneyText3;
    [SerializeField] private TextMeshProUGUI playerMoneyText4;

    [SerializeField] private Text callText;
    [SerializeField] private bool canHalf;
    [SerializeField] private Button[] halfBtn;
    [SerializeField] private ButtonManager bM;
    [SerializeField] private bool IsPPing;
    [SerializeField] private bool IsCheck;

    [SerializeField] private Image[] betImages;
    [SerializeField] private AudioClip[] betAudios;
    [SerializeField] private Sprite[] betSprites;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private SpriteRenderer potSprite;
    [SerializeField] private Sprite[] potSprites;

    [SerializeField] private bool canCheck;
    private string winnerGene;
    // Start is called before the first frame update

    void Awake()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties["name"]);
        PhotonNetwork.Instantiate("Player", new Vector3(0, 0, 0), Quaternion.identity);
        OnConnected(PhotonNetwork.LocalPlayer.ActorNumber, (string)PhotonNetwork.LocalPlayer.CustomProperties["name"], (int)PhotonNetwork.LocalPlayer.CustomProperties["image"]);
        bM = GetComponent<ButtonManager>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        canCheck = true;
        haveMoney = new int[5];
        GetMyNumber();
        started = false;
        cards = new List<int>();
        turn = -3;
        deck = new Stack<int>();
        playerHands = new int[5][];
        playerHands[0] = new int[7];
        playerHands[1] = new int[7];
        playerHands[2] = new int[7];
        playerHands[3] = new int[7];
        playerHands[4] = new int[7];
        geneCount = 0;
        playerTopMark = new int[5];
        playerTopNum = new int[5];
        
        genealogy = genealogy_;
        startDrawCount = 0;
        selectCount = 0;
        selected = false;
        ante = defaultAnte;
        pot = 0;
        setMoneyInt = 0;
        winGene = -1;
        winPlayer = 0;
        playerReady = new bool[5];
        if (PhotonNetwork.IsMasterClient)
            ImReady(actorNumber);
        else
            photonView.RPC("TryGetReady", RpcTarget.MasterClient);
        //readyPeople = 0;
        rate = 0;
        
        
    }


    // Update is called once per frame
    void Update()
    {
        if (pot == 0)
        {
            potSprite.sprite = potSprites[0];
        }
        else if (pot < 500000)
        {
            potSprite.sprite = potSprites[1];
        }
        else if (pot < 1000000)
        {
            potSprite.sprite = potSprites[2];
        }
        else if (pot < 5000000)
        {
            potSprite.sprite = potSprites[3];
        }
        else
        {
            potSprite.sprite = potSprites[4];
        }
        potText.text = pot.ToString();
        moneyText.text = haveMoney[actorNumber].ToString();
        WriteCallText();
        SetUser(actorNumber);
        if (started)
        {
            startBtn.gameObject.SetActive(false);
            readyPanel.SetActive(false);

            geneText.text = GetGeneText(actorNumber);
            if (rate == actorNumber)
            {
                if (canCheck)
                {
                    if (canHalf)
                    {
                        for (int i = 0; i < betBtns.Length; i++)
                        {
                            betBtns[i].interactable = true;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < betBtns.Length; i++)
                        {
                            if (i == 0 || i == 3)
                            {
                                betBtns[i].interactable = true;
                            }
                            else
                            {
                                betBtns[i].interactable = false;
                            }
                        }
                    }
                }
                else
                {
                    if (canHalf)
                    {
                        for (int i = 0; i < betBtns.Length; i++)
                        {
                            if (i == 6 || i == 2)
                            {
                                betBtns[i].interactable = false;
                            }
                            else
                            {
                                betBtns[i].interactable = true;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < betBtns.Length; i++)
                        {
                            if (i == 0 || i == 3)
                            {
                                betBtns[i].interactable = true;
                            }
                            else
                            {
                                betBtns[i].interactable = false;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < betBtns.Length; i++)
                {
                    betBtns[i].interactable = false;
                }
            }

        }
        else if (PhotonNetwork.PlayerList.Length >= 2)
        {
            for (int i = 0; i < betBtns.Length; i++)
            {
                betBtns[i].interactable = false;
            }
            if (!PhotonNetwork.IsMasterClient)
                startBtn.interactable = true;
            else
            {
                if(readyPlayers() == PhotonNetwork.PlayerList.Length)
                    startBtn.interactable = true;
                else
                {
                    startBtn.interactable = false;
                }
            }
        }
        else
        {
            for (int i = 0; i < betBtns.Length; i++)
            {
                betBtns[i].interactable = false;
            }
            startBtn.interactable = false;
        }

        for(int i = 0; i < playerReady.Length; i++)
        {
            if (playerReady[i])
                onReadyPlayerText[i].color = Color.yellow;
            else
                onReadyPlayerText[i].color = Color.white;
        }

      

        


        /*if (started)
        {
            //photonView.RPC("SetCardImage", RpcTarget.All);
            *//*SetCardImage();*/
            /*geneText.text = GetGeneText(PhotonNetwork.LocalPlayer.ActorNumber - 1);*//*
        }*/

        /*if (PhotonNetwork.IsMasterClient)
        {
            if (startDrawCount >= PhotonNetwork.CurrentRoom.PlayerCount)
            {
                turn++;
                startDrawCount -= PhotonNetwork.CurrentRoom.PlayerCount;
                photonView.RPC("RPCGene", RpcTarget.All);
            }

            if (!started && turn == 1)
            {
                photonView.RPC("SelectHand", RpcTarget.All);
            }

            if (selectCount >= PhotonNetwork.CurrentRoom.PlayerCount)
            {
                started = true;
                photonView.RPC("OffSelectHand", RpcTarget.All);
                selectCount = 0;
            }
        }*/

    }

    public void WriteCallText()
    {
        if (IsCheck)
        {
            callText.text = 0.ToString();
        }
        else if (IsPPing)
        {
            callText.text = defaultAnte.ToString();
        }
        else
        {
            callText.text = ante.ToString();
        }
    }

    public int GetActNumber()
    {
        return actorNumber;
    }
    public void SetUser(int n)
    {
        switch (n)
        {
            case 0:
                playerNameText1.text = player2Name;
                playerNameText2.text = player3Name;
                playerNameText3.text = player4Name;
                playerNameText4.text = player5Name;
                images[0].sprite = sprites[player2Image];
                images[1].sprite = sprites[player3Image];
                images[2].sprite = sprites[player4Image];
                images[3].sprite = sprites[player5Image];
                playerMoneyText1.text = haveMoney[1].ToString();
                playerMoneyText2.text = haveMoney[2].ToString();
                playerMoneyText3.text = haveMoney[3].ToString();
                playerMoneyText4.text = haveMoney[4].ToString();
                break;
            case 1:
                playerNameText1.text = player1Name;
                playerNameText2.text = player3Name;
                playerNameText3.text = player4Name;
                playerNameText4.text = player5Name;
                images[0].sprite = sprites[player1Image];
                images[1].sprite = sprites[player3Image];
                images[2].sprite = sprites[player4Image];
                images[3].sprite = sprites[player5Image];
                playerMoneyText1.text = haveMoney[0].ToString();
                playerMoneyText2.text = haveMoney[2].ToString();
                playerMoneyText3.text = haveMoney[3].ToString();
                playerMoneyText4.text = haveMoney[4].ToString();
                break;
            case 2:
                playerNameText1.text = player1Name;
                playerNameText2.text = player2Name;
                playerNameText3.text = player4Name;
                playerNameText4.text = player5Name;
                images[0].sprite = sprites[player1Image];
                images[1].sprite = sprites[player2Image];
                images[2].sprite = sprites[player4Image];
                images[3].sprite = sprites[player5Image];
                playerMoneyText1.text = haveMoney[0].ToString();
                playerMoneyText2.text = haveMoney[1].ToString();
                playerMoneyText3.text = haveMoney[3].ToString();
                playerMoneyText4.text = haveMoney[4].ToString();
                break;
            case 3:
                playerNameText1.text = player1Name;
                playerNameText2.text = player2Name;
                playerNameText3.text = player3Name;
                playerNameText4.text = player5Name;
                images[0].sprite = sprites[player1Image];
                images[1].sprite = sprites[player2Image];
                images[2].sprite = sprites[player3Image];
                images[3].sprite = sprites[player5Image];
                playerMoneyText1.text = haveMoney[0].ToString();
                playerMoneyText2.text = haveMoney[1].ToString();
                playerMoneyText3.text = haveMoney[2].ToString();
                playerMoneyText4.text = haveMoney[4].ToString();
                break;
            case 4:
                playerNameText1.text = player1Name;
                playerNameText2.text = player2Name;
                playerNameText3.text = player3Name;
                playerNameText4.text = player4Name;
                images[0].sprite = sprites[player1Image];
                images[1].sprite = sprites[player2Image];
                images[2].sprite = sprites[player3Image];
                images[3].sprite = sprites[player4Image];
                playerMoneyText1.text = haveMoney[0].ToString();
                playerMoneyText2.text = haveMoney[1].ToString();
                playerMoneyText3.text = haveMoney[2].ToString();
                playerMoneyText4.text = haveMoney[3].ToString();
                break;
        }
    }


    public void GetMyNumber()
    {
        Debug.Log(player2ActorNumber);
        int player = PhotonNetwork.LocalPlayer.ActorNumber;
        if (PhotonNetwork.IsMasterClient)
        {
            if (player1ActorNumber == player)
                actorNumber = 0;
            else if (player2ActorNumber == player)
                actorNumber = 1;
            else if (player3ActorNumber == player)
                actorNumber = 2;
            else if (player4ActorNumber == player)
                actorNumber = 3;
            else if (player5ActorNumber == player)
                actorNumber = 4;
            else
                actorNumber = 5;

            SetMoney(actorNumber);
            whatPlayerText.text = (actorNumber + 1) + " 플레이어";
        }
        else
        {
            photonView.RPC("RPCGetMyNumber", RpcTarget.MasterClient, player);
        }
    }

    [PunRPC]
    public void RPCGetMyNumber(int player)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (player1ActorNumber == player)
                photonView.RPC("RPCGetMyNumber", RpcTarget.Others, player, 1);
            else if (player2ActorNumber == player)
                photonView.RPC("RPCGetMyNumber", RpcTarget.Others, player, 2);
            else if (player3ActorNumber == player)
                photonView.RPC("RPCGetMyNumber", RpcTarget.Others, player, 3);
            else if (player4ActorNumber == player)
                photonView.RPC("RPCGetMyNumber", RpcTarget.Others, player, 4);
            else if (player5ActorNumber == player)
                photonView.RPC("RPCGetMyNumber", RpcTarget.Others, player, 5);
        }
    }

    [PunRPC]
    public void RPCGetMyNumber(int player, int slot)
    {
        if (player == PhotonNetwork.LocalPlayer.ActorNumber && !PhotonNetwork.IsMasterClient)
        {
            actorNumber = slot - 1;
            photonView.RPC("SetMoney", RpcTarget.MasterClient, actorNumber);
            whatPlayerText.text = (actorNumber + 1) + " 플레이어";
        }
    }

    public void ImReady(int player)
    {
        playerReady[player] = !playerReady[player];
        photonView.RPC("SerializeReady", RpcTarget.AllBuffered, playerReady);
    }

    [PunRPC]
    public void TryGetReady()
    {
        photonView.RPC("SerializeReady", RpcTarget.AllBuffered, playerReady);
    }

    [PunRPC]
    public void SerializeReady(bool[] player)
    {
        for (int i = 0; i < playerReady.Length; i++)
            playerReady[i] = player[i];
    }

    public void SetReady()
    {
        started = false;
        playerReady = new bool[5];
        //readyPeople = 0;
        if (PhotonNetwork.IsMasterClient)
            ImReady(actorNumber);
        photonView.RPC("SerializeReady", RpcTarget.AllBuffered, playerReady);
    }

    public int readyPlayers()
    {
        int count = 0;
        for(int i = 0; i< playerReady.Length; i++)
        {
            if (playerReady[i])
                count++;
        }
        return count;
    }

    [PunRPC]
    public void SetMoney(int player)
    {
        haveMoney[player] = 300000000;
        if (setMoneyInt == 0)
            setMoneyInt++;
        else
            setMoneyInt--;
    }

    public void UpdateMoney(int btn)
    {
        photonView.RPC("UpdateMoney", RpcTarget.MasterClient, actorNumber, btn);
    }

    public void UpdateDie(int player)
    {
        canCheck = false;
        playerReady[player] = false;
        NextRate();
        
    }

    [PunRPC]
    public void CantHalf(int player)
    {
        if(player == actorNumber)
            canHalf = false;
    }

    [PunRPC]
    public void UpdateMoney(int player, int btn)
    {
        canCheck = false;
        switch (btn)
        {
            case 0:
                haveMoney[player] -= defaultAnte;
                pot += defaultAnte;
                ante = defaultAnte;
                break;
            case 1:
                if (IsPPing)
                {
                    haveMoney[player] -= defaultAnte;
                    pot += defaultAnte;
                    NextRate();
                    break;
                }
                else if (IsCheck)
                {
                    NextRate();
                    break;
                }
                else
                {
                    haveMoney[player] -= ante;
                    pot += ante;
                    NextRate();
                    break;
                }
            case 2:
                if (IsPPing || IsCheck)
                    bM.BackSequence();
                IsPPing = false;
                IsCheck = false;
                int money = pot / 2;
                haveMoney[player] -= money;
                pot += money;
                ante = money;
                photonView.RPC("CantHalf", RpcTarget.All, player);
                NextRate();
                break;
            case 3:
                if (IsPPing || IsCheck)
                    bM.BackSequence();
                IsPPing = false;
                IsCheck = false;
                money = pot / 4;
                haveMoney[player] -= money;
                pot += money;
                ante = money;
                photonView.RPC("CantHalf", RpcTarget.All, player);
                NextRate();
                break;
            case 4:
                if (IsPPing || IsCheck)
                    bM.BackSequence();
                IsPPing = false;
                IsCheck = false;
                money = ante * 2;
                haveMoney[player] -= money;
                pot += money;
                ante = money;
                photonView.RPC("CantHalf", RpcTarget.All, player);
                NextRate();
                break;
            case 5:
                IsCheck = true;
                NextRate();
                break;
            case 6:
                IsPPing = true;
                money = defaultAnte;
                haveMoney[player] -= money;
                NextRate();
                break;
            case 7:
                break;
        }
    }

    public void GetMarkNumberText(int i)
    {
            string mark = "";
            string num = "";
            if (playerTopMark[i] == 0)
            {
                mark = "♠";
            }
            else if (playerTopMark[i] == 1)
            {
                mark = "◆";
            }
            else if (playerTopMark[i] == 2)
            {
                mark = "♥";
            }
            else if (playerTopMark[i] == 3)
            {
                mark = "♣";
            }

            if (playerTopNum[i] == 0)
            {
                num = "A";
            }
            else if (playerTopNum[i] == 1)
            {
                num = "2";
            }
            else if (playerTopNum[i] == 2)
            {
                num = "3";
            }
            else if (playerTopNum[i] == 3)
            {
                num = "4";
            }
            else if (playerTopNum[i] == 4)
            {
                num = "5";
            }
            else if (playerTopNum[i] == 5)
            {
                num = "6";
            }
            else if (playerTopNum[i] == 6)
            {
                num = "7";
            }
            else if (playerTopNum[i] == 7)
            {
                num = "8";
            }
            else if (playerTopNum[i] == 8)
            {
                num = "9";
            }
            else if (playerTopNum[i] == 9)
            {
                num = "10";
            }
            else if (playerTopNum[i] == 10)
            {
                num = "J";
            }
            else if (playerTopNum[i] == 11)
            {
                num = "Q";
            }
            else if (playerTopNum[i] == 12)
            {
                num = "K";
            }

            winnerGene = mark + " " + num + " " + GetGeneText(i);

            photonView.RPC("GetMarkNumberText", RpcTarget.Others, winnerGene);
    }

    [PunRPC]
    public void GetMarkNumberText(string winnerGene_)
    {
        winnerGene = winnerGene_;
    }

    public string GetGeneText(int i)
    {
        if (genealogy[i] ==11)
        {
            return "로얄 스트레이트 플러시";
        }
        else if (genealogy[i] == 10)
        {
            return "스트레이트 플러시";
        }
        else if (genealogy[i] == 9)
        {
            return "포 카드";
        }
        else if (genealogy[i] == 8)
        {
            return "풀 하우스";
        }
        else if (genealogy[i] == 7)
        {
            return "플러시";
        }
        else if (genealogy[i] == 6)
        {
            return "마운틴";
        }
        else if (genealogy[i] == 5)
        {
            return "백 스트레이트";
        }
        else if (genealogy[i] == 4)
        {
            return "스트레이트";
        }
        else if (genealogy[i] == 3)
        {
            return "트리플";
        }
        else if (genealogy[i] == 2)
        {
            return "투 페어";
        }
        else if (genealogy[i] == 1)
        {
            return "원 페어";
        }
        else if (genealogy[i] == 0)
        {
            return "탑";
        }
        return "";
    }

    [PunRPC]
    public void SetCardImage()
    {
        
        for(int i = 0; i < playerHands.Length; i++)
            for(int j = 0; j < 7; j++)
            {
                if (j < turn + 2)
                {
                    if (i == actorNumber)
                    {
                        localCards[j].gameObject.SetActive(true);
                        localCards[j].sprite = cardSprites[playerHands[i][j]];
                    }
                    else
                    {
                        switch (actorNumber + 1)
                        {
                            case 1:
                                CardSpriteSet(i, j, true);
                                break;
                            case 2:
                                if (i == 0)
                                {
                                    CardSpriteSet(i, j, false);
                                }
                                else
                                {
                                    CardSpriteSet(i, j, true);
                                }
                                break;
                            case 3:
                                if (i < 2)
                                {
                                    CardSpriteSet(i, j, false);
                                }
                                else
                                {
                                    CardSpriteSet(i, j, true);
                                }
                                break;
                            case 4:
                                if (i < 3)
                                {
                                    CardSpriteSet(i, j, false);
                                }
                                else
                                {
                                    CardSpriteSet(i, j, true);
                                }
                                break;
                            case 5:
                                CardSpriteSet(i, j, false);
                                break;
                        }

                    }
                }
                else
                {
                    if (i == actorNumber)
                    {
                        localCards[j].gameObject.SetActive(false);
                    }
                    else
                    {
                        switch (actorNumber + 1)
                        {
                            case 1:
                                RemoteCards[(i - 1) * 7 + j].gameObject.SetActive(false);
                                break;
                            case 2:
                                if (i == 0)
                                {
                                    RemoteCards[j].gameObject.SetActive(false);
                                }
                                else
                                {
                                    RemoteCards[(i - 1) * 7 + j].gameObject.SetActive(false);
                                }
                                break;
                            case 3:
                                if (i < 2)
                                {
                                    RemoteCards[i * 7 + j].gameObject.SetActive(false);
                                }
                                else
                                {
                                    RemoteCards[(i - 1) * 7 + j].gameObject.SetActive(false);
                                }
                                break;
                            case 4:
                                if (i < 3)
                                {
                                    RemoteCards[i * 7 + j].gameObject.SetActive(false);
                                }
                                else
                                {
                                    RemoteCards[(i - 1) * 7 + j].gameObject.SetActive(false);
                                }
                                break;
                            case 5:
                                RemoteCards[i * 7 + j].gameObject.SetActive(false);
                                break;
                        }

                    }
                }
            }
    }

    private void CardSpriteSet(int playerNumber, int Hand, bool after)
    {
        if (!after)
        {
            RemoteCards[playerNumber*7+Hand].gameObject.SetActive(true);
            if (Hand < 2 || Hand == 6)
            {
                RemoteCards[playerNumber * 7 + Hand].sprite = cardSprites[0];
            }
            else
            {
                RemoteCards[playerNumber * 7 + Hand].sprite = cardSprites[playerHands[playerNumber][Hand]];
            }
        }
        else
        {
            RemoteCards[(playerNumber - 1) * 7 + Hand].gameObject.SetActive(true);
            if (Hand < 2 || Hand == 6)
            {
                RemoteCards[(playerNumber - 1) * 7 + Hand].sprite = cardSprites[0];
            }
            else
            {
                RemoteCards[(playerNumber - 1) * 7 + Hand].sprite = cardSprites[playerHands[playerNumber][Hand]];
            }
        }
    }

    [PunRPC]
    public void ReStart()
    {
        started = false;
        cards = new List<int>();
        turn = -3;
        deck = new Stack<int>();
        playerTopMark = new int[5];
        playerTopNum = new int[5];
        genealogy = genealogy_;
        startDrawCount = 0;
        selectCount = 0;
        selected = false;
        rate = FirstRate();
        winGene = -1;
        winPlayer = 0;
        rate = 0;
        //readyPeople = readyPlayers();
        canCheck = true;
    }

    public int FirstRate()
    {
        for(int i = 0; i < playerReady.Length; i++)
        {
            if (playerReady[i])
                return i;
        }
        return 0;
    }

    public void SetCards()
    {
        photonView.RPC("ReStart", RpcTarget.All);
        for (int i = 1; i < 53; i++)
        {
            cards.Add(i);
        }
    }

    public void SetDeck()
    {
        while (cards.Count > 0)
        {
            int i = Random.Range(0, cards.Count);
            deck.Push(cards[i]);
            cards.RemoveAt(i);
        }
    }

    public void SetHand()
    {
        playerHands = new int[5][];
        playerHands[0] = new int[7];
        playerHands[1] = new int[7];
        playerHands[2] = new int[7];
        playerHands[3] = new int[7];
        playerHands[4] = new int[7];
    }

    /*public void StartGame()
    {
        
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        SetCards();
        SetDeck();
        SetHand();
        for (int i = 0; i < 3; i++)
            photonView.RPC("DrawCard", RpcTarget.All);
        started = true;
        //photonView.RPC("SetCardImage", RpcTarget.All);
    }*/

    public void turnUp()
    {
        turn++;
    }

    public void OnSelect()
    {
        firstSelect.SetActive(true);
    }

    [PunRPC]
    public void OffSelectHand(int[][] playerHands_)
    {
        started = true;
        playerHands = playerHands_;
        firstSelect.SetActive(false);
    }

    [PunRPC]
    public void SelectHand()
    {
        if (!selected)
        {
            firstSelect.SetActive(true);
            for (int i = 0; i < selectHandImages.Length; i++)
            {
                selectHandImages[i].gameObject.SetActive(true);
                selectHandImages[i].sprite = cardSprites[playerHands[actorNumber][i]];
            }
        }
    }

    public void VisibleCard(int i)
    {
        selected = true;
        for (int j = 0; j < selectHandImages.Length; j++)
        {
            selectHandImages[j].gameObject.SetActive(false);
        }

        selectText.text = "다른 유저가 선택 중 입니다.";
        photonView.RPC("RPCVisibleCard", RpcTarget.MasterClient, actorNumber, i);
    }

    [PunRPC]
    public void RPCVisibleCard(int player, int i)
    {
        
        if (i != 2) {
            int temp = playerHands[player][i];
            playerHands[player][i] = playerHands[player][2];
            playerHands[player][2] = temp;
        }
       
        selectCount++;
        if (selectCount >= readyPlayers())
        {
            SetFirstRate();
            photonView.RPC("OffSelectHand", RpcTarget.All, playerHands);
            photonView.RPC("SerializeHand", RpcTarget.All, playerHands);
            selectCount = 0;
        }
    }

/*    [PunRPC]
    public void DrawCard()
    {
         photonView.RPC("DrawCard", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber-1);
    }*/

    public bool ImLive(int player)
    {
        return playerReady[player];
    }

    public void DrawCard(int i)
    {
        canCheck = true;
        for (int j = 0; j < playerHands[i].Length; j++)
        {
            if (playerHands[i][j] == 0)
            {
                playerHands[i][j] = deck.Pop();
                startDrawCount++;
                break;
            }
        }
       
        if (startDrawCount >= readyPlayers())
        {
            IsPPing = false;
            IsCheck = false;
            SetFirstRate();
            startDrawCount -= readyPlayers();
            photonView.RPC("SerializeHand", RpcTarget.All, playerHands);

        }
    }

    public void SetFirstRate()
    {
        if (turn < -1 || turn == 4)
        {
            return;
        }
        int top = 0;
        int temp = 0;
        for(int i = 0; i < 5; i++)
        {
            /* if (playerHands[i][turn + 1] != 0)
             {
                 if(1<=temp && temp <= 4)
                 {
                     if(1 <= playerHands[i][turn + 1] && playerHands[i][turn + 1] <= 4)
                     {
                         temp = temp > playerHands[i][turn + 1] ? playerHands[i][turn + 1] : temp;
                     }
                 }
             }*/
            if (playerHands[i][turn + 2] != 0)
            {
                if (playerHands[i][turn + 2] == 1)
                {
                    top = i;
                    break;
                }
                if (i == 0)
                {
                    top = 0;
                    temp = playerHands[i][turn + 2];
                }
                else
                {
                    if (1 <= playerHands[i][turn + 2] && playerHands[i][turn + 2] <= 4)
                    {
                        temp = temp > playerHands[i][turn + 2] ? playerHands[i][turn + 2] : temp;
                        top = i;
                    }
                    else
                    {
                        int a = (temp - 1) / 4;
                        int b = (playerHands[i][turn + 2] - 1) / 4;
                        if (a < b)
                        {
                            temp = playerHands[i][turn + 2];
                            top = i;
                        }
                        else if (a == b)
                        {
                            temp = temp > playerHands[i][turn + 2] ? playerHands[i][turn + 2] : temp;
                            top = i;
                        }
                    }
                }
            }
        }

        rate = top;
        bM.CanPush(rate);
    }

    public void NextRate()
    {
        int count = rate + 1;
        if (count >= 5)
        {
            count = 0;
        }
        while (!playerReady[count])
        {
            count++;
            if(count >= 5)
            {
                count = 0;
            }
        }
        rate = count;
        bM.CanPush(rate);
    }

    [PunRPC]
    public void SerializeHand(int[][] playerHands_)
    {
        turn++;
        canHalf = true;
        playerHands = playerHands_;

        if (!started && turn == 0)
        {
            SelectHand();
        }

        if (started)
        {
            SetCardImage();
        }
        photonView.RPC("RPCGene", RpcTarget.MasterClient, actorNumber);
        
    }

    public void EndGame()
    {
        for (int i = 0; i < genealogy.Length; i++)
        {
            if (genealogy[i] > winGene)
            {
                winGene = genealogy[i];
                winPlayer = i;
            }
            else if (genealogy[i] == winGene)
            {
                if (playerTopNum[i] == playerTopNum[winPlayer])
                {
                    if (playerTopMark[i] < playerTopMark[winPlayer])
                    {
                        winGene = genealogy[i];
                        winPlayer = i;
                    }
                }
                else if (playerTopNum[i] == 0)
                {
                    winGene = genealogy[i];
                    winPlayer = i;
                }
                else if (playerTopNum[winPlayer] == 0)
                {

                }
                else if (playerTopNum[i] > playerTopNum[winPlayer])
                {
                    winGene = genealogy[i];
                    winPlayer = i;
                }
            }
        }
        GetMarkNumberText(winPlayer);
        photonView.RPC("ResultGame", RpcTarget.All, winPlayer, winGene);
        
    }

    public void DieEndGame()
    {
        for (int i = 0; i < playerReady.Length; i++)
        {
            if (playerReady[i])
            {
                GetMarkNumberText(i);
                photonView.RPC("DieGame", RpcTarget.All, i);
                
            }
        }
        

    }

    public void WinnerHandSetSprite(int winner)
    {
        

        winGeneText.text = winnerGene;
        for(int i = 0; i < winnerCards.Length; i++)
        {
            winnerCards[i].sprite = cardSprites[playerHands[winner][i]];
        }
        winnerText.text = (winner+1) + " 플레이어가 이겼습니다.";
    }

    [PunRPC]
    public void DieGame(int player)
    {
        winGene = genealogy[player];
        winPlayer = player;
        winnerPanel.SetActive(true);
        readyPanel.SetActive(true);
        WinnerHandSetSprite(winPlayer);
        if (PhotonNetwork.IsMasterClient)
        {
            SetReady();
            photonView.RPC("ResultGame", RpcTarget.MasterClient, winPlayer);

        }
    }

    [PunRPC]
    public void ResultGame(int winPlayer_, int winGene_)
    {
        winPlayer = winPlayer_;
        winGene = winGene_;
        winnerPanel.SetActive(true);
        readyPanel.SetActive(true);
        WinnerHandSetSprite(winPlayer);
        if (PhotonNetwork.IsMasterClient)
        {
            SetReady();
            photonView.RPC("ResultGame", RpcTarget.MasterClient, winPlayer);

        }
    }

    public void OffWinnerPanel()
    {
        winnerPanel.SetActive(false);
        startBtn.gameObject.SetActive(true);
    }

    [PunRPC]
    public void ResultGame(int player)
    {
        haveMoney[player] += pot;
        pot = 0;
        ante = defaultAnte;
        started = false;
        photonView.RPC("ResultGameSerialize", RpcTarget.All, haveMoney, pot, ante, started);
    }

    [PunRPC]
    public void ResultGameSerialize(int[] haveMoney_, int pot_, int ante_, bool started_)
    {
        haveMoney = haveMoney_;
        pot = pot_;
        ante = ante_;
        started = started_;
    }

    /*[PunRPC]
    public void RPCGene()
    {
        photonView.RPC("RPCGene", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber - 1);
    }*/

    [PunRPC]
    public void RPCGene(int i)
    {
        genealogy[i] = Genealogy(playerHands[i], i);

        photonView.RPC("SerializeGene", RpcTarget.Others, genealogy);
    }

    [PunRPC]
    public void SerializeGene(int[] genealogy_)
    {
        genealogy = genealogy_;
        
    }

    public bool getStarted()
    {
        return started;
    }

    public int getTurn()
    {
        return turn;
    }

    public void PushButton(int btn, int player)
    {
        if (player < actorNumber)
        {
            betImages[player + 1].sprite = betSprites[btn];
            audioSource.clip = betAudios[btn];
            audioSource.Play();
        }
        else if (player == actorNumber)
        {
            betImages[0].sprite = betSprites[btn];
            audioSource.clip = betAudios[btn];
            audioSource.Play();
        }
        else
        {
            betImages[player].sprite = betSprites[btn];
            audioSource.clip = betAudios[btn];
            audioSource.Play();
        }
    }


    [PunRPC]
    public void OnConnected(int player, string name, int image)
    {

        if (PhotonNetwork.IsMasterClient)
        {
            for(int i =  1; i <= 5; i++)
            {
                if((int)PhotonNetwork.CurrentRoom.CustomProperties[i+""] == -1)
                {
                    PhotonNetwork.CurrentRoom.CustomProperties[i + ""] = player;
                    switch (i)
                    {
                        case 1:
                            player1ActorNumber = player;
                            player1Name = name;
                            player1Image = image;
                            break;
                        case 2:
                            player2ActorNumber = player;
                            player2Name = name;
                            player2Image = image;
                            break;
                        case 3:
                            player3ActorNumber = player;
                            player3Name = name;
                            player3Image = image;
                            break;
                        case 4:
                            player4ActorNumber = player;
                            player4Name = name;
                            player4Image = image;
                            break;
                        case 5:
                            player5ActorNumber = player;
                            player5Name = name;
                            player5Image = image;
                            break;
                    }
                    break;
                }
            }
        }    
        else
            photonView.RPC("OnConnected", RpcTarget.MasterClient, player, name, image);
    }

    /*[PunRPC]
    public void RPCOnConnected(int player)
    {
        
            switch (player)
            {
                case 1:
                    player1Connect = true;
                    break;
                case 2:
                    player2Connect = true;
                    break;
                case 3:
                    player3Connect = true;
                    break;
                case 4:
                    player4Connect = true;
                    break;
                case 5:
                    player5Connect = true;
                    break;
            }

    }*/

    [PunRPC]
    public void OnDisConnected(int player)
    {

        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 1; i <= 5; i++)
            {
                if ((int)PhotonNetwork.CurrentRoom.CustomProperties[i + ""] == player)
                {
                    PhotonNetwork.CurrentRoom.CustomProperties[i + ""] =-1;
                    switch (i)
                    {
                        case 1:
                            player1ActorNumber = -1;
                            playerReady[i] = false;
                            player1Name = null;
                            player1Image = 0;
                            break;
                        case 2:
                            player2ActorNumber = -1;
                            playerReady[i] = false;
                            player2Name = null;
                            player2Image = 0;
                            break;
                        case 3:
                            player3ActorNumber = -1;
                            playerReady[i] = false;
                            player3Name = null;
                            player3Image = 0;
                            break;
                        case 4:
                            player4ActorNumber = -1;
                            playerReady[i] = false;
                            player4Name = null;
                            player4Image = 0;
                            break;
                        case 5:
                            player5ActorNumber = -1;
                            playerReady[i] = false;
                            player5Name = null;
                            player5Image = 0;
                            break;
                    }
                    player = i;
                    break;
                }
            }
        }
        else
            photonView.RPC("OnDisConnected", RpcTarget.MasterClient, player);

        playerHands[player-1] = new int[7];
        playerTopMark[player-1] = 0;
        playerTopNum[player-1] = 0;
        genealogy[player-1] = 0;
        haveMoney[player-1] = 0;
        playerReady[player-1] = false;
        photonView.RPC("SerializeHand", RpcTarget.OthersBuffered, playerHands);
        photonView.RPC("SerializeGene", RpcTarget.OthersBuffered, genealogy);
        photonView.RPC("SerializeReady", RpcTarget.OthersBuffered, playerReady);
    }

    /*[PunRPC]
    public void RPCOnDisConnected(int player)
    {
        playerHands[player-1] = new int[7];
        playerTopMark[player-1] = 0;
        playerTopNum[player-1] = 0;
        genealogy[player-1] = 0;
        haveMoney[player-1] = 0;
        playerReady[player-1] = false;
        photonView.RPC("SerializeHand", RpcTarget.OthersBuffered, playerHands);
        photonView.RPC("SerializeGene", RpcTarget.OthersBuffered, genealogy);
        photonView.RPC("SerializeReady", RpcTarget.OthersBuffered, playerReady);
        switch (player)
        {
            case 1:
                player1Connect = false;
                break;
            case 2:
                player2Connect = false;
                break;
            case 3:
                player3Connect = false;
                break;
            case 4:
                player4Connect = false;
                break;
            case 5:
                player5Connect = false;
                break;
        }

    }*/

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            stream.SendNext(rate);
            stream.SendNext(started);
            stream.SendNext(haveMoney);
            stream.SendNext(pot);
            stream.SendNext(ante);
            stream.SendNext(setMoneyInt);
            stream.SendNext(player1ActorNumber);
            stream.SendNext(player2ActorNumber);
            stream.SendNext(player3ActorNumber);
            stream.SendNext(player4ActorNumber);
            stream.SendNext(player5ActorNumber);
            stream.SendNext(player1Name);
            stream.SendNext(player2Name);
            stream.SendNext(player3Name);
            stream.SendNext(player4Name);
            stream.SendNext(player5Name);
            stream.SendNext(player1Image);
            stream.SendNext(player2Image);
            stream.SendNext(player3Image);
            stream.SendNext(player4Image);
            stream.SendNext(player5Image);
            stream.SendNext(IsCheck);
            stream.SendNext(IsPPing);
            stream.SendNext(canCheck);
        }
        else
        {
            rate = (int)stream.ReceiveNext();
            started = (bool)stream.ReceiveNext();
            haveMoney = (int[])stream.ReceiveNext();
            pot = (int)stream.ReceiveNext();
            ante = (int)stream.ReceiveNext();
            setMoneyInt = (int)stream.ReceiveNext();
            player1ActorNumber = (int)stream.ReceiveNext();
            player2ActorNumber = (int)stream.ReceiveNext();
            player3ActorNumber = (int)stream.ReceiveNext();
            player4ActorNumber = (int)stream.ReceiveNext();
            player5ActorNumber = (int)stream.ReceiveNext();
            player1Name = stream.ReceiveNext().ToString();
            player2Name = stream.ReceiveNext().ToString();
            player3Name = stream.ReceiveNext().ToString();
            player4Name = stream.ReceiveNext().ToString();
            player5Name = stream.ReceiveNext().ToString();
            player1Image = (int)stream.ReceiveNext();
            player2Image = (int)stream.ReceiveNext();
            player3Image = (int)stream.ReceiveNext();
            player4Image = (int)stream.ReceiveNext();
            player5Image = (int)stream.ReceiveNext();
            IsCheck = (bool)stream.ReceiveNext();
            IsPPing = (bool)stream.ReceiveNext();
            canCheck = (bool)stream.ReceiveNext();
        }
    }

    public int Genealogy(int[] number, int userNumber)
    {
        if (isRoyalStraightFlush(number, userNumber))
        {
            return 11;
        }
        else if (isStraightFlush(number, userNumber))
        {
            return 10;
        }
        else if (isFourCard(number, userNumber))
        {
            return 9;
        }
        else if (isFullHouse(number, userNumber))
        {
            return 8;
        }
        else if (isFlush(number, userNumber))
        {
            return 7;
        }
        else if (isMountain(number, userNumber))
        {
            return 6;
        }
        else if (isBackStraight(number, userNumber))
        {
            return 5;
        }
        else if (isStraight(number, userNumber))
        {
            return 4;
        }
        else if (isTriple(number, userNumber))
        {
            return 3;
        }
        else if (isTwoPair(number, userNumber))
        {
            return 2;
        }
        else if (isOnePair(number, userNumber))
        {
            return 1;
        }
        else if (isTop(number, userNumber))
        {
            return 0;
        }
        return -1;
    }

	public bool isRoyalStraightFlush(int[] number, int user)
	{
		int[] tempHand = new int[7];
        if (isFlush(number, user))
        {
			for(int i = 0; i < turn + 2; i++)
            {
				if(number[i]!=0 && number[i]%4 == playerTopMark[user]+1)
                {
					tempHand[i] = number[i];
                }
            }
            if (isMountain(tempHand, user))
            {
				return true;
            }
        }
		
		return false;
	}

	public bool isStraightFlush(int[] number,  int user)
	{
		int[] tempHand = new int[7];
		if (isFlush(number, user))
		{
			for (int i = 0; i < turn + 2; i++)
			{
				if (number[i] != 0 && number[i] % 4 == playerTopMark[user] + 1)
				{
					tempHand[i] = number[i];
				}
			}
			if (isBackStraight(tempHand, user)||isStraight(tempHand,user))
			{
				return true;
			}
		}
		return false;
	}

	public bool isFourCard(int[] number, int user)
	{
		List<int> tempHand = new List<int>();
		for (int i = 0; i < turn + 2; i++)
		{
            if(number[i]!=0)
			    tempHand.Add((number[i] - 1) / 4);
		}

		for (int i = 12; i >= 0; i--)
		{

			if(tempHand.FindAll(x => x.Equals(i)).Count == 4)
            {
                playerTopNum[user] = i;
				playerTopMark[user] = 0;
				return true;
            }
			
		}

		return false;
	}

	public bool isFullHouse(int[] number,  int user)
	{
		if (isOnePair(number, user))
		{
            if (isTriple(number, user)) {
                return true;
            }
		}
		return false;
	}

	public bool isFlush(int[] number, int user)
	{
		int spade = 0;
		int heart = 0;
		int clover = 0;
		int diamond = 0;
		for(int i = 0; i < turn + 2; i++)
        {
			if(number[i] % 4 == 1)
            {
				spade++;
            }
			else if(number[i] % 4 == 2)

			{
				diamond++;
			}
			else if (number[i] % 4 == 3)
			{
				heart++;
			}
			else if (number[i]!=0&&number[i] % 4 == 0)
			{
				clover++;
			}
		}
		if (spade > 4)
		{
            for (int i = 0; i < turn + 2; i++)
            {
                if (number[i] % 4 == 1)
                {
                    int temp = (number[i] - 1) / 4;
                    if (temp == 0)
                        playerTopNum[user] = temp;
                    else if (playerTopNum[user] < temp)
                        playerTopNum[user] = temp;
                }
            }
            playerTopMark[user] = 0;
			return true;
		}
		else if(diamond > 4)
		{
            for (int i = 0; i < turn + 2; i++)
            {
                if (number[i] % 4 == 2)
                {
                    int temp = (number[i] - 1) / 4;
                    if (temp == 0)
                        playerTopNum[user] = temp;
                    else if (playerTopNum[user] < temp)
                        playerTopNum[user] = temp;
                }
            }
            playerTopMark[user] = 1;
			return true;
		}
		else if (heart > 4)
		{
            for (int i = 0; i < turn + 2; i++)
            {
                if (number[i] % 4 == 3)
                {
                    int temp = (number[i] - 1) / 4;
                    if (temp == 0)
                        playerTopNum[user] = temp;
                    else if (playerTopNum[user] < temp)
                        playerTopNum[user] = temp;
                }
            }
            playerTopMark[user] = 2;
			return true;
		}
		else if (clover > 4)
		{
            for (int i = 0; i < turn + 2; i++)
            {
                if (number[i] != 0 && number[i] % 4 == 0)
                {
                    int temp = (number[i] - 1) / 4;
                    if (temp == 0)
                        playerTopNum[user] = temp;
                    else if (playerTopNum[user] < temp)
                        playerTopNum[user] = temp;
                }
            }
            playerTopMark[user] = 3;
			return true;
		}
		else
			return false;
	}

	public bool isMountain(int[] number, int user)
	{
		int tempMark = 5;
		bool ace = false;
		bool ten = false;
		bool jack = false;
		bool queen = false;
		bool king = false;
		for(int i =0; i < turn + 2; i++)
        {
			if (1 <= number[i] && number[i] <= 4)
			{
				ace = true;
				if (tempMark > number[i] - 1)
					tempMark = number[i] - 1;
			}
			else if (37 <= number[i] && number[i] <= 40)
				ten = true;
			else if (41 <= number[i] && number[i] <= 44)
				jack = true;
			else if (45 <= number[i] && number[i] <= 48)
				queen = true;
			else if (49 <= number[i] && number[i] <= 52)
				king = true;
		}
		if (ace && ten && jack && queen && king)
		{
            playerTopNum[user] = 0;
			playerTopMark[user] = tempMark;
			return true;

		}
		return false;
	}

	public bool isBackStraight(int[] number, int user)
	{
		int tempMark = 5;
		bool ace = false;
		bool two = false;
		bool three = false;
		bool four = false;
		bool five = false;
		for (int i = 0; i < turn + 2; i++)
		{
			if (1 <= number[i] && number[i] <= 4)
			{
				ace = true;
				if (tempMark > number[i] - 1)
					tempMark = number[i] - 1;
			}
			else if (5 <= number[i] && number[i] <= 8)
				two = true;
			else if (9 <= number[i] && number[i] <= 12)
				three = true;
			else if (13 <= number[i] && number[i] <= 16)
				four = true;
			else if (17 <= number[i] && number[i] <= 20)
				five = true;
		}
		if (ace && two && three && four && five)
		{
            playerTopNum[user] = 0;
            playerTopMark[user] = tempMark;
			return true;

		}
		return false;
	}

	public bool isStraight(int[] number,int user)
	{
		List<int> tempHand = new List<int>();
		int num = 5;
		for(int i = 0; i < number.Length; i++)
        {
            if(number[i]!=0)
			    tempHand.Add((number[i]-1)/4);
        }
		for (int i = 12; i > 4; i--)
		{
			if (tempHand.FindAll(x => x.Equals(i)).Count > 0 && tempHand.FindAll(x => x.Equals(i-1)).Count > 0 && tempHand.FindAll(x => x.Equals(i-2)).Count > 0 &&
                tempHand.FindAll(x => x.Equals(i-3)).Count > 0 && tempHand.FindAll(x => x.Equals(i-4)).Count > 0)
			{
				for (int j = 0; j < tempHand.Count; j++)
				{
					int k = tempHand.FindIndex(j, x => x.Equals(i));
					if (k >= 0 && num < number[k])
						num = number[k];
				}
                playerTopNum[user] = i;
                playerTopMark[user] = (num - 1) % 4;
				return true;
			}
		}

		return false;
	}

	public bool isTriple(int[] number, int user)
	{
		List<int> tempHand = new List<int>();
		for (int i = 0; i < turn + 2; i++)
		{
            if (number[i] != 0)
                tempHand.Add((number[i] - 1) / 4);
		}

		for (int i = 12; i >= 0; i--)
		{

			if (tempHand.FindAll(x => x.Equals(i)).Count == 3)
			{
                playerTopNum[user] = i;
                int tempMark = 4;
				for(int j = 0; j < tempHand.Count;j++)
                {
					if (tempHand[j] == i)
					{
						int mark = (playerHands[user][j] - 1) % 4;
						if (mark == 0)
						{
							playerTopMark[user] = mark;
							return true;
						}
						else if (mark < tempMark)
						{
							tempMark = mark;
						}
					}
                }
				playerTopMark[user] = tempMark;
				return true;
			}

		}

		return false;
	}

	public bool isTwoPair(int[] number, int user)
	{
		List<int> tempHand = new List<int>();
		for (int i = 0; i < turn + 2; i++)
		{
            if (number[i] != 0)
                tempHand.Add((number[i] - 1) / 4);
		}

		for (int i = 0; i <= 11; i++)
		{

			if (tempHand.FindAll(x => x.Equals(i)).Count == 2)
			{
				for (int j = 12; j > i; j--)
				{
					if (tempHand.FindAll(x => x.Equals(j)).Count == 2)
					{
                        int tempMark = 4;
                        if (i == 0)
                        {
                            playerTopNum[user] = 0;
                            for (int k = 0; k < tempHand.Count; k++)
                            {
                                if (tempHand[k] == i)
                                {
                                    int mark = (playerHands[user][k] - 1) % 4;
                                    if (mark == 0)
                                    {
                                        playerTopMark[user] = mark;
                                        return true;
                                    }
                                    else if (mark < tempMark)
                                    {
                                        tempMark = mark;
                                    }
                                }
                            }
                        }
                        else
                        {
                            playerTopNum[user] = j;
                            for (int k = 0; k < tempHand.Count; k++)
                            {
                                if (tempHand[k] == j)
                                {
                                    int mark = (playerHands[user][k] - 1) % 4;
                                    if (mark == 0)
                                    {
                                        playerTopMark[user] = mark;
                                        return true;
                                    }
                                    else if (mark < tempMark)
                                    {
                                        tempMark = mark;
                                    }
                                }
                            }
                        }
						playerTopMark[user] = tempMark;
						return true;
					}
				}
			}

		}

		return false;
	}

	public bool isOnePair(int[] number, int user)
	{
		List<int> tempHand = new List<int>();
		for (int i = 0; i < turn+2; i++)
		{
            if (number[i] != 0)
                tempHand.Add((number[i] - 1) / 4);
		}
        if (tempHand.FindAll(x => x.Equals(0)).Count == 2)
        {
            playerTopNum[user] = 0;
            int tempMark = 4;
            for (int j = 0; j < tempHand.Count; j++)
            {
                if (tempHand[j] == 0)
                {
                    int mark = (playerHands[user][j] - 1) % 4;
                    if (mark == 0)
                    {
                        playerTopMark[user] = mark;
                        return true;
                    }
                    else if (mark < tempMark)
                    {
                        tempMark = mark;
                    }
                }
            }
            playerTopMark[user] = tempMark;
            return true;


        }
        for (int i = 12; i >= 1; i--)
		{
			if (tempHand.FindAll(x => x.Equals(i)).Count == 2)
			{
                playerTopNum[user] = i;
                int tempMark = 4;
						for (int j = 0; j < tempHand.Count;j++)
						{
							if (tempHand[j] == i)
							{
								int mark = (playerHands[user][j] - 1) % 4;
								if (mark == 0)
								{
									playerTopMark[user] = mark;
									return true;
								}
								else if (mark < tempMark)
								{
									tempMark = mark;
								}
							}
						}
						playerTopMark[user] = tempMark;
						return true;
					
				
			}

		}

		return false;
	}

	public bool isTop(int[] number, int user)
	{
		List<int> tempHand = new List<int>();
		for (int i = 0; i < number.Length; i++)
		{
            if (number[i] != 0)
                tempHand.Add((number[i] - 1) / 4);
		}
        if (tempHand.Contains(0))
        {
            playerTopNum[user] = 0;
            playerTopMark[user] = (number[tempHand.IndexOf(0)] - 1) % 4;
            return true;
        }
        else
        {
            for(int i = 12; i > 0; i--)
            {
                if (tempHand.Contains(i))
                {
                    playerTopNum[user] = i;
                    playerTopMark[user] = (number[tempHand.IndexOf(i)] - 1) % 4;
                    return true;
                }
            }
        }

		return false;

	}

	/*public void CheckCard()
    {
        for (int i = 0; i < playerHand.Length; i++)
        {
            Debug.Log(playerHand[i][0]);
            Debug.Log(playerHand[i][1]);
            Debug.Log(playerHand[i][2]);
        }
    }*/
}
