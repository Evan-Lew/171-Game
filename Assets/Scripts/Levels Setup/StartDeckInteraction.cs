using UnityEngine;
using UnityEngine.EventSystems;

public class StartDeckInteraction : MonoBehaviour, IPointerExitHandler
{
    [SerializeField] GameObject startDeckSetup;
    [SerializeField] StarterDeck _starterDeck;
    [SerializeField] PickDeckLevel_1Setup _level02_Setup;

    public void OnPointerExit(PointerEventData eventData)
    {
        _starterDeck.enableTextDescription = false;
    }

    public void Button_Cancel()
    {
        _starterDeck.enableTextDescription = false;
    }

    public void Button_Confirm()
    {
        DeckEditSystem.instance.RemoveAllCardsFromDeck();

        foreach(Card_Basedata card in _starterDeck.deck)
        {
            DeckEditSystem.instance.AddCardToDeck(card);
        }
        startDeckSetup.SetActive(false);
        _level02_Setup.isFinished = true;
    }
}
