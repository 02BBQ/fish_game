using Unity.Cinemachine;
using UnityEngine;

namespace fishing.FSM
{
    public class FishingFightingState : FishingStateBase
    {
        private float xMove;
        private float time;
        private float initTime;
        private float timeout = 10f;
        private float health;
        private float goal;
        private float current;
        private float power;
        private float barWidth;
        private float targetWidth;
        private float halfBarWidth;
        private float halfTargetWidth;
        private Vector2 originalTargetSize;
        private CinemachineImpulseSource impulseSource;
        private float fightingTime = 0f;
        private const float FIGHTING_DURATION = 5f;
        private float successChance = 0.5f;
        private float difficultyMultiplier = 1f;

        public FishingFightingState(Fishing fishing) : base(fishing) 
        {
            impulseSource = fishing.GetComponent<CinemachineImpulseSource>();
            impulseSource.GenerateImpulse();
            fishing.FishingVisual.SetAnchor(true, fishing.Destination);
            fishing.FishCanvas.StartEvent();
            originalTargetSize = fishing.FishCanvas.target.sizeDelta;
        }

        public override void Enter()
        {
            fightingTime = 0f;
            InitializeFishingMinigame();
        }

        private void InitializeFishingMinigame()
        {
            time = Time.time;
            initTime = time;
            health = Random.Range(1.5f, 2.5f);
            goal = Random.Range(1.5f, 2.5f);
            current = 0f;
            power = fishing.DancingStep;
            difficultyMultiplier = CalculateDifficultyMultiplier();

            fishing.FishCanvas.target.sizeDelta = originalTargetSize;
            
            barWidth = (fishing.FishCanvas.bar.transform as RectTransform).rect.height;
            targetWidth = fishing.FishCanvas.target.rect.height;
            halfBarWidth = barWidth * 0.5f;
            halfTargetWidth = targetWidth * 0.5f;
            
            AdjustTargetSize();
        }

        private float CalculateDifficultyMultiplier()
        {
            // 물고기의 희귀도나 크기에 따라 난이도 조절
            float baseMultiplier = 1f;
            if (fishing.FishingFish != null)
            {
                baseMultiplier = Mathf.Clamp(fishing.FishingFish.baseWeight / 10f, 0.5f, 2f);
            }
            return baseMultiplier;
        }

        private void AdjustTargetSize()
        {
            float adjustedSize = originalTargetSize.y / (Mathf.Max(power, 0.7f) * difficultyMultiplier);
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
            fightingTime += Time.deltaTime;
            if (fightingTime >= FIGHTING_DURATION)
            {
                fishing.Success = successChance > 0.5f;
                fishing.ChangeState(Fishing.FishingStateType.Reeling);
                return true;
            }
            return false;
        }

        private void UpdateMinigameLogic()
        {
            time += Time.deltaTime * power * 0.75f;
            float noise = Mathf.PerlinNoise(Time.time * 3f * power, initTime) - 0.5f;
            noise *= 3200 * power * difficultyMultiplier;
            xMove = Mathf.Lerp(xMove, noise, Time.deltaTime * 1.5f);
            xMove = Mathf.Clamp(xMove, -halfBarWidth + halfTargetWidth, halfBarWidth - halfTargetWidth);
        }

        private void UpdateTargetPosition()
        {
            Vector2 pos = fishing.FishCanvas.target.anchoredPosition;
            pos.y = xMove;
            fishing.FishCanvas.target.anchoredPosition = pos;
        }

        private void CheckGameConditions()
        {
            timeout -= Time.deltaTime;

            if (Time.time - initTime <= 0.3f) return;

            float targetCenterX = fishing.FishCanvas.target.position.y;
            float halfWidth = fishing.FishCanvas.target.rect.height * 0.5f;
            float xMin = targetCenterX - halfWidth;
            float xMax = targetCenterX + halfWidth;
            float mouseX = Input.mousePosition.y;

            if (mouseX >= xMin && mouseX <= xMax)
            {
                current += Time.deltaTime;
                fishing.FishCanvas.ToggleRotator(true);
                fishing.FishCanvas.SetColor(true);
            }
            else
            {
                health -= Time.deltaTime * difficultyMultiplier;
                fishing.FishCanvas.ToggleRotator(false);
                fishing.FishCanvas.SetColor(false);
            }

            CheckGameEnd();
        }

        private void CheckGameEnd()
        {
            if (current >= goal)
            {
                fishing.Success = true;
                EndGame();
            }
            else if (timeout <= 0 || health <= 0)
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
            fishing.FishCanvas.target.sizeDelta = originalTargetSize;
            fishing.FishCanvas.SetColor(false);
            fishing.FishingVisual.SetAnchor(false);
        }
    }
}