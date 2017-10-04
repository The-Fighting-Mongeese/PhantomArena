using UnityEngine;
using UnityEngine.Networking;

public class GameSetup : NetworkBehaviour {


    private void Awake()
    {
        DataService.InitializeDbConnection("PhantomArena.db");
    }
}
