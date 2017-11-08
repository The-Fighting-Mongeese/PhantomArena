using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Stamina))]
[RequireComponent(typeof(Mana))]
public class PlayerController : NetworkBehaviour {
    private float JUMP_DURATION = 1.0f;

    public WeaponCollider weapon;

    [SerializeField]
    private float speed = 10.0f;

    [SerializeField]
    float coreHeight = 0.5f;    // from center to top / bottom (actually half height than) 

    [SerializeField]
    float coreRadius = 0.3f;

    // TODO: Probably move this out or find a better way to cache.
    private int phantomLayer, physicalLayer;

    private Rigidbody rb;
    private Health health;
    private AnimateController ac;
    private PhasedMaterial[] phasedMaterials;

    [SerializeField]
    private Skill basicAttack;
    [SerializeField]
    private Skill firstSkill;
    [SerializeField]
    private Skill secondSkill;
    [SerializeField]
    private Skill thirdSkill;

    private Skill currentSkill;

    public bool skillLocked = false;
    public bool phaseLocked = false;
    public bool moveLocked = false;

    bool mouseVisible = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
        ac = GetComponent<AnimateController>();
        phasedMaterials = GetComponentsInChildren<PhasedMaterial>();
        phantomLayer = LayerMask.NameToLayer("Phantom");
        physicalLayer = LayerMask.NameToLayer("Physical");

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Cursor.visible = mouseVisible;
            if (mouseVisible)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            mouseVisible = !mouseVisible;
        }


        if (Input.GetKeyDown(KeyCode.P))
        {
            Health s = GetComponent<Health>();
            s.CmdTakeTrueDamage(20);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Mana m = GetComponent<Mana>();
            m.TryUseMana(50);
        }

        // Handle phase change 
        if (Input.GetKeyDown(KeyCode.F))
        {
            AttemptPhaseChange();
        }

        // attack / skill detection
        if (Input.GetButtonDown("Fire1"))
        {
            if (basicAttack.ConditionsMet())
            {
                Debug.Log("Basic attacking");
                ac.CmdNetworkedTrigger("Attack1Trigger"); // TODO: Move to skill
                basicAttack.ConsumeResources();
                currentSkill = basicAttack;
            }
        }
        else if (Input.GetButtonDown("Skill1"))
        {
            if (firstSkill.ConditionsMet())
            {
                ac.CmdNetworkedTrigger("SkillStrongAttackTrigger"); // TODO: Move to skill
                firstSkill.ConsumeResources();
                currentSkill = firstSkill;
            }
        }
        else if (Input.GetButtonDown("Skill2"))
        {
            if (secondSkill.ConditionsMet())
            {
                ac.CmdNetworkedTrigger("SkillAntiPhaseAttackTrigger");
                secondSkill.ConsumeResources();
                currentSkill = secondSkill;
            }
        }
        else if (Input.GetButtonDown("Skill3"))
        {
            if (thirdSkill.ConditionsMet())
            {
                ac.CmdNetworkedTrigger("SkillForceChangeTrigger");
                thirdSkill.ConsumeResources();
                currentSkill = thirdSkill;
            }
        }
    }

    private void FixedUpdate()
    {
        if (IsGrounded())
        {
            Vector3 vel = (transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal")).normalized * speed;
            if (Input.GetKey(KeyCode.Space))
            {
                vel.y = 9.81f * 0.5f * JUMP_DURATION;
            }
            else
            {
                vel.y = rb.velocity.y;
            }
            rb.velocity = vel;
        }
    }

    [Command]
    public void CmdChangePhase(int layer)
    {
        RpcChangePhase(layer);
    }

    [ClientRpc]
    public void RpcChangePhase(int layer)
    {
        // Basic error checking
        if (layer != physicalLayer && layer != phantomLayer)
        {
            Debug.LogError("ERROR | PlayerController: Attempting to phase change into a non-phase layer # " + layer);
            return;
        }

        // transform.SetAllLayers(layer);           // Change layer for entire object + children (BUG: for transform and direct children only)
        gameObject.layer = layer;                   // Change main body layer
        weapon.gameObject.layer = layer;            // Change weapon layer
        foreach (var pm in phasedMaterials)         // Change appearance
            pm.ShowPhase(layer);
    }


    private bool AttemptPhaseChange()
    {
        // Check if player is phasing into something
        var objectsInCore = Physics.OverlapCapsule(
            transform.position + transform.up * coreHeight / 2,
            transform.position + transform.up * -coreHeight / 2,
            coreRadius);

        // Die if phasing into something other than a player (eg. environment) 
        foreach (var o in objectsInCore)
        {
            if (!o.CompareTag("Player"))
            {
                health.CmdTakeTrueDamage(1000);
                return false;
            }
        }

        // TODO: Check phase change cooldown / resource use 

        // All checks ok, change phase
        CmdChangePhase((gameObject.layer == physicalLayer) ? phantomLayer : physicalLayer);

        return true;
    }

    void OnDrawGizmosSelected()
    {
        // Drawing core bounds (Note: the calculations are correct, do not use half coreHeight)
        DebugExtension.DrawCapsule(transform.position + (transform.up * coreHeight), transform.position - (transform.up * coreHeight), coreRadius);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 2 * coreHeight + 0.01f);
    }
}

/* Function to combine layers. 
int CombineLayers(params string[] layers)
{
    int finalLayerMask = 0;
    foreach (string s in layers)
    {
        var layer = LayerMask.NameToLayer(s);
        finalLayerMask = finalLayerMask | layer;
    }
    return finalLayerMask;
}
*/

