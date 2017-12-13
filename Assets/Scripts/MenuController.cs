using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {
    [SerializeField]
    private Button[] buttonArray;
    private int index;

    // Use this for initialization
    void Start() {
        index = 0;
        buttonArray[index].Select();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Skill5")) {
            index = Mathf.Max(index - 1, 0);
            buttonArray[index].Select();
        }
        if (Input.GetButtonDown("Skill3")) {
            index = Mathf.Min(index + 1, buttonArray.Length - 1);
            buttonArray[index].Select();
        }
        if (Input.GetButtonDown("Submit")) {
            buttonArray[index].onClick.Invoke();
        }
    }

    public void Button1OnClick() {
        SceneManager.LoadScene("JasonWorkSpace");
    }
    public void Button2OnClick() {
        SceneManager.LoadScene("DanielTerrainScene");
    }
    public void Button3OnClick() {
        SceneManager.LoadScene("MarkusWorkSpace");
    }
}
