using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using UnityEditor.SceneManagement;
using static Unity.Collections.Unicode;

public class TreeGenerator : MonoBehaviour
{
    [SerializeField] private float trunkHeight = 5f; // 나무 기둥 높이
    [SerializeField] private float trunkWidth = 1f; // 나무 기둥 너비
    [SerializeField] private float minBranchWeight = 0.3f; // 최소 가지 길이
    [SerializeField] private float maxBranchWeight = 0.7f; // 최대 가지 길이
    [SerializeField] private float fruitProbability = 0.3f; // 열매 생성 확률
    [SerializeField] private int maxDepth = 5; // 최대 가지 깊이
    [SerializeField] private int minBranch = 3; // 최대 가지 깊이
    [SerializeField] private int maxBranch = 6; // 최대 가지 깊이
    [SerializeField] private float branchAngleVariation = 30f; // 가지 각도 변화량
    [SerializeField] private AnimationCurve depthScaleCurve;
    [SerializeField] private float growthDuration = 0.5f; // 각 depth별 성장 시간
    [SerializeField] private Mesh cubeMesh;

    public Material brownMaterial;
    public Material redMaterial;

    // depth별로 오브젝트를 저장하는 2차원 리스트
    private List<List<GameObject>> depthObjects = new List<List<GameObject>>();
    private List<List<Vector3>> depthScales = new List<List<Vector3>>();
    private Sequence growthSequence; // 성장 애니메이션 시퀀스

    [ContextMenu("Generate")]
    private void GenerateTree()
    {
        // 기존 오브젝트들 제거
        DestroyTree();
        
        // depth 리스트 초기화
        depthObjects.Clear();
        depthScales.Clear();
        for (int i = 0; i < maxDepth + 2; i++)
        {
            depthObjects.Add(new List<GameObject>());
            depthScales.Add(new List<Vector3>());
        }

        // 나무 기둥 생성
        GameObject trunk = CreateBranch(transform.position, Quaternion.identity, trunkHeight, trunkWidth, out Vector3 scale);
        trunk.transform.SetParent(transform);

        depthObjects[0].Add(trunk); // 기둥은 depth 0에 추가
        depthScales[0].Add(scale); // 기둥은 depth 0에 추가

        // 첫 번째 가지 생성
        GenerateBranches(trunk.transform.position + trunk.transform.up * trunkHeight, trunk.transform, 0);
        // 성장 애니메이션 시작
        StartGrowthAnimation();
    }

    private void StartGrowthAnimation()
    {
        // 기존 시퀀스가 있다면 제거
        if (growthSequence != null)
        {
            growthSequence.Kill();
        }

        // 새로운 시퀀스 생성
        growthSequence = DOTween.Sequence();
        
        // 각 depth별로 순차적으로 성장
        for (int depth = 0; depth < maxDepth + 2; depth++)
        {
            // 현재 depth의 모든 오브젝트를 동시에 성장시키는 시퀀스
            Sequence depthSequence = DOTween.Sequence();
            
            // 현재 depth의 모든 오브젝트에 대한 scale 애니메이션 추가
            for (int i = 0; i < depthObjects[depth].Count; i++)
            {
                GameObject obj = depthObjects[depth][i];
                Vector3 targetScale = depthScales[depth][i];
                int x = depth;
                // Join을 사용하여 동시에 실행
                depthSequence.Join(obj.transform.DOScale(targetScale, growthDuration)
                    .SetEase(Ease.Linear));
            }

            if(depth == 0)
                growthSequence.Prepend(depthSequence);

            // 현재 depth의 시퀀스를 메인 시퀀스에 추가
            growthSequence.Append(depthSequence);
        }

        // 시퀀스 시작
        growthSequence.Play();
    }

    private void GenerateBranches(Vector3 startPos, Transform parent, int depth)
    {
        if (depth >= maxDepth) return;

        int branchCount = Random.Range(minBranch, maxBranch + 1);
        // 깊이에 따른 크기 보정값 계산
        float depthT = (float)depth / maxDepth;
        float depthScale = depthScaleCurve.Evaluate(depthT);

        for (int i = 0; i < branchCount; i++)
        {
            // 랜덤한 방향 계산
            float angleX = Random.Range(-branchAngleVariation, branchAngleVariation);
            float angleZ = Random.Range(-branchAngleVariation, branchAngleVariation);
            Quaternion direction = parent.rotation * Quaternion.Euler(angleX, 0f, angleZ);

            // 랜덤한 길이와 깊이에 따른 보정
            float weight = Random.Range(minBranchWeight, maxBranchWeight) * depthScale;
            float length = trunkHeight * weight;
            // 부모 가지보다 작은 너비
            float width = trunkWidth * weight;

            // 가지 생성
            GameObject branch = CreateBranch(startPos, direction, length, width, out Vector3 scale);
            branch.name = $"Branch {depth}-{i}";
            branch.transform.SetParent(transform);
            Vector3 tip = branch.transform.position;

            // 현재 depth의 리스트에 가지 추가
            depthObjects[depth+1].Add(branch);
            depthScales[depth+1].Add(scale);

            if (depth+1 == maxDepth && Random.value < fruitProbability)
            {
                GameObject fruit = CreateFruit(tip + branch.transform.up * length);
                fruit.transform.SetParent(transform);
                // 열매도 현재 depth의 리스트에 추가
                depthObjects[depth + 2].Add(fruit);
                depthScales[depth + 2].Add(fruit.transform.localScale);
                fruit.transform.localScale = Vector3.zero; // 초기 크기를 0으로 설정
            }

            // 다음 가지 생성 (현재 가지의 끝점에서 시작)
            GenerateBranches(tip + branch.transform.up * length, branch.transform, depth + 1);
        }
    }

    private GameObject CreateBranch(Vector3 startPos, Quaternion angle, float length, float width, out Vector3 scale)
    {
        GameObject branch = new GameObject();
        branch.AddComponent<MeshFilter>().mesh = cubeMesh;
        branch.AddComponent<MeshRenderer>().material = brownMaterial;
        scale = new Vector3(width, length, width);
        branch.transform.localScale = Vector3.zero;
        branch.transform.rotation = angle;
        branch.transform.position = startPos;
        return branch;
    }

    private GameObject CreateFruit(Vector3 position)
    {
        GameObject fruit = new GameObject();
        fruit.AddComponent<MeshFilter>().mesh = cubeMesh;
        fruit.AddComponent<MeshRenderer>().material = redMaterial;
        fruit.transform.position = position;
        fruit.transform.rotation = Quaternion.Euler(180f, 0f, 0f);
        fruit.transform.localScale = Vector3.one * 0.2f;
        return fruit;
    }

    [ContextMenu("Remove")]
    public void DestroyTree()
    {
        // 시퀀스 정리
        if (growthSequence != null)
        {
            growthSequence.Kill();
            growthSequence = null;
        }
        
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        // depth 리스트 초기화
        depthObjects.Clear();
        depthScales.Clear();
        for (int i = 0; i < maxDepth + 2; i++)
        {
            depthObjects.Add(new List<GameObject>());
            depthScales.Add(new List<Vector3>());
        }
    }
} 