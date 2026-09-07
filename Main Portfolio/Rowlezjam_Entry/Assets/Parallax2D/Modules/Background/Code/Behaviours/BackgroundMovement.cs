using System.Collections.Generic;
using UnityEngine;

namespace Parallax2D.Modules.Background.Code.Behaviours
{
    public class BackgroundMovement : MonoBehaviour
    {     
        public Vector2 Speed;
        public List<CheckPosition> CheckPosition = new List<CheckPosition>();
        public bool isMoveAlone = false;
        public Vector2 AloneSpeed;
        private Transform player;
        private Vector3 plusposition = Vector3.zero;


        public void Construct(Transform player, bool nineImage)
        {
            this.player = player;
            transform.position = player.position;

            foreach (var checkInBound in CheckPosition)
                checkInBound.Construct(player, nineImage);
        }

        public void UpdateMovement()
        {          
            var position = player.position + Vector3.forward;
            if (isMoveAlone) plusposition += (Vector3)AloneSpeed * Time.deltaTime;
            transform.position = new Vector3(position.x * Speed.x, position.y * Speed.y, position.z) + plusposition;

            foreach (var checkPosition in CheckPosition)
                checkPosition.UpdateStatus();
        }
    }
}