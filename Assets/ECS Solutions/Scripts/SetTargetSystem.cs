using System.Linq;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace O4.ECS
{
    /// <summary>
    /// Updates target positions when pressing space key.
    /// </summary>
    partial struct SetTargetSystem : ISystem
    {
        private uint i;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Spawner>();
            i = 123;
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!Input.GetKeyDown(KeyCode.Space))
                return;

            var spawner = SystemAPI.GetSingletonRW<Spawner>();
            spawner.ValueRW.isInit = true;

            float3 position;
            Unity.Mathematics.Random random = new(i++);

            foreach(RefRW<LocalTransform> target in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<Target>())
            {
                position = new float3(random.NextFloat(spawner.ValueRO.maxRange.x, spawner.ValueRO.maxRange.y), 0, random.NextFloat(spawner.ValueRO.maxRange.x, spawner.ValueRO.maxRange.y)) * .9f;
                target.ValueRW.Position = position;
            }
        }
    }
}
