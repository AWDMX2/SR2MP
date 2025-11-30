using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2MP
{
    [BepInPlugin("Egor_ICE", "SR2MP", "0.1.7")]
    public class Loader : BasePlugin
    {
        public override void Load()
        {
            if (!File.Exists(@"BepInEx\plugins\steam_api64.dll"))
            {
                File.Copy(@"SlimeRancher2_Data\Plugins\x86_64\steam_api64.dll", @"BepInEx\plugins\steam_api64.dll");
            }

            new Harmony("SR2MP").PatchAll();

            AddComponent<Core>();
            AddComponent<SteamLobby>();
            AddComponent<UI>();

            ClassInjector.RegisterTypeInIl2Cpp<NetworkPlayer>();
            ClassInjector.RegisterTypeInIl2Cpp<ReadData>();
            ClassInjector.RegisterTypeInIl2Cpp<NetworkActor>();
        }
    }
}
