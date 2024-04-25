using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class FileDataService : IDataService
{
    private ISerializer _serializer;
    private string dataPath;
    private string fileExtension;

    public FileDataService(ISerializer serializer)
    {
        dataPath = Application.persistentDataPath;
        fileExtension = "json";
        _serializer = serializer;
    }

    private string GetPathToFile(string fileName)
    {
        return Path.Combine(dataPath, string.Concat(fileName, ".", fileExtension));
    }

    public GameData Save(GameData data, bool overwrite = true)
    {
#if UNITY_ANDROID || UNITY_IOS

        PlayerPrefs.SetString("SAVEGAME_" + data.Name, _serializer.Serialize(data));

        return Load(data.Name);
        #endif

        string fileLocation = GetPathToFile(data.Name);
        if (File.Exists(fileLocation) && !overwrite)
        {
            throw new InvalidDataException("File: " + data.Name + "." + fileExtension + " already exists and cannot be overwritten.");
        }

        File.WriteAllText(fileLocation, _serializer.Serialize(data));

        return Load(data.Name);
    }

    public GameData Load(string name)
    {
#if UNITY_ANDROID || UNITY_IOS

        return _serializer.Deserialize<GameData>(PlayerPrefs.GetString("SAVEGAME_" + name));

        #endif

        string fileLocation = GetPathToFile(name);

        if (!File.Exists(fileLocation))
        {
            throw new ArgumentException("File: " + name + "." + fileExtension + " not found.");
        }

        return _serializer.Deserialize<GameData>(File.ReadAllText(fileLocation));
    }

    public void Delete(string name)
    {
#if UNITY_ANDROID || UNITY_IOS

        PlayerPrefs.DeleteKey("SAVEGAME_" + name);

        return;

#endif
        string fileLocation = GetPathToFile(name);

        if(File.Exists(fileLocation))
        {
            File.Delete(fileLocation);
        }
    }

    public void DeleteAll()
    {
        foreach(string filePath in Directory.GetFiles(dataPath))
        {
            File.Delete(filePath);
        }
    }

    public IEnumerable<string> ListSaves()
    {
        foreach(string path in Directory.EnumerateFiles(dataPath))
        {
            yield return Path.GetFileNameWithoutExtension(path);
        }
    }

    public bool Contains(string name)
    {
#if UNITY_ANDROID || UNITY_IOS
        return PlayerPrefs.HasKey("SAVEGAME_" + name);
#endif
        return File.Exists(GetPathToFile(name));
    }
}
