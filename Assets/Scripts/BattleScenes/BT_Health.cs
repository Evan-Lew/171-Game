using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class BT_Health : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] TMP_Text textPopup;
    [SerializeField] GameObject descriptionObj;
    [HideInInspector] public bool enableTextDescription = false;

    private void Awake()
    {
        textPopup.text = "Gain 5 Maximum Health";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.PlaySound("sfx_Scroll_Open", 1);
        enableTextDescription = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        enableTextDescription = false;
    }

    void Update()
    {
        DisplayDescription();
    }
    
    void DisplayDescription()
    {
        if (enableTextDescription)
        {
            descriptionObj.SetActive(true);
        }
        else
        {
            descriptionObj.SetActive(false);
        }
    }

}
