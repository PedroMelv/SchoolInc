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
    public HomeworkHandler.HomeworkData homeworkData;
    public TimeHandler.TimeData timeData;
    public GameCurrency.CurrencyData currencyData;
    public AscendedHandler.AscendedUpgradeData ascendedData;
    public List<SchoolData.SchoolDataSave> schoolData = new List<SchoolData.SchoolDataSave>();
    public SuperPowersData superPowersData;

    public void ResetAscended()
    {
        homeworkData.Reset_Ascended();
        currencyData.Reset_Ascended();
        ascendedData.Reset_Ascended();
        schoolData.ForEach(s => s.Reset_Ascended());
    }

    public void Reset()
    {
        homeworkData.Reset();
        currencyData.Reset();
        ascendedData.Reset();
        schoolData.ForEach(s => s.Reset());
    }
}

public interface ISaveable
{
    SerializableGuid Id { get; set; }

    void Reset_Ascended();
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



    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K)) NewOrLoadGame("Retro");
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += EditorPlayModeChanged; ;
#else
        Application.quitting += () =>SaveGame(false);
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
        BindData();
    }

    private void BindData()
    {
        Bind<GameCurrency, GameCurrency.CurrencyData>(gameData.currencyData);
        Bind<SchoolData, SchoolData.SchoolDataSave>(gameData.schoolData);
        Bind<AscendedHandler, AscendedHandler.AscendedUpgradeData>(gameData.ascendedData);
        Bind<HomeworkHandler, HomeworkHandler.HomeworkData>(gameData.homeworkData);
        Bind<TimeHandler, TimeHandler.TimeData>(gameData.timeData);
        Bind<SuperPowers, SuperPowersData>(gameData.superPowersData);
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
            Name = newGameName,
            timeData = new TimeHandler.TimeData
            {
                savedTime = new TimeHandler.TimeInfo(DateTime.Now)
            }
        };

        SceneManager.LoadScene("GameScene");
    }

    public void SaveGame(bool bind = true)
    {
        gameData = _dataService.Save(gameData);

        if(bind) BindData();
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

        InvokeRepeating(nameof(SaveGame), 60, 60);
    }

    public void DeleteGame()
    {
        _dataService.Delete(gameData.Name);
    }
    public void DeleteGame(string gameName)
    {
        _dataService.Delete(gameName);
    }

    public void ResetGame()
    {
        gameData.Reset();
    }

    public void ResetAscendGame()
    {
        gameData.ResetAscended();

        SaveGame(false);
    }
}

