using System.Collections.Generic;
using UnityEngine;

public class ChangeLevel : MonoBehaviour
{
    public void ChangeLevelLow()
    {
        ObjectManager.instance.cardFrontRed = new List<string>
       {"��", "��", "��" };

        ObjectManager.instance.cardFrontBlack = new List<string>
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
        Debug.Log(string.Join(", ", ObjectManager.instance.cardFrontBlack));

        ObjectManager.instance.cardFrontSpecial = new List<string>
       {"C", "B"};
    }

    public void ChangeLevelMiddle()
    {
        ObjectManager.instance.cardFrontRed = new List<string>
       {"��", "��", "��" };

        ObjectManager.instance.cardFrontBlack = new List<string>
       {"��", "��", "��", "��", "��", "��", "��", "��",
        "��", "��", "��",
        "��", "��", "��",
        "��",
        "��", "��", "��", "��",
        "��", "��", "��", "��", "��", "��", "��", "��", "��", "��",
        "��", "��", "��", "��", "��", "��", "��", "��", "��", "��",
        "��", "��", "��", "��", "��", "��", "��", "��",
        "��", "ȭ" };
        Debug.Log(string.Join(", ", ObjectManager.instance.cardFrontBlack));

        ObjectManager.instance.cardFrontSpecial = new List<string>
       {"C", "B"};
    }

    public void ChangeLevelHigh()
    {
        ObjectManager.instance.cardFrontRed = new List<string>
       {"��", "��", "��" };

        ObjectManager.instance.cardFrontBlack = new List<string>
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
        Debug.Log(string.Join(", ", ObjectManager.instance.cardFrontBlack));

        ObjectManager.instance.cardFrontSpecial = new List<string>
       {"C", "B"};
    }
}
