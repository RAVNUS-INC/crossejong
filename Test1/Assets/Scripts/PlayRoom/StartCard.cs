using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StartCard : MonoBehaviour
{
    /*
    public UserCard userCard; // UserCard ����

    void Start()
    {
        
    }

    public void CreateStartCard()
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

            displayedCards.Add(cardInstance); // ������ ī�带 ����Ʈ�� �߰�
            i++;
        }
    }
    */
}