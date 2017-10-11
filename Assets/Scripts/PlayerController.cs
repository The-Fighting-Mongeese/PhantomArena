using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Stamina))]
[RequireComponent(typeof(Mana))]
public class PlayerController : NetworkBehaviour {

    public WeaponCollider weapon;

    [SerializeField]
    private float speed = 10.0f;

    [SerializeField]
    float coreHeight = 0.5f;    // from center to top / bottom (actually half height than) 

    [SerializeField]
    float coreRadius = 0.3f;

    //private Rigidbody rb;

    private Renderer mesh;

    // TODO: Probably move this out or find a better way to cache.
    private int phantomLayer, physicalLayer;
    // TODO: Probably move this to a static Constant class
    private Color physicalColor = new Color(174 / 255f, 51 / 255f, 4 / 255f);
    private Color phantomColor = new Color(37 / 255f, 162 / 255f, 195 / 255f);

    private Rigidbody rb;
    private Health health;
    private AnimateController ac;

    [SerializeField]
    private Skill basicAttack;
    [SerializeField]
    private Skill firstSkill;
    [SerializeField]
    private Skill secondSkill;
    [SerializeField]
    private Skill thirdSkill;

    private Skill currentSkill;


    void Start()
    {
        mesh = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
        ac = GetComponent<AnimateController>();
        phantomLayer = LayerMask.NameToLayer("Phantom");
        physicalLayer = LayerMask.NameToLayer("Physical");
    }

    void Update ()
    {
        // Handle movement 
        //if (!isLocalPlayer) return;
        float deltaX = Input.GetAxis("Horizontal") * speed;
        float deltaZ = Input.GetAxis("Vertical") * speed;

        if (Input.GetKeyDown(KeyCode.P))
        {
            Health s = GetComponent<Health>();
            s.CmdTakeTrueDamage(20);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Stamina m = GetComponent<Stamina>();
            if (m.TryUseStamina(50))
            {
                transform.Translate(deltaX, 0, deltaZ);
            }
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
            ac.CmdNetworkedTrigger("SkillStrongAttackTrigger");
        }
        else if (Input.GetButtonDown("Skill3"))
        {
            ac.CmdNetworkedTrigger("SkillForceChangeTrigger");
        }
    }

    private void FixedUpdate()
    {
        // Retrieve inputs 
        float deltaX = Input.GetAxis("Horizontal") * speed;
        float deltaZ = Input.GetAxis("Vertical") * speed;
        float currentY = rb.velocity.y;

        // Apply movement 
        Vector3 forwardVel = transform.forward * speed * deltaZ;
        Vector3 horizontalVel = transform.right * speed * deltaX;
        Vector3 movement = Vector3.ClampMagnitude(forwardVel + horizontalVel, speed);
        movement.y = currentY;

        rb.velocity = movement;
    }

    [Command]
    public void CmdChangePhase(int layer)
    {
        RpcChangePhase(layer);
    }

    [ClientRpc]
    public void RpcChangePhase(int layer)
    {
        if (layer == physicalLayer)
        {
            transform.SetAllLayers(physicalLayer);  // Change layer 
            mesh.material.color = physicalColor;    // Change appearance
        }
        else
        {
            transform.SetAllLayers(phantomLayer);
            mesh.material.color = phantomColor;
        }
    }

    // Note: Called from animation clip
    public void AnimEvent_ColliderActivate()
    {
        if (!isLocalPlayer)
            return;
        weapon.OnOpponentTrigger += OnWeaponEnter;
        weapon.ActivateCollider();
    }

    // Note: Called from animation clip
    public void AnimEvent_ColliderDeactivate()
    {
        if (!isLocalPlayer)
            return;
        weapon.OnOpponentTrigger -= OnWeaponEnter;
        weapon.DeactivateCollider();
        currentSkill = null;
    }

    public void OnWeaponEnter(GameObject other)
    {
        if (!isLocalPlayer)
            return;
        // other.GetComponent<Health>().CmdTakeTrueDamage(20);
        currentSkill.Activate(other);
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
