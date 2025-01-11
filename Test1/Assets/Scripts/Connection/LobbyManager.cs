using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.PointerEventData;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

//¹æ »ý¼º ¹× ¹æ Âü¿©¿¡ °üÇÑ ÄÚµå
public class LobbyManager : MonoBehaviourPunCallbacks
{
    // ¹æ »ý¼º °ü·Ã UI
    [SerializeField] InputField input_RoomName; //¹æ ÀÌ¸§
    [SerializeField] Button[] btn_MaxPlayers; // ÃÖ´ëÀÎ¿ø ¹öÆ°
    [SerializeField] Button[] btn_Difficulty; // ³­ÀÌµµ ¹öÆ°
    [SerializeField] Button[] btn_TimeLimit; // Á¦ÇÑ½Ã°£ ¹öÆ°

    // ¹æ »ý¼º ½Ã ÀÌ¸§ ±ÔÄ¢ °æ°í¸Þ½ÃÁö
    [SerializeField] Text warningText;

    // ¹æ »ý¼º ¹öÆ°°ú ¹æ Âü¿© ¹öÆ°, ¹æ ¸ñ·ÏÀ» Ç¥½ÃÇÒ ½ºÅ©·Ñºä
    [SerializeField] Button btn_CreateRoom; // ¹æ ¸¸µé±â ¹öÆ°
    [SerializeField] Button btn_JoinRoom; // ¹æ Âü¿© ¹öÆ°
    [SerializeField] GameObject roomListItem; // ¹æ ¸ñ·Ï ÇÁ¸®ÆÕ

    // ¹æ »ý¼º ½Ã ÇÊ¿äÇÑ º¯¼ö ¼±¾ð
    int selectedMaxPlayers = 0; // ÃÖ´ëÀÎ¿ø(2, 3, 4¸í)
    int selectedDifficulty = 0; // ³­ÀÌµµ(ÃÊ±Þ, Áß±Þ, °í±Þ)
    int selectedTimeLimit = 0; // Ä«µå ³õ±â±îÁö Á¦ÇÑ½Ã°£(15ÃÊ, 30ÃÊ, 45ÃÊ)
    public Transform rtContent;
    private const int MaxLength = 12; // ¹æÀÌ¸§ ÃÖ´ë ÀÔ·Â ±æÀÌ

    // ¹æ ¸ñ·ÏÀ» °¡Áö°í ÀÖ´Â Dictionaly º¯¼ö
    Dictionary<string, RoomInfo> dicRoomInfo = new Dictionary<string, RoomInfo>();

    void Start()
    {
        SetDefaultSelection(btn_MaxPlayers, 0, out selectedMaxPlayers);
        SetDefaultSelection(btn_Difficulty, 0, out selectedDifficulty);
        SetDefaultSelection(btn_TimeLimit, 0, out selectedTimeLimit);

        // ¹æ ÀÌ¸§ ÀÔ·Â ÇÊµå ÃÊ±âÈ­
        input_RoomName.text = ""; //¹æ ÀÌ¸§ ±âº» °ø¹é »óÅÂ
        btn_CreateRoom.interactable = false; // Ã³À½¿¡´Â ¹æ »ý¼º ¹öÆ° ºñÈ°¼ºÈ­
        warningText.text = ""; // ÃÊ±â °æ°í ¸Þ½ÃÁö ºñ¿ì±â

        input_RoomName.onValueChanged.AddListener(ValidateRoomName); //¹æ ÀÌ¸§ ÀÛ¼ºÇÒ ½Ã, ¹æ ÀÌ¸§ ±ÔÄ¢ °Ë»ç
        btn_CreateRoom.onClick.AddListener(OnClickCreateRoom); //¹æ »ý¼º ¹öÆ° Å¬¸¯ ½Ã, ¹æ »ý¼º ¼öÇà
        btn_JoinRoom.onClick.AddListener(OnClickJoinRoom); //¹æ Âü¿© ¹öÆ° Å¬¸¯ ½Ã, ¹æ Âü¿© ¼öÇà

        // MaxPlayers ¹öÆ°¿¡ ¸®½º³Ê Ãß°¡
        for (int i = 0; i < btn_MaxPlayers.Length; i++)
        {
            int index = i; // Å¬·ÎÀú¸¦ À§ÇØ ·ÎÄÃ º¯¼ö »ç¿ë
            btn_MaxPlayers[i].onClick.AddListener(() => OnMaxPlayersButtonClicked(index));
        }

        // Difficulty ¹öÆ°¿¡ ¸®½º³Ê Ãß°¡
        for (int i = 0; i < btn_Difficulty.Length; i++)
        {
            int index = i;
            btn_Difficulty[i].onClick.AddListener(() => OnDifficultyButtonClicked(index));
        }

        // TimeLimit ¹öÆ°¿¡ ¸®½º³Ê Ãß°¡
        for (int i = 0; i < btn_TimeLimit.Length; i++)
        {
            int index = i;
            btn_TimeLimit[i].onClick.AddListener(() => OnTimeLimitButtonClicked(index));
        }

        UpdateButtonColors(btn_MaxPlayers, -1); // ÃÊ±âÈ­
        UpdateButtonColors(btn_Difficulty, -1); // ÃÊ±âÈ­
        UpdateButtonColors(btn_TimeLimit, -1); // ÃÊ±âÈ­
    }


    private void ValidateRoomName(string input) //¹æ ÀÌ¸§ÀÇ ±ÔÄ¢¿¡ °üÇÑ ÄÚµå
    {

        // ÇÑ±Û(¿Ï¼ºÇü/ÀÚÀ½/¸ðÀ½)°ú ¼ýÀÚ¸¸ Çã¿ëÇÏ´Â Á¤±Ô½Ä
        string validPattern = @"^[°¡-ÆR¤¡-¤¾¤¿-¤Ó0-9\s]*$";

        // ÀÔ·Â °ªÀÌ ÆÐÅÏ¿¡ ¸ÂÁö ¾ÊÀ¸¸é ¼öÁ¤
        if (!Regex.IsMatch(input, validPattern))
        {
            warningText.text = "ÇÑ±Û, ¼ýÀÚ, °ø¹é¸¸ ÀÔ·Â °¡´ÉÇÕ´Ï´Ù.";
            btn_CreateRoom.interactable = false; // ¹æ »ý¼º ¹öÆ° ºñÈ°¼ºÈ­
        }
        else if (input.Length > MaxLength) // ±æÀÌ Á¦ÇÑ ÃÊ°ú °Ë»ç
        {
            warningText.text = $"ÃÖ´ë {MaxLength}ÀÚ±îÁö¸¸ ÀÔ·Â °¡´ÉÇÕ´Ï´Ù.";
            btn_CreateRoom.interactable = false; // ¹æ »ý¼º ¹öÆ° ºñÈ°¼ºÈ­
        }
        else if (input.Length == 0) // ºó ¹®ÀÚ¿­ °Ë»ç
        {
            warningText.text = "¹æ ÀÌ¸§À» ÀÔ·ÂÇØÁÖ¼¼¿ä.";
            btn_CreateRoom.interactable = false; // ¹æ »ý¼º ¹öÆ° ºñÈ°¼ºÈ­
        }
        else
        {
            warningText.text = ""; // ±ÔÄ¢¿¡ ¸ÂÀ¸¸é °æ°í ¸Þ½ÃÁö Á¦°Å
            btn_CreateRoom.interactable = true; // ¹æ »ý¼º ¹öÆ° È°¼ºÈ­
        }
    }
    private void OnDestroy()
    {
        // ÀÌº¥Æ® ÇØÁ¦
        input_RoomName.onValueChanged.RemoveListener(ValidateRoomName);
    }

    

    //¹æ ¸ñ·ÏÀÇ º¯È­°¡ ÀÖÀ» ¶§ È£ÃâµÇ´Â ÇÔ¼ö(Æ÷Åæ ±âº» Á¦°ø)
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);  // ±âº» ¸Þ¼­µå È£Ãâ

        //Content¿¡ ÀÚ½ÄÀ¸·Î ºÙ¾îÀÖ´Â ItemÀ» ´Ù »èÁ¦
        DeleteRoomListItem();

        //dicRoomInfo º¯¼ö¸¦ roomList¸¦ ÀÌ¿ëÇØ¼­ °»½Å
        UpdateRoomListItem(roomList);

        //dicRoomÀ» ±â¹ÝÀ¸·Î roomListItemÀ» ¸¸µéÀÚ
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
            // dicRoomInfo¿¡ info ÀÇ ¹æÀÌ¸§À¸·Î µÇ¾îÀÖ´Â key°ªÀÌ Á¸ÀçÇÏ´Â°¡
            if (dicRoomInfo.ContainsKey(info.Name))
            {
                // ÀÌ¹Ì »èÁ¦µÈ ¹æÀÌ¶ó¸é?
                if (info.RemovedFromList)
                {
                    dicRoomInfo.Remove(info.Name); // ¹æ Á¤º¸¸¦ »èÁ¦
                    continue;
                }
            }
            dicRoomInfo[info.Name] = info; // ¹æ Á¤º¸¸¦ Ãß°¡, ¾÷µ¥ÀÌÆ®
        }
    }


    // »ý¼ºµÈ ¹æ ¸ñ·ÏÀ» ½ºÅ©·Ñ ºä¿¡ º¸¿©ÁÙ ¶§
    void CreateRoomListItem() 
    {
        foreach (RoomInfo info in dicRoomInfo.Values)
        {
            //¹æ Á¤º¸ »ý¼º°ú µ¿½Ã¿¡ ScrollView-> ContentÀÇ ÀÚ½ÄÀ¸·Î ÇÏÀÚ
            GameObject go = Instantiate(roomListItem, rtContent); //ÀÎÀÚ: ÇÁ¸®ÆÕ, ÄÜÅÙÃ÷ ¼ø

            //»ý¼ºµÈ item¿¡¼­ RoomListItem ÄÄÆ÷³ÍÆ®¸¦ °¡Á®¿Â´Ù.
            RoomListItem item = go.GetComponent<RoomListItem>();

            // ³­ÀÌµµ¿Í Á¦ÇÑ ½Ã°£À» Ä¿½ºÅÒ ÇÁ·ÎÆÛÆ¼¿¡¼­ °¡Á®¿È
            string difficulty = info.CustomProperties.ContainsKey("difficulty") ?
                                info.CustomProperties["difficulty"].ToString() :
                                "¾øÀ½";
            int timeLimit = info.CustomProperties.ContainsKey("timeLimit") ?
                            Convert.ToInt32(info.CustomProperties["timeLimit"]) :
                            0;

            /// °¡Á®¿Â ÄÄÆ÷³ÍÆ®°¡ °¡Áö°í ÀÖ´Â SetInfo ÇÔ¼ö ½ÇÇà
            item.SetInfo(info.Name, info.PlayerCount, info.MaxPlayers, difficulty, timeLimit);

            // item Å¬¸¯µÇ¾úÀ» ¶§ È£ÃâµÇ´Â ÇÔ¼ö µî·Ï
            item.onDelegate = (roomName) =>
            {
                // SelectRoomItemÀ» ¹Ù·Î È£Ãâ
                SelectRoomItem(roomName, go); // roomName°ú ÇöÀç ¹öÆ°(GameObject)À» Àü´Þ -> ¼±ÅÃµÈ ¹æ¸ñ·ÏÀÇ »ö»óÀÌ º¯°æµÇµµ·Ï
            };
        }
    }


    // ¹æ »ý¼º ÇÒ ¶§
    public void OnClickCreateRoom()
    {
        //¹æ ¿É¼Ç
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)selectedMaxPlayers;
        string difficultyText = GetDifficultyText(selectedDifficulty);

        //Ä¿½ºÅÒ ·ë ÇÁ·ÎÆÛÆ¼ ¼³Á¤(Áß¿ä)
        options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            {"maxPlayers", selectedMaxPlayers},   // ÃÖ´ë ÇÃ·¹ÀÌ¾î ¼ö ¼³Á¤
            {"difficulty", difficultyText},
            {"timeLimit", selectedTimeLimit}
        };

        //·Îºñ¿¡¼­µµ º¸¿©ÁÙ ÇÁ·ÎÆÛÆ¼ ¼³Á¤
        options.CustomRoomPropertiesForLobby = new string[] { "maxPlayers", "difficulty", "timeLimit" };

        //¹æ ¸ñ·Ï¿¡ º¸ÀÌ°Ô ÇÒ°ÍÀÎ°¡?
        options.IsVisible = true;

        //¹æ »ý¼º
        PhotonNetwork.CreateRoom(input_RoomName.text, options);
    }


    public override void OnCreatedRoom() // ¹æ »ý¼º¿¡ ¼º°øÇßÀ» ¶§
    {
        base.OnCreatedRoom();

        UnityEngine.Debug.Log("¹æ »ý¼º ¼º°ø");

        PhotonNetwork.LoadLevel("MakeRoom");
    }

    public override void OnCreateRoomFailed(short returnCode, string message) //¹æ »ý¼º¿¡ ½ÇÆÐÇßÀ» ¶§
    {
        base.OnCreateRoomFailed(returnCode, message);
        UnityEngine.Debug.Log("¹æ »ý¼º ½ÇÆÐ" + message);
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

    public override void OnJoinRoomFailed(short returnCode, string message) // ¹æ ÀÔÀå¿¡ ½ÇÆÐÇßÀ» ¶§
    {
        base.OnJoinRoomFailed(returnCode, message);
        UnityEngine.Debug.Log("¹æ ÀÔÀå ½ÇÆÐ" + message);
    }



    // ¹æ »ý¼ºÀ» À§ÇÑ ¿É¼Ç ¼±ÅÃ ½Ã ÀÌ·ïÁö´Â uiÀÇ º¯È­¿Í index ¾÷µ¥ÀÌÆ®¿¡ °üÇÑ ÄÚµå
    void OnMaxPlayersButtonClicked(int index)
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
        UpdateButtonColors(btn_MaxPlayers, index);
        UnityEngine.Debug.Log("Selected Max Players: " + selectedMaxPlayers);
    }

    void OnDifficultyButtonClicked(int index)
    {
        switch (index) // 0: ÃÊ±Þ, 1: Áß±Þ, 2: °í±Þ
        {
            case 0:
                selectedDifficulty = 2;
                break;
            case 1:
                selectedDifficulty = 3;
                break;
            case 2:
                selectedDifficulty = 4;
                break;
        }
        // selectedDifficulty °ªÀ» ±â¹ÝÀ¸·Î ½ÇÁ¦ ¹®ÀÚ¿­·Î ¹ÝÈ¯
        string difficultyText = GetDifficultyText(selectedDifficulty);
        UpdateButtonColors(btn_Difficulty, index);
        UnityEngine.Debug.Log("Selected Difficulty: " + difficultyText);
    }

    // selectedDifficultyÀÇ °ªÀÌ 2, 3, 4ÀÏ ¶§ °¢°¢ "ÃÊ±Þ", "Áß±Þ", "°í±Þ"ÀÌ¶ó´Â ¹®ÀÚ¿­À» Ãâ·Â
    string GetDifficultyText(int difficulty) 
    {
        switch (difficulty)
        {
            case 2:
                return "ÃÊ±Þ";
            case 3:
                return "Áß±Þ";
            case 4:
                return "°í±Þ";
            default:
                return "¾Ë ¼ö ¾øÀ½"; // ´Ù¸¥ °ªÀÏ °æ¿ì ±âº» °ª ¹ÝÈ¯
        }
    }

    void OnTimeLimitButtonClicked(int index)
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
        UpdateButtonColors(btn_TimeLimit, index);
        UnityEngine.Debug.Log("Selected Time Limit: " + selectedTimeLimit);
    }

    private void SetDefaultSelection(Button[] buttons, int defaultIndex, out int selectedValue)
    {
        selectedValue = defaultIndex;

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;

            ColorBlock colorBlock = buttons[i].colors;
            colorBlock.normalColor = Color.white; // ±âº» »ö»ó È­ÀÌÆ®
            colorBlock.selectedColor = Color.yellow; //¼±ÅÃµÈ »ö»ó ½ÎÀÌ¾ð
            buttons[i].colors = colorBlock;

            buttons[i].onClick.AddListener(() =>
            {
                UpdateButtonColors(buttons, index); //¹öÆ° »ö»ó °»½Å
            });
        }

        UpdateButtonColors(buttons, defaultIndex);
    }

    // ¹öÆ° ¹è¿­ÀÇ »ö»ó ¾÷µ¥ÀÌÆ® ÇÔ¼ö
    void UpdateButtonColors(Button[] buttons, int selectedIndex)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            ColorBlock colorBlock = buttons[i].colors;
            if (i == selectedIndex)
            {
                colorBlock.normalColor = Color.yellow;
            }
            else
            {
                colorBlock.normalColor = Color.white;
            }
            buttons[i].colors = colorBlock;
        }
    }
}
