using SQLite4Unity3d;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProfileController{


    SQLiteConnection connection;

    public PlayerProfileController()
    {
        connection = DataService._connection;
        InitializePlayerProfileTable();
    }


    private bool TableExists()
    {
        string tableExistsQuery = "SELECT count(Id) FROM PlayerProfile;";
        string result = "";

        try
        {
            result = connection.ExecuteScalar<string>(tableExistsQuery);
        }
        catch (Exception e)
        {
            Debug.LogError((e.Message != null ? e.Message.ToString() + ", it will be created now." : "An exception has occured"));
        }

        return (result.Length > 0);
    }

    public void InitializePlayerProfileTable()
    {
        if (!TableExists())
            connection.CreateTable<PlayerProfile>();     
    }

    private void InsertMultiplePlayers(PlayerProfile[] players)
    {
        connection.InsertAll(players);
    }

    public PlayerProfile CreatePlayer(string playerName)
    {
        PlayerProfile p = new PlayerProfile
        {
            Name = playerName,
            Level = 1
        };
        connection.Insert(p);
        return p;
    }

    public PlayerProfile GetPlayerProfile(string playerName)
    {
        if (connection.Table<PlayerProfile>().Where(x => x.Name == playerName).FirstOrDefault() != null)
        {
            return connection.Table<PlayerProfile>().Where(x => x.Name == playerName).FirstOrDefault();
        }
        else
        {
            return CreatePlayer(playerName);
        }
    }


    public IEnumerable<PlayerProfile> GetPlayers()
    {
        return connection.Table<PlayerProfile>();
    }

    public void DropTable()
    {
        connection.DropTable<PlayerProfile>();
    }

}
