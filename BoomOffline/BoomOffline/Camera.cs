using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using BoomOffline.Helper;

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
            RoomSetting.Instance.PlusPosCam = new Vector2(0, 0);
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
            set 
            { 
                _pos = value;
                RoomSetting.Instance.PosCam = value;
            }
        }

        public Matrix GetTransformation()
        {
            Console.WriteLine("new pos: " + _pos.X + " " + _pos.Y);
            Console.WriteLine("old pos: " + RoomSetting.Instance.PosCam.X + " " + RoomSetting.Instance.PosCam.Y);
            Console.WriteLine("plus: " + RoomSetting.Instance.PlusPosCam.X + " " + RoomSetting.Instance.PlusPosCam.Y);

            int ratio = 2, limit = 200;
            if (RoomSetting.Instance.MapSize != 21)
            {
                if (_pos.X - RoomSetting.Instance.PosCam.X > 0 && RoomSetting.Instance.PlusPosCam.X <= limit)
                    RoomSetting.Instance.PlusPosCam = new Vector2(RoomSetting.Instance.PlusPosCam.X + ratio, RoomSetting.Instance.PlusPosCam.Y);
                else if (_pos.X - RoomSetting.Instance.PosCam.X < 0 && RoomSetting.Instance.PlusPosCam.X > 10)
                    RoomSetting.Instance.PlusPosCam = new Vector2(RoomSetting.Instance.PlusPosCam.X - ratio, RoomSetting.Instance.PlusPosCam.Y);
                else if (_pos.Y - RoomSetting.Instance.PosCam.Y > 0 && RoomSetting.Instance.PlusPosCam.Y <= limit)
                    RoomSetting.Instance.PlusPosCam = new Vector2(RoomSetting.Instance.PlusPosCam.X, RoomSetting.Instance.PlusPosCam.Y + ratio);
                else if (_pos.Y - RoomSetting.Instance.PosCam.Y < 0 && RoomSetting.Instance.PlusPosCam.Y > 10)
                    RoomSetting.Instance.PlusPosCam = new Vector2(RoomSetting.Instance.PlusPosCam.X, RoomSetting.Instance.PlusPosCam.Y - ratio);

                RoomSetting.Instance.PosCam = _pos;
            }
            var graphicsDevice = Resource.Global.Instance.Graphics;
            _transform =
              Matrix.CreateTranslation(new Vector3(-_pos.X + RoomSetting.Instance.PlusPosCam.X, -_pos.Y + RoomSetting.Instance.PlusPosCam.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));
            return _transform;
        }


        public void Update(GameTime gametime)
        {
            //var currentMouseState = Input.MouseEvent.Instance.currentMouseState;
            //var previousMouseState = Input.MouseEvent.Instance.previousState;

            //if (currentMouseState.LeftButton == ButtonState.Pressed)
            //{
            //    var offsetX = previousMouseState.X - currentMouseState.X;
            //    var offsetY = previousMouseState.Y - currentMouseState.Y;

            //    Vector2 movement = Vector2.Zero;

            //    movement.X += offsetX / Zoom;
            //    movement.Y += offsetY / Zoom;

            //    Move(movement);
            //}


            /*if (previousMouseState.ScrollWheelValue < currentMouseState.ScrollWheelValue)
                Zoom += 0.05f;
            else if (previousMouseState.ScrollWheelValue > currentMouseState.ScrollWheelValue)
                Zoom -= 0.05f;*/
        }
    }
}
