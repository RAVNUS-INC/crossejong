using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SaveGoogleSheets : MonoBehaviour
{
    public static SaveGoogleSheets instance;
    private const string WebAppUrl = "https://script.google.com/macros/s/AKfycbysIGMOIM3bZK4t6aaXw3fQPaRJ098Gpas4aXFZ59cZvEwOcCO1dR4mh8SF3hgg_wvv/exec";




    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void OnUserCreatesWord(string userBirthYear, string newWord)
    {
        string cardLevel = LobbyManager.instance.selectedDifficulty;
        StartCoroutine(SendWordToGoogleSheets(userBirthYear, cardLevel, newWord));
    }

    private IEnumerator SendWordToGoogleSheets(string birthYear, string cardLevel, string word)
    {
        WWWForm form = new WWWForm();
        form.AddField("birthYear", birthYear);
        form.AddField("cardLevel", cardLevel);
        form.AddField("word", word);

        using (UnityWebRequest www = UnityWebRequest.Post(WebAppUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("단어 저장 성공: " + word);
            }
            else
            {
                Debug.LogError("단어 저장 실패: " + www.error);
            }
        }
    }
}
