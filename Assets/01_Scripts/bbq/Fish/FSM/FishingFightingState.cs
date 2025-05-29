using Unity.Cinemachine;
using UnityEngine;

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

    private Vector2 originalTargetSize; // 원본 크기 저장

    private CinemachineImpulseSource impulseSource;

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
        // Initialize fishing minigame values
        time = Time.time;
        initTime = time;
        health = Random.Range(1.5f, 2.5f);
        goal = Random.Range(1.5f, 2.5f);
        current = 0f;
        power = fishing.dancingStep;

        fishing.FishCanvas.target.sizeDelta = originalTargetSize;
        
        // Cache UI values
        barWidth = (fishing.FishCanvas.bar.transform as RectTransform).rect.height;
        targetWidth = fishing.FishCanvas.target.rect.height;
        halfBarWidth = barWidth * 0.5f;
        halfTargetWidth = targetWidth * 0.5f;
        
        fishing.FishCanvas.target.sizeDelta = new Vector2(
            fishing.FishCanvas.target.sizeDelta.x, 
            fishing.FishCanvas.target.sizeDelta.y / Mathf.Max(power, .7f));
    }

    public override void Update()
    {
        time += Time.deltaTime * power * .75f;
        float noise = Mathf.PerlinNoise(Time.time * 3f * power, initTime) - .5f; 
        noise *= 3200 * power;
        xMove = Mathf.Lerp(xMove, noise, Time.deltaTime * 1.5f);
        xMove = Mathf.Clamp(xMove, -halfBarWidth + halfTargetWidth, halfBarWidth - halfTargetWidth);
        
        Vector2 pos = fishing.FishCanvas.target.anchoredPosition;
        pos.y = xMove;
        fishing.FishCanvas.target.anchoredPosition = pos;

        timeout -= Time.deltaTime;

        float targetCenterX = fishing.FishCanvas.target.position.y;
        float halfWidth = fishing.FishCanvas.target.rect.height * 0.5f;
        float xMin = targetCenterX - halfWidth;
        float xMax = targetCenterX + halfWidth;
        float mouseX = Input.mousePosition.y;

        if (Time.time - initTime > 0.3f)
        {
            if (mouseX >= xMin && mouseX <= xMax)
            {
                current += Time.deltaTime;
                fishing.FishCanvas.ToggleRotator(true);
                fishing.FishCanvas.SetColor(true);
            }
            else
            {
                health -= Time.deltaTime;
                fishing.FishCanvas.ToggleRotator(false);
                fishing.FishCanvas.SetColor(false);
            }
        }

        if (current >= goal)
        {
            fishing.Success = true;
            fishing.FishCanvas.ToggleCanvasGroup(false);
            fishing.ChangeState(Fishing.FishingStateType.Reeling);
            return;
        }

        if (timeout <= 0 || health <= 0)
        {
            fishing.Success = false;
            fishing.FishCanvas.ToggleCanvasGroup(false);
            fishing.ChangeState(Fishing.FishingStateType.Reeling);
            return;
        }

        fishing.fishingVisual.bobber.position = fishing.Destination;
    }

    public override void Exit()
    {
        fishing.FishCanvas.SetColor(false);
        fishing.FishingVisual.SetAnchor(false);
    }
}