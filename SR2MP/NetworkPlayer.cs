using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SR2MP
{
    public class NetworkPlayer : MonoBehaviour
    {
        public static Animator PlayerAnimator;

        private Vector3 displacement;
        private float spin;
        private float deltaTime;
        private Vector3 finalPosition;
        private float finalRotation;
        private float timestamp;

        public static bool Extrapolation = true;

        public static Vector3 ReceivedPosition;
        public static float ReceivedRotation;

        public static float HorizontalMovement;
        public static float ForwardMovement;
        public static float Yaw;
        public static int AirborneState;
        public static bool Moving;
        public static float HorizontalSpeed;
        public static float ForwardSpeed;

        public static bool MovementWithAnimationsReceived;

        void Start()
        {
            PlayerAnimator = GetComponent<Animator>();

            this.transform.position = new Vector3(530f, 17.11f, 335f);
            this.transform.rotation = Quaternion.Euler(this.transform.rotation.eulerAngles.x, 40f, this.transform.rotation.eulerAngles.z);
        }

        void Update()
        {
            if (MovementWithAnimationsReceived)
            {
                displacement = ReceivedPosition - finalPosition;
                spin = ReceivedRotation - finalRotation;
                deltaTime = Time.time - timestamp;

                finalPosition = ReceivedPosition;
                finalRotation = ReceivedRotation;
                timestamp = Time.time;

                this.transform.position = ReceivedPosition;
                this.transform.rotation = Quaternion.Euler(this.transform.rotation.eulerAngles.x, ReceivedRotation, this.transform.rotation.eulerAngles.z);

                PlayerAnimator.SetFloat("HorizontalMovement", HorizontalMovement);
                PlayerAnimator.SetFloat("ForwardMovement", ForwardMovement);
                PlayerAnimator.SetFloat("Yaw", Yaw);
                PlayerAnimator.SetInteger("AirborneState", AirborneState);
                PlayerAnimator.SetBool("Moving", Moving);
                PlayerAnimator.SetFloat("HorizontalSpeed", HorizontalSpeed);
                PlayerAnimator.SetFloat("ForwardSpeed", ForwardSpeed);

                MovementWithAnimationsReceived = false;
            }
            else if (deltaTime != 0f && Extrapolation)
            {
                this.transform.position += displacement * Time.deltaTime / deltaTime;

                float y = this.transform.rotation.eulerAngles.y + spin * Time.deltaTime / deltaTime;

                if (y >= 360f || y < 0f)
                    return;

                this.transform.rotation = Quaternion.Euler(this.transform.rotation.eulerAngles.x, y, this.transform.rotation.eulerAngles.z);
            }
        }
    }
}
