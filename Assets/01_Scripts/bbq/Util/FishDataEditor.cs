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
        GUILayout.Label("Google Sheet에서 캐릭터 데이터 불러오기", EditorStyles.boldLabel);

        if (GUILayout.Button("불러오기"))
        {
            FishDataLoader.LoadData(OnDataLoaded);
        }
    }

    private void OnDataLoaded(string data)
    {
         string wrapped = "{\"items\":" + data + "}";
        FishStructTable parsed = JsonUtility.FromJson<FishStructTable>(wrapped);
       
        
        // Debug.Log($"{data}");
        
        // foreach (var character in data)
        // {
        //     // Debug.Log($"{character.id} - {character.spec} / {character.basePrice}₩");
        // }
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

