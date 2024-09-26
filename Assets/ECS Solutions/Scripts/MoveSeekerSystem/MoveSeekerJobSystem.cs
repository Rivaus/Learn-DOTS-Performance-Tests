using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace O4.ECS
{
    /// <summary>
    /// Moves seeker with parallel jobs.
    /// </summary>
    [UpdateAfter(typeof(SetTargetSystem))]
    partial struct MoveSeekerJobSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ECSJobContext>();
            state.RequireForUpdate<Spawner>();
        }

        public void OnUpdate(ref SystemState state)
        {
            Spawner spawner = SystemAPI.GetSingleton<Spawner>();

            if (!spawner.isInit)
                return;

            // 1. Get targets
            NativeArray<float3> targets = new NativeArray<float3>(spawner.numberOfTargets, Allocator.TempJob);
            int i = 0;

            foreach(RefRO<LocalTransform> target in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Target>())
            {
                targets[i] = target.ValueRO.Position; 
                i++;
            }

            // 2. Move seekers
            MoveSeekerJob job = new()
            {
                deltaTime = SystemAPI.Time.DeltaTime,
                targets = targets,
                nbTargets = spawner.numberOfTargets
            };

            job.ScheduleParallel();
        }
    }

    [WithAll(typeof(Seeker))]
    public partial struct MoveSeekerJob : IJobEntity
    {
        [ReadOnly] public float deltaTime;
        [ReadOnly][DeallocateOnJobCompletion] public NativeArray<float3> targets;
        [ReadOnly] public int nbTargets;

        public void Execute(ref LocalTransform seeker)
        {
            // 1. Find the closest target
            float3 closestTarget = targets[0];
            float distanceSQ = math.distancesq(closestTarget, seeker.Position);
            float tmpDistanceSQ;

            for (int i = 1; i < nbTargets; i++)
            {
                tmpDistanceSQ = math.distancesq(targets[i], seeker.Position);

                if (tmpDistanceSQ < distanceSQ)
                {
                    closestTarget = targets[i];
                    distanceSQ = tmpDistanceSQ;
                }
            }

            // 2. Move toward it
            float3 direction = closestTarget - seeker.Position;

            if (math.lengthsq(direction) <= 1)
                direction = float3.zero;
            else
                direction = math.normalize(direction);

            seeker.Position += direction * deltaTime * 2f;
        }
    }
}
