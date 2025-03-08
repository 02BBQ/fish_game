using System;
using Unity.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Fishing : MonoBehaviour
{
    public enum FishingState
    {
        Aiming,
        Casting,
        Reeling,
        Fishing,
        Idle,    
    }

    private RaycastHit _hit;

    private Action LineUpdate;

    public FishingState currentState {get; private set;} = FishingState.Idle;

    [SerializeField] private GameObject _aim;
    public Player player {get; private set;}
    private PlayerMovement playerMovement => player.playerMovement;
    
    public bool isFishing {get; private set;}
    public bool isAiming {get; private set;}

    public bool rodThrown;

    private float _maxDistance = 10;
    private float _distance = 1;

    private event Action Stepped;
    public Vector3 destination;

    [SerializeField] private LayerMask _toAimLayer;
    [SerializeField] private LineRenderer _rodLine;


    void Awake()
    {
        _rodLine.enabled = false;
        _aim.SetActive(false);
        player = GetComponentInParent<Player>();
        if (player == null){
            Debug.LogWarning("Something's Wrong...");
            Destroy(gameObject);
        }

        _rodLine.useWorldSpace = true;

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
        if (currentState == FishingState.Fishing)
        {
            PullReel();
            return;
        }
        else if (currentState == FishingState.Idle)
        {
            Aim();
            return;
        }
            
    }

    private void Aim()
    {
        playerMovement.StopMoveTarget();
        var Rig = transform.parent.GetComponentInChildren<Animator>().gameObject;

        _aim.SetActive(true);
        var dir = Rig.transform.forward;
        var origin = player.transform.position;
        var trans = _aim.transform;
        // player.transform.position = origin;
        trans.position = origin;
        _distance = 1;
        currentState = FishingState.Aiming;

        Action AimUpdate = null; AimUpdate = () => {
            _distance = math.min(_distance+Time.deltaTime * 25, _maxDistance);
            if (currentState != FishingState.Aiming)
            {
                Stepped -= AimUpdate;
                destination = trans.position;
                return;
            }
            trans.position = origin + dir * _distance;
            if (Physics.Raycast(trans.position + Vector3.up*4, -Vector3.up, out _hit, 50, _toAimLayer))
            {
                trans.position = new Vector3(trans.position.x, -_hit.point.y, trans.position.z);
            }
        };
        Stepped += AimUpdate;
    }

    void handleHoldEnd()
    {
        if (currentState == FishingState.Aiming)
        {
            CastRod();
        }
        
    }

    private void CastRod()
    {
        player.playerAnim.SetBool("Fishing", true);
    }

    private void PullReel()
    {
        player.playerAnim.SetBool("Fishing", false);
        currentState = FishingState.Reeling;
        var v = 0.0f;

        var goal = transform.position;
        goal.y = _rodLine.GetPosition(1).y;

        Action RodUpdate = null; RodUpdate = () => {
            v = math.min(v+Time.deltaTime*0.5f, 1);
            _rodLine.SetPosition(0, transform.position);
            _rodLine.SetPosition(1, Vector3.Lerp(_rodLine.GetPosition(1), goal, v));
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
        _rodLine.enabled = false;
        currentState = FishingState.Idle;
    }

    private void HandleCast()
    {
        currentState = FishingState.Casting;
        _aim.SetActive(false);

        var v = 0.0f;

        _rodLine.enabled = true;

        Action RodUpdate = null; RodUpdate = () => {
            v = math.min(v+Time.deltaTime * 8 / ((destination - transform.position).magnitude/2.3f), 1);
            var p1 = (destination + transform.position) / 2;
            p1.y += (destination - transform.position).magnitude/2.3f; // Increase the y value of p1
            var point = quadBezier(transform.position, p1, destination, v);
            _rodLine.SetPosition(0, transform.position);
            _rodLine.SetPosition(1, point);
            if(v == 1)
            {
                Stepped -= RodUpdate;
                currentState = FishingState.Fishing;
                if (_hit.collider != null && _hit.collider.gameObject.layer != LayerMask.NameToLayer("Suimono_Water"))
                {
                    PullReel();
                    return;
                }
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
