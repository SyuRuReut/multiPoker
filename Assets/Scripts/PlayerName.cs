using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerName : MonoBehaviour
{
    [SerializeField] private Text playerNameText;
    // Start is called before the first frame update
    void Start()
    {
        playerNameText = GetComponent<Text>();

        if(Login.firebaseUser != null)
        {
            playerNameText.text = $"Hi {Login.firebaseUser.Email}";
        }
        else
        {
            playerNameText.text = "null error";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
