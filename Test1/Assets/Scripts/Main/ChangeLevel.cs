using System.Collections.Generic;
using UnityEngine;
//�纻
public class ChangeLevel : MonoBehaviour
{   //�Ͻ��� ������ (objectmanger�� ���߿� �� ����)
    public List<string> cardFrontRed;
    public List<string> cardFrontSpecial;
    public List<string> cardFrontBlack;

    public void ChangeLevelLow()
    {
        cardFrontRed = new List<string>
       {"��", "��", "��" };

        cardFrontBlack = new List<string>
       {"��", "��", "��", "��", "��", "��", "��",
        "��",
        "��", "��", "��", "��", "��",
        "��", "��", "��",
        "��",
        "��", "��", "��",
        "��", "��", "��", "��", "��", "��", "��", "��",
        "��", "��", "��", "��", "��", "��", "��", "��", "��", "��",
        "��", "��", "��", "��", "��", "��", "��", "��",
        "��", "��", "��" };
        Debug.Log(string.Join(", ", cardFrontBlack));

        cardFrontSpecial = new List<string>
       {"C", "B"};
    }

    public void ChangeLevelMiddle()
    {
        cardFrontRed = new List<string>
       {"��", "��", "��" };

        cardFrontBlack = new List<string>
       {"��", "��", "��", "��", "��", "��", "��", "��",
        "��", "��", "��",
        "��", "��", "��",
        "��",
        "��", "��", "��", "��",
        "��", "��", "��", "��", "��", "��", "��", "��", "��", "��",
        "��", "��", "��", "��", "��", "��", "��", "��", "��", "��",
        "��", "��", "��", "��", "��", "��", "��", "��",
        "��", "ȭ" };
        Debug.Log(string.Join(", ", cardFrontBlack));

        cardFrontSpecial = new List<string>
       {"C", "B"};
    }

    public void ChangeLevelHigh()
    {
        cardFrontRed = new List<string>
       {"��", "��", "��" };

        cardFrontBlack = new List<string>
       {"��", "��", "��", "��", "��", "��", "��", "��", "��", "��",
        "��", "��", "��",
        "��",
        "��", "��",
        "��", "��", "��",
        "��", "��", "��", "��", "��", "��", "��", "��", "��",
        "��", "��", "��", "��", "��", "��", "��",
        "��", "��", "��", "��", "��", "��", "��", "��", "��", "��",
        "õ",
        "��", "ȣ", "ȭ"};
        Debug.Log(string.Join(", ", cardFrontBlack));

        cardFrontSpecial = new List<string>
       {"C", "B"};
    }
}
