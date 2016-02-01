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
    public class Tuile : PrimitiveDeBase
    {
        const int NB_SOMMETS = 6;
        const int NB_TRIANGLES = 2;

        string NomTexture { get; set; }
        VertexPositionTexture[] Sommets { get; set; }
        Vector3 Origine { get; set; }
        Vector3 Delta { get; set; }
        BasicEffect EffetDeBase { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        Texture2D TextureTuile { get; set; }

        public Tuile(Game game, Vector3 positionInitiale, Vector3 dimension, string nomTexture)
            : base(game, 1f, Vector3.Zero, positionInitiale)
        {
            Delta = dimension;
            Origine = new Vector3(-Delta.X / 2, -Delta.Y / 2, -Delta.Z / 2);
            NomTexture = nomTexture;
        }

        public override void Initialize()
        {
            Sommets = new VertexPositionTexture[NB_SOMMETS];
            base.Initialize();
        }

        protected override void LoadContent()
        {
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            TextureTuile = GestionnaireDeTextures.Find(NomTexture);

            EffetDeBase = new BasicEffect(GraphicsDevice);
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = TextureTuile;
            base.LoadContent();
        }

        protected override void InitialiserSommets()
        {
            Vector2 a = new Vector2(0, 0);
            Vector2 b = new Vector2(1, 0);
            Vector2 c = new Vector2(0, 1);
            Vector2 d = new Vector2(1, 1);

            VertexPositionTexture A = new VertexPositionTexture(Origine, c);
            VertexPositionTexture B = new VertexPositionTexture(new Vector3(Origine.X + Delta.X, Origine.Y, Origine.Z), a);
            VertexPositionTexture C = new VertexPositionTexture(new Vector3(Origine.X + Delta.X, Origine.Y, Origine.Z + Delta.Z), b);
            VertexPositionTexture D = new VertexPositionTexture(new Vector3(Origine.X, Origine.Y, Origine.Z + Delta.Z), d);

            Sommets[0] = A;
            Sommets[1] = B;
            Sommets[2] = C;

            Sommets[3] = C;
            Sommets[4] = D;
            Sommets[5] = A;
        }

        public override void Draw(GameTime gameTime)
        {
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = CaméraJeu.View;
            EffetDeBase.Projection = CaméraJeu.Projection;

            RasterizerState old = GraphicsDevice.RasterizerState;
            RasterizerState ras = new RasterizerState();
            ras.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = ras;

            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, Sommets, 0, NB_TRIANGLES);
            }
            GraphicsDevice.RasterizerState = old;
            base.Draw(gameTime);
        }
    }
}
