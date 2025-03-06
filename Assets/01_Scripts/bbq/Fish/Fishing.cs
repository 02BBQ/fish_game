using System;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Fishing : MonoBehaviour
{
    [SerializeField] private GameObject _aim;
    public Player player {get; private set;}
    private PlayerMovement playerMovement => player.playerMovement;
    
    public bool isFishing {get; private set;}
    public bool isAiming {get; private set;}

    public bool rodThrown;

    private float _maxDistance = 10;
    private float _distance = 1;

    private event Action Stepped;
    private Vector3 _destination;

    [SerializeField] private LayerMask _toAimLayer;
    [SerializeField] private LineRenderer _rodLine;


    void Awake()
    {
        _aim.SetActive(false);
        player = GetComponentInParent<Player>();
        if (player == null){
            UnityEngine.Debug.LogWarning("Something's Wrong...");
            Destroy(gameObject);
        }

        player.playerInput.FishingDown += handleHoldStart;
        player.playerInput.FishingUp += handleHoldEnd;

        player.playerAnim.OnCastRod += HandleCast;
        // player.playerInput
    }

    void Update()
    {
        Stepped?.Invoke();
    }

    void handleHoldStart()
    {
        if (isFishing)
        {
            PullReel();
            return;
        }
        if (!isAiming)
            Aim();
            
    }

    private void Aim()
    {
        var Rig = transform.parent.GetComponentInChildren<Animator>().gameObject;

        _aim.SetActive(true);
        var dir = Rig.transform.forward;
        var origin = player.transform.position;
        var trans = _aim.transform;
        // player.transform.position = origin;
        trans.position = origin;
        _distance = 1;
        isAiming = true;

        Action AimUpdate = null; AimUpdate = () => {
            _distance = math.min(_distance+Time.deltaTime * 25, _maxDistance);
            if (!isAiming)
            {
                Stepped -= AimUpdate;
                _destination = trans.position;
                return;
            }
            trans.position = origin + dir * _distance;
            if (Physics.Raycast(trans.position, -Vector3.up, out RaycastHit hit, 50, _toAimLayer))
            {
                trans.position = new Vector3(trans.position.x, hit.point.y, trans.position.z);
            }
        };
        Stepped += AimUpdate;
    }

    void handleHoldEnd()
    {
        CastRod();
    }

    private void CastRod()
    {
        player.playerAnim.SetBool("Fishing", true);
    }

    private void PullReel()
    {
        player.playerAnim.SetBool("Fishing", false);
        isFishing = false;
        isAiming = false;

        var v = 0.0f;

        Action RodUpdate = null; RodUpdate = () => {
            v = math.min(v+Time.deltaTime, 1);
            _rodLine.SetPosition(1, Vector3.Lerp(_rodLine.GetPosition(1), transform.localPosition, v));
            if (v == 1)
            {
                Stepped -= RodUpdate;
                EndReel();
                return;
            }
        };
        Stepped += RodUpdate;
    }

    private void EndReel()
    {
        throw new NotImplementedException();
    }

    private void HandleCast()
    {
        isFishing = true;
        isAiming = false;
        _aim.SetActive(false);

        var v = 0.0f;

        Action RodUpdate = null; RodUpdate = () => {
            v = math.min(v+Time.deltaTime * 8 / ((_destination - transform.position).magnitude/2.3f), 1);
            var p1 = (_destination - transform.position) / 2;
            p1.y += (_destination - transform.position).magnitude/2.3f; // Increase the y value of p1
            var point = quadBezier(transform.localPosition, p1, _destination - transform.position, v);
            _rodLine.SetPosition(1, point);
            if(v == 1)
            {
                Stepped -= RodUpdate;
                
                return;
            }
        };
        Stepped += RodUpdate;
    }


    private Vector3 quadBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return (Mathf.Pow((1 - t), 2) * p0) + (2 * (1 - t) * t * p1) + (Mathf.Pow(t,2) * p2);
    }
}
