using UnityEngine;
using Debug = UnityEngine.Debug;
using Vector3 = UnityEngine.Vector3;

public class Fishing : MonoBehaviour
{
    public enum FishingStateType
    {
        Aiming,
        Casting,
        Reeling,
        Fishing,
        Fighting,
        Idle,    
    }

    [Header("References")]
    [SerializeField] public FishingVisual fishingVisual;
    [SerializeField] private FishingRegion _fishingRegion;
    [SerializeField] private FishSO _fishSOBase;
    [SerializeField] private FishTray _fishTray;
    
    [Header("Fishing UI")]
    public FishCanvas fishCanvas;

    // State management
    public FishingStateMachine StateMachine { get; private set; }
    public FishingStateType CurrentStateType { get; private set; } = FishingStateType.Idle;
    
    // Fishing properties
    public Player Player { get; private set; }
    public PlayerMovement PlayerMovement => Player.playerMovement;
    public bool IsMouseDown { get; private set; }
    public bool Success { get; set; }
    public Vector3 Destination { get; set; }
    public float Distance { get; set; } = 1;
    public float MaxDistance { get; private set; } = 10;
    public RaycastHit Hit { get; set; }
    public FishData FishingFish { get; set; }
    public FishSO Fish { get; set; }
    public FishingVisual FishingVisual => fishingVisual;
    public FishCanvas FishCanvas => fishCanvas;
    public FishingRegion FishingRegion => _fishingRegion;
    public FishSO FishSOBase => _fishSOBase;
    public FishTray FishTray => _fishTray;
    public string CurrentFishGUID => currentFishGuid;

    // Layer masks
    [SerializeField] private LayerMask _toAimLayer;
    public LayerMask ToAimLayer => _toAimLayer;

    // Fishing properties
    public string currentFishGuid;
    public float dancingStep = 1;

    private void Awake()
    {
        StateMachine = new FishingStateMachine();
    }

    private void OnEnable() 
    {
        Player = Definder.Player;
        if (Player == null)
        {
            Debug.LogWarning("Something's Wrong...");
            Destroy(gameObject);
        }

        Player.playerInput.FishingDown += HandleHoldStart;
        Player.playerInput.FishingUp += HandleHoldEnd;
        Player.playerAnim.OnCastRod += HandleCast;

        // Initialize state machine
        StateMachine.Initialize(new FishingIdleState(this));
    }

    private void OnDisable() 
    {
        Player.playerInput.FishingDown -= HandleHoldStart;
        Player.playerInput.FishingUp -= HandleHoldEnd;
        Player.playerAnim.OnCastRod -= HandleCast;
    }

    void Update()
    {
        StateMachine.Update();
    }

    void HandleHoldStart()
    {
        IsMouseDown = true;
        StateMachine.OnHoldStart();
    }

    void HandleHoldEnd()
    {
        IsMouseDown = false;
        StateMachine.OnHoldEnd();
    }

    void HandleCast()
    {
        if (CurrentStateType == FishingStateType.Aiming)
        {
            ChangeState(FishingStateType.Casting);
        }
    }

    public void ChangeState(FishingStateType newStateType)
    {
        CurrentStateType = newStateType;
        
        switch (newStateType)
        {
            case FishingStateType.Aiming:
                StateMachine.ChangeState(new FishingAimingState(this));
                break;
            case FishingStateType.Casting:
                StateMachine.ChangeState(new FishingCastingState(this));
                break;
            case FishingStateType.Fishing:
                StateMachine.ChangeState(new FishingState(this));
                break;
            case FishingStateType.Fighting:
                StateMachine.ChangeState(new FishingFightingState(this));
                break;
            case FishingStateType.Reeling:
                StateMachine.ChangeState(new FishingReelingState(this));
                break;
            case FishingStateType.Idle:
                StateMachine.ChangeState(new FishingIdleState(this));
                break;
        }
    }

    // Helper methods
    public Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return (Mathf.Pow((1 - t), 2) * p0) + (2 * (1 - t) * t * p1) + (Mathf.Pow(t, 2) * p2);
    }

    public Vector3 QuadBeizer(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        var q0 = Bezier(p0, p1, p2, t);
        var q1 = Bezier(p1, p2, p3, t);
        return Vector3.Lerp(q0, q1, t);
    }

    public int GetCurrentRegionIndex() => Player.GetCurrentOcean();
}