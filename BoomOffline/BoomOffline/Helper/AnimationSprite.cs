using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoomOffline.Helper
{
    class AnimationSprite
    {
        private Rectangle sourceRect;

        private Point[] frameLocations;
        private int currentFrameIndex;
        private int numOfFrame;

        public void Load(Texture2D mainTexture, int frameWidth, int frameHeight, int numOfFrame, int typeIndex)
        {
            this.numOfFrame = numOfFrame;
            int frameY = frameHeight*typeIndex;
            frameLocations = new Point[numOfFrame];
            for (int i = 0; i < numOfFrame; i++)
            {
                frameLocations[i] = new Point(i * frameWidth, frameY);
            }
            sourceRect = new Rectangle(0,0,frameWidth,frameHeight);
            sourceRect.Location = frameLocations[0];

        }

        public void Next()
        {
            currentFrameIndex++;
            if (currentFrameIndex == numOfFrame)
                currentFrameIndex = 0;
            sourceRect.Location = frameLocations[currentFrameIndex];
        }

        public Rectangle Frame
        {
            get
            {
                return sourceRect;
            }
        }

        public void BackToDefault()
        {
            currentFrameIndex = 0;
            sourceRect.Location = frameLocations[currentFrameIndex];
        }
    }
}
