using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserInterfaceController : MonoBehaviour
{
    public static CanvasGroup lobbyGroup;
    public static CanvasGroup gameGroup;
    public static InputField nameInput;

    void Start ()
    {
        nameInput = GameObject.Find("nameInputField").GetComponent<InputField>();
        nameInput.text = "Username";

        lobbyGroup = GameObject.Find("CanvasUI").FindObject("LobbyUI").GetComponent<CanvasGroup>();
        gameGroup = GameObject.Find("CanvasUI").FindObject("GameUI").GetComponent<CanvasGroup>();
    }

    public static void TransitionToGameUI()
    {
        gameGroup.FullyOn(true);
        lobbyGroup.FullyOn(false);
    }

    public static void TransitionToLobbyUI()
    {
        gameGroup.FullyOn(false);
        lobbyGroup.FullyOn(true);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
