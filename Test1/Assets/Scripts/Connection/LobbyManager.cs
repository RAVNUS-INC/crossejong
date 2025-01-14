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
    [SerializeField] Button[] btn_MaxPlayers; // ÃÖ´ëÀÎ¿ø ¹öÆ°
    [SerializeField] Button[] btn_Difficulty; // ³­ÀÌµµ ¹öÆ°
    [SerializeField] Button[] btn_TimeLimit; // Á¦ÇÑ½Ã°£ ¹öÆ°

    // ¹æ »ı¼º ½Ã ÀÌ¸§ ±ÔÄ¢ °æ°í¸Ş½ÃÁö
    [SerializeField] Text warningText;

    // ¹æ »ı¼º ¹öÆ°°ú ¹æ Âü¿© ¹öÆ°, ¹æ ¸ñ·ÏÀ» Ç¥½ÃÇÒ ½ºÅ©·Ñºä
    [SerializeField] Button btn_CreateRoom; // ¹æ ¸¸µé±â ¹öÆ°
    [SerializeField] Button btn_JoinRoom; // ¹æ Âü¿© ¹öÆ°
    [SerializeField] GameObject roomListItem; // ¹æ ¸ñ·Ï ÇÁ¸®ÆÕ

    // ¹æ »ı¼º ½Ã ¿É¼Çµé
    private int selectedMaxPlayers; // ÃÖ´ëÀÎ¿ø(2, 3, 4¸í)
    private int selectedDifficulty; // ³­ÀÌµµ(ÃÊ±Ş, Áß±Ş, °í±Ş)
    private int selectedTimeLimit; // Ä«µå ³õ±â±îÁö Á¦ÇÑ½Ã°£(15ÃÊ, 30ÃÊ, 45ÃÊ)

    private int selectedMaxPlayersIndex; // ÀÎ¿ø ¼±ÅÃ ÀÎµ¦½º
    private int selectedDifficultyIndex; // ³­ÀÌµµ ¼±ÅÃ ÀÎµ¦½º
    private int selectedTimeLimitIndex; // Á¦ÇÑ½Ã°£ ÀÎµ¦½º

    public Transform rtContent; // ÄÜÅÙÃ÷ ¿µ¿ª
    private const int MaxLength = 12; // ¹æÀÌ¸§ ÃÖ´ë ÀÔ·Â ±æÀÌ
    // ¹æ ¸ñ·ÏÀ» °¡Áö°í ÀÖ´Â Dictionaly º¯¼ö
    Dictionary<string, RoomInfo> dicRoomInfo = new Dictionary<string, RoomInfo>();


    //public static LobbyManager Instance { get; private set; } //¾À ÀüÈ¯½Ã¿¡µµ Á¤º¸°¡ ³²¾ÆÀÖ°Ô


    private void Awake() //ÇÊ¿ä
    {
        //if (Instance == null)
        //{
        //    Instance = this;
        //    DontDestroyOnLoad(gameObject); // ¿ÀºêÁ§Æ®¸¦ ¾À ÀüÈ¯ ½Ã Á¦°ÅÇÏÁö ¾ÊÀ½
        //}
        //else
        //{
        //    Destroy(gameObject); // Áßº¹µÈ ÀÎ½ºÅÏ½º ¹æÁö
        //}
        ResetRoomSetPanel(); // Ã¹ ¸ŞÀÎ Á¢¼Ó ½Ã ÃÖÃÊ ½ÇÇà

    }


    private void OnDestroy()
    {

        //// input_RoomNameÀÌ nullÀÌ ¾Æ´Ò °æ¿ì¿¡¸¸ RemoveListener¸¦ È£Ãâ
        //if (input_RoomName != null)
        //{
        //    input_RoomName.onValueChanged.RemoveListener(ValidateRoomName);
        //}
    }
    


    void Start() 
    {
        
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
        btn_CreateRoom.interactable = false; // Ã³À½¿¡´Â ¹æ »ı¼º ¹öÆ° ºñÈ°¼ºÈ­
        warningText.text = ""; // ÃÊ±â °æ°í ¸Ş½ÃÁö ºñ¿ì±â
        input_RoomName.onValueChanged.AddListener(ValidateRoomName); //¹æ ÀÌ¸§ ÀÛ¼ºÇÒ ½Ã, ¹æ ÀÌ¸§ ±ÔÄ¢ °Ë»ç
    }

    private void MaxPlayerSet(Button[] buttons)
    {
        // MaxPlayers ¹öÆ°¿¡ ¸®½º³Ê Ãß°¡
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnMaxPlayersButtonClicked(index,buttons)); //¼±ÅÃÇÑ ¹öÆ°ÀÇ »ö»ó º¯°æ
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
            item.SetInfo(info.Name, info.PlayerCount, info.MaxPlayers, difficulty, timeLimit);  //³­ÀÌµµ¿Í Á¦ÇÑ½Ã°£¸¸ custom properties¿¡ ¼±¾ğ, ³ª¸ÓÁö´Â photon¿¡¼­ ±âº»Á¦°ø

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
        string difficultyText = GetDifficultyText(selectedDifficulty);

        //Ä¿½ºÅÒ ·ë ÇÁ·ÎÆÛÆ¼ ¼³Á¤(Áß¿ä)
        options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            //{"PlayersIndex", selectedMaxPlayersIndex},  // ÇÃ·¹ÀÌ¾î index

            {"DifficultyIndex", selectedDifficultyIndex},  // ³­ÀÌµµ index
            {"TimeLimitIndex", selectedTimeLimitIndex},  // Á¦ÇÑ½Ã°£ index

            {"difficultyInt", selectedDifficulty},  // ³­ÀÌµµ int°ª(2,3,4)

            {"timeLimit", selectedTimeLimit},  // Á¦ÇÑ½Ã°£ int°ª(15,30,45)
            {"difficulty", difficultyText}   // ³­ÀÌµµ str°ª(ÃÊ±Ş,Áß±Ş,°í±Ş)

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



    // ¹æ »ı¼ºÀ» À§ÇÑ ¿É¼Ç ¼±ÅÃ ½Ã ÀÌ·ïÁö´Â uiÀÇ º¯È­¿Í index ¾÷µ¥ÀÌÆ®¿¡ °üÇÑ ÄÚµå
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
        UnityEngine.Debug.Log("Selected Max Players: " + selectedMaxPlayers); //¸Ş½ÃÁö Ãâ·Â
    }

    public void OnDifficultyButtonClicked(int index, Button[] difBtn)
    {
        switch (index) // 0: ÃÊ±Ş, 1: Áß±Ş, 2: °í±Ş
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
        selectedDifficultyIndex = index;
        string difficultyText = GetDifficultyText(selectedDifficulty); // selectedDifficulty °ªÀ» ±â¹İÀ¸·Î ½ÇÁ¦ ¹®ÀÚ¿­·Î ¹İÈ¯
        UpdateButtonColors(difBtn, index); //»ö»ó ¾÷µ¥ÀÌÆ®
        UnityEngine.Debug.Log("Selected Difficulty: " + difficultyText); //¸Ş½ÃÁö Ãâ·Â
    }

    // selectedDifficultyÀÇ °ªÀÌ 2, 3, 4ÀÏ ¶§ °¢°¢ "ÃÊ±Ş", "Áß±Ş", "°í±Ş"ÀÌ¶ó´Â ¹®ÀÚ¿­À» Ãâ·Â
    public string GetDifficultyText(int difficulty) 
    {
        switch (difficulty)
        {
            case 2:
                return "ÃÊ±Ş";
            case 3:
                return "Áß±Ş";
            case 4:
                return "°í±Ş";
            default:
                return "¾Ë ¼ö ¾øÀ½"; // ´Ù¸¥ °ªÀÏ °æ¿ì ±âº» °ª ¹İÈ¯
        }
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

    private void SetDefaultSelection(Button[] buttons, int defaultIndex)
    {

        for (int i = 0; i < buttons.Length; i++) //¼¼¹ø ¹İº¹(°¢ ¹öÆ° ¹è¿­ ±æÀÌ)
        {
            int index = i;

            ColorBlock colorBlock = buttons[i].colors;
            colorBlock.normalColor = Color.white; // ±âº» »ö»ó È­ÀÌÆ®
            colorBlock.selectedColor = Color.yellow; //¼±ÅÃµÈ »ö»ó ³ë¶õ»ö
            buttons[i].colors = colorBlock;

            //buttons[i].onClick.AddListener(() =>
            //{
            //    UpdateButtonColors(buttons, index); //¹öÆ° »ö»ó °»½Å
            //});
        }
        UpdateButtonColors(buttons, defaultIndex);  //±âº»°ª ¹öÆ° »ö»óÀ» ³ë¶õ»öÀ¸·Î
    }

    // ¹öÆ°À» ½ÇÁ¦·Î »öÄ¥ÇÏ´Â ÇÔ¼ö
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
}
