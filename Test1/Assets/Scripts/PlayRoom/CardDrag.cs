using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System.Drawing;
using System;
using Photon.Pun;
using TMPro;

// CardDrag Ŭ������ Unity���� �巡�� ������ ī�带 �����ϴ� ��ũ��Ʈ
public class CardDrag : MonoBehaviourPun, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public UserCard userCard; // UserCard ����
    public GameObject beingDraggedCard; // �巡�� ���� ī��
    private Vector3 startPosition; // ī�� ���� ��ġ
    [SerializeField] public Transform onDragParent; // �巡�� �� ī�尡 ��ġ�� �θ�
    [HideInInspector] public Transform startParent; // ���� �θ�
    public int cardIndex;


    public void OnBeginDrag(PointerEventData eventData)
    {
        ObjectManager.instance.AlaramMsg.gameObject.SetActive(false);

        if (eventData == null) return;
        //if (eventData.pointerId >= 0) return;

        print(gameObject.name);

        ObjectManager.instance.moveCardObejct = gameObject;

        // ī���� ���� ��ġ�� �θ� ����
        startPosition = transform.position;
        startParent = transform.parent;

        ObjectManager.instance.startDragPosition = startPosition;

        // ī�尡 UI ����� ��� Raycast ������ ����
        GetComponent<Image>().raycastTarget = false;

        // �巡���� ī���� �θ� onDragParent�� ����
        transform.SetParent(onDragParent);

    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 newPosition;

        // PC�� ����� �Է� ó��
        if (Input.touchCount > 0)
        {
            // ����� ��ġ �Է�
            newPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, -Camera.main.transform.position.z));
        }
        else
        {
            // PC ���콺 �Է�
            newPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        }

        // �巡�� �߿��� ī�尡 ���콺 �Ǵ� ��ġ ��ġ�� �������� ����
        transform.position = newPosition;

        // ���� �޽��� ������Ʈ �Լ� ȣ��
        ObjectManager.instance.ShowCardSelectingMessage(true);

        //eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<Image>().raycastTarget = true;// Raycast�� �ٽ� Ȱ��ȭ

        // �巡�װ� ���� ��, ī�尡 ���� ��ġ�� ���ƿ����� ó��
        if (transform.parent == onDragParent)
        {
            transform.position = startPosition;
            transform.SetParent(startParent);
            transform.SetSiblingIndex(cardIndex);
            ObjectManager.instance.moveCardObejct = null;

            CardPool.instance.SortCardIndex(UserCard.instance.displayedCards);

            // ī�� �ʵ�� �ű�� UserCard, UserCardPopup ���� �ű�� ����
            // �ϴ� ī�� ������ �׸��� ���� ���� �˾Ƽ�
        }
    }

    
}