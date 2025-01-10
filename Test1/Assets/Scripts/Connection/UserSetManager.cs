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
    public static UserSetManager Instance { get; private set; }

    [SerializeField] InputField inputText; //´Ğ³×ÀÓ ÀÔ·Â(³ªÁß¿¡ ¹Ù²Ü ¼ö ÀÖ´Â Displayname)
    [SerializeField] Button confirmButton; //Á¦Ãâ(ÀúÀå) ¹öÆ°
    [SerializeField] Text warningText; // °æ°í ¸Ş½ÃÁö¸¦ Ãâ·ÂÇÒ UI ÅØ½ºÆ®
    [SerializeField] Text saveText; // ÀúÀå¿Ï·á ¸Ş½ÃÁö¸¦ Ãâ·ÂÇÒ UI ÅØ½ºÆ®
    
    private const int MaxLength = 8; // ÃÖ´ë ÀÔ·Â ±æÀÌ(º¯µ¿°¡´É)

    public Image centralImage; // Áß¾Ó¿¡ Ç¥½ÃµÇ´Â ÇÁ·ÎÇÊÀÌ¹ÌÁö
    public Sprite[] profileImages; // 3°¡Áö ±âº» Á¦°ø ÀÌ¹ÌÁö
    private int currentIndex = 0; // ÇöÀç ¼±ÅÃµÈ ÀÌ¹ÌÁö ÀÎµ¦½º
    private const string PROFILE_IMAGE_INDEX_KEY = "ProfileImageIndex";  // ÀúÀå Å°
    private string displayName; //µğ½ºÇÃ·¹ÀÌ ÀÌ¸§

    public GameObject profilePanel; //ÇÁ·ÎÇÊ ¼³Á¤ ÆĞ³Î(¸ŞÀÎÆĞ³Î¿¡¼­ ¹Ì¸® ÁØºñÇØ¾ß ÀÛµ¿)
    public GameObject usersetPanel; //À¯Àú ÃÊ±â ¼³Á¤ ÆĞ³Î

    void Start()
    {
        if ((usersetPanel.activeSelf || profilePanel.activeSelf))
        {
            DisplayName(); // ±âÁ¸ ÀÌ¸§ Á¤º¸ ºÒ·¯¿Í º¯¼ö¿¡ ÀúÀå
            CheckAndSaveDefaultImageIndex(); // °ÔÀÓ ½ÃÀÛ ½Ã ÀúÀåµÈ ÀÌ¹ÌÁö ÀÎµ¦½º¸¦ ºÒ·¯¿Í º¯¼ö¿¡ ÀúÀå ¹× ÀÌ¹ÌÁö ¾÷µ¥ÀÌÆ®
        }

        confirmButton.interactable = false; // ±âº»ÀûÀ¸·Î ¹öÆ° ºñÈ°¼ºÈ­
        warningText.text = ""; // ÃÊ±â °æ°í ¸Ş½ÃÁö ºñ¿ì±â
        saveText.text = ""; // ÃÊ±â ÀúÀå ¸Ş½ÃÁö ºñ¿ì±â

        //³»¿ëÀÌ º¯°æµÇ¾úÀ»¶§ ±ÔÄ¢ °Ë»ç
        inputText.onValueChanged.AddListener(ValidateNickname);

        //È®ÀÎ ¹öÆ° ´©¸£¸é ÀÌ¸§ ÀúÀå
        confirmButton.onClick.AddListener(OnClickSaveDisplayName);

    }

    // ÀÌ¸§ ´Ğ³×ÀÓ °ü·Ã ÇÔ¼öµé
    // ¼³Á¤ÇÑ ÀÌ¸§ ÀúÀå ÇÔ¼ö
    public void DisplayName() //DisplayName: °íÀ¯ÇÏÁö ¾ÊÀ½
    {
        var request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request,
             result =>
             {
                 // displayNameÀÌ ¾ø´Â °æ¿ì nullÀ» ¹İÈ¯
                 if (string.IsNullOrEmpty(result.AccountInfo.TitleInfo.DisplayName))
                 {
                     displayName = null; // displayNameÀÌ ¾øÀ¸¸é null ÇÒ´ç
                     Debug.Log("displayNameÀÌ ¾ø½À´Ï´Ù.");
                 }
                 else
                 {
                     // displayName °ªÀ» Àü¿ª º¯¼ö¿¡ ÀúÀå
                     displayName = result.AccountInfo.TitleInfo.DisplayName;
                     Debug.Log($"ºÒ·¯¿Â displayName: {displayName}");
                 }
             },
            error =>
            {
                Debug.LogError($"À¯Àú Á¤º¸ ºÒ·¯¿À±â ½ÇÆĞ: {error.GenerateErrorReport()}");
            });
    }


    // À¯Àú ÇÁ·ÎÇÊ ÀÌ¹ÌÁö °ü·Ã ÇÔ¼öµé
    // ÀúÀåµÈ ÀÌ¹ÌÁö ÀÎµ¦½º¸¦ ºÒ·¯¿À±â

    private void CheckAndSaveDefaultImageIndex()
    {
        var request = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(request,
            result =>
            {
                // PROFILE_IMAGE_INDEX_KEY°¡ Á¸ÀçÇÏ´ÂÁö È®ÀÎ
                if (result.Data.ContainsKey(PROFILE_IMAGE_INDEX_KEY))
                {
                    Debug.Log("KEY°¡ Á¸ÀçÇÕ´Ï´Ù. °ªÀ» ºÒ·¯¿É´Ï´Ù.");
                    string value = result.Data[PROFILE_IMAGE_INDEX_KEY].Value;

                    if (!string.IsNullOrEmpty(value)) 
                    {
                        currentIndex = int.Parse(value); 
                    }
                    else 
                    {
                        Debug.LogWarning("KEYÀÇ °ªÀÌ ºñ¾î ÀÖ½À´Ï´Ù. ±âº»°ª(0)À¸·Î ÀúÀåÇÕ´Ï´Ù.");
                        currentIndex = 0; // ±âº»°ª ¼³Á¤
                        SaveSelectedImageIndex(currentIndex); // PlayFab¿¡ ÀúÀå
                    }
                }
                else
                {
                    Debug.LogWarning("KEY°¡ Á¸ÀçÇÏÁö ¾Ê½À´Ï´Ù. ±âº»°ª(0)À» »ı¼ºÇÕ´Ï´Ù.");
                    currentIndex = 0; // ±âº»°ª ¼³Á¤
                    SaveSelectedImageIndex(currentIndex); // PlayFab¿¡ »õ·Î¿î Å° »ı¼º ¹× °ª ÀúÀå
                }

                UpdateCentralImage(); // ÀÌ¹ÌÁö ¾÷µ¥ÀÌÆ®
            },
            error =>
            {
                Debug.LogError($"À¯Àú µ¥ÀÌÅÍ ºÒ·¯¿À±â ½ÇÆĞ: {error.GenerateErrorReport()}");
            });
    }


    // ÀÌ¹ÌÁö ¾÷µ¥ÀÌÆ® ÇÔ¼ö
    private void UpdateCentralImage()
    {
        if (profileImages.Length > 0 && currentIndex >= 0 && currentIndex < profileImages.Length)
        {
            centralImage.sprite = profileImages[currentIndex];  // ÀÎµ¦½º¿¡ ÇØ´çÇÏ´Â ÀÌ¹ÌÁö·Î ¾÷µ¥ÀÌÆ®
            SaveSelectedImageIndex(currentIndex);                // ¼±ÅÃµÈ ÀÌ¹ÌÁö ÀÎµ¦½º ÀúÀå
        }
        else
        {
            Debug.LogWarning("Invalid profile image index.");
        }
    }

    // ¼±ÅÃµÈ ÀÌ¹ÌÁö ÀÎµ¦½º¸¦ ÀúÀå
    private void SaveSelectedImageIndex(int index)
    {
        // ¼±ÅÃµÈ ÀÌ¹ÌÁö ÀÎµ¦½º¸¦ PlayFab Å¸ÀÌÆ² µ¥ÀÌÅÍ¿¡ ÀúÀå
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
        {
            { PROFILE_IMAGE_INDEX_KEY, index.ToString() }
        }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result =>
            {
                Debug.Log($"ÇÁ·ÎÇÊ µ¥ÀÌÅÍ ÀúÀå ¼º°ø: {index}");
            },
            error =>
            {
                Debug.LogError($"À¯Àú µ¥ÀÌÅÍ ÀúÀå ½ÇÆĞ: {error.GenerateErrorReport()}");
            });
    }




    // ¿ŞÂÊ ¹öÆ° Å¬¸¯ ½Ã È£Ãâ
    public void OnLeftButtonClicked()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = profileImages.Length - 1;  // ¼øÈ¯ (¸Ç Ã³À½À¸·Î µ¹¾Æ°¨)
        UpdateCentralImage();  // ÀÌ¹ÌÁö ¾÷µ¥ÀÌÆ®
    }

    // ¿À¸¥ÂÊ ¹öÆ° Å¬¸¯ ½Ã È£Ãâ
    public void OnRightButtonClicked()
    {
        currentIndex++;
        if (currentIndex >= profileImages.Length) currentIndex = 0;  // ¼øÈ¯ (¸Ç ¸¶Áö¸·¿¡¼­ Ã³À½À¸·Î µ¹¾Æ°¨)
        UpdateCentralImage();  // ÀÌ¹ÌÁö ¾÷µ¥ÀÌÆ®
    }





    

    public void OnClickSaveDisplayName()
    {
        // displayNameÀÌ Á¸ÀçÇÏ´ÂÁö È®ÀÎ(Ã¹ À¯ÀúÀÎÁö ¾Æ´ÑÁö¸¦ È®ÀÎ)
        if (string.IsNullOrEmpty(displayName))
        {
            // displayNameÀÌ ¾øÀ» °æ¿ì (Ã¹ À¯ÀúÀÏ °æ¿ì)
            Debug.Log("Ã¹ displayNameÀÌ ¼³Á¤µÇ¾ú½À´Ï´Ù.");
            SaveDisplayName(); //´Ğ³×ÀÓÀ» ÀúÀåÇÏ°í
            OnClickConnect(); //¸ŞÀÎÀ¸·Î ¼­¹öÁ¢¼ÓÀ» ¿äÃ»ÇÑ´Ù
        }
        else //displaynameÀÌ ÀÌ¹Ì Á¸ÀçÇÒ °æ¿ì(ÀçÁ¢¼Ó À¯ÀúÀÏ °æ¿ì)
        {
            // Àü¿ª º¯¼ö·Î displayName¿¡ ÀúÀå
            Debug.Log("»õ·Î¿î displayNameÀÌ ¼³Á¤µÇ¾ú½À´Ï´Ù.");
            SaveDisplayName(); //±âÁ¸ÀÇ ÀÌ¸§À» »õ ÀÌ¸§À¸·Î µ¤¾î¾º¿ö ÀúÀåÇÑ´Ù
            saveText.text = "ÀúÀåµÇ¾ú½À´Ï´Ù"; //ÀúÀå ¸Ş½ÃÁö ¾Ë¸²
        }
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
        else if (displayName == inputText.text && (inputText.isActiveAndEnabled)) //ÀÔ·Â¶õÀÌ ±âÁ¸ ´Ğ³×ÀÓ°ú °°À¸¸é¼­ È°¼ºÈ­µÇ¾îÀÖ´Â °æ¿ì
        {
            warningText.text = "±âÁ¸ ´Ğ³×ÀÓ°ú ´Ş¶ó¾ß ÇÕ´Ï´Ù.";
            confirmButton.interactable = false;
        }
        else
        {
            warningText.text = ""; // ±ÔÄ¢¿¡ ¸ÂÀ¸¸é °æ°í ¸Ş½ÃÁö Á¦°Å
            confirmButton.interactable = true; // È®ÀÎ ¹öÆ° È°¼ºÈ­
        }

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


