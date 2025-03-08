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
    public TurnChange turnChange; // 카드 개수 업데이트를 위해 사용

    public void Start()
    {
        // 처음엔 카드 추가 버튼 비활성화 - 자신의 턴이 오면 바로 활성화
        getCardButton.interactable = false;
    }
    public void GetCardToUserCard()
    {
        int Mynum = PhotonNetwork.LocalPlayer.ActorNumber;
        photonView.RPC("RequestRandomCards", RpcTarget.MasterClient, 1, Mynum);
        Debug.Log("카드를 한 장 추가합니다.");
    }

    [PunRPC]
    void RequestRandomCards(int count, int requestingPlayer)
    {
        // 방장이 카드를 랜덤으로 뽑는 함수
        string[] usedNames = cardPool.GetRandomCardsName(count);

        // 요청한 플레이어에게만 결과를 전달
        Photon.Realtime.Player targetPlayer = PhotonNetwork.CurrentRoom.GetPlayer(requestingPlayer);
        if (targetPlayer != null)
        {
            photonView.RPC("ReceiveRandomCards", targetPlayer, string.Join(", ", usedNames));
        }
    }

    [PunRPC]
    void ReceiveRandomCards(string usedNames)
    {
        string[] cardNames = usedNames.Split(','); // 다시 배열로 변환
        // 받은 카드 이름 배열을 사용하여 카드 객체를 생성하는 함수
        List<GameObject> randomCards = cardPool.GetRandomCardsObject(cardNames);

        if (cardPool.cards.Count > 0)
        {
            cardPool.GetCardsToTarGetArea(randomCards, userCard.userCardContainer, userCard.displayedCards);

            cardDrag = randomCards[0].GetComponent<CardDrag>();
            if (cardDrag == null)
            {
                cardDrag = randomCards[0].AddComponent<CardDrag>(); // CardDrag 컴포넌트 추가
                cardDrag.onDragParent = ObjectManager.instance.moveItemPanel;
            }
        }
        else
        {
            getCardButton.interactable = false;
        }

        // 보드판에 있던 카드를 다시 슬롯으로 되돌려놓기
        turnChange.RollBackAreas();

        // 카드 개수 UI 업데이트 요청
        turnChange.TurnEnd();
    }
}
