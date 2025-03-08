using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using TMPro;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun;


public class GetCard : MonoBehaviourPun

{
    public CardPool cardPool;
    public UserCard userCard;
    public CardDrag cardDrag;
    public Button getCardButton;
    public TurnChange turnChange; // ī�� ���� ������Ʈ�� ���� ���

    public void Start()
    {
        // ó���� ī�� �߰� ��ư ��Ȱ��ȭ - �ڽ��� ���� ���� �ٷ� Ȱ��ȭ
        getCardButton.interactable = false;
    }
    public void GetCardToUserCard()
    {
        int Mynum = PhotonNetwork.LocalPlayer.ActorNumber;
        photonView.RPC("RequestRandomCards", RpcTarget.MasterClient, 1, Mynum);
        Debug.Log("ī�带 �� �� �߰��մϴ�.");
    }

    [PunRPC]
    void RequestRandomCards(int count, int requestingPlayer)
    {
        // ������ ī�带 �������� �̴� �Լ�
        string[] usedNames = cardPool.GetRandomCardsName(count);

        // ��û�� �÷��̾�Ը� ����� ����
        Photon.Realtime.Player targetPlayer = PhotonNetwork.CurrentRoom.GetPlayer(requestingPlayer);
        if (targetPlayer != null)
        {
            photonView.RPC("ReceiveRandomCards", targetPlayer, string.Join(", ", usedNames));
        }
    }

    [PunRPC]
    void ReceiveRandomCards(string usedNames)
    {
        string[] cardNames = usedNames.Split(','); // �ٽ� �迭�� ��ȯ
        // ���� ī�� �̸� �迭�� ����Ͽ� ī�� ��ü�� �����ϴ� �Լ�
        List<GameObject> randomCards = cardPool.GetRandomCardsObject(cardNames);

        if (cardPool.cards.Count > 0)
        {
            cardPool.GetCardsToTarGetArea(randomCards, userCard.userCardContainer, userCard.displayedCards);

            cardDrag = randomCards[0].GetComponent<CardDrag>();
            if (cardDrag == null)
            {
                cardDrag = randomCards[0].AddComponent<CardDrag>(); // CardDrag ������Ʈ �߰�
                cardDrag.onDragParent = ObjectManager.instance.moveItemPanel;
            }
        }
        else
        {
            getCardButton.interactable = false;
        }

        // �����ǿ� �ִ� ī�带 �ٽ� �������� �ǵ�������
        turnChange.RollBackAreas();

        // ī�� ���� UI ������Ʈ ��û
        turnChange.TurnEnd();
    }
}
