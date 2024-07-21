using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace O3.MonoBehavior.Manager.Job
{
    public class SpawnerAndSeekerManagerJob : MonoBehaviour
    {
        [SerializeField] private bool useBurst = false;

        [SerializeField] private int numberOfSeekers = 10000;

        [SerializeField] private Vector2 maxRange = new(-500f, 500f);

        [SerializeField] private GameObject seekerPrefab;

        [SerializeField] private int numberOfTargets = 20;

        [SerializeField] private GameObject targetPrefab;

        private Transform[] targets;

        private TransformAccessArray seekers;

        private NativeArray<Vector3> targetPositions;

        public bool IsInit { get; private set; } = false;

        private JobHandle handle;

        #region Methods

        private void Awake()
        {
            Vector3 spawnPosition = Vector3.zero;
            Quaternion rotation = Quaternion.Euler(-90, 0, 0);
            GameObject go;

            Transform[] seekerTransforms = new Transform[numberOfSeekers];

            // 1. Spawn seekers
            for (int i = 0; i < numberOfSeekers; i++)
            {
                spawnPosition.x = Random.Range(maxRange.x, maxRange.y);
                spawnPosition.z = Random.Range(maxRange.x, maxRange.y);

                go = Instantiate(seekerPrefab, spawnPosition, rotation);
                seekerTransforms[i] = go.transform;
            }

            seekers = new TransformAccessArray(seekerTransforms);

            targets = new Transform[numberOfTargets];
            targetPositions = new NativeArray<Vector3>(numberOfTargets, Allocator.Persistent);

            // 2. Spawn targets
            for (int i = 0; i < numberOfTargets; i++)
            {
                spawnPosition.x = Random.Range(maxRange.x, maxRange.y);
                spawnPosition.z = Random.Range(maxRange.x, maxRange.y);

                GameObject target = Instantiate(targetPrefab, Vector3.zero, rotation);
                targets[i] = target.transform;
            }
        }

        private void Update()
        {
            handle.Complete();

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

            if (useBurst)
            {
                MoveSeekerJobBurst job = new MoveSeekerJobBurst()
                {
                    deltaTime = Time.deltaTime,
                    numberOfTargets = numberOfTargets,
                    targetPositions = targetPositions
                };
                handle = job.Schedule(seekers);
            } else
            {
                MoveSeekerJob job = new MoveSeekerJob()
                {
                    deltaTime = Time.deltaTime,
                    numberOfTargets = numberOfTargets,
                    targetPositions = targetPositions
                };
                handle = job.Schedule(seekers);
            }
        }

        private void OnDestroy()
        {
            handle.Complete();

            targetPositions.Dispose();
            seekers.Dispose();
        }

        #endregion
    }

    [BurstCompile]
    public struct MoveSeekerJobBurst : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<Vector3> targetPositions;
        [ReadOnly] public float deltaTime;
        [ReadOnly] public int numberOfTargets;

        [BurstCompile]
        public void Execute(int index, TransformAccess transform)
        {
            Vector3 closestTarget = targetPositions[0];
            float distanceSQ = Vector3.SqrMagnitude(closestTarget - transform.position);
            float tmpDistanceSQ;

            for (int j = 1; j < numberOfTargets; j++)
            {
                tmpDistanceSQ = Vector3.SqrMagnitude(targetPositions[j] - transform.position);

                if (tmpDistanceSQ < distanceSQ)
                {
                    closestTarget = targetPositions[j];
                    distanceSQ = tmpDistanceSQ;
                }
            }

            Vector3 direction = closestTarget - transform.position;

            if (direction.magnitude <= 1)
                direction = Vector3.zero;
            else
                direction = direction.normalized;

            transform.position += direction * deltaTime * 2f;
        }
    }

    public struct MoveSeekerJob : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<Vector3> targetPositions;
        [ReadOnly] public float deltaTime;
        [ReadOnly] public int numberOfTargets;

        public void Execute(int index, TransformAccess transform)
        {
            Vector3 closestTarget = targetPositions[0];
            float distanceSQ = Vector3.SqrMagnitude(closestTarget - transform.position);
            float tmpDistanceSQ;

            for (int j = 1; j < numberOfTargets; j++)
            {
                tmpDistanceSQ = Vector3.SqrMagnitude(targetPositions[j] - transform.position);

                if (tmpDistanceSQ < distanceSQ)
                {
                    closestTarget = targetPositions[j];
                    distanceSQ = tmpDistanceSQ;
                }
            }

            Vector3 direction = closestTarget - transform.position;

            if (direction.magnitude <= 1)
                direction = Vector3.zero;
            else
                direction = direction.normalized;

            transform.position += direction * deltaTime * 2f;
        }
    }
}

