using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;

public class ScoreBoard : MonoBehaviour {


    [SerializeField]
    GameObject playerScoreboardPrefab;

    [SerializeField]
    Transform playerScoreboardList;

    private void OnEnable()
    {
        //get an array of players
        PlayerMetrics[] players = PlayerManager.GetAllPlayers();

        foreach(var player in players)
        {
            //print(player.playername + ": " + player.netID);

            GameObject itemGO =  Instantiate(playerScoreboardPrefab, playerScoreboardList);

            PlayerScoreboardItem item = itemGO.GetComponent<PlayerScoreboardItem>();
            if(item != null)
            {
                item.Setup(player.playername, player.kills, player.deaths);
            }
        }

        //loop through and set up a list item for each one
        //setting the ui elements equal to the relevant data

        print("starting couruotuine update score board");
        StartCoroutine(GetScores());
    }

    private void OnDisable()
    {
        foreach(Transform child in playerScoreboardList)
        {
            Destroy(child.gameObject);
        }

        StopAllCoroutines();
    }


    IEnumerator GetScores()
    {

        bool flag;

        while (true)
        {
            yield return new WaitForSeconds(2f);


            var players = GameObject.FindGameObjectsWithTag("Player");

            foreach (var player in players)
            {
                flag = false;

                if(playerScoreboardList.childCount == 0)
                {
                    // print("GetScores childCount 0");
                    GameObject itemGO = Instantiate(playerScoreboardPrefab, playerScoreboardList);
                    PlayerScoreboardItem playerScoreItem = itemGO.GetComponent<PlayerScoreboardItem>();
                    if (playerScoreItem != null)
                    {
                        playerScoreItem.Setup(player.GetComponent<PlayerMetrics>().playername, player.GetComponent<PlayerMetrics>().kills, player.GetComponent<PlayerMetrics>().deaths);
                    }
                }
                else //more than 1 player on the list
                {
                    foreach (Transform item in playerScoreboardList)
                    {
                        if (item.GetComponent<PlayerScoreboardItem>().usernameText.text == player.GetComponent<PlayerMetrics>().playername)
                        {
                            PlayerScoreboardItem playerScoreItem = item.GetComponent<PlayerScoreboardItem>();
                            print(player.GetComponent<PlayerMetrics>().deaths);
                            playerScoreItem.Setup(player.GetComponent<PlayerMetrics>().playername, player.GetComponent<PlayerMetrics>().kills, player.GetComponent<PlayerMetrics>().deaths);
                            flag = true;
                            break;
                        }
                    }

                    if (!flag)
                    {
                        GameObject itemGO = Instantiate(playerScoreboardPrefab, playerScoreboardList);
                        PlayerScoreboardItem playerScoreItem = itemGO.GetComponent<PlayerScoreboardItem>();
                        if (playerScoreItem != null)
                        {
                            playerScoreItem.Setup(player.GetComponent<PlayerMetrics>().playername, player.GetComponent<PlayerMetrics>().kills, player.GetComponent<PlayerMetrics>().deaths);
                        }
                    }

                }


            } 
        }
    }

}