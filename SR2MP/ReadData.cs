using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SR2MP
{
    public class ReadData : MonoBehaviour
    {
        private Animator _Animator;

        private Vector3 _Position;
        private float _Rotation;

        private float _HorizontalMovement;
        private float _ForwardMovement;
        private float _Yaw;
        private int _AirborneState;
        private bool _Moving;
        private float _HorizontalSpeed;
        private float _ForwardSpeed;

        private double _Time;

        private int _Currency;
        private int _CurrencyCached;

        public static List<NetworkActor> ActorsToSend = new List<NetworkActor>();

        void Start()
        {
            _Animator = GetComponent<Animator>();
        }

        void Update()
        {
            if (HandleData.CurrencyReceived)
            {
                _CurrencyCached = HandleData.ReceivedCurrency;
                HandleData.CurrencyReceived = false;
            }

            ReadCurrency();
            if (_Currency != _CurrencyCached)
            {
                SendData.SendCurrency(_Currency, false);
                _CurrencyCached = _Currency;
            }
        }

        void FixedUpdate()
        {
            ReadMovementWithAnimations();
            SendData.SendMovementWithAnimations(_Position, _Rotation, _HorizontalMovement, _ForwardMovement, _Yaw, _AirborneState, _Moving, _HorizontalSpeed, _ForwardSpeed);

            if (SteamLobby.Host)
            {
                ReadTime();
                SendData.SendTime(_Time);
            }

            ReadActors();
        }

        private void ReadMovementWithAnimations()
        {
            _Position = this.transform.position;
            _Rotation = this.transform.rotation.eulerAngles.y;
            _HorizontalMovement = _Animator.GetFloat("HorizontalMovement");
            _ForwardMovement = _Animator.GetFloat("ForwardMovement");
            _Yaw = _Animator.GetFloat("Yaw");
            _AirborneState = _Animator.GetInteger("AirborneState");
            _Moving = _Animator.GetBool("Moving");
            _HorizontalSpeed = _Animator.GetFloat("HorizontalSpeed");
            _ForwardSpeed = _Animator.GetFloat("ForwardSpeed");
        }

        private void ReadTime()
        {
            _Time = SRSingleton<SceneContext>.Instance.TimeDirector._worldModel.worldTime;
        }

        private void ReadActors()
        {
            if (ActorsToSend.Count > 0)
            {
                int counter = 0;
                var actors = new List<NetworkActor>();
                foreach (var actor in ActorsToSend)
                {
                    counter++;

                    if (actor != null)
                    {
                        actors.Add(actor);
                    }

                    if (actors.Count == 42 || counter == ActorsToSend.Count)
                    {
                        SendData.SendActors(actors);
                        actors.Clear();
                    }
                }

                ActorsToSend.Clear();
            }
        }

        private void ReadCurrency()
        {
            _Currency = SRSingleton<SceneContext>.Instance.PlayerState._model._currencies[1].Amount;
        }
    }
}
