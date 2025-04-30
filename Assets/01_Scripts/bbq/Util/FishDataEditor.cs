using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using Unity.EditorCoroutines.Editor;
using System.Collections.Generic;
using System.IO;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

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
        
        if (GUILayout.Button("🍳 Fishrod 데이터 불러오기"))
        {
            FishDataLoader.LoadData(OnTraitDataLoaded);
        }
        if (GUILayout.Button("🍳 Fishrod Adressable 업로드"))
        {
            AddScriptableObjectsToAddressables("Assets/10_SO/bbq/FishingRods", "Fishrods");
        }
    }

    public static void AddScriptableObjectsToAddressables(string targetFolderPath, string groupName = "Default Local Group")
    {
        // AddressableAssetSettings 가져오기
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("Addressable Settings not found. Please initialize Addressables first.");
            return;
        }

        // 그룹 찾기 또는 생성
        var group = settings.FindGroup(groupName);
        if (group == null)
        {
            group = settings.CreateGroup(groupName, false, false, true, null);
            Debug.Log($"Created new Addressables group: {groupName}");
        }

        // 경로 정규화 및 존재 확인
        string fullPath = targetFolderPath.Replace("\\", "/");
        if (!Directory.Exists(fullPath))
        {
            Debug.LogError($"Directory not found: {fullPath}");
            return;
        }

        string[] soFiles = Directory.GetFiles(fullPath, "*.asset", SearchOption.AllDirectories);
        int addedCount = 0;
        int updatedCount = 0;
        int skippedCount = 0;

        // 기존 항목 정보 수집
        var existingEntries = new Dictionary<string, AddressableAssetEntry>();
        foreach (var entry in group.entries)
        {
            existingEntries[entry.guid] = entry;
        }

        foreach (string file in soFiles)
        {
            string path = file.Replace("\\", "/");
            string guid = AssetDatabase.AssetPathToGUID(path);
            string expectedAddress = $"{typeof(FishingRod).Name}/{Path.GetFileNameWithoutExtension(path)}";

            // FishingRod SO 로드
            FishingRod so = AssetDatabase.LoadAssetAtPath<FishingRod>(path);
            if (so == null) continue;

            // 이미 등록된 경우
            if (existingEntries.TryGetValue(guid, out var existingEntry))
            {
                // 주소가 다른 경우 업데이트
                if (existingEntry.address != expectedAddress)
                {
                    existingEntry.address = expectedAddress;
                    updatedCount++;
                    Debug.Log($"Updated address for: {path} -> {expectedAddress}");
                }
                else
                {
                    skippedCount++;
                }
                continue;
            }

            // 새 항목 추가
            var newEntry = settings.CreateOrMoveEntry(guid, group);
            newEntry.address = expectedAddress;
            addedCount++;
            Debug.Log($"Added new entry: {path}");
        }

        // 변경사항 저장
        AssetDatabase.SaveAssets();
        Debug.Log($"Addressables update complete - Added: {addedCount}, Updated: {updatedCount}, Skipped: {skippedCount}");
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
}