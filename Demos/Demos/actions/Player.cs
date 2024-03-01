using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.Json;
using BepuPhysics;

namespace engine.physics.actions;


public class Player
{
    private object _lo = new();
    private JsonDocument _jd;

    private int _nextAction = -1;
    private int _nActions = 0;
    
    public readonly HandleMapper<int> MapperBodies = new();
    public readonly HandleMapper<uint> MapperShapes = new();

    private Vector3 _v3DynamicPos = Vector3.Zero;

    public Vector3 CameraPosition
    {
        get
        {
            lock (_lo)
            {
                return _v3DynamicPos;
            }
        }
    }

    public JsonSerializerOptions JsonSerializerOptions = new()
    {
        IncludeFields = true,
        WriteIndented = true
    };
    
    private SortedDictionary<string, Type> _mapActions = new();
    
    
    public void LoadFromFile(string path)
    {
        string strJsonDump = File.ReadAllText(path);
        _jd = JsonDocument.Parse(strJsonDump);
        _nextAction = 0;
        _nActions = _jd.RootElement.GetProperty("actions").GetArrayLength();

        _nextAction = 0;
        _nActions = _jd.RootElement.GetProperty("actions").GetArrayLength();
    }


    /**
     * Apply all actions until the next timestep.
     * Then consume the timestep and return.
     */
    public void PerformNextChunk(Simulation simulation)
    {
        if (-1 != _nextAction)
        {
            bool haveTimestep = false;
            bool foundTimestep = false;
            while (_nextAction < _nActions)
            {
                var je = _jd.RootElement.GetProperty("actions")[_nextAction];
                ++_nextAction;
                string strType = je.GetProperty("type").ToString();

                if (strType == typeof(Timestep).ToString())
                {
                    foundTimestep = true;
                    break;
                }

                Type typeAction = _mapActions[strType];

                actions.ABase physAction = je.Deserialize(typeAction, JsonSerializerOptions) as ABase;

                try
                {
                    physAction.Execute(this, simulation);
                    int a = 1;
                }
                catch (Exception e)
                {
                    // just skip incomplete handles.
                    int a = 1;
                }

                if (strType == "engine.physics.actions.DynamicSnapshot")
                {
                    var ds = physAction as actions.DynamicSnapshot;
                    /*
                     * Note that the off (sleeping) position is negative, all meaningful
                     * action is on Y > 0 
                     */
                    if (ds.MotionState.Pose.Position.Y > 0f)
                    {
                        if (_v3DynamicPos.X == 0)
                        {
                            _v3DynamicPos = ds.MotionState.Pose.Position;
                        }
                        else
                        {
                            if (_v3DynamicPos.X < ds.MotionState.Pose.Position.X)
                            {
                                _v3DynamicPos = ds.MotionState.Pose.Position;
                            }
                        }
                    }
                }
            }
        }
    }
    
    
    private void _addAction(Type t)
    {
        lock (_lo)
        {
            _mapActions.Add(t.ToString(), t);
        }
    }


    private void _addActions()
    {
        _addAction(typeof(CreateDynamic));
        _addAction(typeof(CreateKinematic));
        _addAction(typeof(CreateSphereShape));
        _addAction(typeof(DynamicSnapshot));
        _addAction(typeof(RemoveBody));
        _addAction(typeof(SetBodyAngularVelocity));
        _addAction(typeof(SetBodyAwake));
        _addAction(typeof(SetBodyLinearVelocity));
        _addAction(typeof(SetBodyPoseOrientation));
        _addAction(typeof(SetBodyPosePosition));
        _addAction(typeof(Timestep));
    }


    public Player()
    {
        _addActions();
    }
}