using System.Collections.Generic;
using UnityEngine;

public static class JsonUtilityWrapper
{
    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> Items;
    }

    public static List<T> FromJson<T>(string json)
    {
        string wrappedJson = "{ \"Items\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrappedJson);
        return wrapper.Items;
    }
}
