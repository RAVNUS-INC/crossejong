using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using TMPro;
using JetBrains.Annotations;
using System;
using Photon.Pun;
using UnityEditor;
using Unity.VisualScripting;
using System.Text.RegularExpressions;

public class TurnChange : MonoBehaviour
{
    public UserCard userCard;
    public TurnManager turnManager; // ÀÚ½ÅÀÇ ÀÎµ¦½º ¹øÈ£ ¾Ë±â À§ÇØ »ç¿ë
    public GameResult gameResult; // °á°ú ÆÇ³Ú È°¼ºÈ­¸¦ À§ÇØ »ç¿ë

    public int userCardCount; // º»ÀÎÀÇ Ä«µå °³¼ö
    public TMP_InputField cardInputField; // Ä«µå ³»°í ÀÔ·ÂÇÏ´Â ÇÊµå
    public Button CardDropBtn; // Ä«µå ³»°í³ª¼­ ´©¸£´Â ¹öÆ° - ÅÏ¿¡ µû¶ó ºñÈ°¼ºÈ­ È°¼ºÈ­
    public string wordInput;
    public bool isContinue;
    public WordLists wordLists;
    public DictionaryAPI dictionaryAPI;

    public List<char> charList = new List<char>
    {'¤¡', '¤¢', '¤¤', '¤§', '¤¨', '¤©', '¤±', '¤²', '¤³', '¤µ', '¤¶', '¤·', '¤¸', '¤¹', '¤º', '¤»', '¤¼', '¤½', '¤¾'};

    

    private void Start()
    {
        // Ä«µå¸¦ ³»°í ÀÎÇ²ÇÊµå¿¡ ÀÔ·ÂÇÒ ¶§ ÇÑ±Û¸¸ ÀÔ·Â °¡´ÉÇÏµµ·Ï ÇÔ
        cardInputField.onValueChanged.AddListener(OnlyKoreanOK);

        //Ä«µå ³»±â ¿Ï·á ¹öÆ°À» Ã³À½¿£ ºñÈ°¼ºÈ­
        CardDropBtn.interactable = false;

        CardDropBtn.onClick.AddListener(() => {
            CardDropBtn.gameObject.SetActive(false); // CardDropBtn ºñÈ°¼ºÈ­
            cardInputField.gameObject.SetActive(true); // cardInputField È°¼ºÈ­
            cardInputField.text = ""; // ÀÎÇ²ÇÊµå ÀÔ·Â¶õÀ» ºñ¿ö³õÀ½
        });
    }


    public void IsCreateWord()
    {
        Debug.Log(ObjectManager.instance.dropCount);
        wordInput = cardInputField.text;
        isContinue = true;

        if (wordInput.Length > ObjectManager.instance.dropCount)
        {
            if (ObjectManager.instance.createdWords.Contains(wordInput))  // ±ÛÀÚ·Î ÀÌ·ç¾îÁø ´Ü¾îÀÏ °æ¿ì
            {
                Debug.Log("±ÛÀÚ·Î¸¸ ÀÌ·ç¾îÁø ´Ü¾î¸¦ »çÀü API °Ë»ç¸¦ ½ÃÀÛÇÕ´Ï´Ù");
                // wordInput (»çÀü API °Ë»ç µ¹¸®±â)
                ObjectManager.instance.dropCount = 0;
                ObjectManager.instance.inputWords = wordInput;
                StartCoroutine(dictionaryAPI.CheckWordExists(wordInput));
            }

            for (int i = 0; i < ObjectManager.instance.createdWords.Length; i++)
            {
                if (isContinue == false)
                {
                    break;
                }
                else
                {
                    for (int j = 0; j < 19; j++)
                    {
                        if (ObjectManager.instance.createdWords[i] == charList[j])  // ÀÚÀ½Ä«µå°¡ Æ÷ÇÔµÈ °æ¿ì
                        {
                            List<char> words = wordLists.choDictionary[charList[j]];
                            for (int k = 0; k < 588; k++)
                            {
                                if (wordInput[i] == words[k])
                                {
                                    Debug.Log("ÀÚÀ½ Ä«µå·Î ÀÌ·ç¾îÁø ´Ü¾î¸¦ »çÀü API °Ë»ç¸¦ ½ÃÀÛÇÕ´Ï´Ù");
                                    // wordInput (»çÀü API °Ë»ç µ¹¸®±â)
                                    isContinue = false;
                                    ObjectManager.instance.dropCount = 0;
                                    ObjectManager.instance.inputWords = wordInput;
                                    StartCoroutine(dictionaryAPI.CheckWordExists(wordInput));
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < ObjectManager.instance.createdWords.Length; i++)
            {
                if (ObjectManager.instance.createdWords[i] == 'C' || ObjectManager.instance.createdWords[i] == 'B')  // Æ¯¼öÄ«µå°¡ Æ÷ÇÔµÈ °æ¿ì
                {
                    if (44032 <= wordInput[i] && wordInput[i] <= 54616)
                    {
                        Debug.Log("Æ¯¼ö Ä«µå·Î ÀÌ·ç¾îÁø ´Ü¾î¸¦ »çÀü API °Ë»ç¸¦ ½ÃÀÛÇÕ´Ï´Ù");
                        // wordInput (»çÀü API °Ë»ç µ¹¸®±â)
                        ObjectManager.instance.dropCount = 0;
                        ObjectManager.instance.inputWords = wordInput;
                        StartCoroutine(dictionaryAPI.CheckWordExists(wordInput));
                        break;
                    }
                }
                else
                    break;
            }
        }
        else
        {
            Debug.Log("¿À·ùÀÔ´Ï´Ù");
        }
    }

    public void OnlyKoreanOK(string text) // ´Ü¾î ÀÔ·ÂÇÊµå¿¡ ÇÑ±Û¸¸ ÀÛ¼ºÇÒ ¼ö ÀÖµµ·Ï ÇÔ
    {
        // ÇÑ±ÛÀ» Á¦¿ÜÇÑ ¸ğµç ¹®ÀÚ Á¦¿Ü
        // ÇÑ±Û¸¸ Çã¿ëÇÏ´Â Á¤±Ô½Ä (¶ç¾î¾²±â Æ÷ÇÔ X)
        string koreanPattern = "^[°¡-ÆR]*$";

        if (!Regex.IsMatch(text, koreanPattern))
        {
            cardInputField.text = Regex.Replace(text, "[^°¡-ÆR]", ""); // ÇÑ±Û ÀÌ¿ÜÀÇ ¹®ÀÚ Á¦°Å
        }
    }

    public void TurnEnd()
    {
        ObjectManager.instance.dropCount = 0;

        // ÀÚ½ÅÀÇ UI ÀÎµ¦½º È®ÀÎ ¹× ¾÷µ¥ÀÌÆ®
        turnManager.FindMyIndex();

        CountUserCard(userCard.displayedCards.Count);
    }

    public void CountUserCard(int count) //ÀÚ½ÅÀÇ Ä«µå °³¼ö ¾÷µ¥ÀÌÆ®
    {
        userCardCount = count; // º¯¼ö¿¡ °³¼ö ÀúÀå

        // ¸ğµÎ¿¡°Ô ÀÚ½ÅÀÇ Ä«µå °³¼ö Àü´Ş ¿äÃ»ÇÏ±â - ÀÚ½ÅÀÇ Ä«µå°³¼ö, ÀÚ½ÅÀÇ ÀÎµ¦½º ¹øÈ£
        turnManager.photonView.RPC("SyncAllCardCount", RpcTarget.All, userCardCount, turnManager.MyIndexNum);

        if (userCardCount == 0) // Ä«µå¸¦ ´Ù ¼ÒÁøÇßÀ» ¶§ - Ä«µå °³¼ö°¡ ÇöÀç 0°³ÀÌ¸é
        {
            // ³îÀÌ°¡ Á¾·áµÇ¾úÀ½À» ¾Ë¸®´Â ¸Ş½ÃÁö 1ÃÊ Á¤µµ Ç¥½Ã ÈÄ °á°ú Ã¢ ¶ç¿ì±â
            gameResult.EndGameDelay();
        }
        else if ((userCardCount > 0) && (ObjectManager.instance.IsFirstTurn)) // Ä«µå´Â ³²¾ÆÀÖ°í Áö±İÀÌ Ã¹ ÅÏ¿¡¼­ÀÇ ÇÔ¼ö È£ÃâÀÌ¶ó¸é(Ã¹ »óÈ²¿¡¼­ Ä«µå °³¼ö ¾÷µ¥ÀÌÆ®¸¦ À§ÇÔ)
        {
            // ÅÏ ³Ñ±â±â ¹æÁö¸¦ À§ÇÑ º¯¼ö¸¦ ÀÌÁ¦´Â false·Î º¯°æ
            ObjectManager.instance.IsFirstTurn = false;

            return; // ÅÏÀ» ³Ñ±âÁö ¾ÊÀ½
        }
        else // Ã¹ ÅÏÀÌ ¾Æ´Ñ ÀçÈ£ÃâÀÌ¶ó¸é ÅÏÀ» ³Ñ±è
        {
            turnManager.FindNextPlayer();
        }

    }


}