using System;
using System.Collections;
using Utilities;
using UnityEngine;

namespace Pathfinding
{
    public class Follower : MonoBehaviour
    {
        [SerializeField] private Grid grid;
        [SerializeField] private Transform target;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float timeToCheckForPath;

        private Pathfinder _pathfinder;
        
        private void Start()
        {
            _pathfinder = new Pathfinder();
            StartCoroutine(StartTargetPersuit());
        }

        IEnumerator StartTargetPersuit()
        {
            float l_currentTime = 0;
            
            while (true)
            {
                var l_distance = Vector3.Distance(transform.position.XOZ(), target.position.XOZ());

                if (l_distance <= 0.5f)
                {
                    yield return null;
                    continue;
                }

               var l_position = transform.position;

                if (l_currentTime <=0)
                {
                    _pathfinder.FindPath(l_position.XOZ(), target.position.XOZ(),grid);
                    l_currentTime = timeToCheckForPath;
                }

                var l_point = _pathfinder.GetPoint().XOZ();
                var l_dir = (l_point - l_position.XOZ()).normalized;
                
                var l_distanceToPoint = Vector3.Distance(l_position.XOZ(), l_point);

                if (l_distanceToPoint < 0.1f)
                    _pathfinder.NextPoint();
                
                l_position += l_dir * (moveSpeed * Time.deltaTime);
                transform.position = l_position;
                
                l_currentTime -= Time.deltaTime;

                yield return null;
            }
        }
    }

}
