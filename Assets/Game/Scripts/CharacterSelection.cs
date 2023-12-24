using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.IO;

public class CharacterSelection : MonoBehaviour
{
    public GameObject[] characters1;
    public GameObject[] characters2;
    public GameObject[] map;
    public AudioClip[] music;
    public GameObject characterSelection;
    public GameObject mapSelection;
    public int selectedCharacter1 = 0;
    public int selectedCharacter2 = 0;
    public int selectedMap = 0;
    public int selectedMusic = 0;
    public TMP_Text characterNameTextP1;
    public TMP_Text characterNameTextP2;
    public TMP_Text mapName;
    public TMP_Text musicName;
    public TMP_Text skinName;
    public TMP_Text skinName2;
    public Sprite[] characterSprites1;
    public Sprite[] characterSprites2;
    public Image characterPanel1;
    public Image characterPanel2;
    public Sprite[] mapSprites;
    public Image mapPanel;
    public GameObject videoLoad;
    public GameObject chacracters;
    public AudioManager audioManager;
    public Image[] characterThumbnails1;
    public Image[] characterThumbnails2;
    public Image[] characterBorders;
    private bool isLoadingComplete = false;

    private int currentEnemySkinIndex = 0;
    private int currentPlayerSkinIndex = 0;
    public SkinnedMeshRenderer skinnedMeshRendererGokuPlayer;
    public SkinnedMeshRenderer skinnedMeshRendererGohanPlayer;
    public SkinnedMeshRenderer skinnedMeshRendererGokuEnemy;
    public SkinnedMeshRenderer skinnedMeshRendererGohanEnemy;

    public Texture2D[] skin;
    public GameObject characters;
    public GameObject charactersSelection;
    public GameObject menu;
    private const string skinLinkPath = "Assets/Game/Models/Player/";
    //private const string skinLinkPathEnemy = "Assets/Game/Models/Enemy/";
    private string[] nameSkin = { "/tex3normal.png", "/tex3white.png", "/tex3pink.png", "/tex3yellow.png", "/tex3red.png" };
    private void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        selectedCharacter1 = PlayerPrefs.GetInt("selectedCharacter1");
        selectedCharacter2 = PlayerPrefs.GetInt("selectedCharacter2");
        selectedMap = PlayerPrefs.GetInt("selectedMap");
        selectedMusic = PlayerPrefs.GetInt("selectedMusic");
        characters2[selectedCharacter2].SetActive(true);
        characters1[selectedCharacter1].SetActive(true);
        characterNameTextP1.text = characters1[selectedCharacter1].name;
        characterNameTextP2.text = characters2[selectedCharacter2].name;
        mapName.text = map[selectedMap].name;
        musicName.text = music[selectedMusic].name;
        skinName.text = $"Skin {currentPlayerSkinIndex}";
        skinName2.text = $"Skin {currentEnemySkinIndex}";
        characterPanel1.sprite = characterSprites1[selectedCharacter1];
        characterPanel2.sprite = characterSprites2[selectedCharacter2];
        mapPanel.sprite = mapSprites[selectedMap];
        audioManager.PlayMusic(musicName.text);
        characterBorders[selectedCharacter1].gameObject.SetActive(true);
        characterBorders[selectedCharacter2].gameObject.SetActive(true);
        UpdateEnemyTextures();
        UpdatePlayerTextures();
    }

    public void NextEnemySkin()
    {
        currentEnemySkinIndex++;
        if (currentEnemySkinIndex > 4)
            currentEnemySkinIndex = 0;
        skinName2.text = $"Skin {currentEnemySkinIndex}";
        UpdateEnemyTextures();
    }

    public void PreviousEnemySkin()
    {
        currentEnemySkinIndex--;
        if (currentEnemySkinIndex < 0)
            currentEnemySkinIndex = 4;
        skinName2.text = $"Skin {currentEnemySkinIndex}";
        UpdateEnemyTextures();
    }

    private void UpdateEnemyTextures()
    {
        Material[] materials = skinnedMeshRendererGokuEnemy.materials;
        string filePath = $"{skinLinkPath}{characters2[selectedCharacter2].name}/textures{nameSkin[currentEnemySkinIndex]}";
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);
        if (characters2[selectedCharacter2].name == "Goku")
        {
            materials = skinnedMeshRendererGokuEnemy.materials;
        }
        else if (characters2[selectedCharacter2].name == "Gohan")
        {
            materials = skinnedMeshRendererGohanEnemy.materials;
        }
        foreach (Material material in materials)
        {
            if (material.name == "tex3.png (Instance)" && characters2[selectedCharacter2].name == "Goku")
            {
                PlayerPrefs.SetString("SkinEnemy", filePath);
                material.SetTexture("_BaseMap", texture);
                break;
            }
            else if (material.name == "Gohan - Body (Instance)" && characters2[selectedCharacter2].name == "Gohan")
            {
                PlayerPrefs.SetString("SkinEnemy", filePath);
                material.SetTexture("_BaseMap", texture);
                break;
            }
        }
    }
    public void NextPlayerSkin()
    {
        currentPlayerSkinIndex++;
        if (currentPlayerSkinIndex > 4)
            currentPlayerSkinIndex = 0;
        skinName.text = $"Skin {currentPlayerSkinIndex}";
        UpdatePlayerTextures();
    }

    public void PreviousPlayerSkin()
    {
        currentPlayerSkinIndex--;
        if (currentPlayerSkinIndex < 0)
            currentPlayerSkinIndex = 4;
        skinName.text = $"Skin {currentPlayerSkinIndex}";
        UpdatePlayerTextures();
    }

    private void UpdatePlayerTextures()
    {
        Material[] materials = skinnedMeshRendererGokuPlayer.materials;
        string filePath = $"{skinLinkPath}{characters1[selectedCharacter1].name}/textures{nameSkin[currentPlayerSkinIndex]}";
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);
        if (characters1[selectedCharacter1].name == "Goku")
        {
            materials = skinnedMeshRendererGokuPlayer.materials;
        }
        else if (characters1[selectedCharacter1].name == "Gohan")
        {
            Debug.Log("Gohan");
            materials = skinnedMeshRendererGohanPlayer.materials;
        }

        foreach (Material material in materials)
        {
            if (material.name == "tex3.png (Instance)" && characters1[selectedCharacter1].name == "Goku")
            {
                PlayerPrefs.SetString("SkinPlayer", filePath);
                material.SetTexture("_BaseMap", texture);
                break;
            }
            else if (material.name == "Gohan - Body (Instance)" && characters1[selectedCharacter1].name == "Gohan")
            {
                PlayerPrefs.SetString("SkinPlayer", filePath);
                material.SetTexture("_BaseMap", texture);
                break;
            }
        }
    }
    public void NextCharacter1()
    {
        currentPlayerSkinIndex = 0;
        characters1[selectedCharacter1].SetActive(false);
        selectedCharacter1 = (selectedCharacter1 + 1) % characters1.Length;
        characters1[selectedCharacter1].SetActive(true);
        characterNameTextP1.text = characters1[selectedCharacter1].name;
        characterPanel1.sprite = characterSprites1[selectedCharacter1];
        UpdateCharacterBorders();
    }
    public void PreviousCharacter1()
    {
        characters1[selectedCharacter1].SetActive(false);
        selectedCharacter1--;
        if (selectedCharacter1 < 0)
        {
            selectedCharacter1 += characters1.Length;
        }
        characters1[selectedCharacter1].SetActive(true);
        characterNameTextP1.text = characters1[selectedCharacter1].name;
        characterPanel1.sprite = characterSprites1[selectedCharacter1];
        UpdateCharacterBorders();
    }

    public void NextCharacter2()
    {
        currentEnemySkinIndex = 0;
        characters2[selectedCharacter2].SetActive(false);
        selectedCharacter2 = (selectedCharacter2 + 1) % characters2.Length;
        characters2[selectedCharacter2].SetActive(true);
        characterNameTextP2.text = characters2[selectedCharacter2].name;
        characterPanel2.sprite = characterSprites2[selectedCharacter2];
        UpdateCharacterBorders();
    }

    public void PreviousCharacter2()
    {
        characters2[selectedCharacter2].SetActive(false);
        selectedCharacter2--;
        if (selectedCharacter2 < 0)
        {
            selectedCharacter2 += characters2.Length;
        }
        characters2[selectedCharacter2].SetActive(true);
        characterNameTextP2.text = characters2[selectedCharacter2].name;
        characterPanel2.sprite = characterSprites2[selectedCharacter2];
        UpdateCharacterBorders();
    }
    public void UpdateCharacterThumbnails1()
    {
        for (int i = 0; i < characterThumbnails1.Length; i++)
        {
            characterThumbnails1[i].sprite = characterSprites1[i];
        }
    }

    public void UpdateCharacterThumbnails2()
    {
        for (int i = 0; i < characterThumbnails2.Length; i++)
        {
            characterThumbnails2[i].sprite = characterSprites2[i];
        }
    }

    public void UpdateCharacterBorders()
    {
        for (int i = 0; i < characterBorders.Length; i++)
        {
            characterBorders[i].gameObject.SetActive(i == selectedCharacter1 || i == selectedCharacter2);
        }
    }
    public void NextMap()
    {
        map[selectedMap].SetActive(false);
        selectedMap = (selectedMap + 1) % map.Length;
        map[selectedMap].SetActive(true);
        mapName.text = map[selectedMap].name;
        mapPanel.sprite = mapSprites[selectedMap];
    }

    public void PreviousMap()
    {
        map[selectedMap].SetActive(false);
        selectedMap--;
        if (selectedMap < 0)
        {
            selectedMap += map.Length;
        }
        map[selectedMap].SetActive(true);
        mapName.text = map[selectedMap].name;
        mapPanel.sprite = mapSprites[selectedMap];
    }

    public void NextMusic()
    {
        selectedMusic = (selectedMusic + 1) % music.Length;
        musicName.text = music[selectedMusic].name;
        audioManager.PlayMusic(musicName.text);
    }

    public void PreviousMusic()
    {
        selectedMusic--;
        if (selectedMusic < 0)
        {
            selectedMusic += music.Length;
        }
        musicName.text = music[selectedMusic].name;
        audioManager.PlayMusic(musicName.text);
    }

    public void StartGame(int id)
    {
        PlayerPrefs.SetInt("selectedCharacter1", selectedCharacter1);
        PlayerPrefs.SetInt("selectedCharacter2", selectedCharacter2);
        PlayerPrefs.SetInt("selectedMap", selectedMap);
        PlayerPrefs.SetInt("selectedMusic", selectedMusic);
        StartCoroutine(LoadScene(id, 5.0f));
    }
    public void SelectionMap()
    {
        chacracters.SetActive(false);
        characterSelection.SetActive(false);
        mapSelection.SetActive(true);
    }

    IEnumerator LoadScene(int id, float delay)
    {
        mapSelection.SetActive(false);
        videoLoad.gameObject.SetActive(true);
        yield return new WaitForSeconds(delay);

        AsyncOperation operation = SceneManager.LoadSceneAsync(id);
        while (!operation.isDone)
        {
            yield return null;
        }
    }
    public void Back()
    {
        mapSelection.SetActive(false);
        characters.SetActive(false);
        charactersSelection.SetActive(false);
        menu.SetActive(true);
    }
}
