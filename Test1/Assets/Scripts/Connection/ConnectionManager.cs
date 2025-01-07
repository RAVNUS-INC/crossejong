using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Ã¹ À¯Àú ÇÁ·ÎÇÊ ¼³Á¤ È­¸é¿¡¼­ ÀÛµ¿ÇÏ´Â ÄÚµå(´Ğ³×ÀÓ, À¯ÀúÇÁ·ÎÇÊ»çÁø ¼³Á¤)
public class UserSetManager : MonoBehaviourPunCallbacks
{
    private UserSetManager s_instance;
    public UserSetManager Instance { get { return s_instance; } }

    [SerializeField]
    InputField inputText; //´Ğ³×ÀÓ ÀÔ·Â
    [SerializeField]
    Button confirmButton; //Á¦Ãâ ¹öÆ°
    [SerializeField]
    Text warningText; // °æ°í ¸Ş½ÃÁö¸¦ Ãâ·ÂÇÒ UI ÅØ½ºÆ®

    private const int MaxLength = 5; // ÃÖ´ë ÀÔ·Â ±æÀÌ

    void Start()
    {
        confirmButton.interactable = false; // ±âº»ÀûÀ¸·Î ¹öÆ° ºñÈ°¼ºÈ­
        warningText.text = ""; // ÃÊ±â °æ°í ¸Ş½ÃÁö ºñ¿ì±â

        //³»¿ëÀÌ º¯°æµÇ¾úÀ»¶§
        inputText.onValueChanged.AddListener(ValidateNickname);

        //³»¿ëÀ» Á¦ÃâÇßÀ»¶§
        inputText.onSubmit.AddListener(OnSubmit);

        ////Ä¿¼­°¡ ´Ù¸¥°÷À» ´©¸£¸é
        //inputText.onEndEdit.AddListener(
        //    (string s) =>
        //    {
        //        Debug.Log("OnEndmit" + s);
        //    }
        //);

        confirmButton.onClick.AddListener(OnClickConnect);
    }

    private void ValidateNickname(string input)
    {
        /// ÇÑ±Û(¿Ï¼ºÇü/ÀÚÀ½/¸ğÀ½)°ú ¼ıÀÚ¸¸ Çã¿ëÇÏ´Â Á¤±Ô½Ä
        string validPattern = @"^[°¡-ÆR¤¡-¤¾¤¿-¤Ó0-9]*$";

        // °ø¹é Á¦°Å
        input = input.Replace(" ", "");

        // ÀÔ·Â °ªÀÌ ÆĞÅÏ¿¡ ¸ÂÁö ¾ÊÀ¸¸é ¼öÁ¤
        if (!Regex.IsMatch(input, validPattern))
        {
            warningText.text = "ÇÑ±Û°ú ¼ıÀÚ¸¸ ÀÔ·Â °¡´ÉÇÕ´Ï´Ù.";
            confirmButton.interactable = false; // È®ÀÎ ¹öÆ° ºñÈ°¼ºÈ­
        }
        else if (input.Length > MaxLength) // ±æÀÌ Á¦ÇÑ ÃÊ°ú °Ë»ç
        {
            warningText.text = $"ÃÖ´ë {MaxLength}ÀÚ±îÁö¸¸ ÀÔ·Â °¡´ÉÇÕ´Ï´Ù.";
            confirmButton.interactable = false; // È®ÀÎ ¹öÆ° ºñÈ°¼ºÈ­
        }
        else if (input.Length == 0) // ºó ¹®ÀÚ¿­ °Ë»ç
        {
            warningText.text = "´Ğ³×ÀÓÀ» ÀÔ·ÂÇØÁÖ¼¼¿ä.";
            confirmButton.interactable = false; // È®ÀÎ ¹öÆ° ºñÈ°¼ºÈ­
        }
        else
        {
            warningText.text = ""; // ±ÔÄ¢¿¡ ¸ÂÀ¸¸é °æ°í ¸Ş½ÃÁö Á¦°Å
            confirmButton.interactable = true; // È®ÀÎ ¹öÆ° È°¼ºÈ­
        }

    }

    private void OnDestroy()
    {
        // ÀÌº¥Æ® ÇØÁ¦
        inputText.onValueChanged.RemoveListener(ValidateNickname);
    }

    void OnSubmit(string s) // s´Â ¹®ÀÚ¿­
    {
        Debug.Log("OnSubmit " + s); // ´Ğ³×ÀÓÀ» ÀÔ·ÂÇÏ°í Á¦ÃâÇßÀ½À» ¾Ë¸²
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("¸¶½ºÅÍ ¼­¹ö Á¢¼Ó ¼º°ø");

        //³ªÀÇ ÀÌ¸§À» Æ÷Åæ¿¡ ¼³Á¤
        PhotonNetwork.NickName = inputText.text;

        //·ÎºñÁøÀÔ
        PhotonNetwork.JoinLobby();
    }

    //Lobby ÁøÀÔ¿¡ ¼º°øÇßÀ¸¸é È£ÃâµÇ´Â ÇÔ¼ö
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        //¸ŞÀÎ ¾ÀÀ¸·Î ÀÌµ¿
        PhotonNetwork.LoadLevel("Main");

        print("·Îºñ ÁøÀÔ ¼º°ø");

    }
    public void OnClickConnect()
    {
        // ¸¶½ºÅÍ ¼­¹ö Á¢¼Ó ¿äÃ»
        PhotonNetwork.ConnectUsingSettings();
    }
}
