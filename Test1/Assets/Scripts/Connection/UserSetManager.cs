using Photon.Pun;
using Photon.Realtime; // AuthenticationValues ¹× ±âÅ¸ ½Ç½Ã°£ ±â´É »ç¿ë
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// À¯Àú ÇÁ·ÎÇÊ ¼³Á¤ È­¸é¿¡¼­ ÀÛµ¿ÇÏ´Â ÄÚµå(´Ğ³×ÀÓ, À¯ÀúÇÁ·ÎÇÊ»çÁø º¯°æ ÀúÀå)
public class UserSetManager : MonoBehaviourPunCallbacks
{
    [SerializeField] public InputField inputText; //´Ğ³×ÀÓ ÀÔ·Â(³ªÁß¿¡ ¹Ù²Ü ¼ö ÀÖ´Â Displayname)
    [SerializeField] Button confirmButton; //Á¦Ãâ(ÀúÀå) ¹öÆ°
    [SerializeField] Text warningText; // °æ°í ¸Ş½ÃÁö¸¦ Ãâ·ÂÇÒ UI ÅØ½ºÆ®
    [SerializeField] public Text saveText; // ÀúÀå¿Ï·á ¸Ş½ÃÁö¸¦ Ãâ·ÂÇÒ UI ÅØ½ºÆ®(profile panel¿¡¸¸ Á¸Àç)

    // displayname Á¶°Ç
    private const int MinLength = 3; // ÃÖ¼Ò ÀÔ·Â ±æÀÌ(2±ÛÀÚ ÀÌ»ó)
    private const int MaxLength = 8; // ÃÖ´ë ÀÔ·Â ±æÀÌ(º¯µ¿°¡´É)

    public Image centralImage; // Áß¾Ó¿¡ Ç¥½ÃµÇ´Â ÇÁ·ÎÇÊÀÌ¹ÌÁö
    public Sprite[] profileImages; // 3°¡Áö ±âº» Á¦°ø ÀÌ¹ÌÁö

    private int currentIndex = 0; // ÇöÀç ¼±ÅÃµÈ ÀÌ¹ÌÁö ÀÎµ¦½º
    private const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";  // ÀúÀå Å°
    private string displayName; //µğ½ºÇÃ·¹ÀÌ ÀÌ¸§(ÀÓ½ÃÀúÀåÀ» À§ÇÑ º¯¼ö)

    public GameObject profilePanel; //ÇÁ·ÎÇÊ ¼³Á¤ ÆĞ³Î(¸ŞÀÎÆĞ³Î¿¡¼­ ¹Ì¸® ÁØºñÇØ¾ß ÀÛµ¿)
    public GameObject usersetPanel; //À¯Àú ÃÊ±â ¼³Á¤ ÆĞ³Î

    //playerprefs¿¡ ÀúÀåÇÒ ³»¿ëµé(Key)
    private const string DISPLAYNAME_KEY = "DisplayName"; // À¯ÀúÀÇ DisplayName
    private const string IMAGEINDEX_KEY = "ImageIndex"; // À¯ÀúÀÇ ÀÌ¹ÌÁö ÀÎµ¦½º

    void Start()
    {
        // ÃÊ±â À¯Àú ¼¼ÆÃ ÆĞ³Î OR ±âÁ¸ À¯Àú ÇÁ·ÎÇÊ ÆĞ³ÎÀÌ È°¼ºÈ­µÇ¾îÀÖ´Ù¸é
        if ((usersetPanel.activeSelf || profilePanel.activeSelf)) 
        {
            LoadDefaultDisplayName(); // ±âÁ¸ ÀÌ¸§ Á¤º¸ ºÒ·¯¿Í º¯¼ö¿¡ ÀúÀå
            LoadDefaultImageIndex(); // ±âÁ¸ ÀÌ¹ÌÁö ÀÎµ¦½º¸¦ ºÒ·¯¿Í º¯¼ö¿¡ ÀúÀå, ¾÷µ¥ÀÌÆ®
        }
        
        confirmButton.interactable = false; // ±âº»ÀûÀ¸·Î ¹öÆ° ºñÈ°¼ºÈ­
        warningText.text = ""; // ÃÊ±â °æ°í ¸Ş½ÃÁö ºñ¿ì±â
        saveText.text = ""; // ÃÊ±â ÀúÀå ¸Ş½ÃÁö ºñ¿ì±â

        //³»¿ëÀÌ º¯°æµÇ¾úÀ»¶§ ±ÔÄ¢ °Ë»ç
        inputText.onValueChanged.AddListener(ValidateNickname);

        //È®ÀÎ ¹öÆ° ´©¸£¸é ÀÌ¸§ ¹× ÀÌ¹ÌÁö ÀúÀå(playfab ¹× playerprefs¿¡ ¾÷µ¥ÀÌÆ®)(+ ´Ü¾î¿Ï¼ºÈ½¼ö ¹öÆ°¿¡ Á÷Á¢ ¿¬°á)
        confirmButton.onClick.AddListener(SaveDisplayName); //ÀÌ¸§ ÀúÀå 
        confirmButton.onClick.AddListener(SaveSelectedImageIndex); // ÀÌ¹ÌÁö ÀúÀå
    }

    
    private void LoadDefaultDisplayName() // ÀúÀåµÈ DisplayName ·Îµå ¹× ÀúÀå ÇÔ¼ö(Á¸ÀçÇÏ¸é ÇØ´ç °ªÀ», Á¸ÀçÇÏÁö ¾ÊÀ¸¸é Guest)
    {
        if (PlayerPrefs.HasKey(DISPLAYNAME_KEY))
        {
            // Å°°¡ Á¸ÀçÇÏ¸é ÀúÀåµÈ °ªÀ» °¡Á®¿Â´Ù
            string displayName = PlayerPrefs.GetString(DISPLAYNAME_KEY, "Guest");

            // ºÒ·¯¿Â ÀÌ¸§À» ÀÎÇ²¶õ¿¡ º¸¿©ÁÖ±â
            inputText.text = displayName;

            //ÇÁ·ÎÇÊ ÀÌ¸§ ÃÊ±â ºñÈ°¼ºÈ­
            inputText.interactable = false;
        }
        else
        {
            // Å°°¡ Á¸ÀçÇÏÁö ¾ÊÀ¸¸é ±âº»°ªÀ» ¼³Á¤ÇÑ´Ù
            inputText.text = "";

            // ±âº»°ªÀ» playerprefs¿¡µµ ¹Ù·Î ÀúÀå
            PlayerPrefs.SetString(DISPLAYNAME_KEY, "Guest"); PlayerPrefs.Save();
        }
    }

    private void LoadDefaultImageIndex() // ÀúÀåµÈ ÀÌ¹ÌÁö ÀÎµ¦½º¸¦ ºÒ·¯¿À±â(Á¸ÀçÇÏ¸é ÇØ´ç °ªÀ», Á¸ÀçÇÏÁö ¾ÊÀ¸¸é ±âº»°ª 0)
    {
        if (PlayerPrefs.HasKey(IMAGEINDEX_KEY))
        {
            // Å°°¡ Á¸ÀçÇÏ¸é ÀúÀåµÈ °ªÀ» °¡Á®¿Â´Ù
            int Index = PlayerPrefs.GetInt(IMAGEINDEX_KEY, 0);

            //currentIndex ÀÎµ¦½º º¯¼ö¿¡ °ª ÀúÀå
            currentIndex = Index;
        }
        else
        {
            // Å°°¡ Á¸ÀçÇÏÁö ¾ÊÀ¸¸é ±âº»°ªÀ» ¼³Á¤ÇÑ´Ù
            currentIndex = 0;

            // ±âº»°ªÀ» playerprefs¿¡µµ ¹Ù·Î ÀúÀå
            PlayerPrefs.SetInt(IMAGEINDEX_KEY, currentIndex); PlayerPrefs.Save();
        }
        centralImage.sprite = profileImages[currentIndex];  // ÀÌ¹ÌÁö ¾÷µ¥ÀÌÆ®
    }

    public void SaveDisplayName() //DisplayNameÀ» playfab°ú playerprefs¿¡ ÀúÀå
    { 
        string displayName = inputText.text.Trim();

        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName
        };

        //playfab¿¡ ÀúÀå
        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
            result =>
            {
                Debug.Log($"[Playfab] ´Ğ³×ÀÓ ÀúÀå ¼º°ø: {result.DisplayName}");
            },
            error =>
            {
                Debug.LogError($"[Playfab] ´Ğ³×ÀÓ ÀúÀå ½ÇÆĞ: {error.GenerateErrorReport()}");
            });

        //º¯°æµÈ ÀÌ¹ÌÁö ÀÎµ¦½º¸¦ playerprefs¿¡ ÀúÀå(±âÁ¸ À¯ÀúÀÇ °æ¿ì µ¤¾î¾²±â, ½Å±Ô À¯Àú´Â »õ·Î Ãß°¡ÇÏ´Â »óÈ²)
        UpdateDisplayName(displayName);
        Debug.Log($"[playerprefs] Displayname: {displayName}À» ÀúÀåÇß½À´Ï´Ù");

        if (usersetPanel.activeSelf)
        {
            // ¸ŞÀÎÀ¸·Î ¼­¹öÁ¢¼ÓÀ» ¿äÃ»
            OnClickConnect();
        }
        if (profilePanel.activeSelf)
        {
            // ÀúÀå ¸Ş½ÃÁö ¾Ë¸²
            saveText.text = "ÀúÀåµÇ¾ú½À´Ï´Ù";
        }
    }

    private void SaveSelectedImageIndex() // ¼±ÅÃµÈ ÀÌ¹ÌÁö ÀÎµ¦½º¸¦ ÀúÀåÇØ playfab¿¡ Àü¼Û, playerprefs ¾÷µ«
    {
        string ImageIndex = currentIndex.ToString(); //intÇü -> ¹®ÀÚ¿­·Î º¯È¯

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
        {
            { PROFILE_IMAGE_INDEX_KEY, ImageIndex}
        },
            Permission = UserDataPermission.Public // µ¥ÀÌÅÍ¸¦ °ø°³ »óÅÂ·Î ÀúÀå
        };

        //playfab¿¡ ÀúÀå
        PlayFabClientAPI.UpdateUserData(request,
            result =>
            {
                Debug.Log($"[Playfab] ÇÁ·ÎÇÊÀÌ¹ÌÁö µ¥ÀÌÅÍ ÀúÀå ¼º°ø: {ImageIndex}");
            },
            error =>
            {
                Debug.LogError($"[Playfab] À¯Àú µ¥ÀÌÅÍ ÀúÀå ½ÇÆĞ: {error.GenerateErrorReport()}");
            });

        //¹®ÀÚ¿­ -> intÇüÀ¸·Î º¯È¯
        int RImageIndex = int.Parse(ImageIndex); 

        //º¯°æµÈ ÀÌ¹ÌÁö ÀÎµ¦½º¸¦ playerprefs¿¡ ÀúÀå(±âÁ¸ À¯ÀúÀÇ °æ¿ì µ¤¾î¾²±â, ½Å±Ô À¯Àú´Â »õ·Î Ãß°¡ÇÏ´Â »óÈ²)
        UpdateImageIndex(RImageIndex); 
        Debug.Log($"[playerprefs] Imageindex: {currentIndex}À» ÀúÀåÇß½À´Ï´Ù");
    }

    void UpdateDisplayName(string name) //»õ·Î¿î ÀÌ¸§ ÀúÀå
    {
        PlayerPrefs.SetString(DISPLAYNAME_KEY, name); // »õ·Î¿î °ª ÀúÀå
        PlayerPrefs.Save(); // ÀúÀå À¯Áö
    }

    void UpdateImageIndex(int newIndex) //»õ·Î¿î ÀÎµ¦½º ÀúÀå
    {
        PlayerPrefs.SetInt(IMAGEINDEX_KEY, newIndex); // »õ·Î¿î °ª ÀúÀå
        PlayerPrefs.Save(); // ÀúÀå À¯Áö
    }
    
    public void OnLeftButtonClicked() // ¿ŞÂÊ ¹öÆ° Å¬¸¯ ½Ã È£Ãâ
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = profileImages.Length - 1;  // ¼øÈ¯ (¸Ç Ã³À½À¸·Î µ¹¾Æ°¨)
        centralImage.sprite = profileImages[currentIndex];  // ÀÎµ¦½º¿¡ ÇØ´çÇÏ´Â ÀÌ¹ÌÁö·Î ¾÷µ¥ÀÌÆ®
    }

    public void OnRightButtonClicked() // ¿À¸¥ÂÊ ¹öÆ° Å¬¸¯ ½Ã È£Ãâ
    {
        currentIndex = (currentIndex + 1) % profileImages.Length;
        centralImage.sprite = profileImages[currentIndex];  // ÀÎµ¦½º¿¡ ÇØ´çÇÏ´Â ÀÌ¹ÌÁö·Î ¾÷µ¥ÀÌÆ®
    }

    public void ChangeNameBtn() //ÀÌ¸§ º¯°æ ¹öÆ° Å¬¸¯ÇÒ ¶§ -> ÀÌ¸§ ÀÔ·Â ÀÎÇ² È°¼ºÈ­
    {
        inputText.interactable = true; //ÀÌ¸§ º¯°æ È°¼ºÈ­
    }

    public void ValidateNickname(string input) //ÀÌ¸§ ¼³Á¤ ±ÔÄ¢(displayname)
    {
        /// ÇÑ±Û(¿Ï¼ºÇü/ÀÚÀ½/¸ğÀ½)°ú ¼ıÀÚ¸¸ Çã¿ëÇÏ´Â Á¤±Ô½Ä
        string validPattern = @"^[°¡-ÆR¤¡-¤¾¤¿-¤Ó0-9]*$";

        // °ø¹é Á¦°Å
        string inputname = input.Replace(" ", ""); //°ø¹éÀ» Çã¿ëÇÏÁö ¾Ê´Â´Ù

        // ÀÔ·Â °ªÀÌ ÆĞÅÏ¿¡ ¸ÂÁö ¾ÊÀ¸¸é ¼öÁ¤
        if (!Regex.IsMatch(inputname, validPattern))
        {
            warningText.text = "ÇÑ±Û°ú ¼ıÀÚ¸¸ ÀÔ·Â °¡´ÉÇÕ´Ï´Ù.";
            confirmButton.interactable = false; 
        }
        // ±æÀÌ Á¦ÇÑ ÃÊ°ú °Ë»ç
        else if (GetKoreanCharCount(inputname) > MaxLength) // ÇÑ±Û ÀÚÀ½, ¸ğÀ½À» Æ÷ÇÔÇÑ ÃÖ´ë ±æÀÌ °Ë»ç
        {
            warningText.text = $"ÃÖ´ë {MaxLength}ÀÚ±îÁö¸¸ ÀÔ·Â °¡´ÉÇÕ´Ï´Ù.";
            confirmButton.interactable = false; 
        }
        // ÃÖ¼Ò ±æÀÌ ÃæÁ· °Ë»ç
        else if (GetKoreanCharCount(inputname) < MinLength) // ÇÑ±Û ÀÚÀ½, ¸ğÀ½À» Æ÷ÇÔÇÑ ÃÖ´ë ±æÀÌ °Ë»ç
        {
            warningText.text = $"ÃÖ¼Ò {MinLength}ÀÚ ÀÌ»óÀÌ¾î¾ß ÇÕ´Ï´Ù."; //3ÀÚ ÀÌ»óÀÌ¾î¾ß ÇÔ
            confirmButton.interactable = false;
        }
        else if (inputname.Length == 0) // ºó ¹®ÀÚ¿­ °Ë»ç
        {
            warningText.text = "´Ğ³×ÀÓÀ» ÀÔ·ÂÇØÁÖ¼¼¿ä.";
            confirmButton.interactable = false; 
        }
        else if (displayName == inputname && (inputText.isActiveAndEnabled)) //ÀÔ·Â¶õÀÌ ±âÁ¸ ´Ğ³×ÀÓ°ú °°À¸¸é¼­ È°¼ºÈ­µÇ¾îÀÖ´Â °æ¿ì
        {
            warningText.text = "±âÁ¸ ´Ğ³×ÀÓ°ú ´Ş¶ó¾ß ÇÕ´Ï´Ù.";
            confirmButton.interactable = false;
        }
        else
        {
            warningText.text = ""; // ±ÔÄ¢¿¡ ¸ÂÀ¸¸é °æ°í ¸Ş½ÃÁö Á¦°Å
            confirmButton.interactable = true; // È®ÀÎ ¹öÆ° È°¼ºÈ­
        }
        // ÀÔ·Â¶õ¿¡ °ø¹éÀ» Á¦°ÅÇÑ °ª ¹İ¿µ
        inputText.text = inputname;
    }

    private int GetKoreanCharCount(string input) // ÇÑ±Û À½Àı ÀÚÀ½, ¸ğÀ½À» Æ÷ÇÔÇÏ¿© ±ÛÀÚ ¼ö¸¦ °è»êÇÏ´Â ÇÔ¼ö
    {
        int count = 0;
        foreach (char c in input)
        {
            // ÇÑ±Û À½ÀıÀÎÁö Ã¼Å© (°¡-ÆR ¹üÀ§)
            if (c >= '°¡' && c <= 'ÆR')
            {
                count++;
            }
            // ÇÑ±Û ÀÚÀ½/¸ğÀ½ÀÎÁö Ã¼Å© (¤¡-¤¾, ¤¿-¤Ó ¹üÀ§)
            else if ((c >= '¤¡' && c <= '¤¾') || (c >= '¤¿' && c <= '¤Ó'))
            {
                count++;
            }
            // ¼ıÀÚ(0-9)ÀÎÁö Ã¼Å©
            else if (c >= '0' && c <= '9')
            {
                count++;
            }
        }
        return count;
    }


    public override void OnConnectedToMaster() //¸¶½ºÅÍ ¼­¹ö Á¢¼Ó µÇ¸é
    {
        base.OnConnectedToMaster();
        Debug.Log("¸¶½ºÅÍ ¼­¹ö Á¢¼Ó ¼º°ø");

        //³ªÀÇ ÀÌ¸§À» Æ÷Åæ¿¡ ¼³Á¤
        PhotonNetwork.NickName = inputText.text;

        //·ÎºñÁøÀÔ
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby() //Lobby ÁøÀÔ¿¡ ¼º°øÇßÀ¸¸é È£ÃâµÇ´Â ÇÔ¼ö
    {
        base.OnJoinedLobby();

        //¸ŞÀÎ ¾ÀÀ¸·Î ÀÌµ¿
        PhotonNetwork.LoadLevel("Main");

        print("·Îºñ ÁøÀÔ ¼º°ø");

    }
    public void OnClickConnect() // ¸¶½ºÅÍ ¼­¹ö Á¢¼Ó ¿äÃ»(OkBtn¿¡ ¿¬°á)
    {
        // ¸¶½ºÅÍ ¼­¹ö Á¢¼Ó ¿äÃ»
        PhotonNetwork.ConnectUsingSettings();

        //·Îµù¹Ù ui ¾Ö´Ï¸ŞÀÌ¼Ç º¸¿©ÁÖ±â
        LoadingSceneController.Instance.LoadScene("Main");
    }
    private void OnDestroy() // ÀÌº¥Æ® ÇØÁ¦
    {
        inputText.onValueChanged.RemoveListener(ValidateNickname);
    }


}


