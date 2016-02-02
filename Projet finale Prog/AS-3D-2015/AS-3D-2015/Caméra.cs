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
    public class Cam�ra : Microsoft.Xna.Framework.GameComponent
    {
        #region Attributs
        const float RAYON_COLLISION = 0.5f;
        const float DEFAULT_JUMP_SPEED = 1f;
        const float ASCEND_JUMP_SPEED = 0.5f;
        const float GRAVITY = -1f;
        const float STADE_MARCHE_VITE = 0.73f;
        const float NEAR_PLANE_DISTANCE = 0.05f;
        const float FAR_PLANE_DISTANCE = 1000f;
        const float VOLUME_FAIBLE = 0.1f;
        const float VOLUME_MOYEN = 0.25f;
        const float VOLUME_FORT = 0.4f;
        const float ANGLE_ROTATION = 75f;


        string MusicName { get; set; }
        string NomJumpSon { get; set; }
        string NomLandingSon { get; set; }
        string NomMarche { get; set; }
        string NomMarcheLente { get; set; }

        float VitesseCamera { get; set; }
        float VitesseSaut { get; set; }
        float PositionInitialeCamera { get; set; }

        bool PeutSauter { get; set; }
        bool Remonte { get; set; }
        bool SonDeMarcheActiv� { get; set; }

        Vector3 PositionCamera;
        public Vector3 Position
        {
            get { return PositionCamera; }
            set
            {
                PositionCamera = value;
                UpdateLookAt();
            }
        }

        Vector3 RotationCamera;
        public Vector3 Rotation
        {
            get { return RotationCamera; }
            set
            {
                RotationCamera = value;
                UpdateLookAt();
            }
        }

        Vector3 CameraLookAt;
        Vector3 CameraLookAtInitial { get; set; }
        Vector3 GamePadRotationBuffer;
        Vector3 RotationInitiale { get; set; }

        GamePadState CurrentGamePad { get; set; }
        GamePadState PrevGamePad { get; set; }

        SoundManager GestionSons { get; set; }
        InputManager GestionInput { get; set; }
        CollisionManager GestionCollisions { get; set; }

        public BoundingSphere ZoneCollision { get; set; }

        public Matrix Projection { get; set; }
        public Matrix Vue
        {
            get
            {
                return Matrix.CreateLookAt(PositionCamera, CameraLookAt, Vector3.Up);
            }
        }
        #endregion


        #region Constructeur
        public Cam�ra(Game game, Vector3 position, Vector3 rotation, float vitesse, string nomSonSaut, string nomSonAtterissage, string nomSonMarche, string nomMusique, string nomMarcheLente)
            : base(game)
        {

            MusicName = nomMusique;
            NomJumpSon = nomSonSaut;
            NomLandingSon = nomSonAtterissage;
            NomMarche = nomSonMarche;
            NomMarcheLente = nomMarcheLente;
            PositionInitialeCamera = position.Y;
            VitesseCamera = vitesse;
            RotationInitiale = rotation;

            Rotation = rotation;
            Position = position;

        }
        #endregion


        #region UpdateLookAt
        private void UpdateLookAt()
        {
            Matrix rotationMatrix = Matrix.CreateRotationX(RotationCamera.X) * Matrix.CreateRotationY(RotationCamera.Y);
            Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
            CameraLookAt = PositionCamera + lookAtOffset;
            CameraLookAtInitial = CameraLookAt;
        }
        #endregion


        public override void Initialize()
        {
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionSons = Game.Services.GetService(typeof(SoundManager)) as SoundManager;
            GestionCollisions = Game.Services.GetService(typeof(CollisionManager)) as CollisionManager;
            PeutSauter = true;
            SonDeMarcheActiv� = true;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                         Game.GraphicsDevice.Viewport.AspectRatio, NEAR_PLANE_DISTANCE, FAR_PLANE_DISTANCE);
            PrevGamePad = GamePad.GetState(PlayerIndex.One);

            base.Initialize();
        }

        private Vector3 PreviewMove(Vector3 prochainMouvement)
        {
            Matrix rotate = Matrix.CreateRotationY(RotationCamera.Y);
            Vector3 mouvement = new Vector3(prochainMouvement.X, prochainMouvement.Y, prochainMouvement.Z);
            mouvement = Vector3.Transform(mouvement, rotate);

            foreach (GameComponent c in Game.Components)
            {
                if (EstEnCollision(c, new Vector3(PositionCamera.X + mouvement.X, PositionCamera.Y, PositionCamera.Z)))
                {
                    Position = new Vector3(Position.X - mouvement.X, Position.Y, Position.Z);
                }
                if (EstEnCollision(c, new Vector3(PositionCamera.X, PositionCamera.Y, PositionCamera.Z + mouvement.Z)))
                {
                    Position = new Vector3(Position.X, Position.Y, Position.Z - mouvement.Z);
                }
            }
            SonDeMarcheActiv� = true;
            return Position += mouvement;
        }

        private void Bouger(Vector3 scale)
        {
            PreviewMove(scale);
        }


        public override void Update(GameTime gameTime)
        {
            float Temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            CurrentGamePad = GamePad.GetState(PlayerIndex.One);
            KeyboardState �tatClavier = Keyboard.GetState();
            Vector3 moveVector = Vector3.Zero;


            if (GestionInput.EstManetteActiv�e)
            {
                G�rerManette(Temps�coul�);
            }

            else
            {
                GestionSons.PauseSoundEffect(NomMarcheLente);
                GestionSons.PauseSoundEffect(NomMarche);
            }

            if (!PeutSauter)
            {
                VitesseSaut += Temps�coul� * GRAVITY;
                PositionCamera.Y += VitesseSaut;
                CameraLookAt.Y += VitesseSaut;
            }

            if (PositionCamera.Y < PositionInitialeCamera - 1)
            {
                VitesseSaut = ASCEND_JUMP_SPEED;
                Remonte = true;
            }

            if (Remonte)
            {
                GestionSons.Play(NomLandingSon, false, VOLUME_FAIBLE);
                VitesseSaut -= (Temps�coul� * GRAVITY);
                PositionCamera.Y += VitesseSaut;
                CameraLookAt.Y += VitesseSaut;

                if (PositionCamera.Y > PositionInitialeCamera)
                {
                    float ajusterVue = CameraLookAt.Y - PositionCamera.Y;
                    PositionCamera.Y = PositionInitialeCamera;
                    CameraLookAt.Y = ajusterVue + PositionCamera.Y;
                    PeutSauter = true;
                    Remonte = false;
                }
            }

            base.Update(gameTime);
        }

        #region G�rerManette
        void G�rerManette(float temps�coul�)
        {
            float deltaX, deltaY;
            Vector3 moveVector = Vector3.Zero;

            //G�rer les sauts
            if ((GestionInput.EstBoutonEnfonc�e(Buttons.A)) && PeutSauter)
            {
                GestionSons.PauseSoundEffect(NomMarche);
                GestionSons.Play(NomJumpSon, false, VOLUME_MOYEN);
                VitesseSaut = DEFAULT_JUMP_SPEED;
                PeutSauter = false;
            }
            //G�rer les d�placements du joystick droit
            CameraLookAt.Y += G�rerTouche(Buttons.RightThumbstickUp) - G�rerTouche(Buttons.RightThumbstickDown);
            CameraLookAt.X += G�rerTouche(Buttons.RightThumbstickRight) - G�rerTouche(Buttons.RightThumbstickLeft);
            deltaY = CameraLookAt.Y - (Game.GraphicsDevice.Viewport.Height / 2f);
            deltaX = CameraLookAt.X - (Game.GraphicsDevice.Viewport.Width / 2f);
            GamePadRotationBuffer.Y += GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y * 0.01f * deltaY * temps�coul�;
            GamePadRotationBuffer.X += GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X * 0.01f * deltaX * temps�coul�;


            //D�placement de la cam�ra avec le joystick gauche
            moveVector.Z = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y;
            moveVector.X = -GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X;

            //Fait d�placer le Joueur
            if (moveVector != Vector3.Zero)
            {
                if (!SonDeMarcheActiv�)
                    GestionSons.PauseSoundEffect(NomMarche);
                if (PeutSauter && SonDeMarcheActiv� && (moveVector.X > STADE_MARCHE_VITE || moveVector.X < -STADE_MARCHE_VITE
                                            || moveVector.Z > STADE_MARCHE_VITE || moveVector.Z < -STADE_MARCHE_VITE))
                {
                    GestionSons.PauseSoundEffect(NomMarcheLente);
                    GestionSons.Play(NomMarche, true, VOLUME_MOYEN);
                }
                else if (PeutSauter && SonDeMarcheActiv�)
                {
                    GestionSons.PauseSoundEffect(NomMarche);
                    GestionSons.Play(NomMarcheLente, true, VOLUME_MOYEN);
                }

                moveVector *= temps�coul� * VitesseCamera;
                Bouger(moveVector);
            }

            if (GamePadRotationBuffer.Y < MathHelper.ToRadians(-ANGLE_ROTATION))
                GamePadRotationBuffer.Y = GamePadRotationBuffer.Y - (GamePadRotationBuffer.Y - MathHelper.ToRadians(-ANGLE_ROTATION));

            if (GamePadRotationBuffer.Y > MathHelper.ToRadians(ANGLE_ROTATION))
                GamePadRotationBuffer.Y = GamePadRotationBuffer.Y - (GamePadRotationBuffer.Y - MathHelper.ToRadians(ANGLE_ROTATION));

            Rotation = new Vector3(MathHelper.Clamp(GamePadRotationBuffer.Y + RotationInitiale.X, MathHelper.ToRadians(-ANGLE_ROTATION), MathHelper.ToRadians(ANGLE_ROTATION)),
                MathHelper.WrapAngle(GamePadRotationBuffer.X + RotationInitiale.Y), 0);


            deltaX = 0;
            deltaY = 0;
        }
        #endregion


        #region G�rerTouche

        private int G�rerTouche(Buttons b)
        {
            return GestionInput.EstBoutonEnfonc�e(b) ? 1 : 0;
        }
        #endregion

        public bool EstEnCollision(object autreObjet, Vector3 camPos)
        {
            bool estEnCollision = false;
            ZoneCollision = new BoundingSphere(camPos, RAYON_COLLISION);

            if (autreObjet is CubeColor�)
            {
                CubeColor� mur = autreObjet as CubeColor�;
                estEnCollision = GestionCollisions.CollisionJoueurMur(this, mur);

            }

            //if (autreObjet is Cam�ra)
            //{
            //    Cam�ra joueur = autreObjet as Cam�ra;
            //    GestionCollisions.CollisionJoueurs(this, joueur);
            //}

            //if (autreObjet is MONSTRE)
            //{

            //FuturZombie zombie = autreObjet as FuturZombie
            //GestionCollisions.CollisionJoueurMonstre(this, zombie);
            //}

            return estEnCollision;
        }
    }
}
