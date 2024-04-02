using UnityEngine;

public class JsonSerializer : ISerializer
{
    public string Serialize<T>(T data)
    {
        return JsonUtility.ToJson(data, true);
    }

    public T Deserialize<T>(string json)
    {
        return JsonUtility.FromJson<T>(json);
    }
}