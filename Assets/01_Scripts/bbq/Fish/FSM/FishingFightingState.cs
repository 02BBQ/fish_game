using Unity.Cinemachine;
using UnityEngine;

namespace fishing.FSM
{
    public class FishingFightingState : FishingStateBase
    {
        private float _xMove;
        private float _time;
        private float _initTime;
        private float _timeout = 10f;
        private float _health;
        private float _goal;
        private float _current;
        private float _power;
        private float _barWidth;
        private float _targetWidth;
        private float _halfBarWidth;
        private float _halfTargetWidth;
        private Vector2 _originalTargetSize;
        private CinemachineImpulseSource _impulseSource;
        private float _fightingTime = 0f;
        private const float FIGHTING_DURATION = 5f;
        private float _successChance = 0.5f;
        private float _difficultyMultiplier = 1f;

        public FishingFightingState(Fishing fishing) : base(fishing) 
        {
            _impulseSource = fishing.GetComponent<CinemachineImpulseSource>();
        }

        public override void Enter()
        {
            _originalTargetSize = fishing.FishCanvas.target.sizeDelta;
            _impulseSource.GenerateImpulse();
            fishing.FishingVisual.SetAnchor(true, fishing.Destination);
            fishing.FishCanvas.StartEvent();
            _fightingTime = 0f;
            InitializeFishingMinigame();
        }

        private void InitializeFishingMinigame()
        {
            _time = Time.time;
            _initTime = _time;
            _health = Random.Range(1.5f, 2.5f);
            _goal = Random.Range(1.5f, 2.5f);
            _current = 0f;
            _power = fishing.DancingStep;
            _difficultyMultiplier = CalculateDifficultyMultiplier();

            fishing.FishCanvas.target.sizeDelta = _originalTargetSize;
            
            _barWidth = (fishing.FishCanvas.bar.transform as RectTransform).rect.height;
            _targetWidth = fishing.FishCanvas.target.rect.height;
            _halfBarWidth = _barWidth * 0.5f;
            _halfTargetWidth = _targetWidth * 0.5f;
            
            AdjustTargetSize();
        }

        private float CalculateDifficultyMultiplier()
        {
            float baseMultiplier = 1f;
            if (fishing.FishingFish != null)
            {
                baseMultiplier = Mathf.Clamp(fishing.FishingFish.baseWeight / 10f, 0.5f, 2f);
            }
            return baseMultiplier;
        }

        private void AdjustTargetSize()
        {
            float adjustedSize = _originalTargetSize.y / (Mathf.Max(_power, 0.7f) * _difficultyMultiplier);
            fishing.FishCanvas.target.sizeDelta = new Vector2(
                fishing.FishCanvas.target.sizeDelta.x, 
                adjustedSize);
        }

        public override void Update()
        {
            if (UpdateFightingTime()) return;
            UpdateMinigameLogic();
            UpdateTargetPosition();
            CheckGameConditions();
            UpdateVisuals();
        }

        private bool UpdateFightingTime()
        {
            _fightingTime += Time.deltaTime;
            if (_fightingTime >= FIGHTING_DURATION)
            {
                fishing.Success = _successChance > 0.5f;
                fishing.ChangeState(Fishing.FishingStateType.Reeling);
                return true;
            }
            return false;
        }

        private void UpdateMinigameLogic()
        {
            _time += Time.deltaTime * _power * 0.75f;
            float noise = Mathf.PerlinNoise(Time.time * 3f * _power, _initTime) - 0.5f;
            noise *= 3200 * _power * _difficultyMultiplier;
            _xMove = Mathf.Lerp(_xMove, noise, Time.deltaTime * 1.5f);
            _xMove = Mathf.Clamp(_xMove, -_halfBarWidth + _halfTargetWidth, _halfBarWidth - _halfTargetWidth);
        }

        private void UpdateTargetPosition()
        {
            Vector2 pos = fishing.FishCanvas.target.anchoredPosition;
            pos.y = _xMove;
            fishing.FishCanvas.target.anchoredPosition = pos;
        }

        private void CheckGameConditions()
        {
            _timeout -= Time.deltaTime;

            if (Time.time - _initTime <= 0.3f) return;

            float targetCenterX = fishing.FishCanvas.target.position.y;
            float halfWidth = fishing.FishCanvas.target.rect.height * 0.5f;
            float xMin = targetCenterX - halfWidth;
            float xMax = targetCenterX + halfWidth;
            float mouseX = Input.mousePosition.y;

            if (mouseX >= xMin && mouseX <= xMax)
            {
                _current += Time.deltaTime;
                fishing.FishCanvas.ToggleRotator(true);
                fishing.FishCanvas.SetColor(true);
            }
            else
            {
                _health -= Time.deltaTime * _difficultyMultiplier;
                fishing.FishCanvas.ToggleRotator(false);
                fishing.FishCanvas.SetColor(false);
            }

            CheckGameEnd();
        }

        private void CheckGameEnd()
        {
            if (_current >= _goal)
            {
                fishing.Success = true;
                EndGame();
            }
            else if (_timeout <= 0 || _health <= 0)
            {
                fishing.Success = false;
                EndGame();
            }
        }

        private void EndGame()
        {
            fishing.FishCanvas.ToggleCanvasGroup(false);
            fishing.ChangeState(Fishing.FishingStateType.Reeling);
        }

        private void UpdateVisuals()
        {
            fishing.FishingVisual.Bobber.position = fishing.Destination;
        }

        public override void Exit()
        {
            fishing.FishCanvas.target.sizeDelta = _originalTargetSize;
            fishing.FishCanvas.SetColor(false);
            fishing.FishingVisual.SetAnchor(false);
        }
    }
}