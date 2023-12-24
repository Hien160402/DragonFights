using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class LoadCharacter : MonoBehaviour
{
    public GameObject[] characterPrefabs;
    public GameObject[] mapPrefabs;
    public Transform P1;
    public Transform P2;
    public Material skyboxMaterial;
    private const string skinIndexKey = "CurrentEnemySkinIndex";
    void Start()
    {
        //LoadCharacter1
        int selectedCharacter1 = PlayerPrefs.GetInt("selectedCharacter1");
        GameObject clone1 = Instantiate(characterPrefabs[selectedCharacter1], P1.transform.position, Quaternion.identity);
        clone1.name = "Player";
        
        if (characterPrefabs[selectedCharacter1].name == "goku" || characterPrefabs[selectedCharacter1].name == "gohan")
        {
            UpdateSkinPlayer(characterPrefabs[selectedCharacter1].name);
        }
        clone1.layer = LayerMask.NameToLayer("Player");

        //LoadCharacter2
        int selectedCharacter2 = PlayerPrefs.GetInt("selectedCharacter2");
        GameObject clone2 = Instantiate(characterPrefabs[selectedCharacter2], P2.transform.position, Quaternion.identity);
        clone2.name = "Enemy";
        if (characterPrefabs[selectedCharacter2].name == "goku" || characterPrefabs[selectedCharacter2].name == "gohan")
        {
            UpdateSkinEnemy(characterPrefabs[selectedCharacter2].name);
        }
        clone2.layer = LayerMask.NameToLayer("Enemy");
        Instantiate(mapPrefabs[PlayerPrefs.GetInt("selectedMap")]);
        RenderSettings.skybox = skyboxMaterial;
    }

    private void UpdateSkinEnemy(string name)
    {
        SkinnedMeshRenderer body;
        GameObject enemy = GameObject.Find("Enemy");
        if (name == "goku")
        {
            body = enemy.transform.Find("Body").GetComponent<SkinnedMeshRenderer>();
        }
        else
        {
            body = enemy.transform.Find("Gohan").GetComponent<SkinnedMeshRenderer>();
        }
        Material[] materials = body.materials;
        Texture2D texture = new Texture2D(2, 2);
        byte[] fileData = File.ReadAllBytes(PlayerPrefs.GetString("SkinEnemy"));
        texture.LoadImage(fileData);
        foreach (Material material in materials)
        {
            if (material.name == "tex3.png (Instance)" || material.name == "Gohan - Body (Instance)")
            {
                Debug.Log("?");
                material.SetTexture("_BaseMap", texture);
                break;
            }
        }
    }
    private void UpdateSkinPlayer(string name)
    {
        SkinnedMeshRenderer body;
        GameObject player = GameObject.Find("Player");
        if (name == "goku")
        {
            body = player.transform.Find("Body").GetComponent<SkinnedMeshRenderer>();
        }
        else
        {
            body = player.transform.Find("Gohan").GetComponent<SkinnedMeshRenderer>();
        }
        Material[] materials = body.materials;
        Texture2D texture = new Texture2D(2, 2);
        byte[] fileData = File.ReadAllBytes(PlayerPrefs.GetString("SkinPlayer"));
        texture.LoadImage(fileData);
        foreach (Material material in materials)
        {
            if (material.name == "tex3.png (Instance)" || material.name == "Gohan - Body (Instance)")
            {
                material.SetTexture("_BaseMap", texture);
                break;
            }
        }
    }
}
