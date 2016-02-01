using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class Caméra : Microsoft.Xna.Framework.GameComponent
    {
        #region Attribues
        string musicName;
        string nomJumpSon;
        string nomLandingSon;
        string nomMarche;
        string nomMarcheLente;
        const float RAYON_COLLISION = 1f;
        Vector3 cameraPosition;
        Vector3 cameraRotation;
        float cameraSpeed;
        Vector3 cameraLookAt;
        Vector3 cameraLookAtInit;
        Vector3 gamePadRotationBuffer;
        GamePadState currentGamePad;
        GamePadState prevGamePad;
        const float DEFAULT_JUMP_SPEED = 1f;
        float jumpSpeed = 0f;
        const float GRAVITY = -1f;
        const float STADE_MARCHE_VITE = 0.73f;
        bool canJump = true;
        bool remonte = false;
        float cameraPositionInit;
        bool walkingSound = true;
        public BoundingSphere ZoneCollision { get; set; }
        SoundManager GestionSounds { get; set; }
        InputManager GestionInput { get; set; }
        Vector3 rotationInitiale;
        #endregion

        #region Constructeur
        public Caméra(Game game, Vector3 position, Vector3 rotation, float speed, string jumpSoundName, string landingSoundName, string nomWalkName, string nomMusique, string nomMarcheSlow)
            : base(game)
        {
            musicName = nomMusique;
            nomJumpSon = jumpSoundName;
            nomLandingSon = landingSoundName;
            nomMarche = nomWalkName;
            nomMarcheLente = nomMarcheSlow;
            cameraPositionInit = position.Y;
            cameraSpeed = speed;
            rotationInitiale = rotation;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                Game.GraphicsDevice.Viewport.AspectRatio, 0.05f, 1000.0f);
            MoveTo(position, rotation);
            prevGamePad = GamePad.GetState(PlayerIndex.One);
        }
        private void MoveTo(Vector3 pos, Vector3 rot)
        {
            Rotation = rot;
            Position = pos;
           
        }
        #endregion

        #region Propriétés
        public Vector3 Position
        {
            get { return cameraPosition; }
            set
            {
                cameraPosition = value;
                UpdateLookAt();
            }
        }
        public Vector3 Rotation
        {
            get { return cameraRotation; }
            set
            {

                cameraRotation = value;

                UpdateLookAt();

            }
        }
        public Matrix Projection
        {
            get;
            protected set;
        }

        public Matrix View
        {
            get
            {
                return Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
            }
        }
        #endregion

        #region UpdateLookAt
        private void UpdateLookAt()
        {
            Matrix rotationMatrix = Matrix.CreateRotationX(cameraRotation.X) * Matrix.CreateRotationY(cameraRotation.Y);
            Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
            cameraLookAt = cameraPosition + lookAtOffset;
            cameraLookAtInit = cameraLookAt;
        }
        #endregion

        #region Initialize
        
        public override void Initialize()
        {
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionSounds = Game.Services.GetService(typeof(SoundManager)) as SoundManager;

            base.Initialize();
        }
        #endregion

        #region PreviewMove
        private Vector3 PreviewMove(Vector3 amount)
        {
            Matrix rotate = Matrix.CreateRotationY(cameraRotation.Y);
            Vector3 movement = new Vector3(amount.X, amount.Y, amount.Z);
            movement = Vector3.Transform(movement, rotate);
            foreach (GameComponent c in Game.Components) 
            { 
                if(EstEnCollision(c,cameraPosition+movement))
                {
                    walkingSound = false;
                    return cameraPosition;
                }
            }
            walkingSound = true;

            return cameraPosition + movement;
        }
        #endregion

        #region Move
        private void Move(Vector3 scale)
        {
            MoveTo(PreviewMove(scale), Rotation);
        }
        #endregion

        #region Update
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            currentGamePad = GamePad.GetState(PlayerIndex.One);
            KeyboardState ks = Keyboard.GetState();
            Vector3 moveVector = Vector3.Zero;


            if (GestionInput.EstManetteActivé)
            {
                GérerManette(dt);

            }
            else
            {
                GestionSounds.PauseSoundEffect(nomMarcheLente);
                GestionSounds.PauseSoundEffect(nomMarche);
            }

            if (!canJump)
            {
                jumpSpeed += dt * GRAVITY;
                cameraPosition.Y += jumpSpeed;
                cameraLookAt.Y += jumpSpeed;
            }
            if (cameraPosition.Y < cameraPositionInit - 1)
            {
                jumpSpeed = 0.05f;
                remonte = true;
            }

            if (remonte)
            {
                GestionSounds.Play(nomLandingSon, false, 0.1f);
                jumpSpeed -= (dt * GRAVITY);
                cameraPosition.Y += jumpSpeed;
                cameraLookAt.Y += jumpSpeed;

                if (cameraPosition.Y > cameraPositionInit)
                {
                    float ajusterVue = cameraLookAt.Y - cameraPosition.Y;
                    cameraPosition.Y = cameraPositionInit;
                    cameraLookAt.Y = ajusterVue + cameraPosition.Y;
                    canJump = true;
                    remonte = false;
                }
               
            }
            base.Update(gameTime);
        }
        #endregion

        #region GérerManette
        void GérerManette(float dt)
        {

            float deltaX, deltaY;
            Vector3 moveVector = Vector3.Zero;

            //Gérer les sauts
            if ((GestionInput.EstBoutonEnfoncée(Buttons.A)) && canJump)
            {
                GestionSounds.PauseSoundEffect(nomMarche);
                GestionSounds.Play(nomJumpSon, false, 0.4f);
                jumpSpeed = DEFAULT_JUMP_SPEED;
                canJump = false;
            }
            //Gérer les déplacements du joystick droit
            cameraLookAt.Y += GérerTouche(Buttons.RightThumbstickUp) - GérerTouche(Buttons.RightThumbstickDown);
            cameraLookAt.X += GérerTouche(Buttons.RightThumbstickRight) - GérerTouche(Buttons.RightThumbstickLeft);
            deltaY = cameraLookAt.Y - (Game.GraphicsDevice.Viewport.Height / 2);
            deltaX = cameraLookAt.X - (Game.GraphicsDevice.Viewport.Width / 2);
            gamePadRotationBuffer.Y += GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y * 0.01f * deltaY * dt;
            gamePadRotationBuffer.X += GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X * 0.01f * deltaX * dt;
            

            //Déplacement de la caméra avec le joystick gauche
            moveVector.Z = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y;
            moveVector.X = -GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X;
            
            //Fait déplacer le Joueur
            if (moveVector != Vector3.Zero)
            {
                if(!walkingSound)
                    GestionSounds.PauseSoundEffect(nomMarche);
                if (canJump && walkingSound && (moveVector.X > STADE_MARCHE_VITE || moveVector.X < -STADE_MARCHE_VITE 
                                            || moveVector.Z > STADE_MARCHE_VITE || moveVector.Z < -STADE_MARCHE_VITE))
                {
                    GestionSounds.PauseSoundEffect(nomMarcheLente);
                    GestionSounds.Play(nomMarche, true, 0.25f);
                }
                else if (canJump && walkingSound)
                {
                    GestionSounds.PauseSoundEffect(nomMarche);
                    GestionSounds.Play(nomMarcheLente, true, 0.25f);
                }


                //moveVector.Normalize();
                moveVector *= dt * cameraSpeed;
                Move(moveVector);
            }

            if (gamePadRotationBuffer.Y < MathHelper.ToRadians(-75.0f))
                gamePadRotationBuffer.Y = gamePadRotationBuffer.Y - (gamePadRotationBuffer.Y - MathHelper.ToRadians(-75.0f));

            if (gamePadRotationBuffer.Y > MathHelper.ToRadians(75.0f))
                gamePadRotationBuffer.Y = gamePadRotationBuffer.Y - (gamePadRotationBuffer.Y - MathHelper.ToRadians(75.0f));

            Rotation = new Vector3(MathHelper.Clamp(gamePadRotationBuffer.Y+rotationInitiale.X, MathHelper.ToRadians(-75.0f), MathHelper.ToRadians(75.0f)),
                MathHelper.WrapAngle(gamePadRotationBuffer.X+rotationInitiale.Y), 0);


            deltaX = 0;
            deltaY = 0;
        }
        #endregion

        #region GérerTouche

        private int GérerTouche(Buttons b)
        {
            return GestionInput.EstBoutonEnfoncée(b) ? 1 : 0;
        }
        #endregion

        public bool EstEnCollision(object autreObjet, Vector3 camPos)
        {
            bool estEnCollision = false;
            ZoneCollision = new BoundingSphere(camPos, 0.5f);

            if (autreObjet is CubeColoré)
            {
                CubeColoré mur = autreObjet as CubeColoré;
                estEnCollision = ZoneCollision.Intersects(mur.ZoneCollision);
            }

            //if (autreObjet is Caméra)
            //{
            //    Caméra joueur = autreObjet as Caméra;
            //    Vector3 distance = this.ZoneCollision.Center - joueur.ZoneCollision.Center;
            //    float normeDist = distance.Length();
            //    float somme = this.ZoneCollision.Radius + joueur.ZoneCollision.Radius;

            //    estEnCollision = somme > normeDist;
            //}

            //if (autreObjet is MONSTRE)
            //{

            //CaméraSubjective joueur = autreObjet as CaméraSubjective;
            //Vector3 distance = this.ZoneCollision.Center - joueur.ZoneCollision.Center;
            //float normeDist = distance.Length();
            //float somme = this.ZoneCollision.Radius + joueur.ZoneCollision.Radius;

            //estEnCollision =  somme > normeDist;
            //}
            return estEnCollision;
        }
    }
}
