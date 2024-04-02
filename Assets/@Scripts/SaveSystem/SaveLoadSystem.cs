using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class GameData
{
    public string Name;
    public GameCurrency.CurrencyData currencyData;
    public List<SchoolData.SchoolDataSave> schoolData = new List<SchoolData.SchoolDataSave>();

    public void Reset()
    {
        currencyData.Reset();
        schoolData.ForEach(s => s.Reset());
    }
}

public interface ISaveable
{
    SerializableGuid Id { get; set; }

    void Reset();
}

public interface IBind<TData> where TData : ISaveable
{
    SerializableGuid Id { get; set; }
    void Bind(TData data);
}

public class SaveLoadSystem : SingletonPersistent<SaveLoadSystem>
{
    [SerializeField] public GameData gameData;

    IDataService _dataService;

    protected override void Awake()
    {
        base.Awake();
        _dataService = new FileDataService(new JsonSerializer());
    }

    private void Start()
    {
        NewOrLoadGame("PaulinhoBacana");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K)) NewOrLoadGame("PaulinhoBacana");
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += EditorPlayModeChanged; ;
#else
        Application.quitting += SaveGame;
#endif

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            SaveGame();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveGame();
        }
    }

#if UNITY_EDITOR
    private void EditorPlayModeChanged(PlayModeStateChange mode)
    {
        if(mode == PlayModeStateChange.ExitingPlayMode)
        {
            SaveGame();
        }
    }
#endif

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Bind<GameCurrency, GameCurrency.CurrencyData>(gameData.currencyData);
        Bind<SchoolData, SchoolData.SchoolDataSave>(gameData.schoolData);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
    {
        var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
        if (entity != null)
        {
            if (data == null)
            {
                data = new TData { Id = entity.Id };
            }
            entity.Bind(data);
        }
    }

    void Bind<T, TData>(List<TData> datas) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
    {
        var entities = FindObjectsByType<T>(FindObjectsSortMode.None);
        
        foreach(var entity in entities)
        {
            var data = datas.FirstOrDefault(d => d.Id == entity.Id);
            if (data == null)
            {
                data = new TData { Id = entity.Id };
                datas.Add(data);
            }
            entity.Bind(data);
        }
    }


    public void NewGame(string newGameName)
    {
        gameData = new GameData
        {
            Name = newGameName
        };

        SceneManager.LoadScene("GameScene");
    }

    public void SaveGame()
    {
        _dataService.Save(gameData);
    }

    public void LoadGame(string gameName)
    {
        gameData = _dataService.Load(gameName);

        SceneManager.LoadScene("GameScene");
    }

    public void NewOrLoadGame(string gameName)
    {
        if(_dataService.Contains(gameName))
        {
            LoadGame(gameName);
        }
        else
        {
            NewGame(gameName);
        }
    }

    public void DeleteGame(string gameName)
    {
        _dataService.Delete(gameName);
    }

    public void ResetGame()
    {
        gameData.Reset();
    }
}

