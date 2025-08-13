using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleRPG3D.Scripts.Characters
{
    public partial class SmoothCameraArm:SpringArm3D
    {
        [Export]
        public Node3D Target;
        [Export]
        public float Decay = 10.0f;

        public override void _PhysicsProcess(double delta)
        {
            var transformWeight = 1.0f - MathF.Exp((float)(-Decay * delta));
            GlobalTransform = GlobalTransform.InterpolateWith(Target.GlobalTransform, transformWeight);
        }
    }
}
