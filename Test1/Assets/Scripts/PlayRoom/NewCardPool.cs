//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections.Generic;

//public class NewCardPool : MonoBehaviour
//{
//    public Transform cardContainer; // ī�� �θ� ������Ʈ
//    public List<GameObject> cardList = new List<GameObject>(); // ī�� ���

//    void Start()
//    {
//        // ī�� ����
//        CreateCard();
//    }


//    private void CreateCard()
//    {
//        // ī�� 1~54 ����
//        for (int i = 0; i < 54; i++)
//        {
//            Button card = Instantiate(cardPrefab, cardContainer);
//            BeforeCard cardComponent = card.GetComponent<BeforeCard>();

//            // �ո�� �޸� �̹��� ����
//            cardComponent.InitializeCard(cardBackImage, cardFrontSprites[i]);

//            // ī�� ����Ʈ�� �߰�
//            cards.Add(card);
//        }

//    }

//    // �������� 11���� ī�� ����
//    public List<GameObject> GetRandomCards(int count)
//    {
//        List<GameObject> randomCards = new List<GameObject>();
//        HashSet<int> usedIndices = new HashSet<int>();

//        while (randomCards.Count < count)
//        {
//            int randomIndex = Random.Range(0, cards.Count);
//            if (!usedIndices.Contains(randomIndex))
//            {
//                usedIndices.Add(randomIndex);
//                randomCards.Add(cards[randomIndex]);
//            }
//        }

//        return randomCards;
//    }


//    // ī�� �̵�
//    public void MoveCardToParent(GameObject card, Transform parent)
//    {
//        card.transform.SetParent(parent, false);
//    }
//}
