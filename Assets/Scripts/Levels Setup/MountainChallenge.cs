using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class MountainChallenge : MonoBehaviour
{
    [SerializeField] GameObject Canvas;

    private void Awake()
    {
        GameController.instance.changePlayerSprite();
        Canvas.SetActive(true);
    }
    
    public void Button_ClimbMountain()
    {
        Canvas.SetActive(false);
        BattleController.battleNum = 99;
        SceneManager.LoadScene("BattleLevel");
    }

    public void Button_LoopMountain()
    {
        Canvas.SetActive(false);
        BattleController.battleNum = 0;
        SceneManager.LoadScene("BattleLevel");
    }

}
