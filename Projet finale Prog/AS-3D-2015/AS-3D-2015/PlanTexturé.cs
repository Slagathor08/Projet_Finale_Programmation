using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    class PlanTexturé : Plan
    {
        RessourcesManager<Texture2D> gestionnaireDeTextures;

        Texture2D textureTuile;
        protected VertexPositionTexture[] Sommets { get; set; }
        Vector2[,] PtsTexture { get; set; }
        string NomTextureTuile { get; set; }
        BlendState GestionAlpha { get; set; }
        

        public PlanTexturé(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector2 étendue, Vector2 charpente, string nomTextureTuile, float intervalleMAJ)
            : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, étendue, charpente, intervalleMAJ)
        {
            NomTextureTuile = nomTextureTuile;
        }

        protected override void CréerTableauSommets()
        {
            PtsTexture = new Vector2[NbColonnes+1, NbRangées+1];
            Sommets = new VertexPositionTexture[NbSommets];
            CréerTableauPointsTexture();
        }

        private void CréerTableauPointsTexture()
        {
            Vector2 graduation = new Vector2(1f / NbColonnes, 1f / NbRangées);
            for (int j = NbRangées; j >= 0;--j)
            {
                for(int i = 0; i <= NbColonnes; ++i)
                {
                    PtsTexture[i, NbRangées-j] = new Vector2(i * graduation.X, j * graduation.Y);
                }
            }
                
        }

        protected override void InitialiserSommets() 
        {   
            int NoSommet = -1;
            for (int j = 0; j < NbRangées; ++j)
            {
                for (int i = 0; i < NbColonnes +1; ++i)
                {
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i, j], PtsTexture[i, j]);
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i, j + 1], PtsTexture[i, j + 1]);
                }
            }
        }

        protected override void LoadContent()
        {
            gestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            textureTuile = gestionnaireDeTextures.Find(NomTextureTuile);
            base.LoadContent();
        }

        protected override void InitialiserParamètresEffetDeBase()
        {
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = textureTuile;
            GestionAlpha = BlendState.AlphaBlend;
        }

        public override void Draw(GameTime gameTime)
        {
            BlendState oldBlendState = GraphicsDevice.BlendState;
            GraphicsDevice.BlendState = GestionAlpha;
            base.Draw(gameTime);
            GraphicsDevice.BlendState = oldBlendState;
        }

        protected override void DessinerTriangleStrip(int noStrip)
        {
            GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, Sommets, noStrip * (NbTrianglesParStrip + 2), NbTrianglesParStrip);
        }
    }
}

