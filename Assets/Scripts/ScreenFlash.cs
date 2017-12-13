using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour {

    public static GameObject screenFlashUi;

    private void Awake()
    {
        screenFlashUi = GameObject.Find("HealthFlash");
    }

    public static IEnumerator FlashScreen(int numOfFlashes, float timeBetweenEachFlash)
    {

        print("flas screen");

        int i = 0;

        //screenFlashUi.SetActive(true);

        while (i < numOfFlashes)
        {

            screenFlashUi.GetComponent<Image>().color = Color.red;

            yield return new WaitForSeconds(timeBetweenEachFlash);

            screenFlashUi.GetComponent<Image>().color = new Color(0,0,0,0);

            yield return new WaitForSeconds(timeBetweenEachFlash);
            print("flas asdfasdf");
            i++;
        }
    }

}
