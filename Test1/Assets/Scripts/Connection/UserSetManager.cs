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


// ¿Ø¿˙ «¡∑Œ«  º≥¡§ »≠∏Èø°º≠ ¿€µø«œ¥¬ ƒ⁄µÂ(¥–≥◊¿”, ¿Ø¿˙«¡∑Œ« ªÁ¡¯ ∫Ø∞Ê ¿˙¿Â)
public class UserSetManager : MonoBehaviourPunCallbacks
{
    private UserSetManager s_instance;
    public UserSetManager Instance { get { return s_instance; } }

    [SerializeField] InputField inputText; //¥–≥◊¿” ¿‘∑¬(≥™¡ﬂø° πŸ≤‹ ºˆ ¿÷¥¬ Displayname)
    [SerializeField] Button confirmButton; //¡¶√‚(¿˙¿Â) πˆ∆∞
    [SerializeField] Text warningText; // ∞Ê∞Ì ∏ﬁΩ√¡ˆ∏¶ √‚∑¬«“ UI ≈ÿΩ∫∆Æ
    [SerializeField] Text saveText; // ¿˙¿Âøœ∑· ∏ﬁΩ√¡ˆ∏¶ √‚∑¬«“ UI ≈ÿΩ∫∆Æ

    [SerializeField] private RawImage rawImage;  // º±≈√µ» ¿ÃπÃ¡ˆ∏¶ «•Ω√«“ UI
    [SerializeField] private GameObject imagePrefab; // «¡∑Œ« ªÁ¡¯ «¡∏Æ∆’ (UI > Image «¸≈¬)
    [SerializeField] private Transform canvasTransform; // Canvas¿« Transform

    private const int MaxLength = 8; // √÷¥Î ¿‘∑¬ ±Ê¿Ã(∫Øµø∞°¥…)

    void Awake()
    {
        // ΩÃ±€≈Ê ¿ŒΩ∫≈œΩ∫∏¶ º≥¡§
        if (s_instance == null)
        {
            s_instance = this;
        }
        else if (s_instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject); // æ¿¿Ã ∫Ø∞Êµ«¥ı∂Ûµµ ∞¥√º∞° ∆ƒ±´µ«¡ˆ æ µµ∑œ º≥¡§
    }

    void Start()
    {
        confirmButton.interactable = false; // ±‚∫ª¿˚¿∏∑Œ πˆ∆∞ ∫Ò»∞º∫»≠
        warningText.text = ""; // √ ±‚ ∞Ê∞Ì ∏ﬁΩ√¡ˆ ∫ÒøÏ±‚
        saveText.text = ""; // √ ±‚ ¿˙¿Â ∏ﬁΩ√¡ˆ ∫ÒøÏ±‚

        //≥ªøÎ¿Ã ∫Ø∞Êµ«æ˙¿ª∂ß ±‘ƒ¢ ∞ÀªÁ
        inputText.onValueChanged.AddListener(ValidateNickname);

        //»Æ¿Œ πˆ∆∞ ¥©∏£∏È ¿Ã∏ß ¿˙¿Â
        confirmButton.onClick.AddListener(DisplayName);

    }

    //¿Ã∏ß ∫Ø∞Ê πˆ∆∞ ≈¨∏Ø«“ ∂ß -> ¿Ã∏ß ¿‘∑¬ ¿Œ«≤ »∞º∫»≠
    public void ChangeNameBtn() 
    {
        inputText.interactable = true; //¿Ã∏ß ∫Ø∞Ê »∞º∫»≠
    }
     
    //¿Ã∏ß º≥¡§ ±‘ƒ¢
    public void ValidateNickname(string input)
    {
        /// «—±€(øœº∫«¸/¿⁄¿Ω/∏¿Ω)∞˙ º˝¿⁄∏∏ «„øÎ«œ¥¬ ¡§±‘Ωƒ
        string validPattern = @"^[∞°-∆R§°-§æ§ø-§”0-9]*$";

        // ∞¯πÈ ¡¶∞≈
        input = input.Replace(" ", ""); //∞¯πÈ¿ª «„øÎ«œ¡ˆ æ ¥¬¥Ÿ

        // ¿‘∑¬ ∞™¿Ã ∆–≈œø° ∏¬¡ˆ æ ¿∏∏È ºˆ¡§
        if (!Regex.IsMatch(input, validPattern))
        {
            warningText.text = "«—±€∞˙ º˝¿⁄∏∏ ¿‘∑¬ ∞°¥…«’¥œ¥Ÿ.";
            confirmButton.interactable = false; 
        }
        else if (input.Length > MaxLength) // ±Ê¿Ã ¡¶«— √ ∞˙ ∞ÀªÁ
        {
            warningText.text = $"√÷¥Î {MaxLength}¿⁄±Ó¡ˆ∏∏ ¿‘∑¬ ∞°¥…«’¥œ¥Ÿ.";
            confirmButton.interactable = false; 
        }
        else if (input.Length == 0) // ∫Û πÆ¿⁄ø≠ ∞ÀªÁ
        {
            warningText.text = "¥–≥◊¿”¿ª ¿‘∑¬«ÿ¡÷ººø‰.";
            confirmButton.interactable = false; 
        }
        else
        {
            warningText.text = ""; // ±‘ƒ¢ø° ∏¬¿∏∏È ∞Ê∞Ì ∏ﬁΩ√¡ˆ ¡¶∞≈
            confirmButton.interactable = true; // »Æ¿Œ πˆ∆∞ »∞º∫»≠
        }

    }

    //º≥¡§«— ¿Ã∏ß ¿˙¿Â
    public void DisplayName() //DisplayName: ∞Ì¿Ø«œ¡ˆ æ ¿Ω
    {
        var request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request,
             result =>
             {
                 // displayName¿Ã ¡∏¿Á«œ¥¬¡ˆ »Æ¿Œ(√π ¿Ø¿˙¿Œ¡ˆ æ∆¥—¡ˆ∏¶ »Æ¿Œ)
                 if (string.IsNullOrEmpty(result.AccountInfo.TitleInfo.DisplayName))
                 {
                     // displayName¿Ã æ¯¿ª ∞ÊøÏ (√π ¿Ø¿˙¿œ ∞ÊøÏ)
                     Debug.Log("displayName¿Ã º≥¡§µ«¡ˆ æ æ“Ω¿¥œ¥Ÿ. º≥¡§ ¡ﬂ...");
                     SaveDisplayName(); //¥–≥◊¿”¿ª ¿˙¿Â«œ∞Ì
                     OnClickConnect(); //∏ﬁ¿Œ¿∏∑Œ º≠πˆ¡¢º”¿ª ø‰√ª«—¥Ÿ
                 }
                 else //displayname¿Ã ¿ÃπÃ ¡∏¿Á«“ ∞ÊøÏ(¿Á¡¢º” ¿Ø¿˙¿œ ∞ÊøÏ)
                 {
                     SaveDisplayName(); //±‚¡∏¿« ¿Ã∏ß¿ª ªı ¿Ã∏ß¿∏∑Œ µ§æÓæ∫øˆ ¿˙¿Â«—¥Ÿ
                     saveText.text = "¿˙¿Âµ«æ˙Ω¿¥œ¥Ÿ"; //¿˙¿Â ∏ﬁΩ√¡ˆ æÀ∏≤
                 }
             },
            error =>
            {
                Debug.LogError($"¿Ø¿˙ ¡§∫∏ ∫“∑Øø¿±‚ Ω«∆–: {error.GenerateErrorReport()}");
            });
    }

    public void SaveDisplayName() //¥‹º¯»˜ ¿Ã∏ß ¿˙¿Â«œ±‚
    {
        string displayName = inputText.text.Trim();

        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
           result =>
           {
               Debug.Log($"¥–≥◊¿” ¿˙¿Â º∫∞¯: {result.DisplayName}");
           },

           error =>
           {
               Debug.LogError($"¥–≥◊¿” ¿˙¿Â Ω«∆–: {error.GenerateErrorReport()}");
           });
    }

    // ∞∂∑Ø∏Æø°º≠ ¿ÃπÃ¡ˆ º±≈√
    public void OpenGallery()
    {
        if (NativeGallery.IsMediaPickerBusy())
            return;

        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                StartCoroutine(LoadImage(path)); // ¿ÃπÃ¡ˆ∏¶ ∑ŒµÂ«œø© «•Ω√
            }
        });

        Debug.Log("Gallery permission: " + permission);
    }

    // ¿ÃπÃ¡ˆ ∑ŒµÂ π◊ ¿˚øÎ
    private IEnumerator LoadImage(string path)
    {
        // ∞∂∑Ø∏Æø°º≠ º±≈√«— ¿ÃπÃ¡ˆ∏¶ Texture2D∑Œ ∑ŒµÂ
        Texture2D texture = NativeGallery.LoadImageAtPath(path);
        if (texture == null)
        {
            Debug.LogError("Failed to load image.");
            yield break;
        }

        // º±≈√µ» ¿ÃπÃ¡ˆ∏¶ RawImageø° ¿˚øÎ (Texture2D)
        if (rawImage != null)
        {
            rawImage.texture = texture;

            // RawImage¿« RectTransform¿ª ªÁøÎ«œø© 1:1 ∫Ò¿≤∑Œ ¿Ø¡ˆ
            float aspectRatio = (float)texture.width / texture.height;
            if (aspectRatio > 1)
            {
                rawImage.rectTransform.sizeDelta = new Vector2(texture.width, texture.width); // ≥ ∫Ò ±‚¡ÿ¿∏∑Œ ≈©±‚ º≥¡§
            }
            else
            {
                rawImage.rectTransform.sizeDelta = new Vector2(texture.height, texture.height); // ≥Ù¿Ã ±‚¡ÿ¿∏∑Œ ≈©±‚ º≥¡§
            }
        }

        // ¿ÃπÃ¡ˆ «¡∏Æ∆’ø° ¿˚øÎ
        if (imagePrefab != null)
        {
            GameObject newImageObject = Instantiate(imagePrefab, canvasTransform);
            Image imageComponent = newImageObject.GetComponent<Image>();
            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            imageComponent.sprite = newSprite;

            // ¿ÃπÃ¡ˆ ∫Ò¿≤ ¿Ø¡ˆ
            imageComponent.preserveAspect = true; // ∫Ò¿≤¿ª ¿Ø¡ˆ«œµµ∑œ º≥¡§

            // «¡∏Æ∆’¿« ≈©±‚ º≥¡§ (¿ÃπÃ¡ˆ ∫Ò¿≤ø° ∏¬∞‘)
            float imageAspectRatio = (float)texture.width / texture.height;
            if (imageAspectRatio > 1)
            {
                newImageObject.GetComponent<RectTransform>().sizeDelta = new Vector2(texture.width, texture.width); // ≥ ∫Ò ±‚¡ÿ
            }
            else
            {
                newImageObject.GetComponent<RectTransform>().sizeDelta = new Vector2(texture.height, texture.height); // ≥Ù¿Ã ±‚¡ÿ
            }
        }

        Debug.Log("Image successfully applied!");
    }




    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("∏∂Ω∫≈Õ º≠πˆ ¡¢º” º∫∞¯");

        //≥™¿« ¿Ã∏ß¿ª ∆˜≈Êø° º≥¡§
        PhotonNetwork.NickName = inputText.text;

        //∑Œ∫Ò¡¯¿‘
        PhotonNetwork.JoinLobby();
    }

    //Lobby ¡¯¿‘ø° º∫∞¯«ﬂ¿∏∏È »£√‚µ«¥¬ «‘ºˆ
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        //∏ﬁ¿Œ æ¿¿∏∑Œ ¿Ãµø
        PhotonNetwork.LoadLevel("Main");

        print("∑Œ∫Ò ¡¯¿‘ º∫∞¯");

    }
    public void OnClickConnect()
    {
        // ∏∂Ω∫≈Õ º≠πˆ ¡¢º” ø‰√ª
        PhotonNetwork.ConnectUsingSettings();
    }

    private void OnDestroy()
    {
        // ¿Ã∫•∆Æ «ÿ¡¶
        inputText.onValueChanged.RemoveListener(ValidateNickname);
    }

    
}
