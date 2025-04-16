using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Linq;


public static class FishSOGenerator
{
    private const string SAVE_PATH = "Assets/10_SO/bbq/FishDataGen";
     private const string SAVE_RARITY_PATH = "Assets/10_SO/bbq/RarityTable.asset";
    private const string SAVE_IMG_PATH = "Assets/10_SO/bbq/FishSprites";

    public static IEnumerator Generate(FishStruct[] table)
    {
        if (!Directory.Exists(SAVE_PATH)) Directory.CreateDirectory(SAVE_PATH);
        if (!Directory.Exists(SAVE_IMG_PATH)) Directory.CreateDirectory(SAVE_IMG_PATH);

        foreach (var item in table)
        {
            string filePath = Path.Combine(SAVE_PATH, $"{item.spec}_{item.id}.asset");
            string imagePath = Path.Combine(SAVE_IMG_PATH, $"{item.id}_Sprite.png");
            
            FishData asset = AssetDatabase.LoadAssetAtPath<FishData>(filePath);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<FishData>();
                 AssetDatabase.CreateAsset(asset, filePath);
            }

            Sprite sprite = null;

            if (item.imgUri != null && item.imgUri != string.Empty)
            {
                UnityWebRequest www = UnityWebRequestTexture.GetTexture(item.imgUri);
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    // 2. Texture 저장
                    Texture2D tex = DownloadHandlerTexture.GetContent(www);
                    byte[] png = tex.EncodeToPNG();
                    File.WriteAllBytes(imagePath, png);
                    AssetDatabase.ImportAsset(imagePath);
                    AssetDatabase.Refresh();

                    yield return null; // wait for import


                    // 3. Sprite로 로드
                    Texture2D loadedTex = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath);
                    loadedTex.filterMode = FilterMode.Point;

                    sprite = AssetDatabase.LoadAssetAtPath<Sprite>(imagePath);
                    if (sprite == null && loadedTex != null)
                    {
                        TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(imagePath);
                        ti.textureType = TextureImporterType.Sprite;
                        ti.spriteImportMode = SpriteImportMode.Single;
                        ti.SaveAndReimport();
                        AssetDatabase.Refresh();
                        yield return null;
                        sprite = AssetDatabase.LoadAssetAtPath<Sprite>(imagePath);
                    }
                }
                else 
                {
                    Debug.LogWarning($"Image download failed: {www.result} - {www.error}");
                }
            }


            asset.id = item.id;
            asset.name = $"{item.spec}_{item.id}";
            asset.spec = item.koreanName;
            asset.basePrice = item.basePrice;
            asset.baseWeight = item.baseWeight;
            asset.MinWeightMultiplier = item.minWeightMultiplier;
            asset.MaxWeightMultiplier = item.maxWeightMultiplier;
            asset.DancingStepMin = item.dancingStepMin;
            asset.DancingStepMax = item.dancingStepMax;
            asset.description = item.description;
            asset.rarity = item.rarity;

            if (sprite != null)
            {
                asset.fishSprite = sprite;
            }

            
            EditorUtility.SetDirty(asset);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void ProcessRarityTable(RarityWeight[] weights)
    {
        FishData[] allFishes = AssetDatabase.FindAssets("t:FishData")
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .Select(path => AssetDatabase.LoadAssetAtPath<FishData>(path))
            .ToArray();

        List<RarityData> rarityList = new List<RarityData>();
        foreach (var w in weights)
        {
            var matchingFishes = allFishes
                .Where(f => f != null && f.rarity == w.rarity)
                .ToArray();

            RarityData data = new RarityData
            {
                name = w.rarity,
                weight = w.weight,
                fishes = matchingFishes
            };

            rarityList.Add(data);
        }

        // Gen SO
        RarityTable table = ScriptableObject.CreateInstance<RarityTable>();
        table.rarities = rarityList.ToArray();

        AssetDatabase.CreateAsset(table, SAVE_RARITY_PATH);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅ RarityTable.asset 생성 완료!");
    }
}
