using Godot;
using System;
using System.Diagnostics;

namespace BattleRPG3D.Scripts.Characters
{
    public partial class Player : CharacterBody3D
    {
        public const float Speed = 5.0f;
        public const float JumpVelocity = 4.5f;
        [Export(PropertyHint.Range, "0,0.01,0.000001")]
        public float MouseSensitivity = 0.00075f;
        [Export]
        public float MinBoundary = -60;
        [Export]
        public float MaxBoundary = 10;
        [Export]
        public float AnimationDecay = 20.0f;
        private Node3D _horizontalPivot;
        private Node3D _verticalPivot;
        private SpringArm3D _springArm3D;
        private Rig _rig;

        private Node3D _rigPivot;
        private Vector2 _look = Vector2.Zero;
        public override void _Ready()
        {
            base._Ready();
            _horizontalPivot = GetNode<Node3D>("HorizontalPivot");
            _verticalPivot = GetNode<Node3D>("HorizontalPivot/VerticalPivot");
            _springArm3D = GetNode<SpringArm3D>("SmoothCameraArm");
            _rigPivot = GetNode<Node3D>("RigPivot");
            _rig = GetNode<Rig>("RigPivot/Rig");

            //进入游戏时不显示鼠标
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
        public override void _PhysicsProcess(double delta)
        {
            FrameCameraRotation();
            var velocity = Velocity;

            // Add the gravity.
            if (!IsOnFloor())
            {
                velocity += GetGravity() * (float)delta;
            }

            // Handle Jump.
            if (Input.IsActionJustPressed("jump") && IsOnFloor())
            {
                velocity.Y = JumpVelocity;
            }
            
            var direction = GetMovementDirection();
            _rig.UpdateAnimationTree(direction);
            if (direction != Vector3.Zero)
            {
                velocity.X = direction.X * Speed;
                velocity.Z = direction.Z * Speed;
                LookTowardDirection(direction, (float)delta);
            }
            else
            {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
                velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
            }

            Velocity = velocity;
            MoveAndSlide();
        }
        public override void _UnhandledInput(InputEvent @event)
        {
            base._UnhandledInput(@event);
            //如果按下了ESC退出键，显示鼠标
            if (@event.IsActionPressed("ui_cancel"))
            {
                Input.MouseMode = Input.MouseModeEnum.Visible;
            }
            //如果鼠标处于隐藏状态，则鼠标左右移动可以对应的旋转视角
            if (Input.MouseMode != Input.MouseModeEnum.Captured) return;
            if (@event is not InputEventMouseMotion mouseMotion) return;
            //获取鼠标的相对位置并且乘以灵敏度来获取视角旋转的角度
            _look += -mouseMotion.Relative * MouseSensitivity;
            GD.Print(_look);
        }

        private Vector3 GetMovementDirection()
        {
            // Get the input direction and handle the movement/deceleration.
            //读取输入的方向并且控制移动
            // As good practice, you should replace UI actions with custom gameplay actions.
            var inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
            //这里的 Normalized() 会把 inputDir 生成的三维向量归一化
            var inputVector = new Vector3(inputDir.X, 0, inputDir.Y).Normalized();
            return _horizontalPivot.GlobalTransform.Basis * inputVector;
        }

        private void FrameCameraRotation()
        {
            _horizontalPivot.RotateY(_look.X);
            _verticalPivot.RotateX(_look.Y);
            var xRadian = Mathf.Clamp(_verticalPivot.Rotation.X,
                Mathf.DegToRad(MinBoundary),
                Mathf.DegToRad(MaxBoundary));
            _verticalPivot.Rotation = new Vector3(xRadian, _verticalPivot.Rotation.Y, _verticalPivot.Rotation.Z);
            _look = Vector2.Zero;
        }

        private void LookTowardDirection(Vector3 direction, float delta)
        {
            var targetTransform = _rigPivot.GlobalTransform.LookingAt(
                _rigPivot.GlobalPosition + direction,Vector3.Up, true
            );

            var transformWeight = 1.0f - MathF.Exp((float)(-AnimationDecay * delta));
            _rigPivot.GlobalTransform = _rigPivot.GlobalTransform.InterpolateWith(targetTransform, transformWeight);

        }
    }
}

