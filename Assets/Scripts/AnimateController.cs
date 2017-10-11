using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AnimateController : NetworkBehaviour {

    public Animator anim;
    NetworkAnimator networkAnimator;
    private float networkedAnimationSpeed = 0;

    private void Start()
    {
        anim = GetComponent<Animator>();
        networkAnimator = GetComponent<NetworkAnimator>();
        // networkAnimator.SetParameterAutoSend(0, true);
    }

    void Update ()
    {
        if (isLocalPlayer)
        {
            float hor = Input.GetAxisRaw("Horizontal");
            float ver = Input.GetAxisRaw("Vertical");

            anim.SetFloat("Input X", hor);
            anim.SetFloat("Input Z", ver);

            if (hor != 0 || ver != 0)  //if there is some input
            {
                //set that character is moving
                anim.SetBool("Moving", true);
                anim.SetBool("Running", true);
            }
            else
            {
                //character is not moving
                anim.SetBool("Moving", false);
                anim.SetBool("Running", false);
            }

            // attack / skill detection
            if (Input.GetButtonDown("Fire1"))
            {
                Debug.Log("Fire1 called from server " + isServer);
                CmdNetworkedTrigger("Attack1Trigger");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                CmdNetworkedTrigger("SkillStrongAttackTrigger");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                CmdNetworkedTrigger("SkillStrongAttackTrigger");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                CmdNetworkedTrigger("SkillForceChangeTrigger");
            }

            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                anim.SetBool("Dead", !anim.GetBool("Dead"));
                CmdNetworkedTrigger("Die");
            }
        }
    }

    [Command]
    public void CmdNetworkedTrigger(string param)
    {
        RpcNetworkedTrigger(param);
    }

    [ClientRpc]
    void RpcNetworkedTrigger(string param)
    {
        anim.SetTrigger(param);

    }

    public IEnumerator COStunPause(float pauseTime)
    {
        yield return new WaitForSeconds(pauseTime);
    }


}


// networkAnimator.SetTrigger("Attack1Trigger"); // WARNING: Sadly there's a bug where the trigger plays twice on the server player 
// anim.SetTrigger("Attack1Trigger");
// StartCoroutine(COStunPause(1.2f));   // ?