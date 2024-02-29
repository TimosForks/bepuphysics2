using System.IO;
using System.Numerics;
using System.Security.AccessControl;
using System.Text.Json;
using BepuPhysics;
using BepuPhysics.Constraints;
using BepuUtilities;
using DemoContentLoader;
using DemoRenderer;
using DemoRenderer.UI;
using DemoUtilities;

namespace Demos.Demos;

public class ReplayJoyceDemo : Demo
{
    private engine.physics.actions.Player _player = new();
    
    
    public unsafe override void Initialize(ContentArchive content, Camera camera)
    {
        camera.Position = new Vector3(-30, 8, -110);
        camera.Yaw = MathHelper.Pi * 3f / 4;
        camera.Pitch = 0;
        Simulation = Simulation.Create(BufferPool, new DemoNarrowPhaseCallbacks(new SpringSettings(30, 1)), new DemoPoseIntegratorCallbacks(new Vector3(0, -10, 0)), new SolveDescription(8, 1));

        _player.LoadFromFile("..\\..\\..\\..\\Demos\\Content\\joyce-physics-dump-20240229183707.json");
    }


    public override void Update(Window window, Camera camera, Input input, float dt)
    {
        _player.PerformNextChunk(Simulation);
        base.Update(window, camera, input, dt);
    }

    
    public override void Render(Renderer renderer, Camera camera, Input input, TextBuilder text, Font font)
    {
        base.Render(renderer, camera, input, text, font);
    }
}