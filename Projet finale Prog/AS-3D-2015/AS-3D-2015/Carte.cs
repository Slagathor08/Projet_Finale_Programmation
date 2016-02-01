using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    public class Carte : Microsoft.Xna.Framework.GameComponent
    {
        Vector3 Étendue { get; set; }
        string NomCarteTerrain { get; set; }

        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        Texture2D CarteTerrain { get; set; }
        Vector3 Origine { get; set; }

        int NbColonnes { get; set; }
        int NbRangées { get; set; }
        Color[] DataTexture { get; set; }
        Vector4[,] PtsSommets { get; set; } 
        Vector3 Delta { get; set; }

        public Carte(Game jeu, Vector3 étendue, string nomCarteTerrain)
            : base(jeu)
        {
            Étendue = étendue;
            NomCarteTerrain = nomCarteTerrain;
        }

        public override void Initialize()
        {
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            InitialiserDonnéesCarte();
            Origine = new Vector3(-Étendue.X / 2, 0, -Étendue.Z / 2);
            PtsSommets = new Vector4[NbColonnes, NbRangées];
            CréerTableauPoints();
            base.Initialize();
        }

        void InitialiserDonnéesCarte()
        {
            CarteTerrain = GestionnaireDeTextures.Find(NomCarteTerrain);
            NbColonnes = CarteTerrain.Width;
            NbRangées = CarteTerrain.Height;

            DataTexture = new Color[NbColonnes * NbRangées];
            CarteTerrain.GetData<Color>(DataTexture);
            Delta = new Vector3(Étendue.X / NbColonnes); 
        }

        private void CréerTableauPoints()
        {
            int k = 0, h = 0;
            for (int j = 0; j < NbRangées; ++j)
            {
                for (int i = 0; i < NbColonnes; ++i)
                {
                    h = DataTexture[k].R == 0 ? 0 : 1;
                    PtsSommets[i, j] = new Vector4(Origine.X + (i * Delta.X), Origine.Y, Origine.Z + (j * Delta.Z), h);

                    if (PtsSommets[i, j].W == 1)
                    {
                        Game.Components.Add(new Tuile(Game, new Vector3(PtsSommets[i, j].X, PtsSommets[i, j].Y, PtsSommets[i, j].Z), Delta, "Floor"));
                        Game.Components.Add(new Tuile(Game,
                                               new Vector3(PtsSommets[i, j].X, PtsSommets[i, j].Y + 1.5f * Delta.Y, PtsSommets[i, j].Z), Delta, "Ceiling"));
                    }
                    else
                    {
                        Game.Components.Add(new CubeColoré(Game,
                                               new Vector3(PtsSommets[i, j].X, PtsSommets[i, j].Y, PtsSommets[i, j].Z), new Vector3(Delta.X, Delta.Y, Delta.Z), "Wall"));
                        Game.Components.Add(new CubeColoré(Game,
                                               new Vector3(PtsSommets[i, j].X, PtsSommets[i, j].Y + Delta.Y, PtsSommets[i, j].Z), new Vector3(Delta.X, Delta.Y, Delta.Z), "Wall"));
                    }
                    ++k;
                }
            }
        }
    }
}
