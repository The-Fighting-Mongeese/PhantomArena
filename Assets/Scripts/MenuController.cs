using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {
    [SerializeField]
    Button button1, button2, button3;
    Button[] buttonArray;
    private int index;

    // Use this for initialization
    void Start() {
        index = 0;
        buttonArray = new Button[] { button1, button2, button3 };
        buttonArray[index].Select();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Skill5")) {
            index--;
            if (index < 0) {
                index = buttonArray.Length - 1;
            }
            buttonArray[index].Select();
        }
        if (Input.GetButtonDown("Skill3")) {
            index = (index + 1) % buttonArray.Length;
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
