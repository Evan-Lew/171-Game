using UnityEngine;
using UnityEngine.SceneManagement;
public class CreditsMenu : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        SoundManager.PlaySound("sfx_Page_Flip", 1);
        SceneManager.LoadScene("Main Menu");
    }
}
