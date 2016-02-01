using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    public abstract class Plan : PrimitiveDeBaseAnimée
    {
        protected Vector3 Origine { get; private set; }  
        Vector2 Delta { get; set; } 
        protected Vector3[,] PtsSommets { get; private set; } 
        protected int NbColonnes { get; private set; } 
        protected int NbRangées { get; private set; } 
        protected int NbTrianglesParStrip { get; private set; } 
        protected BasicEffect EffetDeBase { get; private set; }

        public Plan(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector2 étendue, Vector2 charpente, float intervalleMAJ)
            : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            NbColonnes = (int)charpente.X;
            NbRangées = (int)charpente.Y;
            Delta = étendue / charpente;
            Origine = new Vector3(-étendue.X / 2, -étendue.Y / 2, 0);
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

        protected abstract void CréerTableauSommets();

        protected override void LoadContent()
        {
            EffetDeBase = new BasicEffect(GraphicsDevice);
            InitialiserParamètresEffetDeBase();
            base.LoadContent();
        }

        protected abstract void InitialiserParamètresEffetDeBase();

        private void CréerTableauPoints()
        {
            for (int y = 0; y < NbRangées + 1; ++y)
            {
                for (int x = 0; x < NbColonnes + 1; ++x)
                {
                    PtsSommets[x, y] = new Vector3(Origine.X + Delta.X * x, Origine.Y + Delta.Y * y, Origine.Z);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = CaméraJeu.View;
            EffetDeBase.Projection = CaméraJeu.Projection;
            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                for (int i = 0; i < NbRangées; ++i)
                {
                    DessinerTriangleStrip(i);
                }
            }
            base.Draw(gameTime);
        }

        protected abstract void DessinerTriangleStrip(int noStrip);
    }
}
