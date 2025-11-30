using Il2CppMonomiPark.SlimeRancher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SR2MP
{
    public class NetworkActor : MonoBehaviour
    {
        public int Id;
        public bool IsLocal;

        private Vacuumable vacuumable;

        public Vector3 ReceivedPosition;
        public Vector3 ReceivedRotation;

        public bool DestroyReceived;

        private Vector3 _CachedPosition;
        private Quaternion _CachedRotation;

        void Start()
        {
            vacuumable = GetComponent<Vacuumable>();

            ReceivedPosition = this.transform.position;
            ReceivedRotation = this.transform.rotation.eulerAngles;
        }

        void Update()
        {
            if (!IsLocal)
            {
                this.transform.position = ReceivedPosition;
                this.transform.rotation = Quaternion.Euler(ReceivedRotation);

                if (vacuumable != null)
                {
                    if (vacuumable.IsCaptive() || vacuumable.IsHeld())
                    {
                        IsLocal = true;
                        SendData.SendSwitchRights(Id);
                    }
                }
            }
        }

        void FixedUpdate()
        {
            if (IsLocal)
            {
                if (this.transform.position != _CachedPosition || this.transform.rotation != _CachedRotation)
                {
                    ReadData.ActorsToSend.Add(this);

                    _CachedPosition = this.transform.position;
                    _CachedRotation = this.transform.rotation;
                }
            }
        }

        void OnDestroy()
        {
            if (!DestroyReceived)
            {
                SendData.SendDestroy(Id);
                Core.Actors.Remove(Id);
            }
        }
    }
}
