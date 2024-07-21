using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace O4.ECS
{
    /// <summary>
    /// System to spawn seekers and targets when launches.
    /// </summary>
    partial struct SpawnerSeekerAndTargetSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Spawner>();
        }

        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            Spawner spawner = SystemAPI.GetSingleton<Spawner>();

            float3 spawnPosition;
            Random random = new Random(123);
            
            EntityCommandBuffer cmd = new EntityCommandBuffer(Allocator.Temp);

            quaternion rotation = quaternion.Euler(math.radians(-90), 0, 0);

            // 1. Spawn seekers
            for (int i = 0; i < spawner.numberOfSeekers; i++)
            {
                spawnPosition = new float3(random.NextFloat(spawner.maxRange.x, spawner.maxRange.y), 0, random.NextFloat(spawner.maxRange.x, spawner.maxRange.y));
                Entity seeker = cmd.Instantiate(spawner.seekerPrefab);
                cmd.SetComponent(seeker, new LocalTransform()
                {
                    Position = spawnPosition,
                    Rotation = rotation,
                    Scale = 100
                });
                cmd.AddComponent<Seeker>(seeker);
            }

            // 2. Spawn targets
            for (int i = 0; i < spawner.numberOfTargets; i++)
            {
                spawnPosition = new float3(random.NextFloat(spawner.maxRange.x, spawner.maxRange.y), 0, random.NextFloat(spawner.maxRange.x, spawner.maxRange.y)) *.9f;
                Entity target = cmd.Instantiate(spawner.targetPrefab);
                cmd.SetComponent(target, new LocalTransform()
                {
                    Position = spawnPosition,
                    Rotation = rotation,
                    Scale = 100
                });
                cmd.AddComponent<Target>(target);
            }

            cmd.Playback(state.EntityManager);
            cmd.Dispose();
        }
    }

}
