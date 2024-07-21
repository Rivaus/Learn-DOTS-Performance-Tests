using UnityEngine;

namespace O1.Individual.MonoBehaviors
{
    /// <summary>
    /// Move toward the closes target
    /// </summary>
    public class TargetSeeker : MonoBehaviour
    {
        TargetSeekerSpawner spawnerManager;

        public void Init(TargetSeekerSpawner spawnerManager)
        {
            this.spawnerManager = spawnerManager;
        }

        Vector3 closestTarget;

        private void Update()
        {
            if (spawnerManager is not null && spawnerManager.IsInit)
            {
                Vector3 target = FindClosestTarget();
                Vector3 direction = target - transform.position;

                if (direction.magnitude <= 1)
                    direction = Vector3.zero;
                else
                    direction = direction.normalized;

                transform.position += direction * Time.deltaTime * 2f;
            }
        }

        private Vector3 FindClosestTarget()
        {
            closestTarget = spawnerManager.TargetPositions[0];
            float distanceSQ = Vector3.SqrMagnitude(closestTarget - transform.position);
            float tmpDistanceSQ;

            Vector3[] targets = spawnerManager.TargetPositions;

            for (int i = 1; i < targets.Length; i++)
            {
                tmpDistanceSQ = Vector3.SqrMagnitude(targets[i] - transform.position);

                if (tmpDistanceSQ < distanceSQ)
                {
                    closestTarget = targets[i];
                    distanceSQ = tmpDistanceSQ;
                }
            }

            return closestTarget;
        }
    }
}


