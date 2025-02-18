﻿using Microsoft.Xna.Framework;
using MonoGo.Engine;
using MonoGo.Engine.Cameras;
using MonoGo.Engine.EC;
using MonoGo.Engine.SceneSystem;
using MonoGo.Engine.Utils;

namespace MonoGo.Samples
{
    /// <summary>
    /// Controls the camera.
    /// </summary>
    public class CameraController : Entity
    {
        Camera MainCamera;

        float _cameraSpeed = 400;

        float _minZoom = 0.1f;
        float _maxZoom = 30f;

        float _zoomSpeed = 1;

        float _rotationSpeed = 120;

        public const Buttons UpButton = Buttons.NumPad8;
        public const Buttons DownButton = Buttons.NumPad5;
        public const Buttons LeftButton = Buttons.NumPad4;
        public const Buttons RightButton = Buttons.NumPad6;
        public const Buttons ZoomInButton = Buttons.Add;
        public const Buttons ZoomOutButton = Buttons.Subtract;
        public const Buttons RotateRightButton = Buttons.NumPad7;
        public const Buttons RotateLeftButton = Buttons.NumPad9;

        public CameraController(Camera camera) : base(SceneMgr.GUILayer)
        {
            MainCamera = camera;
            MainCamera.Offset = MainCamera.Size.ToVector3() / 2;
            Reset();
        }

        public override void Update()
        {
            base.Update();

            // Movement.
            var movementVector3 = new Vector3(
                Input.CheckButton(LeftButton).ToInt() - Input.CheckButton(RightButton).ToInt(),
                Input.CheckButton(UpButton).ToInt() - Input.CheckButton(DownButton).ToInt(),
                0
            );
            movementVector3 *= -1; // Invert the movement vector for a more natural "camera-experience".
            movementVector3 = Vector3.Transform(
                movementVector3,
                Matrix.CreateRotationZ(MainCamera.Rotation.RadiansF)
            ); // Rotating by the camera's rotation, so camera will always move relatively to screen. 

            var rotatedMovementVector = new Vector2(movementVector3.X, movementVector3.Y);

            MainCamera.Position += TimeKeeper.Global.Time(_cameraSpeed / MainCamera.Zoom) * rotatedMovementVector.ToVector3();
            // Movement.

            // Zoom.
            var zoomDirection = Input.CheckButton(ZoomInButton).ToInt() - Input.CheckButton(ZoomOutButton).ToInt();
            MainCamera.Zoom += TimeKeeper.Global.Time(_zoomSpeed) * zoomDirection;

            if (MainCamera.Zoom < _minZoom)
            {
                MainCamera.Zoom = _minZoom;
            }
            if (MainCamera.Zoom > _maxZoom)
            {
                MainCamera.Zoom = _maxZoom;
            }
            // Zoom.

            // Rotation.
            var rotationDirection = Input.CheckButton(RotateLeftButton).ToInt() - Input.CheckButton(RotateRightButton).ToInt();
            MainCamera.Rotation += TimeKeeper.Global.Time(_rotationSpeed) * rotationDirection;
            // Rotation.			
        }

        public void Reset()
        {
            MainCamera.Zoom = 1;
            MainCamera.Rotation = Angle.Right;
            MainCamera.Position = MainCamera.Offset;
        }
    }
}