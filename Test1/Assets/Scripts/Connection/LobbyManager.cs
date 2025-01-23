using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using UnityEngine.UIElements;
//using static UnityEditor.Progress;
using static UnityEngine.EventSystems.PointerEventData;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

//¹æ »ı¼º ¹× ¹æ Âü¿©¿¡ °üÇÑ ÄÚµå
public class LobbyManager : MonoBehaviourPunCallbacks
{

    // ¹æ »ı¼º °ü·Ã UI
    [SerializeField] InputField input_RoomName; //¹æ ÀÌ¸§
    [SerializeField] Button[] btn_MaxPlayers, btn_Difficulty, btn_TimeLimit; // ÃÖ´ëÀÎ¿ø, ³­ÀÌµµ, Á¦ÇÑ½Ã°£ ¹öÆ°

    // ¹æ »ı¼º ½Ã ÀÌ¸§ ±ÔÄ¢ °æ°í¸Ş½ÃÁö
    [SerializeField] Text warningText;

    // ¹æ »ı¼º ¹öÆ°°ú ¹æ Âü¿© ¹öÆ°, ¹æ ¸ñ·ÏÀ» Ç¥½ÃÇÒ ½ºÅ©·Ñºä
    [SerializeField] Button btn_CreateRoom, btn_JoinRoom; // ¹æ ¸¸µé±â, Âü¿© ¹öÆ°
    [SerializeField] GameObject roomListItem; // ¹æ ¸ñ·Ï ÇÁ¸®ÆÕ

    // ¹æ »ı¼º ½Ã ¿É¼Çµé(¼±ÅÃÇÏÁö ¾Ê¾Æµµ ±âº»À¸·Î ¼³Á¤)
    private int selectedMaxPlayers = 2; // ÃÖ´ëÀÎ¿ø(2, 3, 4) +5¸í±îÁöµµ Ãß°¡ÇØ¾ßÇÔ!!
    private string selectedDifficulty = "ÃÊ±Ş"; // ³­ÀÌµµ(ÃÊ±Ş, Áß±Ş, °í±Ş)
    private int selectedTimeLimit = 15; // Ä«µå ³õ±â±îÁö Á¦ÇÑ½Ã°£(15ÃÊ, 30ÃÊ, 45ÃÊ)

    // ¹æ »ı¼º ¿É¼Ç-ÀÎ¿øÀ» Á¦¿ÜÇÑ Ç×¸ñÀÇ ÀÎµ¦½º
    private int selectedDifficultyIndex = 0; // ³­ÀÌµµ ¼±ÅÃ ÀÎµ¦½º
    private int selectedTimeLimitIndex = 0; // Á¦ÇÑ½Ã°£ ÀÎµ¦½º

    //»ı¼ºµÈ ¹æ¸ñ·ÏÀ» º¸¿©ÁÙ ½ºÅ©·Ñºä ÄÜÅÙÃ÷ ¿µ¿ª
    public Transform rtContent;
    // ¹æÀÌ¸§ ÃÖ´ë ÀÔ·Â ±æÀÌ
    private const int MaxLength = 12; 
    // ¹æ ¸ñ·ÏÀ» °¡Áö°í ÀÖ´Â Dictionary º¯¼ö
    Dictionary<string, RoomInfo> dicRoomInfo = new Dictionary<string, RoomInfo>();


    private void Awake() 
    {
        ResetRoomSetPanel(); // Ã¹ ¸ŞÀÎ Á¢¼Ó ½Ã ÃÖÃÊ ½ÇÇà
    }

    // ¹æ ¸¸µé ¶§ ¼±ÅÃ ¿É¼Ç ¹öÆ°°ú ¹æÀÌ¸§ ±ÔÄ¢¿¡ °üÇÑ ÃÊ±âÈ­
    public void ResetRoomSetPanel()
    {
        // ±âº» ¹öÆ° ¼³Á¤°ª (0,0,0) ³ë¶õ»öÀ¸·Î Ç¥½Ã
        SetDefaultSelection(btn_MaxPlayers, 0);
        SetDefaultSelection(btn_Difficulty, 0);
        SetDefaultSelection(btn_TimeLimit, 0);

        // ¿É¼Ç ¹öÆ° ¼±ÅÃÇÏ¸é ³ë¶õ»öÀ¸·Î ¹Ù²îµµ·Ï ÇÏ´Â ÄÚµå
        MaxPlayerSet(btn_MaxPlayers);
        DifficultySet(btn_Difficulty);
        TimeLimitSet(btn_TimeLimit);

        // ¹æ ÀÌ¸§ ÀÔ·Â ÇÊµå ÃÊ±âÈ­
        input_RoomName.text = ""; //¹æ ÀÌ¸§ ±âº» °ø¹é »óÅÂ
        warningText.text = ""; // ÃÊ±â °æ°í ¸Ş½ÃÁö ºñ¿ì±â
        btn_CreateRoom.interactable = false; // Ã³À½¿¡´Â ¹æ »ı¼º ¹öÆ° ºñÈ°¼ºÈ­
        input_RoomName.onValueChanged.AddListener(ValidateRoomName); //¹æ ÀÌ¸§ ÀÛ¼ºÇÒ ½Ã, ¹æ ÀÌ¸§ ±ÔÄ¢ °Ë»ç
    }

    private void MaxPlayerSet(Button[] buttons)
    {
        // MaxPlayers ¹öÆ°¿¡ ¸®½º³Ê Ãß°¡
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnMaxPlayersButtonClicked(index, buttons)); //¼±ÅÃÇÑ ¹öÆ°ÀÇ »ö»ó º¯°æ
        }
    }

    public void DifficultySet(Button[] buttons)
    {
        // Difficulty ¹öÆ°¿¡ ¸®½º³Ê Ãß°¡
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnDifficultyButtonClicked(index, buttons));
        }
    }

    public void TimeLimitSet(Button[] buttons)
    {
        // TimeLimit ¹öÆ°¿¡ ¸®½º³Ê Ãß°¡
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnTimeLimitButtonClicked(index, buttons));
        }
    }

    private void ValidateRoomName(string input) //¹æ ÀÌ¸§ÀÇ ±ÔÄ¢¿¡ °üÇÑ ÄÚµå
    {

        // ÇÑ±Û(¿Ï¼ºÇü/ÀÚÀ½/¸ğÀ½)°ú ¼ıÀÚ¸¸ Çã¿ëÇÏ´Â Á¤±Ô½Ä
        string validPattern = @"^[°¡-ÆR¤¡-¤¾¤¿-¤Ó0-9\s]*$";

        // ÀÔ·Â °ªÀÌ ÆĞÅÏ¿¡ ¸ÂÁö ¾ÊÀ¸¸é ¼öÁ¤
        if (!Regex.IsMatch(input, validPattern))
        {
            warningText.text = "ÇÑ±Û, ¼ıÀÚ, °ø¹é¸¸ ÀÔ·Â °¡´ÉÇÕ´Ï´Ù.";
            btn_CreateRoom.interactable = false; // ¹æ »ı¼º ¹öÆ° ºñÈ°¼ºÈ­
        }
        else if (input.Length > MaxLength) // ±æÀÌ Á¦ÇÑ ÃÊ°ú °Ë»ç
        {
            warningText.text = $"ÃÖ´ë {MaxLength}ÀÚ±îÁö¸¸ ÀÔ·Â °¡´ÉÇÕ´Ï´Ù.";
            btn_CreateRoom.interactable = false; // ¹æ »ı¼º ¹öÆ° ºñÈ°¼ºÈ­
        }
        else if (input.Length == 0) // ºó ¹®ÀÚ¿­ °Ë»ç
        {
            warningText.text = "¹æ ÀÌ¸§À» ÀÔ·ÂÇØÁÖ¼¼¿ä.";
            btn_CreateRoom.interactable = false; // ¹æ »ı¼º ¹öÆ° ºñÈ°¼ºÈ­
        }
        else
        {
            warningText.text = ""; // ±ÔÄ¢¿¡ ¸ÂÀ¸¸é °æ°í ¸Ş½ÃÁö Á¦°Å
            btn_CreateRoom.interactable = true; // ¹æ »ı¼º ¹öÆ° È°¼ºÈ­
        }
    }


    //¹æ ¸ñ·ÏÀÇ º¯È­°¡ ÀÖÀ» ¶§ È£ÃâµÇ´Â ÇÔ¼ö(Æ÷Åæ ±âº» Á¦°ø)
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);  // ±âº» ¸Ş¼­µå È£Ãâ

        //Content¿¡ ÀÚ½ÄÀ¸·Î ºÙ¾îÀÖ´Â ItemÀ» ´Ù »èÁ¦
        DeleteRoomListItem();

        //dicRoomInfo º¯¼ö¸¦ roomList¸¦ ÀÌ¿ëÇØ¼­ °»½Å
        UpdateRoomListItem(roomList);

        //dicRoomÀ» ±â¹İÀ¸·Î roomListItemÀ» ¸¸µéÀÚ
        CreateRoomListItem();

    }

    // ¹æ ¸ñ·ÏÀ» ¼±ÅÃÇßÀ» ¶§
    void SelectRoomItem(string roomName, GameObject button)
    {
        input_RoomName.text = roomName;
        // ÀÌÀü¿¡ ¼±ÅÃµÈ ¹öÆ°ÀÇ »ö»óÀ» ÃÊ±âÈ­
        if (roomListItem != null)
        {
            var prevImage = roomListItem.GetComponent<Image>();
            if (prevImage != null)
            {
                prevImage.color = Color.white; // ±âº» »ö»óÀ¸·Î º¹¿ø
            }
        }

        // ÇöÀç ¼±ÅÃµÈ ¹öÆ°ÀÇ »ö»óÀ» ³ë¶õ»öÀ¸·Î º¯°æ
        roomListItem = button;
        var currentImage = roomListItem.GetComponent<Image>();
        if (currentImage != null)
        {
            currentImage.color = Color.yellow; // ³ë¶õ»öÀ¸·Î ¼³Á¤
        }

        // ¹æ ÀÌ¸§À» ÀÔ·Â ÇÊµå¿¡ ¼³Á¤
        input_RoomName.text = roomName;
    }



    



    // ¹æ ¿É¼Ç ¼±ÅÃ ½Ã ÀÌ·ïÁö´Â ui¿Í index ¾÷µ¥ÀÌÆ®¿¡ °üÇÑ ÄÚµå
    void OnMaxPlayersButtonClicked(int index, Button[] PlayBtn)
    {
        switch (index) // 2, 3, 4 ÇÃ·¹ÀÌ¾î ¿É¼Ç
        {
            case 0:
                selectedMaxPlayers = 2;
                break;
            case 1:
                selectedMaxPlayers = 3;
                break;
            case 2:
                selectedMaxPlayers = 4;
                break; 
        }
        UpdateButtonColors(PlayBtn, index);
        UnityEngine.Debug.Log("Selected Max Players: " + selectedMaxPlayers); //Å¬¸¯ÇÒ ¶§¸¶´Ù ¸Ş½ÃÁö Ãâ·Â
    }

    public void OnDifficultyButtonClicked(int index, Button[] difBtn)
    {
        switch (index) // 0: ÃÊ±Ş, 1: Áß±Ş, 2: °í±Ş
        {
            case 0:
                selectedDifficulty = "ÃÊ±Ş";
                break;
            case 1:
                selectedDifficulty = "Áß±Ş";
                break;
            case 2:
                selectedDifficulty = "°í±Ş";
                break;
        }
        selectedDifficultyIndex = index;
        UpdateButtonColors(difBtn, index); //»ö»ó ¾÷µ¥ÀÌÆ®
        UnityEngine.Debug.Log("Selected Difficulty: " + selectedDifficulty); //¸Ş½ÃÁö Ãâ·Â
    }

    public void OnTimeLimitButtonClicked(int index, Button[] TimBtn)
    {
        switch (index) // 0: 15ÃÊ, 1: 30ÃÊ, 2: 45ÃÊ
        {
            case 0:
                selectedTimeLimit = 15;
                break;
            case 1:
                selectedTimeLimit = 30;
                break;
            case 2:
                selectedTimeLimit = 45;
                break;

        }
        selectedTimeLimitIndex = index;
        UpdateButtonColors(TimBtn, index);
        UnityEngine.Debug.Log("Selected Time Limit: " + selectedTimeLimit);
    }

    // ¹æ »ı¼º ÈÄ À¯Àú°¡ ¿É¼Ç º¯°æ ÆĞ³ÎÀ» ¿­¾úÀ» ¶§, ÇöÀç ¹æ ¿É¼Ç »ö»óÀ» Ä¥ÇØ¼­ º¸¿©ÁØ´Ù
    private void SetDefaultSelection(Button[] buttons, int defaultIndex)
    {

        for (int i = 0; i < buttons.Length; i++) //¼¼¹ø ¹İº¹(°¢ ¹öÆ° ¹è¿­ ±æÀÌ)
        {
            int index = i;

            ColorBlock colorBlock = buttons[i].colors;
            colorBlock.normalColor = Color.white; // ±âº» »ö»ó È­ÀÌÆ®
            colorBlock.selectedColor = Color.yellow; //¼±ÅÃµÈ »ö»ó ³ë¶õ»ö
            buttons[i].colors = colorBlock;
        }
        UpdateButtonColors(buttons, defaultIndex);  //±âº»°ª ¹öÆ° »ö»óÀ» ³ë¶õ»öÀ¸·Î
    }

    // ¼±ÅÃµÈ ¹öÆ°À» ½ÇÁ¦·Î »öÄ¥ÇÏ´Â ÇÔ¼ö
    private void UpdateButtonColors(Button[] buttons, int selectedIndex)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            ColorBlock colorBlock = buttons[i].colors; //colorBlock¿¡ »ö»ó Á¤º¸ ³Ñ°ÜÁÖ±â

            if (i == selectedIndex) //ÇöÀç ¼±ÅÃÇÑ ÀÎµ¦½º¿Í i°ªÀÌ °°À»¶§
            {
                colorBlock.normalColor = Color.yellow; //³ë¶õ»ö
            }
            else //ÇöÀç ¼±ÅÃÇÑ ÀÎµ¦½º¿Í i°ªÀÌ ´Ù¸¦¶§
            {
                colorBlock.normalColor = Color.white; //ÇÏ¾á»ö
            }
            buttons[i].colors = colorBlock; //¹öÆ°¿¡ »ö»ó ¾÷µ¥ÀÌÆ®
        }
    }



    void DeleteRoomListItem() // ±âÁ¸ ¹æ Á¤º¸ »èÁ¦ÇÒ ¶§
    {

        foreach (Transform tr in rtContent)
        {
            Destroy(tr.gameObject);
        }
    }


    // ½ºÅ©·Ñ ºä¿¡ º¸¿©Áö´Â ¹æ ¸ñ·ÏÀ» °»½Å ÇÒ ¶§
    void UpdateRoomListItem(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                dicRoomInfo.Remove(info.Name); // ¹æ Á¤º¸ »èÁ¦
            }
            else
            {
                dicRoomInfo[info.Name] = info; // ¹æ Á¤º¸ Ãß°¡ ¶Ç´Â ¾÷µ¥ÀÌÆ®
            }
        }
    }


    // »ı¼ºµÈ ¹æ ¸ñ·ÏÀ» ½ºÅ©·Ñ ºä¿¡ º¸¿©ÁÙ ¶§
    void CreateRoomListItem()
    {
        foreach (RoomInfo info in dicRoomInfo.Values)
        {
            //¹æ Á¤º¸ »ı¼º°ú µ¿½Ã¿¡ ScrollView-> ContentÀÇ ÀÚ½ÄÀ¸·Î ÇÏÀÚ
            GameObject go = Instantiate(roomListItem, rtContent); //ÀÎÀÚ: ÇÁ¸®ÆÕ, ÄÜÅÙÃ÷ ¼ø

            //»ı¼ºµÈ item¿¡¼­ RoomListItem ÄÄÆ÷³ÍÆ®¸¦ °¡Á®¿Â´Ù.
            RoomListItem item = go.GetComponent<RoomListItem>();

            // ³­ÀÌµµ¿Í Á¦ÇÑ ½Ã°£À» Ä¿½ºÅÒ ÇÁ·ÎÆÛÆ¼¿¡¼­ °¡Á®¿È
            string difficulty = info.CustomProperties.ContainsKey("difficulty") ?
                                info.CustomProperties["difficulty"].ToString() :
                                "¾øÀ½";
            int timeLimit = info.CustomProperties.ContainsKey("timeLimit") ?
                            Convert.ToInt32(info.CustomProperties["timeLimit"]) :
                            0;

            // °¡Á®¿Â ÄÄÆ÷³ÍÆ®°¡ °¡Áö°í ÀÖ´Â SetInfo ÇÔ¼ö ½ÇÇà(Ãâ·Â ÇüÅÂ ¼³Á¤)
            item.SetInfo(info.Name, info.PlayerCount, info.MaxPlayers, difficulty, timeLimit);
            //³­ÀÌµµ¿Í Á¦ÇÑ½Ã°£¸¸ custom properties¿¡ ¼±¾ğ, ³ª¸ÓÁö´Â photon¿¡¼­ ±âº»Á¦°ø

            // item Å¬¸¯µÇ¾úÀ» ¶§ È£ÃâµÇ´Â ÇÔ¼ö µî·Ï
            item.onDelegate = (roomName) =>
            {
                // SelectRoomItemÀ» ¹Ù·Î È£Ãâ
                SelectRoomItem(roomName, go); // roomName°ú ÇöÀç ¹öÆ°(GameObject)À» Àü´Ş -> ¼±ÅÃµÈ ¹æ¸ñ·ÏÀÇ »ö»óÀÌ º¯°æµÇµµ·Ï
            };
        }
    }


    // ¹æ »ı¼º ÇÒ ¶§(º¯¼ö°íÁ¤ºÒº¯)
    public void OnClickCreateRoom()
    {
        //¹æ ¿É¼Ç
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)selectedMaxPlayers;

        //Ä¿½ºÅÒ ·ë ÇÁ·ÎÆÛÆ¼ ¼³Á¤(Áß¿ä)
        options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
           //³­ÀÌµµ, Á¦ÇÑ½Ã°£ °¢°¢¿¡ ´ëÇÑ ÀÎµ¦½º¿Í ½ÇÁ¦ ÅØ½ºÆ®·Î ¹İ¿µµÉ °ªµé
            {"DifficultyIndex", selectedDifficultyIndex},  // ³­ÀÌµµ index
            {"difficulty", selectedDifficulty} ,  // ³­ÀÌµµ str°ª(ÃÊ±Ş,Áß±Ş,°í±Ş)
            {"TimeLimitIndex", selectedTimeLimitIndex},  // Á¦ÇÑ½Ã°£ index
            {"timeLimit", selectedTimeLimit}  // Á¦ÇÑ½Ã°£ int°ª(15,30,45)

        };

        //·Îºñ¿¡µµ º¸ÀÌ°Ô ÇÒ °ÍÀÎ°¡?(¸ñ·Ï¿¡)->°Çµå¸®¸é X
        options.CustomRoomPropertiesForLobby = new string[] { "difficulty", "timeLimit" };

        //¹æ ¸ñ·Ï¿¡ º¸ÀÌ°Ô ÇÒ°ÍÀÎ°¡?
        options.IsVisible = true;

        //¹æ »ı¼º
        PhotonNetwork.CreateRoom(input_RoomName.text, options);
    }


    public override void OnCreatedRoom() // ¹æ »ı¼º¿¡ ¼º°øÇßÀ» ¶§
    {
        base.OnCreatedRoom();

        UnityEngine.Debug.Log("¹æ »ı¼º ¼º°ø");
        PhotonNetwork.LoadLevel("MakeRoom");
    }

    public override void OnCreateRoomFailed(short returnCode, string message) //¹æ »ı¼º¿¡ ½ÇÆĞÇßÀ» ¶§
    {
        base.OnCreateRoomFailed(returnCode, message);
        UnityEngine.Debug.Log("¹æ »ı¼º ½ÇÆĞ" + message);
    }

    public void OnClickJoinRoom() // ¹æ ÀÔÀå
    {
        PhotonNetwork.JoinRoom(input_RoomName.text);

    }

    public override void OnJoinedRoom() // ¹æ ÀÔÀå¿¡ ¼º°øÇßÀ» ¶§
    {
        base.OnJoinedRoom();

        UnityEngine.Debug.Log("¹æ ÀÔÀå ¼º°ø");

        PhotonNetwork.LoadLevel("MakeRoom");
    }

    public override void OnJoinRoomFailed(short returnCode, string message) // ¹æ ÀÔÀå¿¡ ½ÇÆĞÇßÀ» ¶§
    {
        base.OnJoinRoomFailed(returnCode, message);
        UnityEngine.Debug.Log("¹æ ÀÔÀå ½ÇÆĞ" + message);
    }
}
