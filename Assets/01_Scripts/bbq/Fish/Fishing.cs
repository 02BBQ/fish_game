using UnityEngine;
using fishing.FSM;
using fishing.Network;
using Debug = UnityEngine.Debug;
using Vector3 = UnityEngine.Vector3;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace fishing.FSM
{
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
        [SerializeField] private FishingVisual fishingVisual;
        [SerializeField] private FishingRegion fishingRegion;
        [SerializeField] private FishSO fishSOBase;
        [SerializeField] private FishTray fishTray; 
        [SerializeField] private LayerMask toAimLayer;
        
        [Header("Fishing UI")]
        [SerializeField] private FishCanvas fishCanvas;

        // State management
        public FishingStateMachine StateMachine { get; private set; }
        public Dictionary<FishingStateType, FishingStateBase> States { get; private set; } = new Dictionary<FishingStateType, FishingStateBase>();
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
        public string CurrentFishGUID => currentFishGuid;
        public FishingRod FishingRodSO { get; private set; }
        public Item UsedBait { get; set; }

        // Public properties
        public FishingVisual FishingVisual => fishingVisual;
        public FishCanvas FishCanvas => fishCanvas;
        public FishingRegion FishingRegion => fishingRegion;
        public FishSO FishSOBase => fishSOBase; 
        public FishTray FishTray => fishTray;
        public LayerMask ToAimLayer => toAimLayer;

        // Private properties
        private string currentFishGuid;
        private float dancingStep = 1;

        public float DancingStep => dancingStep;

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
                return;
            }

            SubscribeToEvents();

            States.Add(FishingStateType.Aiming, new FishingAimingState(this));
            States.Add(FishingStateType.Casting, new FishingCastingState(this));
            States.Add(FishingStateType.Fighting, new FishingFightingState(this));
            States.Add(FishingStateType.Fishing, new FishingState(this));
            States.Add(FishingStateType.Idle, new FishingIdleState(this));
            States.Add(FishingStateType.Reeling, new FishingReelingState(this));
            
            StateMachine.Initialize(States[FishingStateType.Idle]);
        }

        private void OnDisable() 
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            Player.playerInput.FishingDown += HandleHoldStart;
            Player.playerInput.FishingUp += HandleHoldEnd;
            Player.playerAnim.OnCastRod += HandleCast;
        }

        private void UnsubscribeFromEvents()
        {
            Player.playerInput.FishingDown -= HandleHoldStart;
            Player.playerInput.FishingUp -= HandleHoldEnd;
            Player.playerAnim.OnCastRod -= HandleCast;
        }

        private void Update()
        {
            StateMachine.Update();
        }

        private void HandleHoldStart()
        {
            IsMouseDown = true;
            StateMachine.OnHoldStart();
        }

        private void HandleHoldEnd()
        {
            IsMouseDown = false;
            StateMachine.OnHoldEnd();
        }

        private void HandleCast()
        {
            if (CurrentStateType == FishingStateType.Aiming)
            {
                ChangeState(FishingStateType.Casting);
            }
        }

        public void ChangeState(FishingStateType newStateType)
        {
            CurrentStateType = newStateType;
            
            StateMachine.ChangeState(States[newStateType]);
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

        public void SetModel(FishingRod fishingRod)
        {
            FishingRodSO = fishingRod;
            fishCanvas = fishingRod.FishCanvas;
            fishingVisual = fishingRod.FishingVisual;
        }

        public void UpdateState(string guid, float step)
        {
            currentFishGuid = guid;
            dancingStep = step;
        }
    }
}