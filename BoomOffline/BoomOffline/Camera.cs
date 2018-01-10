using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoomOffline
{
    class Camera
    {
        protected float _zoom; // Camera Zoom
        public Matrix _transform; // Matrix Transform
        public Vector2 startPos;
        public Vector2 _pos; // Camera Position
        protected float _rotation; // Camera Rotation

        public Vector2 Offset
        {
            get { return new Vector2(_pos.X - startPos.X, _pos.Y - startPos.Y); }
        }
        public Camera()
        {
            var graphicsDevice = Resource.Global.Instance.Graphics;
            _zoom = 1.0f;
            _rotation = 0.0f;
            startPos = new Vector2(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
            _pos = new Vector2(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
        }

        public float Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;
            } // Negative zoom will flip image
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        // Auxiliary function to move the camera
        public void Move(Vector2 amount)
        {
            _pos += amount;
        }
        // Get set position
        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public Matrix GetTransformation()
        {
            var graphicsDevice = Resource.Global.Instance.Graphics;
            _transform =       // Thanks to o KB o for this solution
              Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));
            return _transform;
        }


        public void Update(GameTime gametime)
        {
            var currentMouseState = Input.MouseEvent.Instance.currentMouseState;
            var previousMouseState = Input.MouseEvent.Instance.previousState;

            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                var offsetX = previousMouseState.X - currentMouseState.X;
                var offsetY = previousMouseState.Y - currentMouseState.Y;

                Vector2 movement = Vector2.Zero;

                movement.X += offsetX / Zoom;
                movement.Y += offsetY / Zoom;

                //Move(movement);
            }


            /*if (previousMouseState.ScrollWheelValue < currentMouseState.ScrollWheelValue)
                Zoom += 0.05f;
            else if (previousMouseState.ScrollWheelValue > currentMouseState.ScrollWheelValue)
                Zoom -= 0.05f;*/
        }
    }
}
