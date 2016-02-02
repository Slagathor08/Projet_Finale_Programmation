using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace AtelierXNA
{
    public class CubeColoré : PrimitiveDeBase
    {
        const int NB_SOMMETS1 = 24;
        const int NB_TRIANGLES1 = 8;
        const int NB_SOMMETS2 = 6;
        const int NB_TRIANGLES2 = 2;

        string NomTexture { get; set; }
        VertexPositionTexture[] Sommets1 { get; set; }
        VertexPositionTexture[] Sommets2 { get; set; }
        Vector3 Origine { get; set; }
        Vector3 Delta { get; set; }
        BasicEffect EffetDeBase { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        Texture2D TextureCube { get; set; }
        public BoundingBox ZoneCollision { get; set; }
        public BoundingSphere ZoneVerifCollision { get; set; }

        public CubeColoré(Game game, Vector3 positionInitiale, Vector3 dimension, string nomTexture)
            : base(game, 1f, Vector3.Zero, positionInitiale)
        {
            ZoneCollision = new BoundingBox(new Vector3(positionInitiale.X - dimension.X / 2, positionInitiale.Y - dimension.Y /2, positionInitiale.Z - dimension.Z / 2), 
                            new Vector3(positionInitiale.X + dimension.X / 2, positionInitiale.Y + dimension.Y , positionInitiale.Z + dimension.Z / 2));
            Delta = dimension;
            ZoneVerifCollision = new BoundingSphere(positionInitiale, dimension.X);
            Origine = new Vector3(-Delta.X / 2, -Delta.Y / 2, -Delta.Z / 2);
            NomTexture = nomTexture;
        }

        public override void Initialize()
        {
            Sommets1 = new VertexPositionTexture[NB_SOMMETS1];
            Sommets2 = new VertexPositionTexture[NB_SOMMETS2];
            base.Initialize();
        }

        protected override void LoadContent()
        {
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            TextureCube = GestionnaireDeTextures.Find(NomTexture);

            EffetDeBase = new BasicEffect(GraphicsDevice);
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = TextureCube;
            base.LoadContent();
        }

        protected override void InitialiserSommets()
        {
            Vector2 a = new Vector2(0, 0);
            Vector2 b = new Vector2(1, 0);
            Vector2 c = new Vector2(0, 1);
            Vector2 d = new Vector2(1, 1);

            VertexPositionTexture A = new VertexPositionTexture(Origine, c);
            VertexPositionTexture B = new VertexPositionTexture(new Vector3(Origine.X + Delta.X, Origine.Y, Origine.Z), d);
            VertexPositionTexture C = new VertexPositionTexture(new Vector3(Origine.X + Delta.X, Origine.Y, Origine.Z + Delta.Z), c);
            VertexPositionTexture D = new VertexPositionTexture(new Vector3(Origine.X, Origine.Y, Origine.Z + Delta.Z), d);

            VertexPositionTexture E = new VertexPositionTexture(new Vector3(Origine.X, Origine.Y + Delta.Y, Origine.Z), a);
            VertexPositionTexture F = new VertexPositionTexture(new Vector3(Origine.X + Delta.X, Origine.Y + Delta.Y, Origine.Z), b);
            VertexPositionTexture G = new VertexPositionTexture(new Vector3(Origine.X + Delta.X, Origine.Y + Delta.Y, Origine.Z + Delta.Z), a);
            VertexPositionTexture H = new VertexPositionTexture(new Vector3(Origine.X, Origine.Y + Delta.Y, Origine.Z + Delta.Z), b);
            
            #region TRIANGLELIST 1
            Sommets1[0] = F;
            Sommets1[1] = G;
            Sommets1[2] = B;

            Sommets1[3] = B;
            Sommets1[4] = G;
            Sommets1[5] = C;

            Sommets1[6] = C;
            Sommets1[7] = G;
            Sommets1[8] = D;

            Sommets1[9] = D;
            Sommets1[10] = G;
            Sommets1[11] = H;

            Sommets1[12] = H;
            Sommets1[13] = E;
            Sommets1[14] = D;

            Sommets1[15] = D;
            Sommets1[16] = E;
            Sommets1[17] = A;

            Sommets1[18] = A;
            Sommets1[19] = E;
            Sommets1[20] = B;

            Sommets1[21] = B;
            Sommets1[22] = E;
            Sommets1[23] = F;
            #endregion
            Sommets2[0] = E;
            Sommets2[1] = H;
            Sommets2[2] = G;
            Sommets2[3] = G;
            Sommets2[4] = F;
            Sommets2[5] = E;
        }

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
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, Sommets1, 0, 8);
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, Sommets2, 0, NB_TRIANGLES2);
            }
            GraphicsDevice.RasterizerState = old;
            base.Draw(gameTime);
        }
    }
}