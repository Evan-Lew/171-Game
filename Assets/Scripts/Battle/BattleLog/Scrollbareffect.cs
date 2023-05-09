using UnityEngine;
using UnityEngine.EventSystems;

public class Scrollbareffect : MonoBehaviour , IPointerClickHandler
{
    [SerializeField] private Transform battleLogSystem;
    [SerializeField] private AnimationCurve curve;
    bool IsAlreadyDisplayed = true;
    public Transform inScene;
    public Transform outScene;

    public float Speed = 1.3f;
    float current = 1, target = 1;
    
    // Variables for the tutorial
    public static Scrollbareffect instance;
    [HideInInspector] public bool firstHistoryClick = false;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        current = Mathf.MoveTowards(current, target, Speed * Time.deltaTime);
        battleLogSystem.position = Vector3.Lerp(inScene.position, outScene.position, curve.Evaluate(current));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.PlaySound("sfx_Scroll_Open", 0.2f);
        firstHistoryClick = true;
        //timeLerped += Time.deltaTime;
        if (IsAlreadyDisplayed == false)
        {
            IsAlreadyDisplayed = true;
            target = 1;
        }
        else
        {
            IsAlreadyDisplayed = false;
            target = 0;
        }
    }
}
