using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "1";

    [SerializeField] private Text connectionText;
    [SerializeField] private Button joinBtn;
    [SerializeField] private TMP_InputField nickName;
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] images;
    [SerializeField] private int imageNum;
    [SerializeField] private bool joinedMaster;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();

        joinedMaster = false;
        connectionText.text = "연결 시도 중...";

    }

    public override void OnConnectedToMaster()
    {
        joinedMaster = true;
        connectionText.text = "연결 완료";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        joinedMaster = false;
        connectionText.text = $"연결 실패 {cause.ToString()}  - 재연결 중...";

        PhotonNetwork.ConnectUsingSettings();
    }

    public void Connect()
    {
        joinedMaster = false;
        if (PhotonNetwork.IsConnected)
        {
            connectionText.text = "랜덤 매칭 중...";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            connectionText.text = "연결 실패 - 재연결 중...";

            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionText.text = "방이 없어 방을 만듭니다.";
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 5;
        ro.CustomRoomProperties = new Hashtable()
            {
                {"1", -1}, {"2", -1},{"3", -1},{"4", -1},{"5", -1}
            };

        PhotonNetwork.CreateRoom(null, ro);

    }

    public override void OnJoinedRoom()
    {
        SetUserSetting(nickName.text, imageNum);
        Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties["name"]);
        connectionText.text = "방에 접속";
        PhotonNetwork.LoadLevel("Main");
    }

    public void SetUserSetting(string name, int image)
    {
        Debug.Log(name);
        PhotonNetwork.LocalPlayer.CustomProperties["name"] = name;
        PhotonNetwork.LocalPlayer.CustomProperties["image"] = image+1;
    }

    public void PrevImage()
    {
        if (imageNum == 0)
        {
            imageNum = images.Length - 1;
        }
        else
        {
            imageNum--;
        }
        image.sprite = images[imageNum];
    }

    public void NextImage()
    {
        if (imageNum == images.Length - 1)
        {
            imageNum = 0;
        }
        else
        {
            imageNum++;
        }
        image.sprite = images[imageNum];
    }

    // Update is called once per frame
    void Update()
    {
        if(nickName.text ==null ||nickName.text == "" || !joinedMaster)
        {
            joinBtn.interactable = false;
        }
        else
        {
            joinBtn.interactable = true;
        }
    }
}
