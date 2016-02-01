using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AtelierXNA
{
    class Cylindre : PrimitiveDeBaseAnimée
    {
        const int DIVISION_MOITIÉ = 2;
        string NomTexture { get; set; }
        RessourcesManager<Texture2D> gestionnaireDeTextures;
        Texture2D image;
        int NbTrianglesParStrip { get; set; }
        Vector3[,] PtsSommets { get; set; }
        Vector3 Origine { get; set; }
        int NbColonnes { get; set; }
        int NbRangées { get; set; }
        Vector2 Delta { get; set; }
        Vector2[,] PtsTexture { get; set; }
        VertexPositionTexture[] Sommets { get; set; }
        BasicEffect EffetDeBase { get; set; }
        BlendState GestionAlpha { get; set; }

        public Cylindre(Game jeu, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector2 étendue,
            Vector2 charpente, string nomTexture, float intervalleMAJ)
            : base(jeu, échelleInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            NbColonnes = (int)charpente.X;
            NbRangées = (int)charpente.Y;
            Delta = new Vector2((float)(DIVISION_MOITIÉ * Math.PI * étendue.X) / NbColonnes, étendue.Y / (NbRangées - DIVISION_MOITIÉ));
            Origine = new Vector3(-étendue.X, -étendue.Y / DIVISION_MOITIÉ, étendue.X);
            NomTexture = nomTexture;
        }
        public override void Initialize()
        {
            NbTrianglesParStrip = NbColonnes * 2;
            NbSommets = (NbTrianglesParStrip + 2) * NbRangées;
            PtsSommets = new Vector3[NbColonnes + 1, NbRangées + 1];
            CréerTableauSommets();
            CréerTableauPoints();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            EffetDeBase = new BasicEffect(GraphicsDevice);
            gestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            image = gestionnaireDeTextures.Find(NomTexture);
            InitialiserParamètresEffetDeBase();
            base.LoadContent();
        }
        void InitialiserParamètresEffetDeBase()
        {
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = image;
            GestionAlpha = BlendState.AlphaBlend;
        }
        void CréerTableauPoints()
        {
            float graduationAngles = (float)(2*Math.PI / NbColonnes);
            CréerFaceLatérale(graduationAngles);
        }
        protected override void InitialiserSommets()
        {
            int NoSommet = -1;
            for (int j = 1; j < NbRangées-1; ++j)
            {
                for (int i = 0; i < NbColonnes+1; ++i)
                {
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i, j], PtsTexture[i, j]);
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i, j + 1], PtsTexture[i, j + 1]);
                }
            }
        }
        void CréerTableauSommets()
        {
            PtsTexture = new Vector2[NbColonnes + 1, NbRangées + 1];
            Sommets = new VertexPositionTexture[NbSommets];
            Vector2 graduation = new Vector2(1f / NbColonnes, 1f / NbRangées);
            for (int j = NbRangées; j >= 0; --j)
            {
                for (int i = 0; i <= NbColonnes; ++i)
                {
                    PtsTexture[i, NbRangées - j] = new Vector2(i * graduation.X, j * graduation.Y);
                }
            }
        }
        //void CréerPremièreBase(float graduationAngles)
        //{
        //    for (int x = 0; x < NbColonnes + 1; ++x)
        //    {
        //        PtsSommets[x, 0] = new Vector3(0, Origine.Y, 0);
        //    }

        //}
        void CréerFaceLatérale(float graduationAngles)
        {
            for (int y = 1; y < NbRangées; ++y)
            {
                for (int x = 0; x < NbColonnes + 1; ++x)
                {
                    PtsSommets[x, y] = new Vector3((float)(Origine.X * Math.Cos(graduationAngles * x)), Origine.Y + Delta.Y * (y), (float)(Origine.Z * Math.Sin(graduationAngles * x)));
                }
            }
        }
        //void CréerDeuxièmeBase(float graduationAngles)
        //{
        //    for (int x = 0; x < NbColonnes + 1; ++x)
        //    {
        //        PtsSommets[x, NbRangées] = new Vector3(0, -Origine.Y, 0);
        //    }

        //}


        public override void Draw(GameTime gameTime)
        {
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = CaméraJeu.View;
            EffetDeBase.Projection = CaméraJeu.Projection;
            RasterizerState old = GraphicsDevice.RasterizerState;
            RasterizerState ras = new RasterizerState();
            ras.CullMode = CullMode.CullClockwiseFace;
            GraphicsDevice.RasterizerState = ras;
            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                for (int i = 0; i < NbRangées; ++i)
                {
                    DessinerTriangleStrip(i);
                }
            }
            GraphicsDevice.RasterizerState = old;
            base.Draw(gameTime);
        }
        void DessinerTriangleStrip(int noStrip)
        {
            GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, Sommets, noStrip * (NbTrianglesParStrip + DIVISION_MOITIÉ), NbTrianglesParStrip);
        }
    }
}
