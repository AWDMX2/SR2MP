using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Economy;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SR2MP
{
    [HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.SaveGame))]
    class AutoSaveDirector_SaveGame
    {
        public static bool Prefix()
        {
            if (!Core.IsMultiplayer)
                return true;

            return SteamLobby.Host;
        }
    }

    [HarmonyPatch(typeof(InstantiationHelpers), nameof(InstantiationHelpers.InstantiateActor))]
    class InstantiationHelpers_InstantiateActor
    {
        public static void Postfix(GameObject __result, GameObject original, SceneGroup sceneGroup, Vector3 position, Quaternion rotation)
        {
            if (!Core.IsMultiplayer)
                return;

            if (!Networking.HandlePacket)
            {
                if (Core.SyncActors)
                {
                    var networkActor = __result.AddComponent<NetworkActor>();
                    networkActor.Id = Core.SpawnID;
                    networkActor.IsLocal = true;
                    Core.Actors.Add(networkActor.Id, networkActor);

                    SendData.SendSpawn(networkActor.Id, original.name, sceneGroup.name, position, rotation);
                }
            }
        }
    }

    [HarmonyPatch(typeof(LandPlot), nameof(LandPlot.AddUpgrade))]
    class LandPlot_AddUpgrade
    {
        public static void Postfix(LandPlot __instance, LandPlot.Upgrade upgrade)
        {
            if (!Core.IsMultiplayer)
                return;

            if (!Networking.HandlePacket)
            {
                var name = __instance.GetComponentInParent<LandPlotLocation>().name;
                SendData.SendLandPlotUpgrade(name, (int)upgrade);
            }
        }
    }

    [HarmonyPatch(typeof(LandPlotLocation), nameof(LandPlotLocation.Replace))]
    class LandPlotLocation_Replace
    {
        public static void Postfix(LandPlotLocation __instance, GameObject replacementPrefab)
        {
            if (!Core.IsMultiplayer)
                return;

            if (!Networking.HandlePacket)
            {
                var type = (int)replacementPrefab.GetComponent<LandPlot>().TypeId;
                SendData.SendLandPlotReplace(__instance.name, type);
            }
        }
    }

    [HarmonyPatch(typeof(RanchHouseUI), nameof(RanchHouseUI.SleepUntil))]
    class RanchHouseUI_SleepUntil
    {
        public static void Postfix(double endTime)
        {
            if (!Core.IsMultiplayer)
                return;

            SendData.SendSleep(endTime);
        }
    }

    //[HarmonyPatch(typeof(PlortEconomyDirector), nameof(PlortEconomyDirector.ResetPrices))]
    class EconomyDirector_ResetPrices
    {
        public static float[] ReceivedPrices;
        public static void Postfix(PlortEconomyDirector __instance)
        {
            if (!Core.IsMultiplayer)
                return;

            if (SteamLobby.Host)
            {
                SendData.SendPrices(__instance._currValueMap);
            }
            else if (Networking.HandlePacket)
            {
                int index = 0;
                foreach (var price in __instance._currValueMap)
                {
                    price.value.CurrValue = ReceivedPrices[index];
                    index++;
                }
            }
        }
    }

    [HarmonyPatch(typeof(TechUIInteractable), nameof(TechUIInteractable.OnInteract))]
    class TechUIInteractable_OnInteract
    {
        public static void Postfix(TechUIInteractable __instance)
        {
            if (!Core.IsMultiplayer)
                return;

            if (!Networking.HandlePacket)
            {
                SendData.SendOpenMap(__instance.name);
            }
        }
    }

    [HarmonyPatch(typeof(GordoEat), nameof(GordoEat.SetEatenCount))]
    class GordoEat_SetEatenCount
    {
        public static void Postfix(GordoEat __instance, int eatenCount)
        {
            if (!Core.IsMultiplayer)
                return;

            if (!Networking.HandlePacket)
            {
                SendData.SendGordoEat(__instance.name, eatenCount);
            }
        }
    }

    [HarmonyPatch(typeof(TreasurePod), nameof(TreasurePod.Activate))]
    class TreasurePod_Activate
    {
        public static void Postfix(TreasurePod __instance)
        {
            if (!Core.IsMultiplayer)
                return;

            if (!Networking.HandlePacket)
            {
                SendData.SendTreasurePod(__instance.name);
            }
        }
    }
}
