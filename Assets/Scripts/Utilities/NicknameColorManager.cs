using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class NicknameColorManager {
    /// <summary>
    /// Hi Foxyyy basically how this works is that we get the colored names from the json URL so you can manually and remotely update them
    /// The JSON should look something like this
    /// {
    ///    "8ae3fa57-014a-4e27-8331-5431aa5dea9d": "#FF0000",
    ///    "GRADIENTEXAMPLE": "1,0,0, 0,1,0, 0,0,1, 1,1,0
    /// }
    /// We can make a static color that doesn't move with a simple hex color
    /// for gradients we use 12 comma-separated floats
    /// 
    /// </summary>
    private const string URL = "https://raw.githubusercontent.com/ArianLust/NSMB-MarioVsLuigi/refs/heads/FBOC-PR/colored_names.json";

    private static Dictionary<string, string> _colorMap;

    public static async Task Initialize() {
        using UnityWebRequest request = UnityWebRequest.Get(URL);
        var operation = request.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        if (request.result == UnityWebRequest.Result.Success) {
            string json = request.downloadHandler.text;
            _colorMap = JsonUtilityWrapper.FromJson<SerializableDictionary>(json).ToDictionary();
        } else {
            Debug.LogWarning("Failed to fetch nickname colors: " + request.error);
        }
    }

    public static bool TryGetColor(string userId, out string color) {
        color = null;
        return _colorMap != null && _colorMap.TryGetValue(userId, out color);
    }

    
    [System.Serializable]
    private class SerializableDictionary {
        public Entry[] entries;

        [System.Serializable]
        public struct Entry {
            public string key;
            public string value;
        }

        public Dictionary<string, string> ToDictionary() {
            var dict = new Dictionary<string, string>();
            foreach (var e in entries) {
                dict[e.key] = e.value;
            }
            return dict;
        }
    }

    private static class JsonUtilityWrapper {
        public static SerializableDictionary FromJson<T>(string json) where T : class {
            string wrappedJson = "{\"entries\":" + json + "}";
            return JsonUtility.FromJson<SerializableDictionary>(wrappedJson);
        }
    }
}
