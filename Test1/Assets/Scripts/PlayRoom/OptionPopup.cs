using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OptionPopup : MonoBehaviour
{
    public GameObject optionPopupPanel;
    public Button openOptionPopupButton; 
    public Button closeOptionPopupButton;
    public Button surrenderButton; 

    private void Awake() {
        
    }

    void Start()
    {
        // GameObject h = new GameObject();
        // GameObject j = new GameObject();
        // h.transform.SetParent(j.transform);
        // h.SetActive(false);
        // h.transform.position = h.transform.parent.position;
    }

    public void OpenPopup() 
    {
        optionPopupPanel.SetActive(true); 
        openOptionPopupButton.gameObject.SetActive(false);
        closeOptionPopupButton.gameObject.SetActive(true); 
        surrenderButton.gameObject.SetActive(true);
    }

    public void ClosePopup() 
    {
        optionPopupPanel.SetActive(false);
        openOptionPopupButton.gameObject.SetActive(true); 
        closeOptionPopupButton.gameObject.SetActive(false); 
        surrenderButton.gameObject.SetActive(false); 
    }
}
