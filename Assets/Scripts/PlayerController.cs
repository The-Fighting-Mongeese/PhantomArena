using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Stamina))]
[RequireComponent(typeof(Mana))]
public class PlayerController : NetworkBehaviour
{
    private float JUMP_DURATION = 0.7f;

    public WeaponCollider weapon;
    public ThirdPersonCameraRig rig;
    public AudioSource phaseChangeSfx;
    public ParticleSystem phaseChangePhantomVfx, phaseChangePhysicalVfx;

    private float currentSpeed = 0f;
    [SerializeField]
    private float speed = 10.0f;
    [SerializeField]
    private float acceleration = 5f;
    [SerializeField]
    private float strafeMultiplier = 0.5f;

    [SerializeField]
    float coreHeight = 0.5f;    // from center to top / bottom (actually half height than) 

    [SerializeField]
    float coreRadius = 0.3f;

    // TODO: Probably move this out or find a better way to cache.
    private int phantomLayer, physicalLayer;

    private Rigidbody rb;
    private Collider col;
    private Health health;
    private AnimateController ac;
    private PhasedMaterial[] phasedMaterials;
    private SkillStateMachine deathBehaviour;

    [SerializeField]
    private Skill basicAttack;
    [SerializeField]
    private Skill firstSkill;
    [SerializeField]
    private Skill secondSkill;
    [SerializeField]
    private Skill thirdSkill;
    [SerializeField]
    private Skill fourthSkill;
    [SerializeField]
    private Skill fifthSkil;

    private SkillIndicator basicAttackIndicator;
    private SkillIndicator firstSkillIndicator;
    private SkillIndicator secondSkillIndicator;
    private SkillIndicator thirdSkillIndicator;

    private uint skillLockCount = 0;   // keep track of how many things are disabling this
    private uint phaseLockCount = 0;
    private uint moveLockCount = 0;

    public bool skillLocked
    {
        get { return skillLockCount != 0; }
        set { if (value) skillLockCount++; else skillLockCount--; }
    }
    public bool phaseLocked
    {
        get { return phaseLockCount != 0; }
        set { if (value) phaseLockCount++; else phaseLockCount--; }
    }
    public bool moveLocked
    {
        get { return moveLockCount != 0; }
        set { if (value) moveLockCount++; else moveLockCount--; }
    }

    bool mouseVisible = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        health = GetComponent<Health>();
        ac = GetComponent<AnimateController>();
        phasedMaterials = GetComponentsInChildren<PhasedMaterial>();
        phantomLayer = LayerMask.NameToLayer("Phantom");
        physicalLayer = LayerMask.NameToLayer("Physical");

        basicAttackIndicator = GameObject.Find("CanvasUI").FindObject("BasicAttackIndicator").GetComponent<SkillIndicator>();
        firstSkillIndicator = GameObject.Find("CanvasUI").FindObject("FirstSkillIndicator").GetComponent<SkillIndicator>();
        secondSkillIndicator = GameObject.Find("CanvasUI").FindObject("SecondSkillIndicator").GetComponent<SkillIndicator>();
        thirdSkillIndicator = GameObject.Find("CanvasUI").FindObject("ThirdSkillIndicator").GetComponent<SkillIndicator>();
        
    #if UNITY_PS4
        basicAttackIndicator.SetButtonNameText("???");
        firstSkillIndicator.SetButtonNameText("???");
        secondSkillIndicator.SetButtonNameText("???");
        thirdSkillIndicator.SetButtonNameText("???");
    #endif
    #if UNITY_EDITOR
        basicAttackIndicator.SetButtonNameText("Left-Click");
        firstSkillIndicator.SetButtonNameText("1");
        secondSkillIndicator.SetButtonNameText("2");
        thirdSkillIndicator.SetButtonNameText("3");
    #endif
    #if UNITY_STANDALONE_WIN
        basicAttackIndicator.SetButtonNameText("Left-Click");
        firstSkillIndicator.SetButtonNameText("1");
        secondSkillIndicator.SetButtonNameText("2");
        thirdSkillIndicator.SetButtonNameText("3");
#endif

        if (isLocalPlayer)  // Note: Unnecessary check - PlayerSetup disable this component;
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        // customize death
        deathBehaviour = ac.anim.GetBehaviour<SkillStateMachine>("Death");
        if (deathBehaviour != null)
        {
            Debug.Log("Found death behaviour");
            deathBehaviour.OnStateEntered += OnDeath;
            deathBehaviour.OnStateExited += OnRespawn;
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
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
        if (!phaseLocked)
        {
            if (Input.GetButtonDown("PhaseChange"))
            {
                AttemptPhaseChange();
            }
        }

        // attack / skill detection
        if (!skillLocked)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (basicAttack.ConditionsMet())
                {
                    ac.CmdNetworkedTrigger("Attack1Trigger"); // TODO: Move to skill
                    basicAttack.ConsumeResources();
                }
            }
            else if (Input.GetButtonDown("Skill1"))
            {
                if (firstSkill.ConditionsMet())
                {
                    ac.CmdNetworkedTrigger("SkillStrongAttackTrigger"); // TODO: Move to skill
                    firstSkill.ConsumeResources();
                }
            }
            else if (Input.GetButtonDown("Skill2"))
            {
                if (secondSkill.ConditionsMet())
                {
                    ac.CmdNetworkedTrigger("SkillAntiPhaseAttackTrigger");
                    secondSkill.ConsumeResources();
                }
            }
            else if (Input.GetButtonDown("Skill3"))
            {
                if (thirdSkill.ConditionsMet())
                {
                    ac.CmdNetworkedTrigger("SkillForceChangeTrigger");
                    thirdSkill.ConsumeResources();
                }
            }
            else if (Input.GetButtonDown("Skill4"))
            {
                if (fourthSkill.ConditionsMet())
                {
                    fourthSkill.Activate(null);     // note: AnimLessSkill
                    fourthSkill.ConsumeResources();
                }
            }
            else if (Input.GetButtonDown("Skill5"))
            {
                if (fifthSkil.ConditionsMet())
                {
                    fifthSkil.ConsumeResources();
                    fifthSkil.Activate(null);     // warning: order matters note: AnimLessSkill 
                }
            }
        }

        basicAttackIndicator.UpdateUI(basicAttack.ConditionsMet() && !skillLocked, basicAttack.cooldownCounter);
        firstSkillIndicator.UpdateUI(firstSkill.ConditionsMet() && !skillLocked, firstSkill.cooldownCounter);
        secondSkillIndicator.UpdateUI(secondSkill.ConditionsMet() && !skillLocked, secondSkill.cooldownCounter);
        thirdSkillIndicator.UpdateUI(thirdSkill.ConditionsMet() && !skillLocked, thirdSkill.cooldownCounter);
    }

    private void FixedUpdate()
    {
        // if movelocked zero horizontal velocity and return
        if (moveLocked)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            return;
        }

        float forwardInput = Input.GetAxisRaw("Vertical");
        float sideInput = Input.GetAxisRaw("Horizontal");
        // get input vector
        Vector3 vel = (rig.FlatForward() * forwardInput) + (rig.FlatRight() * sideInput);
        // only normalize input vector if magnitude greater than 1 as to allow analog input
        if (vel.magnitude > 1) 
        {
            vel.Normalize();
        }
        // realign player with camera if player is moving
        if ((forwardInput != 0) || (sideInput != 0)) 
        {
            transform.rotation = Quaternion.LookRotation(rig.FlatForward());
        }

        vel *= speed;
        // strafing 
        if (forwardInput <= 0)
        {
            vel *= strafeMultiplier;
        }

        if (IsGrounded())
        {
            ac.anim.SetBool("InAir", false);

            // jumping
            // GetKey(not down) here to keep applying the jump vel until IsGrounded is false
            if (Input.GetButton("Jump"))    
            {
                ac.CmdNetworkedTrigger("JumpTrigger");
                vel.y = -Physics.gravity.y * 0.5f * JUMP_DURATION;
            }
            else
            {
                // keep the player sticking to the ground
                vel.y = Mathf.Min(-0.1f, rb.velocity.y);  
            }
        }
        else // is not grounded
        {
            ac.anim.SetBool("InAir", true);

            vel.y += rb.velocity.y;
        }

        rb.velocity = vel;
    }

    public void OnDestroy()
    {
        if (isLocalPlayer)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (deathBehaviour != null)
        {
            deathBehaviour.OnStateEntered -= OnDeath;
            deathBehaviour.OnStateExited -= OnRespawn;
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
        if (layer == LayerHelper.PhysicalLayer)     // Play vfx
            phaseChangePhysicalVfx.Play();
        else
            phaseChangePhantomVfx.Play();
        phaseChangeSfx.Play();                      // Play sfx 
    }

    [ClientRpc]
    public void RpcSetPhaseLock(bool locked)
    {
        phaseLocked = locked;
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
        Debug.DrawLine(transform.position, transform.position - (Vector3.up * 1.1f));
    }

    bool IsGrounded()
    {
        if (gameObject.layer == physicalLayer) 
        {
            return Physics.Raycast(transform.position, -Vector3.up, 1.1f, LayerHelper.WalkablePhysical);
        }
        if (gameObject.layer == phantomLayer)
        {
            return Physics.Raycast(transform.position, -Vector3.up, 1.1f, LayerHelper.WalkablePhantom);
        }
        return Physics.Raycast(transform.position, -Vector3.up, 1.1f);
    }

    private void OnDeath()
    {
        if (isLocalPlayer)
        {
            moveLocked = true;
            skillLocked = true;
            phaseLocked = true;
        }
    }

    private void OnRespawn()
    {
        if (isLocalPlayer)
        {
            moveLocked = false;
            skillLocked = false;
            phaseLocked = false;
            CmdChangePhase(LayerHelper.PhysicalLayer);
        }
    }
}