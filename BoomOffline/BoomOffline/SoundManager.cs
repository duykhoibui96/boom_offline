using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using BoomOffline.Resource;
using Microsoft.Xna.Framework.Audio;

namespace BoomOffline.Sound
{
    class SoundManager
    {
        public enum SoundType
        {
            Click,
            Explosion,
            Win,
            Lose
        }

        private static SoundManager instance;
        private float soundVolumn;
        private SoundEffectInstance background;

        public static SoundManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new SoundManager();
                return instance;
            }
            set { instance = value; }
        }

        private SoundManager()
        {
            soundVolumn = 1;
            background = ResManager.Instance.BackgroundMusic.CreateInstance();
            background.Volume = 0.5f;
            background.IsLooped = true;
        }

        public void PlayBackgroundMusic()
        {
            background.Play();
        }

        public void PlaySound(SoundType type)
        {
            var res = ResManager.Instance;
            SoundEffect effect = res.Click;
            switch (type)
            {
               case SoundType.Explosion:
                    effect = res.BombExplosion;
                    break;
               case SoundType.Win:
                    effect = res.Win;
                    break;
               case SoundType.Lose:
                    effect = res.Lose;
                    break;
            }

            var instance = effect.CreateInstance();
            instance.Volume = soundVolumn;
            instance.Play();
        }
    }
}
