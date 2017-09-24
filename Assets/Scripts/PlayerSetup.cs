using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour { 
    
    [SerializeField]
    Behaviour[] componentsToDisable;

    private void Start()
    {
        if (!isLocalPlayer) //if this object isn't controlled by the system, then we have to disable all the components.
        {
            print("i am not local player.");
            DisableComponents();
        }
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }


}
