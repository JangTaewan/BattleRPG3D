using Godot;
using System;
using static Godot.TextServer;

namespace BattleRPG3D.Scripts.Characters
{
    public partial class Rig : Node3D
    {
        [Export]
        public float AnimationSpeed = 10.0f;
        private string _runPath = "parameters/MoveSpace/blend_position";
        private float _runWeightTarget = -1.0f;
        private AnimationTree _animationTree;
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();
            _animationTree = GetNode<AnimationTree>("AnimationTree");
        }
        public override void _PhysicsProcess(double delta)
        {
            _animationTree.Set(_runPath,
              Mathf.MoveToward((float)_animationTree.Get(_runPath), _runWeightTarget, (float)delta * AnimationSpeed));
        }
        public void UpdateAnimationTree(Vector3 direction)
        {
            if (direction.IsZeroApprox())
            {
                _animationTree.Set(_runPath, -1.0f);
                _runWeightTarget = -1.0f;

            }
            else
            {
                _animationTree.Set(_runPath, 1.0f);
                _runWeightTarget = 1.0f;
            }
        }

    }
}

