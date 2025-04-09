using BB;
using Latios;
using Latios.Psyshock;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Latios.Transforms;
using static Unity.Entities.SystemAPI;
[BurstCompile]
public partial struct BounceSystem : ISystem
{
    LatiosWorldUnmanaged latiosWorld;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        latiosWorld = state.GetLatiosWorldUnmanaged();
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var staticEnvironmentCollisionLayer = latiosWorld.sceneBlackboardEntity
            .GetCollectionComponent<StaticEnvironmentCollisionLayer>().layer;
        var rigidBodyCollisionLayer =
            latiosWorld.sceneBlackboardEntity.GetCollectionComponent<RigidBodyCollisionLayer>().layer;
        var bounceProcessor = new BounceProcessor
        {
            rbLookup = GetComponentLookup<RigidBody>(),
            bounceLookup = GetComponentLookup<Bounce>(),
        };
        state.Dependency = Physics.FindPairs(rigidBodyCollisionLayer, staticEnvironmentCollisionLayer, bounceProcessor)
            .ScheduleParallel(state.Dependency);
        
        state.Dependency = Physics.FindPairs(rigidBodyCollisionLayer, bounceProcessor)
            .ScheduleParallel(state.Dependency);
    }

    struct BounceProcessor : IFindPairsProcessor
    {
        public PhysicsComponentLookup<RigidBody> rbLookup;
        public PhysicsComponentLookup<Bounce> bounceLookup;
        public void Execute(in FindPairsResult result)
        {
            var maxDistance = Collision.GetMaxDistance(result.entityA,result.entityB, in rbLookup);
            if (Physics.DistanceBetween(result.colliderA, result.transformA, result.colliderB, result.transformB,
                    maxDistance, out var hitResult))
            {
                Collision.DoBounce(result.entityA, ref rbLookup, in bounceLookup, hitResult.normalB);
                Collision.DoBounce(result.entityB, ref rbLookup, in bounceLookup, hitResult.normalA);
            }
        }
    }
}

public static class Collision
{
    public static float GetMaxDistance(in SafeEntity entityA, in SafeEntity entityB, in PhysicsComponentLookup<RigidBody> bodyLookup)
    {
        var rigidBodyA = new RigidBody();
        var rigidBodyB = new RigidBody();
        switch (bodyLookup.HasComponent(entityA), bodyLookup.HasComponent(entityB))
        {
            case (true, true):
                rigidBodyA = bodyLookup[entityA];
                rigidBodyB = bodyLookup[entityB];
                return UnitySim.MotionExpansion.GetMaxDistance(rigidBodyA.motionExpansion, rigidBodyB.motionExpansion);
            case (true, false):
                rigidBodyA = bodyLookup[entityA];
                return UnitySim.MotionExpansion.GetMaxDistance(rigidBodyA.motionExpansion);
            case (false, true)davc:
                rigidBodyB = bodyLookup[entityB];
                return UnitySim.MotionExpansion.GetMaxDistance(rigidBodyB.motionExpansion);
            case (false, false):
                return 0f;
        }
    }

    public static void DoBounce(in SafeEntity entity, ref PhysicsComponentLookup<RigidBody> rbLookup,
        in PhysicsComponentLookup<Bounce> bounceLookup, float3 hitNormal)
    {
        if (!rbLookup.HasComponent(entity))
            return;
        if (!bounceLookup.HasComponent(entity))
            return;
        var rigidBody = rbLookup[entity];
        var bounce = bounceLookup[entity];
        
        rigidBody.velocity.linear = math.reflect(rigidBody.velocity.linear * bounce.value * math.dot(rigidBody.velocity.linear, hitNormal), hitNormal);
        rbLookup[entity] = rigidBody;
    }
}