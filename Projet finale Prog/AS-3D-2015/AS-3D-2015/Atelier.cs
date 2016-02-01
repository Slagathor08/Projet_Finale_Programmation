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
   public class Atelier : Microsoft.Xna.Framework.Game
   {
      const float INTERVALLE_CALCUL_FPS = 1f;
      const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
      GraphicsDeviceManager PériphériqueGraphique { get; set; }
      SpriteBatch GestionSprites { get; set; }

      RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
      RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
      RessourcesManager<Model> GestionnaireDeModèles { get; set; }
      RessourcesManager<Effect> GestionnaireDeShaders { get; set; }
      RessourcesManager<SoundEffect> GestionnaireDeSoundEffect { get; set; }
      RessourcesManager<Song> GestionnaireDeSong { get; set; }
      Caméra CaméraJeu { get; set; }
      List<GameComponent> ListeMurs { get; set; }

      public InputManager GestionInput { get; private set; }
      public SoundManager GestionSounds { get; private set; }

      public Atelier()
      {
         PériphériqueGraphique = new GraphicsDeviceManager(this);
         PériphériqueGraphique.IsFullScreen = true;
         Content.RootDirectory = "Content";
         PériphériqueGraphique.SynchronizeWithVerticalRetrace = false;
         IsFixedTimeStep = false;
         IsMouseVisible = false;
      }

      protected override void Initialize()
      {
         const int DIMENSION_TERRAIN = 256;
         Vector2 étenduePlan = new Vector2(DIMENSION_TERRAIN, DIMENSION_TERRAIN);
         Vector2 charpentePlan = new Vector2(4, 3);
         Vector3 positionCaméra = new Vector3(0, 20, 125);
         Vector3 cibleCaméra = new Vector3(0, 0, 0);
         Vector3 positionARC170 = new Vector3(25, 15, 0);
         Vector3 positionBiplan = new Vector3(20, 10, 0);
         Vector3 positionFeisar = new Vector3(20, 15, 20);
         Vector3 positionCylindre1 = new Vector3(-90, 10, -90);
         Vector3 positionCylindre2 = new Vector3(-20, 10, -20);
         Vector3 positionCylindre3 = new Vector3(-90, 10, 90);
         ListeMurs = new List<GameComponent>();

         GestionnaireDeFonts = new RessourcesManager<SpriteFont>(this, "Fonts");
         GestionnaireDeTextures = new RessourcesManager<Texture2D>(this, "Textures");
         GestionnaireDeModèles = new RessourcesManager<Model>(this, "Models");
         GestionnaireDeShaders = new RessourcesManager<Effect>(this, "Effects");
         GestionnaireDeSoundEffect = new RessourcesManager<SoundEffect>(this, "Sounds");
         GestionnaireDeSong = new RessourcesManager<Song>(this, "Songs");
         GestionInput = new InputManager(this);
         GestionSounds = new SoundManager(this);
         CaméraJeu = new Caméra(this, new Vector3(-105f, 0f, 4f), new Vector3(0f,(float)Math.PI/2f,0f), 50f, "jump", "landing","walk", "backgroundMusic", "walk_slow");

         Components.Add(GestionSounds);
         Components.Add(GestionInput);
         Components.Add(CaméraJeu);

         Components.Add(new Afficheur3D(this));

         Components.Add(new Carte(this, new Vector3(DIMENSION_TERRAIN, 25, DIMENSION_TERRAIN), "Terrain"));

         //Components.Add(new PlanTexturé(this, 1f, new Vector3(0, MathHelper.PiOver2, 0), new Vector3(-DIMENSION_TERRAIN / 2, DIMENSION_TERRAIN / 2, 0), étenduePlan, charpentePlan, "CielGauche", INTERVALLE_MAJ_STANDARD));
         //Components.Add(new PlanTexturé(this, 1f, new Vector3(0, -MathHelper.PiOver2, 0), new Vector3(DIMENSION_TERRAIN / 2, DIMENSION_TERRAIN / 2, 0), étenduePlan, charpentePlan, "CielDroite", INTERVALLE_MAJ_STANDARD));
         //Components.Add(new PlanTexturé(this, 1f, Vector3.Zero, new Vector3(0, DIMENSION_TERRAIN / 2, -DIMENSION_TERRAIN / 2), étenduePlan, charpentePlan, "CielAvant", INTERVALLE_MAJ_STANDARD));
         //Components.Add(new PlanTexturé(this, 1f, new Vector3(0, -MathHelper.Pi, 0), new Vector3(0, DIMENSION_TERRAIN / 2, DIMENSION_TERRAIN / 2), étenduePlan, charpentePlan, "CielArrière", INTERVALLE_MAJ_STANDARD));
         //Components.Add(new PlanTexturé(this, 1f, new Vector3(MathHelper.PiOver2, 0, 0), new Vector3(0, DIMENSION_TERRAIN - 1, 0), étenduePlan, charpentePlan, "CielDessus", INTERVALLE_MAJ_STANDARD));
         
         Components.Add(new Cylindre(this, 1f, new Vector3(0, MathHelper.PiOver2, 0), positionCylindre1, new Vector2(5f, 10f), new Vector2(30, 30), "old_brick_01", INTERVALLE_MAJ_STANDARD));
         
         Services.AddService(typeof(Random), new Random());
         Services.AddService(typeof(RessourcesManager<SpriteFont>), GestionnaireDeFonts);
         Services.AddService(typeof(RessourcesManager<Texture2D>), GestionnaireDeTextures);
         Services.AddService(typeof(RessourcesManager<Model>), GestionnaireDeModèles);
         Services.AddService(typeof(RessourcesManager<Effect>), GestionnaireDeShaders);
         Services.AddService(typeof(RessourcesManager<SoundEffect>), GestionnaireDeSoundEffect);
         Services.AddService(typeof(RessourcesManager<Song>), GestionnaireDeSong);
         Services.AddService(typeof(InputManager), GestionInput);
         Services.AddService(typeof(SoundManager), GestionSounds);
         Services.AddService(typeof(Caméra), CaméraJeu);
         GestionSprites = new SpriteBatch(GraphicsDevice);
         Services.AddService(typeof(SpriteBatch), GestionSprites);

         base.Initialize();
         GestionSounds.Play("backgroundMusic");
         //GestionSounds.Play("First_Sentence", false, 0.5f);

      }

      protected override void Update(GameTime gameTime)
      {
           if (GestionInput.EstEnfoncée(Keys.Escape))
         {
            Exit();
         }
         base.Update(gameTime);
      }
      

      protected override void Draw(GameTime gameTime)
      {
         GraphicsDevice.Clear(Color.Black);
         base.Draw(gameTime);
      }
   }
}





