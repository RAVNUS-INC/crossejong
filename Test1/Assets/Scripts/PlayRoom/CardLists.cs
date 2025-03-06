using System.Collections.Generic;
using UnityEngine;

public class CardLists : MonoBehaviour
{
    public List<string> cardFrontRed;
    public List<string> cardFrontBlack;
    public List<string> cardFrontSpecial;

    
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

        cardFrontSpecial = new List<string>
       {"C", "B"};
    }
}
