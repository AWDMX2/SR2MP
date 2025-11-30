using Il2CppMonomiPark.SlimeRancher.Economy;
using Il2CppMonomiPark.SlimeRancher.Player;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Object;

namespace SR2MP
{
    public class HandleData
    {
        public static void HandleMessage(Packet _packet)
        {
            var msg = _packet.ReadString();
            Console.WriteLine(msg);
        }

        public static void HandleMovementWithAnimations(Packet _packet)
        {
            NetworkPlayer.ReceivedPosition = _packet.ReadVector3();
            NetworkPlayer.ReceivedRotation = _packet.ReadFloat();
            NetworkPlayer.HorizontalMovement = _packet.ReadFloat();
            NetworkPlayer.ForwardMovement = _packet.ReadFloat();
            NetworkPlayer.Yaw = _packet.ReadFloat();
            NetworkPlayer.AirborneState = _packet.ReadInt();
            NetworkPlayer.Moving = _packet.ReadBool();
            NetworkPlayer.HorizontalSpeed = _packet.ReadFloat();
            NetworkPlayer.ForwardSpeed = _packet.ReadFloat();
            NetworkPlayer.MovementWithAnimationsReceived = true;
        }

        public static void HandleRequestSave(Packet _packet)
        {
            var memoryStream = new Il2CppSystem.IO.MemoryStream();
            GameContext.Instance.AutoSaveDirector.SaveGame();
            var save = GameContext.Instance.AutoSaveDirector.GetSaveToContinue();
            GameContext.Instance.AutoSaveDirector._storageProvider.GetGameData(save.SaveName, memoryStream);

            memoryStream.Seek(0L, Il2CppSystem.IO.SeekOrigin.Begin);

            var saveArray = memoryStream.ToArray();
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    gzipStream.Write(saveArray, 0, saveArray.Length);
                }
                saveArray = outputStream.ToArray();
            }

            SendData.SendSave(saveArray);

            Core.FindActors = true;

            #if DEBUG
            Core.IsMultiplayer = true;
            #endif
        }

        public static void HandleSave(Packet _packet)
        {
            var length = _packet.ReadInt();
            var array = _packet.ReadBytes(length);

            using (MemoryStream inputStream = new MemoryStream(array))
            {
                using (GZipStream gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                {
                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        gzipStream.CopyTo(outputStream);
                        array = outputStream.ToArray();
                    }
                }
            }

            var memoryStream = new Il2CppSystem.IO.MemoryStream(array);
            memoryStream.Seek(0L, Il2CppSystem.IO.SeekOrigin.Begin);

            var gsi = new Il2CppMonomiPark.SlimeRancher.GameSaveIdentifier() { GameName = "SR2MP", SaveName = "SR2MP" };
            GameContext.Instance.AutoSaveDirector.BeginLoad(gsi, memoryStream);

            Core.FindActors = true;

            #if DEBUG
            Core.IsMultiplayer = true;
            #endif
        }

        public static void HandleTime(Packet _packet)
        {
            var time = _packet.ReadDouble();

            if (SceneContext.Instance != null)
            {
                if (SceneContext.Instance.TimeDirector != null)
                {
                    SceneContext.Instance.TimeDirector._worldModel.worldTime = time;
                }
            }
        }

        public static void HandleLandPlotUpgrade(Packet _packet)
        {
            var name = _packet.ReadString();
            var upgrade = _packet.ReadInt();

            var gameObject = GameObject.Find(name);
            if (gameObject != null)
            {
                var landPlot = gameObject.GetComponentInChildren<LandPlot>();
                landPlot.AddUpgrade((LandPlot.Upgrade)upgrade);
            }
        }

        public static void HandleLandPlotReplace(Packet _packet)
        {
            var name = _packet.ReadString();
            var type = _packet.ReadInt();

            var gameObject = GameObject.Find(name);
            if (gameObject != null)
            {
                var landPlotLocation = gameObject.GetComponent<LandPlotLocation>();
                var oldLandPlot = landPlotLocation.GetComponentInChildren<LandPlot>();
                var replacementPrefab = GameContext.Instance.LookupDirector.GetPlotPrefab((LandPlot.Id)type);
                landPlotLocation.Replace(oldLandPlot, replacementPrefab);
            }
        }

        public static int ReceivedCurrency;
        public static bool CurrencyReceived;
        public static void HandleCurrency(Packet _packet)
        {
            var currency = _packet.ReadInt();

            if (SceneContext.Instance != null)
            {
                SceneContext.Instance.PlayerState._model.SetCurrency(CurrencyUtility.DefaultCurrency, currency);
            }

            ReceivedCurrency = currency;
            CurrencyReceived = true;
        }

        public static void HandleSleep(Packet _packet)
        {
            var endTime = _packet.ReadDouble();

            if (LockOnDeath.Instance != null)
            {
                LockOnDeath.Instance.LockUntil(endTime, 0f);
            }
        }

        public static void HandlePrices(Packet _packet)
        {
            var count = _packet.ReadInt();

            var prices = new float[count];
            for (int i = 0; i < count; i++)
            {
                prices[i] = _packet.ReadFloat();
            }
            EconomyDirector_ResetPrices.ReceivedPrices = prices;
        }

        public static void HandleOpenMap(Packet _packet)
        {
            var name = _packet.ReadString();

            var gameObject = GameObject.Find(name);
            if (gameObject != null)
            {
                var techUIInteractable = gameObject.GetComponent<TechUIInteractable>();

                if (techUIInteractable != null)
                    techUIInteractable.OnInteract();
            }
        }

        public static void HandleGordoEat(Packet _packet)
        {
            var name = _packet.ReadString();
            var count = _packet.ReadInt();

            var gameObject = GameObject.Find(name);
            if (gameObject != null)
            {
                var gordoEat = gameObject.GetComponent<GordoEat>();
                gordoEat.GordoModel.GordoEatenCount = count;

                if (gordoEat.GetEatenCount() >= gordoEat.GetTargetCount())
                {
                    gordoEat.StartCoroutine(gordoEat.ReachedTarget());
                }
            }
        }

        public static void HandleTreasurePod(Packet _packet)
        {
            var name = _packet.ReadString();

            var gameObject = GameObject.Find(name);
            if (gameObject != null)
            {
                var treasurePod = gameObject.GetComponent<TreasurePod>();
                treasurePod.Activate();
            }
        }

        public static void HandleActors(Packet _packet)
        {
            var count = _packet.ReadInt();

            for (int i = 0; i < count; i++)
            {
                var id = _packet.ReadInt();
                var pos = _packet.ReadVector3();
                var rot = _packet.ReadVector3();

                if (Core.Actors.TryGetValue(id, out NetworkActor actor))
                {
                    actor.ReceivedPosition = pos;
                    actor.ReceivedRotation = rot;
                }
            }
        }

        public static void HandleSpawn(Packet _packet)
        {
            var id = _packet.ReadInt();
            var prefabName = _packet.ReadString();
            var sceneName = _packet.ReadString();
            var pos = _packet.ReadVector3();
            var rot = _packet.ReadQuaternion();

            Core.Prefabs.TryGetValue(prefabName, out GameObject prefab);
            Core.Scenes.TryGetValue(sceneName, out SceneGroup scene);

            if (prefab != null && scene != null)
            {
                if (!Core.Actors.ContainsKey(id))
                {
                    var gameObject = InstantiationHelpers.InstantiateActor(prefab, scene, pos, rot);
                    var networkActor = gameObject.AddComponent<NetworkActor>();
                    networkActor.Id = id;
                    Core.Actors.Add(networkActor.Id, networkActor);
                }
            }
        }

        public static void HandleDestroy(Packet _packet)
        {
            var id = _packet.ReadInt();

            if (Core.Actors.TryGetValue(id, out NetworkActor actor))
            {
                actor.DestroyReceived = true;
                Destroy(actor.gameObject);

                Core.Actors.Remove(id);
            }
        }

        public static void HandleSwitchRights(Packet _packet)
        {
            var id = _packet.ReadInt();

            if (Core.Actors.TryGetValue(id, out NetworkActor actor))
            {
                actor.IsLocal = false;
            }
        }
    }
}
