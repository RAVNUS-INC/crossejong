using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UserCard : MonoBehaviour
{
    public List<GameObject> cardPrefabs; // ī�� ������ ����Ʈ
    public Transform cardContainer; // Scroll View�� Content
    public Button createCardButton; // ī�� ���� ��ư

    private List<GameObject> displayedCards = new List<GameObject>(); // ȭ�鿡 ǥ�õ� ī�� ����Ʈ
    private HashSet<int> selectedCardIndices = new HashSet<int>(); // ���õ� ī�� �ε��� ����

    void Start()
    {
        createCardButton.onClick.AddListener(OnCreateCard); // ��ư Ŭ�� �̺�Ʈ�� ī�� ���� �޼��� �߰�
    }

    private void OnCreateCard()
    {
        createCardButton.gameObject.SetActive(false); // ���� ���� ��ư ������ ������� �ϱ�

        CreateCards(); // ī�� ���� ����
    }

    public void CreateCards()
    {
        // �̹� ǥ�õ� ī�尡 �ִٸ� ����
        foreach (var card in displayedCards)
        {
            Destroy(card);
        }
        displayedCards.Clear(); // ����Ʈ �ʱ�ȭ
        selectedCardIndices.Clear(); // ���õ� ī�� �ε��� �ʱ�ȭ

        // �������� 11���� ī�� �ε����� ����
        while (selectedCardIndices.Count < 11)
        {
            int randomIndex = Random.Range(0, cardPrefabs.Count);
            selectedCardIndices.Add(randomIndex);
        }

        // ���õ� ī�� �ε����� ���� ī�� ����
        int i = 0;
        foreach (int index in selectedCardIndices)
        {
            GameObject cardInstance = Instantiate(cardPrefabs[index], cardContainer);
            RectTransform rectTransform = cardInstance.GetComponent<RectTransform>();

            // ī�� ũ�� ���� (200x200)
            rectTransform.sizeDelta = new Vector2(200, 200);

            // ī���� ��������Ʈ ������ �̹� �����տ� �����Ǿ� �����Ƿ� ���⼭�� �ʿ� ����
            // ī�� �����տ��� �ո�� �޸� ��������Ʈ�� �̹� �����Ǿ� �־�� �մϴ�.

            displayedCards.Add(cardInstance); // ������ ī�带 ����Ʈ�� �߰�
            i++;
        }
    }
}
