using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace O4.ECS
{
    public class SpawnerAuthoring : MonoBehaviour
    {
        public int numberOfSeekers = 10000;

        public Vector2 maxRange = new(-500f, 500f);

        public GameObject seekerPrefab;

        public int numberOfTargets = 20;

        public GameObject targetPrefab;

        private class Baker : Baker<SpawnerAuthoring>
        {
            public override void Bake(SpawnerAuthoring authoring)
            {
                Entity spawner = GetEntity(TransformUsageFlags.None);
                AddComponent(spawner, new Spawner()
                {
                    numberOfSeekers = authoring.numberOfSeekers,
                    maxRange = authoring.maxRange,
                    seekerPrefab = GetEntity(authoring.seekerPrefab, TransformUsageFlags.Dynamic),
                    numberOfTargets = authoring.numberOfTargets,
                    targetPrefab = GetEntity(authoring.targetPrefab, TransformUsageFlags.Dynamic),
                    isInit = false,
                });
            }
        }
    }

    public struct Spawner : IComponentData
    {
        public int numberOfSeekers;

        public float2 maxRange;

        public Entity seekerPrefab;

        public int numberOfTargets;

        public Entity targetPrefab;

        public bool isInit;
    }

    /// <summary>
    /// Component to tag seeker.
    /// </summary>
    public struct Seeker : IComponentData
    {

    }

    /// <summary>
    /// Component to tag target.
    /// </summary>
    public struct Target : IComponentData
    {

    }
}
