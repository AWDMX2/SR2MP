using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2MP
{
    public class Networking
    {
        private delegate void PacketHandler(Packet _packet);
        private static Dictionary<int, PacketHandler> packetHandlers;

        public static bool HandlePacket;

        public static void ListenData()
        {
            while (SteamNetworking.IsP2PPacketAvailable(out uint size))
            {
                byte[] _data = new byte[size];

                if (SteamNetworking.ReadP2PPacket(_data, size, out uint bytesRead, out CSteamID remoteId))
                {
                    HandleReceivedData(_data);
                }
            }
        }

        public static void SendReliableData(Packet packet)
        {
            byte[] data = packet.ToArray();
            SteamNetworking.SendP2PPacket(SteamLobby.Receiver, data, (uint)data.Length, EP2PSend.k_EP2PSendReliable, 0);
        }

        public static void SendUnreliableData(Packet packet)
        {
            byte[] data = packet.ToArray();
            SteamNetworking.SendP2PPacket(SteamLobby.Receiver, data, (uint)data.Length, EP2PSend.k_EP2PSendUnreliable, 0);
        }

        private static void HandleReceivedData(byte[] _data)
        {
            HandlePacket = true;

            using (Packet _packet = new Packet(_data))
            {
                byte _packetId = _packet.ReadByte();
                packetHandlers[_packetId].Invoke(_packet);
            }

            HandlePacket = false;
        }

        public static void InitializePackets()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)Packets.Message, HandleData.HandleMessage },
                { (int)Packets.MovementWithAnimations, HandleData.HandleMovementWithAnimations },
                { (int)Packets.RequestSave, HandleData.HandleRequestSave },
                { (int)Packets.Save, HandleData.HandleSave },
                { (int)Packets.Time, HandleData.HandleTime },
                { (int)Packets.LandPlotUpgrade, HandleData.HandleLandPlotUpgrade },
                { (int)Packets.LandPlotReplace, HandleData.HandleLandPlotReplace },
                { (int)Packets.Currency, HandleData.HandleCurrency },
                { (int)Packets.Sleep, HandleData.HandleSleep },
                { (int)Packets.Prices, HandleData.HandlePrices },
                { (int)Packets.OpenMap, HandleData.HandleOpenMap },
                { (int)Packets.GordoEat, HandleData.HandleGordoEat },
                { (int)Packets.TreasurePod, HandleData.HandleTreasurePod },
                { (int)Packets.Actors, HandleData.HandleActors },
                { (int)Packets.Spawn, HandleData.HandleSpawn },
                { (int)Packets.Destroy, HandleData.HandleDestroy },
                { (int)Packets.SwitchRights, HandleData.HandleSwitchRights }
            };
        }
    }
}
