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

        private float backupMusicVolumn;
        private float backupSoundVolumn;

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

        public float MusicVolumn
        {
            get { return background.Volume; }
            set { background.Volume = value; }
        }

        public float SoundVolumn
        {
            get
            {
                return soundVolumn;
                
            }
            set { soundVolumn = value; }
        }

        private SoundManager()
        {
            soundVolumn = 1;
            background = ResManager.Instance.BackgroundMusic.CreateInstance();
            background.Volume = 0.2f;
            background.IsLooped = true;
        }

        public void PlayBackgroundMusic()
        {
            background.Play();
        }

        public void Backup()
        {
            backupMusicVolumn = background.Volume;
            backupSoundVolumn = soundVolumn;
        }

        public void Recover()
        {
            background.Volume = backupMusicVolumn;
            soundVolumn = backupSoundVolumn;
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
