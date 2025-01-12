//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using System.Collections.Generic;
//using System.Collections;

//public class Player : MonoBehaviour
//{
//    public List<string> playerLists = new List<string>
//       {"ȣ����", "�浿��", "������" };
//    public Transform overallContainer; // PlayRoomPanel
//    public CardPool cardPool; // CardPool ����

//    void Start()
//    {
//        CreatePlayerInfo(playerLists, Color.black);
//    }

//    void CreatePlayerInfo(List<string> playerList, Color color)
//    {
//        for (int i = 0; i < playerList.Count; i++)
//        {
//            // ī�� GameObject ����
//            GameObject player = new GameObject(playerList[i]);
//            player.transform.SetParent(overallContainer, false);

//            // RectTransform ����
//            RectTransform rectTransform = player.AddComponent<RectTransform>();
//            rectTransform.sizeDelta = new Vector2(100, 100); // ī�� ũ�� ����

//            // Button ������Ʈ �߰�
//            Button playerInfo = player.AddComponent<Button>();
//            playerInfo.interactable = false;

//            GameObject textObjectCountDown = new GameObject("CountDown");
//            textObjectCountDown.transform.SetParent(player.transform, false);
//            RectTransform textRectCountDown = textObjectCountDown.AddComponent<RectTransform>();
//            textRectCountDown.anchorMin = Vector2.zero;
//            textRectCountDown.anchorMax = Vector2.one;
//            textRectCountDown.offsetMin = Vector2.zero;
//            textRectCountDown.offsetMax = Vector2.zero;

//            TextMeshProUGUI tmpTextCountDown = textObjectCountDown.AddComponent<TextMeshProUGUI>();
//            tmpTextCountDown.font = cardPool.newFont;
//            tmpTextCountDown.text = "25"; // �ؽ�Ʈ ����
//            tmpTextCountDown.fontSize = 120;
//            tmpTextCountDown.alignment = TextAlignmentOptions.Center;
//            tmpTextCountDown.color = color;

//            GameObject textObjectPlayerName = new GameObject("PlayerName");
//            textObjectPlayerName.transform.SetParent(player.transform, false);
//            RectTransform textRectPlayerName = textObjectPlayerName.AddComponent<RectTransform>();
//            textRectPlayerName.anchorMin = Vector2.zero;
//            textRectPlayerName.anchorMax = Vector2.one;
//            textRectPlayerName.offsetMin = Vector2.zero;
//            textRectPlayerName.offsetMax = Vector2.zero;

//            TextMeshProUGUI tmpText = textObjectPlayerName.AddComponent<TextMeshProUGUI>();
//            tmpText.font = cardPool.newFont;
//            tmpText.text = playerList[i]; // �ؽ�Ʈ ����
//            tmpText.fontSize = 120;
//            tmpText.alignment = TextAlignmentOptions.Center;
//            tmpText.color = color;
//        }
//    }

//    private void StartCountDown()
//    {
//        StartCoroutine(CountDownRoutine(3));
//    }

//    private IEnumerator CountDownRoutine(int count)
//    {
//        // ī��Ʈ�ٿ� ǥ��
//        while (count > 0)
//        {
//            countDownText.text = count.ToString(); // TMP_Text�� ����
//            yield return new WaitForSeconds(1f);
//            count--;
//        }

//        // "����!" ǥ��
//        countDownText.text = "Start!"; // TMP_Text�� ����
//        yield return new WaitForSeconds(startDelay);

//        countDownText.gameObject.SetActive(false); // ī��Ʈ�ٿ� �ؽ�Ʈ ����
//        StartGame();

//    }
//}
