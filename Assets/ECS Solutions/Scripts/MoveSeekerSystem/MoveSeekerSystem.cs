using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace O4.ECS
{
    /// <summary>
    /// Moves seeker with simple system.
    /// </summary>
    [UpdateAfter(typeof(SetTargetSystem))]
    partial struct MoveSeekerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimpleECSContext>();
            state.RequireForUpdate<Spawner>();
        }

        public void OnUpdate(ref SystemState state)
        {
            Spawner spawner = SystemAPI.GetSingleton<Spawner>();

            if (!spawner.isInit)
                return;

            foreach(RefRW<LocalTransform> seeker in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<Seeker>())
            {
                // 1. Find the closest target
                float3 closestTarget = float3.zero;
                float distanceSQ = float.PositiveInfinity;
                float tmpDistanceSQ;

                foreach (RefRO<LocalTransform> target in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Target>())
                {
                    tmpDistanceSQ = math.distancesq(target.ValueRO.Position, seeker.ValueRO.Position);

                    if (tmpDistanceSQ < distanceSQ)
                    {
                        closestTarget = target.ValueRO.Position;
                        distanceSQ = tmpDistanceSQ;
                    }
                }

                // 2. Move towards it
                float3 direction = closestTarget - seeker.ValueRO.Position;

                if (math.lengthsq(direction) <= 1)
                    direction = float3.zero;
                else
                    direction = math.normalize(direction);

                seeker.ValueRW.Position += direction * SystemAPI.Time.DeltaTime * 2f;
            }
        }
    }

}
