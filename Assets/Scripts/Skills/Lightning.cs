using UnityEngine.Networking;
using UnityEngine;
using System;

//Lightning attack

public class Lightning : AnimLessSkill {

    public GameObject lightningStrikePrefab;
    public float lightningAliveTime = 10.0f;
    public int manaCost = 20;

    private Mana mana;


    protected override void Awake()
    {
        base.Awake();
        mana = GetComponent<Mana>();
    }

    [Command]
    private void CmdLightningBolt(Vector3 spawnPosition)
    {
        GameObject lightningStrike = Instantiate(lightningStrikePrefab, spawnPosition, lightningStrikePrefab.transform.rotation);
        NetworkServer.Spawn(lightningStrike);
        Destroy(lightningStrike, lightningAliveTime);
    }

    public override bool ConditionsMet()
    {
        return (mana.CurrentMana >= manaCost && cooldownCounter <= 0);
    }

    public override void ConsumeResources()
    {
        mana.TryUseMana(manaCost);
        cooldownCounter = cooldown;
    }

    public override void Activate(GameObject other)
    {
        if (!isLocalPlayer) return;

        transform.rotation = Quaternion.LookRotation(player.rig.FlatForward());                     // face targetted direction

        Vector3 spawnPos = transform.position + (Vector3.up * 12.5f) + (transform.forward * 5);     // calculate spawn position

        CmdLightningBolt(spawnPos);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        DebugExtension.DrawCircle(transform.position + (Vector3.up * 12.5f) + (transform.forward * 5));
    }
}
