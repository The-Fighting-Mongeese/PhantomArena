using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class TowerAttack : NetworkBehaviour
{
    public float TurnSpeed = 5.0f;
    public float ReloadSpeed = 2.0f;
    public float ActivationRange = 10.0f;
    public GameObject Projectile;
    public Transform ExitPoint;

    private List<GameObject> _players;
    private Transform _target;
    private float _targetDistance;
    private Coroutine _attacking;

    public void Awake ()
	{
        ClientScene.RegisterPrefab(Projectile);
        ClientScene.RegisterPrefab(gameObject);
    }

    public void OnEnable()
    {
        _players = new List<GameObject>();
        _target = null;
        _attacking = null;
        StartCoroutine(FindPlayers());
    }

    public void Update()
    {
        if (_players.Count == 0) return;
        if (_targetDistance < ActivationRange)
        {
            if (_attacking == null)
                _attacking = StartCoroutine(Shoot());
        }
        else if(_attacking != null)
        {
            StopCoroutine(_attacking);
            _attacking = null;
            _target = null;
        }

        if (_target != null)
        {
            var lookPos = _target.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * TurnSpeed);
        }
    }

    private IEnumerator FindPlayers()
    {
        while (true)
        {
            _players = GameObject.FindGameObjectsWithTag("Player").ToList();
            float min = float.MaxValue;
            foreach(var t in _players)
            {
                float d = Vector3.Distance(gameObject.transform.position, t.transform.position);
                if (d < min)
                {
                    min = d;
                    _target = t.transform;
                    _targetDistance = min;
                }
            }
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator Shoot()
    {
        while(true)
        {
            CmdSpawn();
            yield return new WaitForSeconds(ReloadSpeed);
        }
    }

    [Command]
    public void CmdSpawn()
    {
        var go = Instantiate(Projectile, ExitPoint.position, transform.rotation);
        NetworkServer.Spawn(go);
    }
    
}
