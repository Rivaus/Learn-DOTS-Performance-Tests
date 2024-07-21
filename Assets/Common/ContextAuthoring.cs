using Unity.Entities;
using UnityEngine;

/// <summary>
/// As ISystem runs automatically, a class to prevent system to only run when desired.
/// </summary>
public class ContextAuthoring : MonoBehaviour
{
    public enum SystemContext
    {
        SimpleECS,
        SimpleECSBurst,
        ECSJob,
        ECSJobBurst
    }

    public SystemContext context;

    class Baker : Baker<ContextAuthoring>
    {
        public override void Bake(ContextAuthoring authoring)
        {
            Entity context = GetEntity(TransformUsageFlags.None);

            switch (authoring.context)
            {
                case SystemContext.SimpleECS:
                    AddComponent<SimpleECSContext>(context);
                    break;
                case SystemContext.SimpleECSBurst:
                    AddComponent<SimpleECSBurstContext>(context);
                    break;
                case SystemContext.ECSJob:
                    AddComponent<ECSJobContext>(context);
                    break;
                case SystemContext.ECSJobBurst:
                    AddComponent<ECSJobBurstContext>(context);
                    break;
            }
        }
    }

}

public struct SimpleECSContext : IComponentData { }

public struct SimpleECSBurstContext : IComponentData { }

public struct ECSJobContext : IComponentData { }

public struct ECSJobBurstContext : IComponentData { }