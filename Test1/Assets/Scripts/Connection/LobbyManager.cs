using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.PointerEventData;
using Button = UnityEngine.UI.Button;

//¹æ »ı¼º ¹× ¹æ Âü¿©¿¡ °üÇÑ ÄÚµå
public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] InputField input_RoomName;
    [SerializeField] Button[] btn_MaxPlayers; // 3°³ÀÇ ¹öÆ°À» ¹è¿­·Î ¼±¾ğ
    [SerializeField] Button[] btn_Difficulty;
    [SerializeField] Button[] btn_TimeLimit;

    [SerializeField] Text warningText; // °æ°í ¸Ş½ÃÁö¸¦ Ãâ·ÂÇÒ UI ÅØ½ºÆ®
    [SerializeField] Button btn_CreateRoom; // ¹æ ¸¸µé±â ¹öÆ°
    [SerializeField] Button btn_JoinRoom; // ¹æ Âü¿© ¹öÆ°
    [SerializeField] GameObject roomListItem; // ¹æ ¸ñ·ÏÀ» º¸¿©ÁÖ´Â ½ºÅ©·Ñºä

    int selectedMaxPlayers = 0; // ÃÖ´ëÀÎ¿ø(2, 3, 4¸í)
    int selectedDifficulty = 0; // ³­ÀÌµµ(ÃÊ±Ş, Áß±Ş, °í±Ş)
    int selectedTimeLimit = 0; // Ä«µå ³õ±â±îÁö Á¦ÇÑ½Ã°£(15ÃÊ, 30ÃÊ, 45ÃÊ)
    public Transform rtContent;
    private const int MaxLength = 12; // ¹æÀÌ¸§ ÃÖ´ë ÀÔ·Â ±æÀÌ

    // ¹æ ¸ñ·ÏÀ» °¡Áö°í ÀÖ´Â Dictionaly º¯¼ö
    Dictionary<string, RoomInfo> dicRoomInfo = new Dictionary<string, RoomInfo>();

    void Start()
    {
        // ±âº»°ª ¼³Á¤: ¸ğµç ¹öÆ°ÀÇ Ã¹ ¹øÂ° Ç×¸ñÀ» ±âº» ¼±ÅÃÇÏµµ·Ï ¼³Á¤
        SetDefaultSelection(btn_MaxPlayers, 0, out selectedMaxPlayers);
        SetDefaultSelection(btn_Difficulty, 0, out selectedDifficulty);
        SetDefaultSelection(btn_TimeLimit, 0, out selectedTimeLimit);

        // ¹æ ÀÌ¸§ ÀÔ·Â ÇÊµå ÃÊ±âÈ­
        input_RoomName.text = ""; //¹æ ÀÌ¸§ ±âº» °ø¹é »óÅÂ
        btn_CreateRoom.interactable = false; // Ã³À½¿¡´Â ¹æ »ı¼º ¹öÆ° ºñÈ°¼ºÈ­
        warningText.text = ""; // ÃÊ±â °æ°í ¸Ş½ÃÁö ºñ¿ì±â

        input_RoomName.onValueChanged.AddListener(ValidateRoomName); //¹æ ÀÌ¸§ ÀÛ¼ºÇÒ ½Ã, ¹æ ÀÌ¸§ ±ÔÄ¢ °Ë»ç
        btn_CreateRoom.onClick.AddListener(OnClickCreateRoom); //¹æ »ı¼º ¹öÆ° Å¬¸¯ ½Ã, ¹æ »ı¼º ¼öÇà
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

        // TimeLimi ¹öÆ°¿¡ ¸®½º³Ê Ãß°¡
        for (int i = 0; i < btn_TimeLimit.Length; i++)
        {
            int index = i;
            btn_TimeLimit[i].onClick.AddListener(() => OnTimeLimitButtonClicked(index));
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
    private void OnDestroy()
    {
        // ÀÌº¥Æ® ÇØÁ¦
        input_RoomName.onValueChanged.RemoveListener(ValidateRoomName);
    }
    
    private void SetDefaultSelection(Button[] buttons, int defaultIndex, out int selectedValue) // ¹öÆ° ¹è¿­ÀÇ ±âº»°ª ¼³Á¤ ÇÔ¼ö
    {
        // ¼±ÅÃµÈ °ªÀ» Ãâ·Â¿ëÀ¸·Î ¹İÈ¯
        selectedValue = defaultIndex;

        // ¸ğµç ¹öÆ°ÀÇ »óÅÂ ÃÊ±âÈ­
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // Å¬·ÎÀú ¹®Á¦ ÇØ°áÀ» À§ÇÑ Áö¿ª º¯¼ö

            // ¸ğµç ¹öÆ°ÀÇ »ö»ó ±âº»°ª ¼³Á¤
            ColorBlock colorBlock = buttons[i].colors;
            colorBlock.normalColor = Color.white; // ±âº» »ö»ó: È­ÀÌÆ®
            colorBlock.selectedColor = Color.yellow; // ¼±ÅÃµÈ ¹öÆ° »ö»ó: ³ë¶û
            buttons[i].colors = colorBlock;

            // ¹öÆ° Å¬¸¯ ÀÌº¥Æ® ¼³Á¤
            buttons[i].onClick.AddListener(() =>
            {
                UpdateButtonColors(buttons, index); // ¹öÆ° »ö»ó °»½Å
            });
        }

        // ±âº»°ª(Ã¹ ¹øÂ° ¹öÆ°) °­Á¶
        UpdateButtonColors(buttons, defaultIndex);
    }

    // ¹öÆ° ¹è¿­ÀÇ »ö»ó °»½Å ÇÔ¼ö
    private void UpdateButtonColors(Button[] buttons, int selectedIndex) 
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            ColorBlock colorBlock = buttons[i].colors;
            if (i == selectedIndex)
            {
                colorBlock.normalColor = Color.yellow; // ¼±ÅÃµÈ ¹öÆ°
            }
            else
            {
                colorBlock.normalColor = Color.white; // ¼±ÅÃµÇÁö ¾ÊÀº ¹öÆ°
            }
            buttons[i].colors = colorBlock;
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

    // ¹æÀ» ¼±ÅÃÇßÀ» ¶§
    void SelectRoomItem(string roomName)
    {
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


    // »ı¼ºµÈ ¹æ ¸ñ·ÏÀ» ½ºÅ©·Ñ ºä¿¡ º¸¿©ÁÙ ¶§
    void CreateRoomListItem() 
    {
        foreach (RoomInfo info in dicRoomInfo.Values)
        {
            //¹æ Á¤º¸ »ı¼º°ú µ¿½Ã¿¡ ScrollView-> ContentÀÇ ÀÚ½ÄÀ¸·Î ÇÏÀÚ
            GameObject go = Instantiate(roomListItem, rtContent); //ÀÎÀÚ: ÇÁ¸®ÆÕ, ÄÜÅÙÃ÷ ¼ø

            //»ı¼ºµÈ item¿¡¼­ RoomListItem ÄÄÆ÷³ÍÆ®¸¦ °¡Á®¿Â´Ù.
            RoomListItem item = go.GetComponent<RoomListItem>();

            //°¡Á®¿Â ÄÄÆ÷³ÍÆ®°¡ °¡Áö°í ÀÖ´Â SetInfo ÇÔ¼ö ½ÇÇà
            item.SetInfo(info.Name, info.PlayerCount, info.MaxPlayers);

            //item Å¬¸¯µÇ¾úÀ» ¶§ È£ÃâµÇ´Â ÇÔ¼ö µî·Ï
            item.onDelegate = SelectRoomItem;
        }
    }


    // ¹æ »ı¼º ÇÒ ¶§
    public void OnClickCreateRoom()
    {
        //¹æ ¿É¼Ç
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)selectedMaxPlayers;

        //Ä¿½ºÅÒ ·ë ÇÁ·ÎÆÛÆ¼ ¼³Á¤(Áß¿ä)
        options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            {"maxPlayers", selectedMaxPlayers},   // ÃÖ´ë ÇÃ·¹ÀÌ¾î ¼ö ¼³Á¤
            {"difficulty", selectedDifficulty},
            {"timeLimit", selectedTimeLimit}
        };

        //·Îºñ¿¡¼­µµ º¸¿©ÁÙ ÇÁ·ÎÆÛÆ¼ ¼³Á¤
        options.CustomRoomPropertiesForLobby = new string[] { "maxPlayers", "difficulty", "timeLimit" };

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


    // ¹æ »ı¼ºÀ» À§ÇÑ ¿É¼Ç ¼±ÅÃ ½Ã ÀÌ·ïÁö´Â uiÀÇ º¯È­¿Í index ¾÷µ¥ÀÌÆ®¿¡ °üÇÑ ÄÚµå
    void OnMaxPlayersButtonClicked(int index)
    {
        selectedMaxPlayers = (index + 2); // 2, 3, 4 ÇÃ·¹ÀÌ¾î ¿É¼Ç
        UpdateButtonColors(btn_MaxPlayers, index);
        UnityEngine.Debug.Log("Selected Max Players: " + selectedMaxPlayers);
    }

    void OnDifficultyButtonClicked(int index)
    {
        selectedDifficulty = index; // 0: ÃÊ±Ş, 1: Áß±Ş, 2: °í±Ş
        UpdateButtonColors(btn_Difficulty, index);
        UnityEngine.Debug.Log("Selected Difficulty: " + selectedDifficulty);
    }

    void OnTimeLimitButtonClicked(int index)
    {
        selectedTimeLimit = (index + 1) * 15; // 15, 30, 45ÃÊ ¿É¼Ç
        UpdateButtonColors(btn_TimeLimit, index);
        UnityEngine.Debug.Log("Selected Time Limit: " + selectedTimeLimit);
    }

}
