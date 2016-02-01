using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace AtelierXNA
{
    
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SoundManager : Microsoft.Xna.Framework.GameComponent
    {
        SoundEffect soundEffect { get; set; }
        SoundEffectInstance soundEffectInstance;
        List<SoundEffectInstance> soundEffectInstanceList = new List<SoundEffectInstance>();
        Song song { get; set; }
        List<string> listeSound = new List<string>();
        bool estRentré = false;
        int index;
        bool test = false;

        RessourcesManager<SoundEffect> gestionnaireDeSoundEffect;
        RessourcesManager<Song> gestionnaireDeSong;

        public SoundManager(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            gestionnaireDeSoundEffect = Game.Services.GetService(typeof(RessourcesManager<SoundEffect>)) as RessourcesManager<SoundEffect>;
            gestionnaireDeSong = Game.Services.GetService(typeof(RessourcesManager<Song>)) as RessourcesManager<Song>;
            base.Initialize();
        }
        public void Play(string songName)
        {
            song = gestionnaireDeSong.Find(songName);
            if (MediaPlayer.State == MediaState.Paused)
                MediaPlayer.Resume();
            if(MediaPlayer.State == MediaState.Stopped)
                MediaPlayer.Play(song);
        }
        public void Play(string soundEffectName, bool isLooped,float volume)
        {
            soundEffect = gestionnaireDeSoundEffect.Find(soundEffectName);
            
            if (!(listeSound.Contains(soundEffectName)))
            {
                listeSound.Add(soundEffectName);
                soundEffectInstanceList.Add(soundEffect.CreateInstance());
            }
            for(int i = 0; i < listeSound.Count;++i)
            {
                test = string.Compare(listeSound[i], soundEffectName) == 0;
                if (test)
                {
                    index = i;
                    break;
                }

            }
            test = false;
            soundEffectInstance = soundEffectInstanceList[index];
            
            if (isLooped && !estRentré)
            {
                soundEffectInstance.IsLooped = true;
                estRentré = true;
            }
            soundEffectInstance.Volume = volume;
            if(!(soundEffectInstance.State == SoundState.Playing))
                soundEffectInstance.Play();
        }
        public void PauseSong(string songName)
        {
            song = gestionnaireDeSong.Find(songName);
            
            MediaPlayer.Pause();
        }
        public void PauseSoundEffect(string soundEffectName)
        {
            soundEffect = gestionnaireDeSoundEffect.Find(soundEffectName);
            if (!(listeSound.Contains(soundEffectName)))
            {
                listeSound.Add(soundEffectName);
                soundEffectInstanceList.Add(soundEffect.CreateInstance());
            }
            for (int i = 0; i < listeSound.Count; ++i)
            {
                test = string.Compare(listeSound[i], soundEffectName) == 0;
                if (test)
                {
                    index = i;
                    break;
                }

            }
            test = false;
            soundEffectInstance = soundEffectInstanceList[index];
            soundEffectInstance.Pause();
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}
