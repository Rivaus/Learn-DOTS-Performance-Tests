using UnityEngine;

namespace O2.MonoBehavior.Manager
{
    public class SpawnerAndSeekerManager : MonoBehaviour
    {
        [SerializeField] private int numberOfSeekers = 10000;

        [SerializeField] private Vector2 maxRange = new(-500f, 500f);

        [SerializeField] private GameObject seekerPrefab;

        [SerializeField] private int numberOfTargets = 20;

        [SerializeField] private GameObject targetPrefab;

        private Transform[] targets, seekers;

        private Vector3[] targetPositions;

        public bool IsInit { get; private set; } = false;

        #region Methods

        private void Awake()
        {
            Vector3 spawnPosition = Vector3.zero;
            Quaternion rotation = Quaternion.Euler(-90, 0, 0);
            GameObject go;

            seekers = new Transform[numberOfSeekers];

            // 1. Spawn seekers
            for (int i = 0; i < numberOfSeekers; i++)
            {
                spawnPosition.x = Random.Range(maxRange.x, maxRange.y);
                spawnPosition.z = Random.Range(maxRange.x, maxRange.y);

                go = Instantiate(seekerPrefab, spawnPosition, rotation);
                seekers[i] = go.transform;
            }

            targets = new Transform[numberOfTargets];
            targetPositions = new Vector3[numberOfTargets];

            // 2. Spawn targets
            for (int i = 0; i < numberOfTargets; i++)
            {
                spawnPosition.x = Random.Range(maxRange.x, maxRange.y);
                spawnPosition.z = Random.Range(maxRange.x, maxRange.y);

                GameObject target = Instantiate(targetPrefab, Vector3.zero, rotation);
                targets[i] = target.transform;
            }
        }

        Vector3 closestTarget;
        Transform seeker;


        public void Update()
        {
            // Modify target positions
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 position = Vector3.zero;

                for (int i = 0; i < numberOfTargets; i++)
                {
                    position.x = Random.Range(maxRange.x, maxRange.y) * .9f;
                    position.z = Random.Range(maxRange.x, maxRange.y) * .9f;

                    targets[i].position = position;
                    targetPositions[i] = position;
                }

                IsInit = true;
            }

            if (!IsInit)
                return;

            // Moves seekers
            for (int i = 0; i < numberOfSeekers; i++)
            {
                seeker = seekers[i];

                closestTarget = targetPositions[0];
                float distanceSQ = Vector3.SqrMagnitude(closestTarget - seeker.position);
                float tmpDistanceSQ;

                for (int j = 1; j < numberOfTargets; j++)
                {
                    tmpDistanceSQ = Vector3.SqrMagnitude(targetPositions[j] - seeker.position);

                    if (tmpDistanceSQ < distanceSQ)
                    {
                        closestTarget = targetPositions[j];
                        distanceSQ = tmpDistanceSQ;
                    }
                }

                Vector3 direction = closestTarget - seeker.position;

                if (direction.magnitude <= 1)
                    direction = Vector3.zero;
                else
                    direction = direction.normalized;

                seeker.position += direction * Time.deltaTime * 2f;
            }
        }

        #endregion
    }
}

