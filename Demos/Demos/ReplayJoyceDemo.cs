using System.IO;
using System.Numerics;
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
    private JsonDocument _jd;
    private int _nextAction = -1;
    private int _nActions = 0;
    
    public unsafe override void Initialize(ContentArchive content, Camera camera)
    {
        camera.Position = new Vector3(-30, 8, -110);
        camera.Yaw = MathHelper.Pi * 3f / 4;
        camera.Pitch = 0;
        Simulation = Simulation.Create(BufferPool, new DemoNarrowPhaseCallbacks(new SpringSettings(30, 1)), new DemoPoseIntegratorCallbacks(new Vector3(0, -10, 0)), new SolveDescription(8, 1));
        string strJsonDump = File.ReadAllText("..\\..\\..\\..\\Demos\\Content\\joyce-physics-dump-20240228172940.json");
        _jd = JsonDocument.Parse(strJsonDump);
        _nextAction = 0;
        _nActions = _jd.RootElement.GetProperty("actions").GetArrayLength();
    }


    public override void Update(Window window, Camera camera, Input input, float dt)
    {
        /*
         * Apply all actions until the next timestep.
         * Then consume the timestep and wait for the update.
         */
        if (-1 != _nextAction)
        {
            bool haveTimestep = false;
            bool foundTimestep = false;
            for (; _nextAction < _nActions; _nextAction++)
            {
                var je = _jd.RootElement.GetProperty("actions")[_nextAction];
                switch (je.GetProperty("type").GetString())
                {
                    case "engine.physics.actions.CreateDynamic":
                        break;
                    case "engine.physics.actions.CreateKinematic":
                        break;
                    case "engine.physics.actions.DynamicSnapshot":
                        break;
                    case "engine.physics.actions.Timestep":
                        foundTimestep = true;
                        break;
                    case "engine.physics.actions.SetBodyAwake":
                        break;
                    case "engine.physics.actions.SetBodyAngularVelocity":
                        break;
                    case "engine.physics.actions.SetBodyLinearVelocity":
                        break;
                    case "engine.physics.actions.SetBodyPoseOrientation":
                        break;
                    case "engine.physics.actions.SetBodyPosePosition":
                        break;
                    default:
                        break;
                }

                if (foundTimestep)
                {
                    break;
                }
            }
        }
        base.Update(window, camera, input, dt);
    }

    
    public override void Render(Renderer renderer, Camera camera, Input input, TextBuilder text, Font font)
    {
        base.Render(renderer, camera, input, text, font);
    }
}