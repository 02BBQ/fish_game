/*using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

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

        if (GUILayout.Button("🎣 Rarity 데이터 불러오기"))
        {
            FishDataLoader.LoadData(OnRarityDataLoaded);
        }

        if (GUILayout.Button("🥓 Trait 데이터 불러오기"))
        {
            FishDataLoader.LoadData(OnTraitDataLoaded);
        }
    }

    private void OnFishDataLoaded(string rawJson)
    {
        var arrays = JsonHelper.SplitJsonArray3(rawJson);
        string fishWrapped = "{\"items\":" + arrays[0] + "}";

        FishStructTable parsed = JsonUtility.FromJson<FishStructTable>(fishWrapped);
        EditorCoroutineUtility.StartCoroutineOwnerless(FishSOGenerator.Generate(parsed.items));
    }

    private void OnRarityDataLoaded(string rawJson)
    {
        var arrays = JsonHelper.SplitJsonArray3(rawJson);
        string rarityWrapped = "{\"items\":" + arrays[1] + "}";

        RarityWeightTable parsed = JsonUtility.FromJson<RarityWeightTable>(rarityWrapped);
        FishSOGenerator.ProcessRarityTable(parsed.items); // 임시용 처리 함수
    }

    private void OnTraitDataLoaded(string rawJson)
    {
        var arrays = JsonHelper.SplitJsonArray3(rawJson);
        string traitWrapped = "{\"traits\":" + arrays[2] + "}";

        Debug.Log(traitWrapped);

        TraitStringWrapper parsed = JsonUtility.FromJson<TraitStringWrapper>(traitWrapped);

        if (parsed.traits == null || parsed.traits.Length == 0)
        {
            Debug.LogWarning("⚠️ Trait 배열이 비어 있습니다.");
            return;
        }

        TraitListSO so = ScriptableObject.CreateInstance<TraitListSO>();
        so.traits = parsed.traits;

        const string path = "Assets/10_SO/bbq/TraitTable.asset";
        AssetDatabase.CreateAsset(so, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

public static class JsonHelper
{
    public static string[] SplitJsonArray3(string json)
    {
        List<string> result = new();
        int depth = 0;
        int startIdx = 1;

        for (int i = 1; i < json.Length - 1; i++)
        {
            if (json[i] == '[') depth++;
            else if (json[i] == ']') depth--;

            if (depth == 0 && json[i] == ',')
            {
                string part = json.Substring(startIdx, i - startIdx).Trim();
                result.Add(part);
                startIdx = i + 1;
            }
        }

        // 마지막 요소 추가
        result.Add(json.Substring(startIdx, json.Length - 1 - startIdx).Trim());

        return result.ToArray();
    }
}


[System.Serializable]
public class TraitStringWrapper
{
    public string[] traits;
}*/

//build