using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using Photon.Pun;
using Photon.Realtime;

public static class PhotonExtensions {

    private static readonly Dictionary<string, string> SPECIAL_PLAYERS = new() {
        ["719bef8979a8452bb83d778ffd1f6b8062e8e4193700156e4afb301e57ff9f87"] = "Foxyyy",
        ["54100a304da2609613d2ad8a602a6c1413ea5e010458ece966c14dff6f8eaa64"] = "FrostyCake",
        ["1e18a301bec9e219262b11b442350a9fafcde83a1ccc82fa54fe0eaccf1d33db"] = "BluCor",
        ["ea2f23d83904ba5f6646f58b1b926f57166e8898711cea4400c2e7777dfeee7d"] = "Zogistra",
        ["f0e6d35759562db51f00e1e0d89ccf524f364069f1863c10e336f8dbd054b43b"] = "andriuf",
        ["f93b1e51e82b99a93851e03254444f10c09da651ed9e68ac656638556a9b443c"] = "zomblebobble",
        ["6cae1c7e59ef7ec0f5b1566b2e63a5bb42263f48951f647fb1eaf5fc6419457f"] = "Windows10V",
        ["d3a0482dd323d35f44f82efed7a714af8ddacf00182de45c631b0a00a65a1f70"] = "KingKittyTurnip",
        ["e68fc02565c776e9283cd80ec1c48c2fe389b03e2e9dc856c3f29460ff2093ca"] = "Lust",
    };

    public static bool IsMineOrLocal(this PhotonView view) {
        return !view || view.IsMine;
    }

    public static bool HasRainbowName(this Player player) {
        if (player == null || player.UserId == null)
            return false;

        byte[] bytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(player.UserId));
        StringBuilder sb = new();
        foreach (byte b in bytes)
            sb.Append(b.ToString("X2"));

        string hash = sb.ToString().ToLower();
        return SPECIAL_PLAYERS.ContainsKey(hash) && player.NickName == SPECIAL_PLAYERS[hash];
    }

    //public static void RPCFunc(this PhotonView view, Delegate action, RpcTarget target, params object[] parameters) {
    //    view.RPC(nameof(action), target, parameters);
    //}
}