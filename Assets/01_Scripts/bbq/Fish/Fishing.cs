using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;
using DG.Tweening;
using Unity.VisualScripting;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

public class Fishing : MonoBehaviour
{
    // public FishRodStat FishRodStat;
    public enum FishingState
    {
        Aiming,
        Casting,
        Reeling,
        Fishing,
        Fighting,
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
    public FishingVisual fishingVisual;

    [SerializeField] private FishingRegion _fishingRegion;

    private int _currentRegionIndex => player.GetCurrentOcean();

    private FishData getFish => _fishingRegion.fishWeights[_currentRegionIndex-1].GetFish();
    private FishData _fishingFish;

    [SerializeField] private FishSO _fishSOBase;

    private FishSO fish;

    public bool Suc;

    public bool isMosueDown = false;

    [Header("Fishing")]
    public FishCanvas fishCanvas;

    private RectTransform target => fishCanvas.target;
    
    private RectTransform bar => fishCanvas.bar;

    float barWidth => (bar.transform as RectTransform).rect.width;
    float targetWidth => target.rect.width;
    float halfBarWidth => barWidth * 0.5f;
    float halfTargetWidth => targetWidth * 0.5f; 

    void Awake()
    {
        // _rodLine.enabled = false;
        _aim.SetActive(false);
        player = Definder.Player;
        if (player == null){
            Debug.LogWarning("Something's Wrong...");
            Destroy(gameObject);
        }

        // _rodLine.useWorldSpace = true;

        player.playerInput.FishingDown += handleHoldStart;
        player.playerInput.FishingUp += handleHoldEnd;

        player.playerAnim.OnCastRod += HandleCast;  
        // player.playerInput
    }

    private void OnDestroy() {
        player.playerInput.FishingDown -= handleHoldStart;
        player.playerInput.FishingUp -= handleHoldEnd;
        player.playerAnim.OnCastRod -= HandleCast;  
    }

    void Update()
    {
        Stepped?.Invoke();
    }

    void handleHoldStart()
    {
        isMosueDown = true;
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
            _distance = isMosueDown ? math.min(_distance+Time.deltaTime * 25, _maxDistance) : _distance;
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
        isMosueDown = false;
        if (currentState == FishingState.Aiming)
        {
            CastRod();
        }
        
    }

    private void CastRod()
    {
        // player.playerMovement.enabled = false;
        playerMovement.movable = false;
        player.playerSlot.CanChange = false;
        player.playerAnim.SetBool("Fishing", true);
    }

    private void PullReel()
    {
        if (Suc && _fishingFish != null)
        {
            fish = Instantiate(_fishSOBase);
            fish.Initialize(_fishingFish);      
        }
        player.playerAnim.SetBool("Fishing", false);
        currentState = FishingState.Reeling;
        var v = 0.0f;

        var goal = transform.position;
        // goal.y = _rodLine.GetPosition(1).y;

        Action RodUpdate = null; RodUpdate = () => {
            v = math.min(v+Time.deltaTime*0.75f, 1);
            // _rodLine.SetPosition(0, transform.position);
            // _rodLine.SetPosition(1, Vector3.Lerp(_rodLine.GetPosition(1), goal, v));
            fishingVisual.bobber.position = Vector3.Lerp(fishingVisual.bobber.position, goal, v);
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
        // _rodLine.enabled = false;
        if (Suc)
        {
            InventoryManager.Instance.AddItem(fish);
        }
        else
        {
            Destroy(fish);
        }
        fishingVisual.ResetBobber();
        // player.playerMovement.enabled = true;
        playerMovement.movable = true;
        currentState = FishingState.Idle;
        player.playerSlot.CanChange = true;
    }

    private void HandleCast()
    {
        currentState = FishingState.Casting;
        _aim.SetActive(false);

        var v = 0.0f;

        // _rodLine.enabled = true;

        Suc = false;

        var yDiff = transform.position.y - destination .y;

        

        var p0 = transform.position - transform.forward;
        var p3 = destination;

        Action RodUpdate = null; RodUpdate = () => {
            v = math.min(v+Time.deltaTime * 8 / ((destination - p0).magnitude/2.3f), 1);
            var p1 = p0 + Vector3.up * Vector3.Distance(p0, p3)/1.75f;
            Vector3 p2 = (p1+destination)/2; 
            p2.y = p1.y;
            var point = QuadBeizer(p0, p1, p2, destination, v);
            // _rodLine.SetPosition(0, transform.position);
            fishingVisual.bobber.position = point;
            if(v == 1)
            {
                Stepped -= RodUpdate;
                currentState = FishingState.Fishing;

                float timeout = UnityEngine.Random.Range(5f, 21f);
                Action FishingUpdate = null; FishingUpdate = () => {
                    fishingVisual.bobber.position = point;
                    timeout -= Time.deltaTime;
                    // _rodLine.SetPosition(0, transform.position);
                    if (timeout <= 0 && FishingState.Fishing == currentState)
                    {
                        Stepped -= FishingUpdate;
                        FishingEvent();
                        return;
                    }    
                    if (FishingState.Fishing != currentState)
                    {
                        Stepped -= FishingUpdate;
                        return;
                    }
                };

                Stepped += FishingUpdate;

                if (_hit.collider != null && _hit.collider.gameObject.layer != LayerMask.NameToLayer("Suimono_Water"))
                {
                    PullReel();
                    return;
                }
                else
                {
                    // print(_currentRegionIndex);
                    // print(_fishingRegion.fishWeights[_currentRegionIndex]);
                    _fishingFish = getFish;
                }
                return;
            }
        };
        Stepped += RodUpdate;
    }


    private Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return (Mathf.Pow((1 - t), 2) * p0) + (2 * (1 - t) * t * p1) + (Mathf.Pow(t,2) * p2);
    }

    private Vector3 QuadBeizer(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        var q0 = Bezier(p0, p1, p2, t);
        var q1 = Bezier(p1, p2, p3, t);
        return Vector3.Lerp(q0, q1, t);
    }

    private void FishingEvent()
    {
        if (currentState == FishingState.Fishing && _fishingFish != null)
        {
            currentState = FishingState.Fighting;
            fishCanvas.ToggleCanvasGroup(true);

            target.position = Vector3.zero;

            float xMove = 0;

            float time = Time.time;
            float initTime = time;

            float timeout = 10f;
            float health = 2f;
            float goal = 2f;
            float current = 0f;

            float power = _fishingFish.GetDancingStep();

            target.sizeDelta = new Vector2(target.sizeDelta.x / power, target.sizeDelta.y);

            Action FishingUpdate = null; FishingUpdate = () => {
                time += Time.deltaTime* power*.75f;
                float noise = Mathf.PerlinNoise(time, initTime) -.5f; noise *= 3200 * power;
                xMove = Mathf.Lerp(xMove, noise, Time.deltaTime * 2);
                xMove = Mathf.Clamp(xMove, -halfBarWidth + halfTargetWidth, halfBarWidth - halfTargetWidth);
                target.anchoredPosition = new Vector2(xMove * halfBarWidth, 0);

                Vector2 pos = target.anchoredPosition;
                pos.x = xMove;
                target.anchoredPosition = pos;

                timeout -= Time.deltaTime;

                float targetCenterX = target.position.x;
                float halfWidth = target.rect.width * 0.5f;
                float xMin = targetCenterX - halfWidth;
                float xMax = targetCenterX + halfWidth;

                // 현재 마우스의 x 좌표
                float mouseX = Input.mousePosition.x;

                // _rodLine.SetPosition(0, transform.position);

                if (time - initTime > 0.5f)
                {
                    if (mouseX >= xMin && mouseX <= xMax)
                    {
                        current += Time.deltaTime;
                    }
                    else
                    {
                        health -= Time.deltaTime;
                    }
                }

                if (current >= goal)
                {
                    Suc = true;
                    Stepped -= FishingUpdate;
                    fishCanvas.ToggleCanvasGroup(false);
                    PullReel();
                    return;
                }

                // 실패
                if (timeout <= 0 || health <= 0)
                {
                    Suc = false;
                    Stepped -= FishingUpdate;
                    fishCanvas.ToggleCanvasGroup(false);
                    PullReel();
                    return;
                }
            };

            Stepped += FishingUpdate;
        }
        else
        {
            fishCanvas.ToggleCanvasGroup(false);
        }
    }

    // private ienumrator FishEvent()
    // {
    //     yield return new WaitForSeconds(5);
    //     ToggleCanvasGroup(false);
    // }
}
