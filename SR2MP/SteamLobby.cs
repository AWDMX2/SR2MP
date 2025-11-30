using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SR2MP
{
    public class SteamLobby : MonoBehaviour
    {
        public static CSteamID Receiver = CSteamID.Nil;
        public static CSteamID Lobby;
        public static bool Host;
        public static string ConnectedPlayerName = "None";

        Callback<LobbyCreated_t> lobbyCreated;
        Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
        Callback<LobbyEnter_t> lobbyEntered;
        Callback<LobbyChatUpdate_t> lobbyChatUpdate;

        void Start()
        {
            SteamAPI.Init();
            Networking.InitializePackets();
            CreateCallbacks();
        }

        void Update()
        {
            Networking.ListenData();
            SteamAPI.RunCallbacks();
        }

        public void CreateCallbacks()
        {
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);

            Console.WriteLine("Callbacks created");
        }

        public static void CreateLobby()
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 2);
            Host = true;
        }

        public static void InviteFriend()
        {
            SteamFriends.ActivateGameOverlayInviteDialog(Lobby);
        }

        public void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
                return;

            Lobby = new CSteamID(callback.m_ulSteamIDLobby);

            Debug.Log("Lobby created successfully!");
        }

        public void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
        {
            Debug.Log("Request to join lobby...");
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        public void OnLobbyEntered(LobbyEnter_t callback)
        {
            var lobby = new CSteamID(callback.m_ulSteamIDLobby);
            var members = SteamMatchmaking.GetNumLobbyMembers(lobby);
            for (int i = 0; i < members; i++)
            {
                var member = SteamMatchmaking.GetLobbyMemberByIndex(lobby, i);
                if (member != SteamUser.GetSteamID())
                {
                    Receiver = member;
                    ConnectedPlayerName = SteamFriends.GetFriendPersonaName(Receiver);
                    Host = false;

                    //SendData.SendMessage("Punch");

                    SendData.SendRequestSave();

                    Core.IsMultiplayer = true;
                }
            }
            Debug.Log("You have successfully joined the lobby!");
        }

        public void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
        {
            Receiver = new CSteamID(callback.m_ulSteamIDUserChanged);
            ConnectedPlayerName = SteamFriends.GetFriendPersonaName(Receiver);
            Debug.Log($"Player {ConnectedPlayerName} successefully connected!");

            SendData.SendMessage("Punch");

            Core.IsMultiplayer = true;
        }
    }
}
