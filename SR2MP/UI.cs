using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SR2MP
{
    public class UI : MonoBehaviour
    {
        AssetBundle bundle;
        GUISkin sr2mp;
        Texture2D Egor_ICE;
        void Start()
        {
            bundle = AssetBundle.LoadFromMemory(Properties.Resources.sr2mp);
            sr2mp = bundle.LoadAsset("SR2MP").Cast<GUISkin>();
            Egor_ICE = bundle.LoadAsset("Egor_ICE").Cast<Texture2D>();
        }

        bool menuState = true;
        bool debug;
        bool lobbyCreated;

        void OnGUI()
        {
            if (menuState)
            {
                GUI.skin = sr2mp;

                GUI.Box(new Rect(10, 10, 160, 300), "SR2MP");

                if (!inGame)
                {
                    GUI.Label(new Rect(15, 15, 150, 290), "Just start the game and you can invite a friend here!");
                }
                else
                {
                    if (SteamLobby.ConnectedPlayerName == "None")
                    {
                        if (GUI.Button(new Rect(15, 35, 150, 25), "Invite friend"))
                        {
                            if (!lobbyCreated)
                            {
                                SteamLobby.CreateLobby();
                                lobbyCreated = true;
                            }
                            SteamLobby.InviteFriend();
                        }
                    }
                    else
                    {
                        GUI.Label(new Rect(15, 15, 150, 290), "Have a good play together!");
                        if (GUI.Button(new Rect(40, 185, 100, 25), "Close"))
                        {
                            menuState = false;
                        }
                    }

                    GUI.Label(new Rect(15, 65, 150, 25), "Connected friend:");
                    GUI.Label(new Rect(15, 95, 150, 25), SteamLobby.ConnectedPlayerName);
                }

                GUI.DrawTexture(new Rect(15, 275, 150, 25), Egor_ICE);


                if (debug)
                {
                    GUI.Box(new Rect(10, 315, 160, 160), "Debug");

                    GUI.Label(new Rect(15, 340, 150, 25), $"Actors sending: {ReadData.ActorsToSend.Count}");

                    if (GUI.Button(new Rect(15, 370, 150, 25), "Request Save"))
                    {
                        SendData.SendRequestSave();
                    }

                    var extrapolation = NetworkPlayer.Extrapolation ? "<color=green>ON</color>" : "<color=red>OFF</color>";
                    if (GUI.Button(new Rect(15, 400, 150, 25), $"Extrapolation: {extrapolation}"))
                    {
                        NetworkPlayer.Extrapolation = !NetworkPlayer.Extrapolation;
                    }
                }

                if (Event.current.Equals(Event.KeyboardEvent(KeyCode.O.ToString())))
                {
                    debug = !debug;
                }
            }

            if (Event.current.Equals(Event.KeyboardEvent(KeyCode.BackQuote.ToString())))
            {
                menuState = !menuState;
            }
        }

        bool inGame;
        void Update()
        {
            inGame = SceneContext.Instance?.Player != null;

            if (sr2mp == null)
            {
                sr2mp = bundle.LoadAsset("SR2MP").Cast<GUISkin>();
            }

            if (Egor_ICE == null)
            {
                Egor_ICE = bundle.LoadAsset("Egor_ICE").Cast<Texture2D>();
            }
        }
    }
}
