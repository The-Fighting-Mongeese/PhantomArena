using SQLite4Unity3d;
using System;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
using UnityEngine;
#endif

public class DataService  {

	public static SQLiteConnection _connection;

    public static void InitializeDbConnection(string DatabaseName)
    {
#if UNITY_EDITOR
        var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        //var pathString = Path.Combine(Directory.GetCurrentDirectory(), "WorldData");
        //var filepath = string.Format("{0}\\{1}", pathString, DatabaseName);
        //if (!Directory.Exists(pathString))  Directory.CreateDirectory(pathString);
        //if (!File.Exists(filepath)) {}
        //Debug.Log("Db path is: " + dbPath);

        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);
        var dbPath = filepath;
#endif

        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        //Debug.Log("Final PATH: " + dbPath); 
    }
}