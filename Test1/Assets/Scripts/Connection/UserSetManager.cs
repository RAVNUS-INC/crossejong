using Photon.Pun;
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
using NativeGalleryNamespace;


// À¯Àú ÇÁ·ÎÇÊ ¼³Á¤ È­¸é¿¡¼­ ÀÛµ¿ÇÏ´Â ÄÚµå(´Ğ³×ÀÓ, À¯ÀúÇÁ·ÎÇÊ»çÁø º¯°æ ÀúÀå)
public class UserSetManager : MonoBehaviourPunCallbacks
{

    private UserSetManager s_instance;
    public UserSetManager Instance { get { return s_instance; } }

    [SerializeField] InputField inputText; //´Ğ³×ÀÓ ÀÔ·Â(³ªÁß¿¡ ¹Ù²Ü ¼ö ÀÖ´Â Displayname)
    [SerializeField] Button confirmButton; //Á¦Ãâ(ÀúÀå) ¹öÆ°
    [SerializeField] Text warningText; // °æ°í ¸Ş½ÃÁö¸¦ Ãâ·ÂÇÒ UI ÅØ½ºÆ®
    [SerializeField] Text saveText; // ÀúÀå¿Ï·á ¸Ş½ÃÁö¸¦ Ãâ·ÂÇÒ UI ÅØ½ºÆ®
    
    private const int MaxLength = 8; // ÃÖ´ë ÀÔ·Â ±æÀÌ(º¯µ¿°¡´É)

    public Image centralImage; // Áß¾Ó¿¡ Ç¥½ÃµÇ´Â ÇÁ·ÎÇÊÀÌ¹ÌÁö
    public Sprite[] profileImages; // 3°¡Áö ±âº» Á¦°ø ÀÌ¹ÌÁö
    private int currentIndex = 0; // ÇöÀç ¼±ÅÃµÈ ÀÌ¹ÌÁö ÀÎµ¦½º


    void Awake()
    {
        // ½Ì±ÛÅæ ÀÎ½ºÅÏ½º¸¦ ¼³Á¤
        if (s_instance == null)
        {
            s_instance = this;
        }
        else if (s_instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject); // ¾ÀÀÌ º¯°æµÇ´õ¶óµµ °´Ã¼°¡ ÆÄ±«µÇÁö ¾Êµµ·Ï ¼³Á¤
    }

    void Start()
    {
        confirmButton.interactable = false; // ±âº»ÀûÀ¸·Î ¹öÆ° ºñÈ°¼ºÈ­
        warningText.text = ""; // ÃÊ±â °æ°í ¸Ş½ÃÁö ºñ¿ì±â
        saveText.text = ""; // ÃÊ±â ÀúÀå ¸Ş½ÃÁö ºñ¿ì±â

        //³»¿ëÀÌ º¯°æµÇ¾úÀ»¶§ ±ÔÄ¢ °Ë»ç
        inputText.onValueChanged.AddListener(ValidateNickname);

        //È®ÀÎ ¹öÆ° ´©¸£¸é ÀÌ¸§ ÀúÀå
        confirmButton.onClick.AddListener(DisplayName);
        confirmButton.onClick.AddListener(SaveProfileImageToPlayFab);

        LoadSavedImage();  // °ÔÀÓ ½ÃÀÛ ½Ã ÀúÀåµÈ ÀÌ¹ÌÁö ÀÎµ¦½º¸¦ ºÒ·¯¿À±â

    }


    public void SaveProfileImageToPlayFab()
    {
        // PlayerPrefs¿¡¼­ ÀúÀåµÈ ÀÌ¹ÌÁö ÀÎµ¦½º¸¦ °¡Á®¿À±â
        if (PlayerPrefs.HasKey("ProfileImageIndex"))
        {
            int profileImageIndex = PlayerPrefs.GetInt("ProfileImageIndex");

            // PlayFab¿¡ ÀúÀåÇÒ µ¥ÀÌÅÍ¸¦ ÁØºñ
            var request = new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
            {
                { "ProfileImageIndex", profileImageIndex.ToString() }
            }
            };

            // PlayFab¿¡ µ¥ÀÌÅÍ ¾÷µ¥ÀÌÆ® ¿äÃ»
            PlayFabClientAPI.UpdateUserData(request,
                result => {
                    Debug.Log("Successfully saved ProfileImageIndex to PlayFab.");
                },
                error => {
                    Debug.LogError("Error saving ProfileImageIndex to PlayFab: " + error.GenerateErrorReport());
                });
        }
        else
        {
            Debug.LogWarning("ProfileImageIndex not found in PlayerPrefs.");
        }
    }

    // ÁÂÃø È­»ìÇ¥ Å¬¸¯ ½Ã
    public void OnLeftArrowClicked()
    {
        currentIndex = (currentIndex - 1 + profileImages.Length) % profileImages.Length;
        UpdateCentralImage();
    }

    // ¿ìÃø È­»ìÇ¥ Å¬¸¯ ½Ã
    public void OnRightArrowClicked()
    {
        currentIndex = (currentIndex + 1) % profileImages.Length;
        UpdateCentralImage();
    }

    // Áß¾Ó ÀÌ¹ÌÁö¸¦ ¾÷µ¥ÀÌÆ®
    private void UpdateCentralImage()
    {
        if (centralImage == null)
        {
            Debug.LogError("centralImage is not initialized!");
            return;
        }

        if (profileImages == null || profileImages.Length == 0)
        {
            Debug.LogError("profileImages array is not initialized or is empty!");
            return;
        }

        // currentIndex°¡ profileImages ¹è¿­ÀÇ À¯È¿ÇÑ ¹üÀ§ ³»¿¡ ÀÖ´ÂÁö È®ÀÎ
        if (currentIndex < 0 || currentIndex >= profileImages.Length)
        {
            Debug.LogError("currentIndex is out of range!");
            return;
        }

        // ÀÌ¹ÌÁö ¾÷µ¥ÀÌÆ®
        centralImage.sprite = profileImages[currentIndex];

        // ¼±ÅÃÇÑ ÀÌ¹ÌÁö ÀÎµ¦½º¸¦ ÀúÀå
        SaveSelectedImageIndex(currentIndex);
    }

    // ÀÌ¹ÌÁö ÀÎµ¦½º¸¦ PlayerPrefs¿¡ ÀúÀå
    private void SaveSelectedImageIndex(int index)
    {
        PlayerPrefs.SetInt("ProfileImageIndex", index);
        PlayerPrefs.Save();
    }

    // ½ÃÀÛ ½Ã ÀúÀåµÈ ÀÌ¹ÌÁö ÀÎµ¦½º¸¦ ºÒ·¯¿À±â
    public void LoadSavedImage()
    {
        if (PlayerPrefs.HasKey("ProfileImageIndex"))
        {
            currentIndex = PlayerPrefs.GetInt("ProfileImageIndex");
            UpdateCentralImage();
        }
        else
        {
            // ±âº» ÀÌ¹ÌÁö ¼³Á¤ (Ã¹ ½ÃÀÛ ½Ã ÀÎµ¦½º 0, ÇÑ¹ø º¯°æÇß´Ù¸é º¯°æÇÑ ÀÌ¹ÌÁö¿¡¼­ ½ÃÀÛ)
            UpdateCentralImage();
        }
    }

    //ÀÌ¸§ º¯°æ ¹öÆ° Å¬¸¯ÇÒ ¶§ -> ÀÌ¸§ ÀÔ·Â ÀÎÇ² È°¼ºÈ­
    public void ChangeNameBtn() 
    {
        inputText.interactable = true; //ÀÌ¸§ º¯°æ È°¼ºÈ­
    }
     
    //ÀÌ¸§ ¼³Á¤ ±ÔÄ¢
    public void ValidateNickname(string input)
    {
        /// ÇÑ±Û(¿Ï¼ºÇü/ÀÚÀ½/¸ğÀ½)°ú ¼ıÀÚ¸¸ Çã¿ëÇÏ´Â Á¤±Ô½Ä
        string validPattern = @"^[°¡-ÆR¤¡-¤¾¤¿-¤Ó0-9]*$";

        // °ø¹é Á¦°Å
        input = input.Replace(" ", ""); //°ø¹éÀ» Çã¿ëÇÏÁö ¾Ê´Â´Ù

        // ÀÔ·Â °ªÀÌ ÆĞÅÏ¿¡ ¸ÂÁö ¾ÊÀ¸¸é ¼öÁ¤
        if (!Regex.IsMatch(input, validPattern))
        {
            warningText.text = "ÇÑ±Û°ú ¼ıÀÚ¸¸ ÀÔ·Â °¡´ÉÇÕ´Ï´Ù.";
            confirmButton.interactable = false; 
        }
        else if (input.Length > MaxLength) // ±æÀÌ Á¦ÇÑ ÃÊ°ú °Ë»ç
        {
            warningText.text = $"ÃÖ´ë {MaxLength}ÀÚ±îÁö¸¸ ÀÔ·Â °¡´ÉÇÕ´Ï´Ù.";
            confirmButton.interactable = false; 
        }
        else if (input.Length == 0) // ºó ¹®ÀÚ¿­ °Ë»ç
        {
            warningText.text = "´Ğ³×ÀÓÀ» ÀÔ·ÂÇØÁÖ¼¼¿ä.";
            confirmButton.interactable = false; 
        }
        else
        {
            warningText.text = ""; // ±ÔÄ¢¿¡ ¸ÂÀ¸¸é °æ°í ¸Ş½ÃÁö Á¦°Å
            confirmButton.interactable = true; // È®ÀÎ ¹öÆ° È°¼ºÈ­
        }

    }

    //¼³Á¤ÇÑ ÀÌ¸§ ÀúÀå
    public void DisplayName() //DisplayName: °íÀ¯ÇÏÁö ¾ÊÀ½
    {
        var request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request,
             result =>
             {
                 // displayNameÀÌ Á¸ÀçÇÏ´ÂÁö È®ÀÎ(Ã¹ À¯ÀúÀÎÁö ¾Æ´ÑÁö¸¦ È®ÀÎ)
                 if (string.IsNullOrEmpty(result.AccountInfo.TitleInfo.DisplayName))
                 {
                     // displayNameÀÌ ¾øÀ» °æ¿ì (Ã¹ À¯ÀúÀÏ °æ¿ì)
                     Debug.Log("displayNameÀÌ ¼³Á¤µÇÁö ¾Ê¾Ò½À´Ï´Ù. ¼³Á¤ Áß...");
                     SaveDisplayName(); //´Ğ³×ÀÓÀ» ÀúÀåÇÏ°í
                     OnClickConnect(); //¸ŞÀÎÀ¸·Î ¼­¹öÁ¢¼ÓÀ» ¿äÃ»ÇÑ´Ù
                 }
                 else //displaynameÀÌ ÀÌ¹Ì Á¸ÀçÇÒ °æ¿ì(ÀçÁ¢¼Ó À¯ÀúÀÏ °æ¿ì)
                 {
                     SaveDisplayName(); //±âÁ¸ÀÇ ÀÌ¸§À» »õ ÀÌ¸§À¸·Î µ¤¾î¾º¿ö ÀúÀåÇÑ´Ù
                     saveText.text = "ÀúÀåµÇ¾ú½À´Ï´Ù"; //ÀúÀå ¸Ş½ÃÁö ¾Ë¸²
                 }
             },
            error =>
            {
                Debug.LogError($"À¯Àú Á¤º¸ ºÒ·¯¿À±â ½ÇÆĞ: {error.GenerateErrorReport()}");
            });
    }

    public void SaveDisplayName() //´Ü¼øÈ÷ ÀÌ¸§ ÀúÀåÇÏ±â
    {
        string displayName = inputText.text.Trim();

        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
           result =>
           {
               Debug.Log($"´Ğ³×ÀÓ ÀúÀå ¼º°ø: {result.DisplayName}");
           },

           error =>
           {
               Debug.LogError($"´Ğ³×ÀÓ ÀúÀå ½ÇÆĞ: {error.GenerateErrorReport()}");
           });
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

    private void OnDestroy()
    {
        // ÀÌº¥Æ® ÇØÁ¦
        inputText.onValueChanged.RemoveListener(ValidateNickname);
    }


}


