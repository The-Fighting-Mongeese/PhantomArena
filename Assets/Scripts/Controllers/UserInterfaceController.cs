using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceController : MonoBehaviour {

    public GameObject chatPanel;
    public static GameObject UIBars;
    #region Lobby Interface Elements

    public static CanvasGroup canvasGroup;
    public static InputField nameInput;

    #endregion

    void Start () {
        chatPanel = GameObject.Find("ChatPanel");
        nameInput = GameObject.Find("nameInputField").GetComponent<InputField>();
        canvasGroup = nameInput.GetComponent<CanvasGroup>();
        UIBars = GameObject.Find("CanvasUI").FindObject("UIBars");
    }

    public static void TransitionToGameUI()
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = 0f;
        nameInput.enabled = false;
        canvasGroup.blocksRaycasts = false; //prevents the ui from receiving input events
        UIBars.SetActive(true);
    }

    public static void TransitionToLobbyUI()
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = 1f;
        nameInput.enabled = true;
        canvasGroup.blocksRaycasts = true;
        UIBars.SetActive(false);
    }

    public void ToggleChatBox()
    {
        chatPanel.gameObject.SetActive(!chatPanel.activeSelf);
    }
}
