using Il2CppMonomiPark.SlimeRancher.Economy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SR2MP
{
    public class SendData
    {
        public static void SendMessage(string msg)
        {
            using (Packet _packet = new Packet((int)Packets.Message))
            {
                _packet.Write(msg);
                Networking.SendReliableData(_packet);
            }
        }

        public static void SendMovementWithAnimations(Vector3 pos, float rot, float a1, float a2, float a3, int a4, bool a5, float a6, float a7)
        {
            using (Packet _packet = new Packet((int)Packets.MovementWithAnimations))
            {
                _packet.Write(pos);
                _packet.Write(rot);
                _packet.Write(a1);
                _packet.Write(a2);
                _packet.Write(a3);
                _packet.Write(a4);
                _packet.Write(a5);
                _packet.Write(a6);
                _packet.Write(a7);
                Networking.SendUnreliableData(_packet);
            }
        }

        public static void SendRequestSave()
        {
            using (Packet _packet = new Packet((int)Packets.RequestSave))
            {
                Networking.SendReliableData(_packet);
            }
        }

        public static void SendSave(byte[] save)
        {
            using (Packet _packet = new Packet((int)Packets.Save))
            {
                _packet.Write(save.Length);
                _packet.Write(save);
                Networking.SendReliableData(_packet);
            }
        }

        public static void SendTime(double time)
        {
            using (Packet _packet = new Packet((int)Packets.Time))
            {
                _packet.Write(time);
                Networking.SendUnreliableData(_packet);
            }
        }

        public static void SendLandPlotUpgrade(string name, int upgrade)
        {
            using (Packet _packet = new Packet((int)Packets.LandPlotUpgrade))
            {
                _packet.Write(name);
                _packet.Write(upgrade);
                Networking.SendReliableData(_packet);
            }
        }

        public static void SendLandPlotReplace(string name, int type)
        {
            using (Packet _packet = new Packet((int)Packets.LandPlotReplace))
            {
                _packet.Write(name);
                _packet.Write(type);
                Networking.SendReliableData(_packet);
            }
        }

        public static void SendCurrency(int currency, bool reliable)
        {
            using (Packet _packet = new Packet((int)Packets.Currency))
            {
                _packet.Write(currency);
                if (reliable)
                {
                    Networking.SendReliableData(_packet);
                }
                else
                {
                    Networking.SendUnreliableData(_packet);
                }
            }
        }

        public static void SendSleep(double endTime)
        {
            using (Packet _packet = new Packet((int)Packets.Sleep))
            {
                _packet.Write(endTime);
                Networking.SendReliableData(_packet);
            }
        }

        public static void SendPrices(Il2CppSystem.Collections.Generic.Dictionary<IdentifiableType, PlortEconomyDirector.CurrValueEntry> prices)
        {
            using (Packet _packet = new Packet((int)Packets.Prices))
            {
                _packet.Write(prices.Count);
                foreach (var price in prices)
                {
                    _packet.Write(price.Value.CurrValue);
                }
                Networking.SendReliableData(_packet);
            }
        }

        public static void SendOpenMap(string name)
        {
            using (Packet _packet = new Packet((int)Packets.OpenMap))
            {
                _packet.Write(name);
                Networking.SendReliableData(_packet);
            }
        }

        public static void SendGordoEat(string name, int eatenCount)
        {
            using (Packet _packet = new Packet((int)Packets.GordoEat))
            {
                _packet.Write(name);
                _packet.Write(eatenCount);
                Networking.SendReliableData(_packet);
            }
        }

        public static void SendTreasurePod(string name)
        {
            using (Packet _packet = new Packet((int)Packets.TreasurePod))
            {
                _packet.Write(name);
                Networking.SendReliableData(_packet);
            }
        }

        public static void SendActors(List<NetworkActor> actorsToSend)
        {
            using (Packet _packet = new Packet((int)Packets.Actors))
            {
                _packet.Write(actorsToSend.Count);

                foreach (var actor in actorsToSend)
                {
                    _packet.Write(actor.Id);
                    _packet.Write(actor.transform.position);
                    _packet.Write(actor.transform.rotation.eulerAngles);
                }
                Networking.SendUnreliableData(_packet);
            }
        }

        public static void SendSpawn(int id, string prefab, string scene, Vector3 position, Quaternion rotation)
        {
            using (Packet _packet = new Packet((int)Packets.Spawn))
            {
                _packet.Write(id);
                _packet.Write(prefab);
                _packet.Write(scene);
                _packet.Write(position);
                _packet.Write(rotation);
                Networking.SendReliableData(_packet);
            }
        }

        public static void SendDestroy(int id)
        {
            using (Packet _packet = new Packet((int)Packets.Destroy))
            {
                _packet.Write(id);
                Networking.SendReliableData(_packet);
            }
        }

        public static void SendSwitchRights(int id)
        {
            using (Packet _packet = new Packet((int)Packets.SwitchRights))
            {
                _packet.Write(id);
                Networking.SendReliableData(_packet);
            }
        }
    }
}
