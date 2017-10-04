using UnityEngine;

public class GameSetup : MonoBehaviour {


    private void Awake()
    {
        DataService.InitializeDbConnection("PhantomArena.db");
    }

}
