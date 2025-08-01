using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NSMB.Utilities;
using Quantum;
using System.Security.Cryptography;
using System.Text;

public static class ColoredNameManager
{
    /// <summary>
    /// Hi Foxyyy basically how this works is that we get the colored names from the json URL so you can manually and remotely update them
    /// The JSON should look something like this
    /// {
    ///    "REDSTATICEXAMPLE": "#FF0000",
    ///    "GRADIENTEXAMPLE": "1,0,0, 0,1,0, 0,0,1, 1,1,0
    /// }
    /// We can make a static color that doesn't move with a simple #hex color
    /// for gradients we use 12 comma-separated floats
    /// The colors above are red on "REDSTATICEXAMPLE" and a rainbow gradient on "GRADIENTEXAMPLE"
    /// </summary>
    private const string JsonUrl =
        "https://raw.githubusercontent.com/Foxyyy92/MvL-FBOC/you-know-what-that-means/colored_names.json";

    private static Dictionary<string, NicknameColor> _map;
    private static Task _initTask;

    public static Task InitializeAsync() {
        return _initTask ??= LoadAndParseJsonAsync();
    }

    public static bool TryGetColor(RuntimePlayer player, out NicknameColor color) {
        color = default;

        if (player == null || !player.UseColoredNickname || string.IsNullOrEmpty(player.UserId) || _map == null)
            return false;

        string hashedId = ComputeSha256Hash(player.UserId);
        return _map.TryGetValue(hashedId, out color);
    }

    private static async Task LoadAndParseJsonAsync() {
        try {
            using var http = new HttpClient();
            string json = await http.GetStringAsync(JsonUrl).ConfigureAwait(false);

            var raw = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            _map = new Dictionary<string, NicknameColor>(raw.Count);
            foreach (var kv in raw) {
                var nc = NicknameColor.Parse(kv.Value.AsSpan());
                _map[kv.Key] = nc;
            }
        } catch (Exception e) {
            UnityEngine.Debug.LogWarning($"Failed to load colored_names.json: {e}");
        }
    }

    private static string ComputeSha256Hash(string input) {
        using var sha256 = SHA256.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        var builder = new StringBuilder(bytes.Length * 2);
        foreach (byte b in bytes)
            builder.Append(b.ToString("x2"));
        return builder.ToString();
    }
}
