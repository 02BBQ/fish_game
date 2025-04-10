using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using Unity.EditorCoroutines.Editor;

public class FishDataEditor : EditorWindow
{
    [MenuItem("Tools/Load Character Data")]
    public static void ShowWindow()
    {
        GetWindow<FishDataEditor>("Character Data Loader");
    }

    private void OnGUI()
    {
        GUILayout.Label("🦈 Google Sheet에서 데이터 불러오기", EditorStyles.boldLabel);

        if (GUILayout.Button("🐟 Fish 데이터 불러오기"))
        {
            FishDataLoader.LoadData(OnFishDataLoaded);
        }

        if (GUILayout.Button("🎣 Rarity Table 불러오기"))
        {
            FishDataLoader.LoadData(OnRarityDataLoaded);
        }
    }

    private void OnFishDataLoaded(string rawJson)
    {
        var arrays = JsonHelper.SplitJsonArray(rawJson);
        string fishWrapped = "{\"items\":" + arrays[0] + "}";

        FishStructTable parsed = JsonUtility.FromJson<FishStructTable>(fishWrapped);
        EditorCoroutineUtility.StartCoroutineOwnerless(FishSOGenerator.GenerateFish(parsed.items));
    }

    private void OnRarityDataLoaded(string rawJson)
    {
        var arrays = JsonHelper.SplitJsonArray(rawJson);
        string rarityWrapped = "{\"items\":" + arrays[1] + "}";

        RarityWeightTable parsed = JsonUtility.FromJson<RarityWeightTable>(rarityWrapped);
        FishSOGenerator.ProcessRarityTable(parsed.items); // 임시용 처리 함수
    }
}

public static class JsonHelper
{
    public static string[] SplitJsonArray(string json)
    {
        int depth = 0;
        int splitIndex = -1;

        for (int i = 0; i < json.Length; i++)
        {
            if (json[i] == '[') depth++;
            else if (json[i] == ']') depth--;

            if (depth == 1 && json[i] == ',' && splitIndex == -1)
            {
                splitIndex = i;
                break;
            }
        }

        if (splitIndex == -1)
        {
            Debug.LogError("⚠️ JSON 구조가 잘못되었습니다.");
            return new[] { "[]", "[]" };
        }

        string first = json.Substring(1, splitIndex - 1).Trim();
        string second = json.Substring(splitIndex + 1, json.Length - splitIndex - 2).Trim();

        return new[] { first, second };
    }
}

// public static class SOGenerator
// {
//     private const string SAVE_PATH = "Assets/Data/Characters";

//     public static void CreateCharacterSOs(CharacterData[] data)
//     {
//         if (!Directory.Exists(SAVE_PATH))
//             Directory.CreateDirectory(SAVE_PATH);

//         foreach (var entry in data)
//         {
//             CharacterSO asset = ScriptableObject.CreateInstance<CharacterSO>();
//             asset.id = entry.id;
//             asset.key = entry.key;
//             asset.spec = entry.spec;
//             asset.koreanName = entry.koreanName;
//             asset.baseWeight = entry.baseWeight;
//             asset.basePrice = entry.basePrice;
//             asset.maxWeightMultiplier = entry.maxWeightMultiplier;
//             asset.minWeightMultiplier = entry.minWeightMultiplier;
//             asset.dancingStepMax = entry.dancingStepMax;
//             asset.dancingStepMin = entry.dancingStepMin;
//             asset.description = entry.description;

//             string assetPath = Path.Combine(SAVE_PATH, $"Character_{entry.id}_{entry.koreanName}.asset");

//             AssetDatabase.CreateAsset(asset, assetPath);
//         }

//         AssetDatabase.SaveAssets();
//         AssetDatabase.Refresh();
//         Debug.Log($"✅ {data.Length}개의 ScriptableObject가 생성되었습니다.");
//     }
// }

