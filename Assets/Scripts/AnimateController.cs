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
        networkAnimator.SetParameterAutoSend(0, true);
    }

    void Update () {

        if (isLocalPlayer)
        {
            float movement = Input.GetAxisRaw("Vertical") + Input.GetAxisRaw("Horizontal");

            if (movement != 0)
            {
                //anim.Play ("Dude Walk", -1, 0.0f); from beginning
                anim.SetFloat("speed", 1.0f);
            }
            else
            {
                anim.SetFloat("speed", 0.0f);
                //anim.Play("Idle"); anim.SetFloat("speed", 1.0f);
            }

        }
        else
        {
            networkedAnimationSpeed = float.Parse(networkAnimator.param0.Substring(18));
            anim.SetFloat("speed", networkedAnimationSpeed);
        }



    }
}
