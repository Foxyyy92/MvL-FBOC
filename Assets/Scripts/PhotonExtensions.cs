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
        ["3b02afb97260d991405e79649235c2e4297d8455967488c73ebce4142ccb40af"] = "Windows10V",
        ["d3a0482dd323d35f44f82efed7a714af8ddacf00182de45c631b0a00a65a1f70"] = "KingKittyTurnip",
        ["fb52464942771a8b7365a843fba0bb888886c1b7e6dfeff40eb170d64ef62592"] = "Lust",
        ["53f3fba59ea20b1c90d4adcb8108becfb278e60fd5ff4a340e8d95bdcb294bbf"] = "IvythePoS",
        ["b67cac6f3df9446a4dda9c8dc24a3354b5b0e00e53ff864af273c8eb434f0180"] = "Lee",
        ["9583e702d5fc0d02acedb6e86a998ba24838ab66ac0300ad95717c8e231949f3"] = "Mangleee",
        ["142bd469e05423e21095e3d685d12b7e7283e33caa8d89d74b2a1b8b422aaa5c"] = "WaluigiTheLagger",
        ["e9ce4f3c780497870ff9b61b33dd821c23fe61838979451248b989e1f2d7ce59"] = "NotFoxyyy",
        ["9e6918e67a93669942dd6cb45eab2bd8b8cefdaed64d46920073908a024c02a8"] = "notossdekasaihassei",
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