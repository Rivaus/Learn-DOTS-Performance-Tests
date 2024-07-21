using UnityEngine;

namespace O1.Individual.MonoBehaviors
{
    public class TargetSeekerSpawner : MonoBehaviour
    {
        [SerializeField] private int numberOfSeekers = 10000;

        [SerializeField] private Vector2 maxRange = new(-500f, 500f);

        [SerializeField] private TargetSeeker seekerPrefab;

        [SerializeField] private int numberOfTargets = 20;

        [SerializeField] private GameObject targetPrefab;

        private Transform[] targets;

        private Vector3[] targetPositions;

        public bool IsInit { get; private set; } = false;

        public Vector3[] TargetPositions => targetPositions;

        #region Methods

        private void Awake()
        {
            Vector3 spawnPosition = Vector3.zero;
            Quaternion rotation = Quaternion.Euler(-90, 0, 0);
            TargetSeeker seeker;

            // 1. Spawn seekers
            for (int i = 0; i < numberOfSeekers; i++)
            {
                spawnPosition.x = Random.Range(maxRange.x, maxRange.y);
                spawnPosition.z = Random.Range(maxRange.x, maxRange.y);

                seeker = Instantiate(seekerPrefab, spawnPosition, rotation);
                seeker.Init(this);
            }

            targets = new Transform[numberOfTargets];
            targetPositions = new Vector3[numberOfTargets];

            // 2. Spawn targets
            for (int i = 0;i < numberOfTargets; i++)
            {
                spawnPosition.x = Random.Range(maxRange.x, maxRange.y);
                spawnPosition.z = Random.Range(maxRange.x, maxRange.y);

                GameObject target = Instantiate(targetPrefab, Vector3.zero, rotation);
                targets[i] = target.transform;
            }
        }

        public void Update()
        {
            // Modifies target positions
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
        }

        #endregion
    }
}

