using Il2CppMonomiPark.SlimeRancher.Regions;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SR2MP
{
    public class Core : MonoBehaviour
    {
        public static bool IsMultiplayer;

        public static Dictionary<int, NetworkActor> Actors = new Dictionary<int, NetworkActor>();
        public static bool FindActors;
        public static bool SyncActors;

        GameObject localPlayer;
        bool getBeatrix = true;
        bool setUpBeatrix = false;

        private static int spawnId;
        public static int SpawnID
        {
            get
            {
                if (SteamLobby.Host)
                    spawnId++;
                else
                    spawnId--;

                return spawnId;
            }
        }

        void Update()
        {
            if (FindActors)
            {
                var actors = FindObjectsOfType<IdentifiableActor>();
                if (actors.Length > 1)
                {
                    int maxId = 0;
                    foreach (var actor in actors)
                    {
                        if (!actor.identType.name.Equals("Player"))
                        {
                            var networkActor = actor.gameObject.AddComponent<NetworkActor>();
                            networkActor.Id = (int)actor._model.actorId.Value;
                            networkActor.IsLocal = SteamLobby.Host;
                            Actors.Add(networkActor.Id, networkActor);

                            if (networkActor.Id > maxId)
                                maxId = networkActor.Id;
                        }
                    }

                    if (SteamLobby.Host)
                        spawnId = maxId;

                    FindActors = false;
                    SyncActors = true;
                }
            }

            if (localPlayer == null)
            {
                if (SceneContext.Instance != null)
                {
                    if (SceneContext.Instance.Player != null)
                    {
                        SceneContext.Instance.Player.AddComponent<ReadData>();
                        localPlayer = SceneContext.Instance.Player;

                        OnGameLoaded();
                    }
                }
            }

            if (setUpBeatrix)
            {
                if (localPlayer != null)
                {
                    SetUpNetworkPlayer();
                    setUpBeatrix = false;
                }
            }

            if (getBeatrix)
            {
                var activeScene = SceneManager.GetActiveScene();
                if (activeScene.name.Equals("MainMenuEnvironment"))
                {
                    CreateNetworkPlayer();
                    getBeatrix = false;
                    setUpBeatrix = true;
                }
            }
        }

        private void CreateNetworkPlayer()
        {
            var networkPlayer = GameObject.Find("BeatrixMainMenu");
            Instantiate(networkPlayer);

            networkPlayer.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            networkPlayer.AddComponent<NetworkPlayer>();
            DontDestroyOnLoad(networkPlayer);
        }

        private void SetUpNetworkPlayer()
        {
            var localPlayerAnimator = localPlayer.GetComponent<Animator>();
            NetworkPlayer.PlayerAnimator.avatar = localPlayerAnimator.avatar;
            NetworkPlayer.PlayerAnimator.runtimeAnimatorController = localPlayerAnimator.runtimeAnimatorController;

            var networkPlayer = GameObject.Find("BeatrixMainMenu");
            var regionLoader = networkPlayer.AddComponent<RegionLoader>();
            regionLoader.LoadSize = new Vector3(200f, 200f, 200f);
            regionLoader.WakeSize = new Vector3(50f, 200f, 50f);
        }

        public static Dictionary<string, GameObject> Prefabs = new Dictionary<string, GameObject>();
        public static Dictionary<string, SceneGroup> Scenes = new Dictionary<string, SceneGroup>();
        private void OnGameLoaded()
        {
            var identifiables = Resources.FindObjectsOfTypeAll<IdentifiableType>();
            foreach (var identifiable in identifiables)
            {
                if (identifiable.prefab != null)
                {
                    if (!Prefabs.ContainsKey(identifiable.prefab.name))
                    {
                        Prefabs.Add(identifiable.prefab.name, identifiable.prefab);
                    }
                }
            }

            var sceneGroups = Resources.FindObjectsOfTypeAll<SceneGroup>();
            foreach (var sceneGroup in sceneGroups)
            {
                if (!Scenes.ContainsKey(sceneGroup.name))
                {
                    Scenes.Add(sceneGroup.name, sceneGroup);
                }
            }
        }
    }
}
