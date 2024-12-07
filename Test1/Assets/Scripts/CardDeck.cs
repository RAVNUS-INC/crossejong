using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardDeck : MonoBehaviour
{
    public List<GameObject> cardPrefabs; // ī�� ������ ����Ʈ
    public Transform cardContainer; // ī�带 ��ġ�� �����̳�
    public Button createCardButton; // ī�� ���� ��ư
    public Button appealButton; // ���ǽ�û ��ư
    public Button changeCardButton; // ����ī�� �ٲٱ� ��ư
    public OptionPopup optionPopup; //OptionPopup ��ũ��Ʈ ����

    public float cardSpacing = 115f; // ī�� ����
    public Vector2 startPosition = new Vector2(385, 142); // ī�� ���� ��ġ

    private List<GameObject> displayedCards = new List<GameObject>(); // ȭ�鿡 ǥ�õ� ī�� ����Ʈ
    private HashSet<int> selectedCardIndices = new HashSet<int>(); // ���õ� ī�� �ε��� ����

    void Start()
    {
        createCardButton.onClick.AddListener(OnCreateCard); // ��ư Ŭ�� �̺�Ʈ�� ī�� ���� �޼��� �߰�
        optionPopup.openOptionPopupButton.onClick.AddListener(optionPopup.OpenPopup);  // �ɼ� �˾� ���� ��ư�� �̺�Ʈ �߰�
        optionPopup.closeOptionPopupButton.onClick.AddListener(optionPopup.ClosePopup);  // �ɼ� �˾� �ݱ� ��ư�� �̺�Ʈ �߰�

        appealButton.gameObject.SetActive(false); // ���ǽ�û ��ư ��Ȱ��ȭ
        changeCardButton.gameObject.SetActive(false); // ����ī�� �ٲٱ� ��ư ��Ȱ��ȭ
        optionPopup.optionPopupPanel.SetActive(false); // �ɼ� �˾� ��Ȱ��ȭ
        optionPopup.openOptionPopupButton.gameObject.SetActive(false); // �ɼ� �˾� ���� ��ư�� ��Ȱ��ȭ
        optionPopup.closeOptionPopupButton.gameObject.SetActive(false); // �ɼ� �˾� �ݱ� ��ư�� ��Ȱ��ȭ
    }

    private void OnCreateCard()
    {
        createCardButton.gameObject.SetActive(false); // ���� ���� ��ư ������ ������� �ϱ�

        CreateCards(); // ī�� ���� ����

        appealButton.gameObject.SetActive(true); // ���ǽ�û ��ư Ȱ��ȭ
        changeCardButton.gameObject.SetActive(true); // ����ī�� �ٲٱ� ��ư Ȱ��ȭ
        optionPopup.openOptionPopupButton.gameObject.SetActive(true); // �ɼ� �˾� ���� ��ư�� Ȱ��ȭ
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

            // ī�� ��ġ ����
            rectTransform.anchoredPosition = new Vector2(startPosition.x + (i * cardSpacing), startPosition.y);

            // ī���� ��������Ʈ ������ �̹� �����տ� �����Ǿ� �����Ƿ� ���⼭�� �ʿ� ����
            // ī�� �����տ��� �ո�� �޸� ��������Ʈ�� �̹� �����Ǿ� �־�� �մϴ�.

            displayedCards.Add(cardInstance); // ������ ī�带 ����Ʈ�� �߰�
            i++;
        }
    }
}
