using System;
using System.IO;
using System.Numerics;
using System.Security.AccessControl;
using System.Text.Json;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;
using BepuUtilities;
using DemoContentLoader;
using DemoRenderer;
using DemoRenderer.UI;
using DemoUtilities;

namespace Demos.Demos;

public class ReplayJoyceDemo : Demo, Replay.IContactEventHandler
{
    public unsafe override void Initialize(ContentArchive content, Camera camera)
    {
        camera.Position = new Vector3(-30, 8, -110);
        
        /*
         * Look -z.
         */
        camera.Yaw = MathHelper.Pi * 2f / 4f;
        
        /*
         * Look down.
         */
        camera.Pitch = MathHelper.Pi * 3f / 4f;
        

        _contactEvents = new Replay.ContactEvents<ReplayJoyceDemo>(
            this,
            BufferPool,
            new ThreadDispatcher(4)
        );       Simulation = Simulation.Create(
            BufferPool,
            new Replay.NarrowPhaseCallbacks<ReplayJoyceDemo>(
                _contactEvents) /* { Properties = properties } */,
            new Replay.PoseIntegratorCallbacks(new Vector3(0, -9.81f, 0)),
            new SolveDescription(8, 1)
        );


        _player.LoadFromFile("..\\..\\..\\..\\Demos\\Content\\joyce-physics-dump-20240229191529.json");
    }


    public override void Update(Window window, Camera camera, Input input, float dt)
    { 
        /*
         * Be well above the scene
         */
        Vector3 v3CamOffset = new Vector3(0f, 80f, 00f);

        _player.PerformNextChunk(Simulation);
        camera.Position = _player.CameraPosition + v3CamOffset;
        base.Update(window, camera, input, dt);
    }

    
    public override void Render(Renderer renderer, Camera camera, Input input, TextBuilder text, Font font)
    {
        base.Render(renderer, camera, input, text, font);
    }

    public void OnContactAdded<TManifold>(CollidableReference eventSource, CollidablePair pair,
        ref TManifold contactManifold,
        in Vector3 contactOffset, in Vector3 contactNormal, float depth, int featureId, int contactIndex,
        int workerIndex) where TManifold : struct, IContactManifold<TManifold>
    {
        Console.WriteLine("Collision");
    }
    
    private Replay.ContactEvents<ReplayJoyceDemo> _contactEvents;
    private engine.physics.actions.Player _player = new();

}