using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
   public class Terrain : PrimitiveDeBaseAnimée
   {
      const int NB_TRIANGLES_PAR_TUILE = 2;
      const int NB_SOMMETS_PAR_TRIANGLE = 3;
      const int NB_SOMMETS_PAR_TUILE = 4;
      const float MAX_COULEUR = 255f;

      Vector3 Étendue { get; set; }
      string NomCarteTerrain { get; set; }
      string NomTextureTerrain { get; set; }
      int NbNiveauTexture { get; set; }
 
      BasicEffect EffetDeBase { get; set; }
      RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
      Texture2D CarteTerrain { get; set; }
      Texture2D TextureTerrain { get; set; }
      Vector3 Origine { get; set; }
      
      float[,] TableauHeight { get; set; }
      Vector2[,] TableauTexture { get; set; }
      Vector3[,] TableauPoints { get; set; }
      VertexPositionTexture[] TableauSommets { get; set; }


      public Terrain(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale,
                     Vector3 étendue, string nomCarteTerrain, string nomTextureTerrain, int nbNiveauxTexture, float intervalleMAJ)
         : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
      {
         Étendue = étendue;
         NomCarteTerrain = nomCarteTerrain;
         NomTextureTerrain = nomTextureTerrain;
         NbNiveauTexture = nbNiveauxTexture;
      }

      public override void Initialize()
      {
         GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
         InitialiserDonnéesCarte();
         InitialiserDonnéesTexture();
         Origine = new Vector3(-Étendue.X / 2, 0, Étendue.Z / 2); //pour centrer la primitive au point (0,0,0)
         AllouerTableaux();
         CréerTableauPoints();
         base.Initialize();
      }

      //
      // à partir de la texture servant de carte de hauteur (HeightMap), on initialise les données
      // relatives à la structure de la carte
      //
      void InitialiserDonnéesCarte()
      {
         CarteTerrain = GestionnaireDeTextures.Find(NomCarteTerrain);
         Color[] couleurs = new Color[CarteTerrain.Width*CarteTerrain.Height];
         CarteTerrain.GetData<Color>(couleurs);
         TableauHeight = new float[CarteTerrain.Width, CarteTerrain.Height];
         for (int i = 0; i < TableauHeight.GetLength(0); ++i)
         {
            for (int j = 0; j < TableauHeight.GetLength(1); ++j)
            {
               TableauHeight[i, TableauHeight.GetLength(1) - j - 1] = (couleurs[i + j * TableauHeight.GetLength(1)].B/MAX_COULEUR);
            }
         }
      }

      //
      // à partir de la texture contenant les textures carte de hauteur (HeightMap), on initialise les données
      // relatives à l'application des textures de la carte
      //
      void InitialiserDonnéesTexture()
      {
         TextureTerrain = GestionnaireDeTextures.Find(NomTextureTerrain);
         TableauTexture = new Vector2[2, NbNiveauTexture+1];
         for (int i = 0; i < TableauTexture.GetLength(0); ++i)
         {
            for (int j = 0; j < TableauTexture.GetLength(1); ++j)
            {
               TableauTexture[i, j] = new Vector2(i, j / (float)NbNiveauTexture);
            }
         }
      }

      //
      // Allocation des deux tableaux
      //    1) celui contenant les points de sommet (les points uniques), 
      //    2) celui contenant les sommets servant à dessiner les triangles
      void AllouerTableaux()
      {
         TableauPoints = new Vector3[TableauHeight.GetLength(0), TableauHeight.GetLength(1)];
         TableauSommets = new VertexPositionTexture[NB_SOMMETS_PAR_TRIANGLE * NB_TRIANGLES_PAR_TUILE * TableauHeight.Length];
      }

      protected override void LoadContent()
      {
         base.LoadContent();
         EffetDeBase = new BasicEffect(GraphicsDevice);
         InitialiserParamètresEffetDeBase();
      }

      void InitialiserParamètresEffetDeBase()
      {
         EffetDeBase.TextureEnabled = true;
         EffetDeBase.Texture = TextureTerrain;
      }

      //
      // Création du tableau des points de sommets
      //
      private void CréerTableauPoints()
      {
         float DeltaX = Étendue.X / TableauPoints.GetLength(0);
         float DeltaY = Étendue.Y;
         float DeltaZ = -Étendue.Z / TableauPoints.GetLength(1);
         for (int i = 0; i < TableauPoints.GetLength(0); ++i)
         {
            for (int j = 0; j < TableauPoints.GetLength(1); ++j)
            {
               TableauPoints[i, j] = Origine + new Vector3(DeltaX * i, DeltaY*TableauHeight[i, j], DeltaZ * j);
            }
         }
      }

      //
      // Création des sommets.
      //
      protected override void InitialiserSommets()
      {
         int cptSommet = 0;
         for (int i = 0; i < TableauPoints.GetLength(0)-1; ++i)
         {
            for (int j = 0; j < TableauPoints.GetLength(1)-1; ++j)
            {
               int positionDansTexture = TrouverHeightMoyen(TableauPoints[i, j].Y, TableauPoints[i + 1, j].Y, TableauPoints[i, j + 1].Y);
               TableauSommets[cptSommet++] = new VertexPositionTexture(TableauPoints[i, j + 1], TableauTexture[0, positionDansTexture + 1]);
               TableauSommets[cptSommet++] = new VertexPositionTexture(TableauPoints[i + 1, j], TableauTexture[1, positionDansTexture]);
               TableauSommets[cptSommet++] = new VertexPositionTexture(TableauPoints[i, j], TableauTexture[0, positionDansTexture]);

               positionDansTexture = TrouverHeightMoyen(TableauPoints[i + 1, j].Y, TableauPoints[i, j + 1].Y, TableauPoints[i + 1, j + 1].Y);
               TableauSommets[cptSommet++] = new VertexPositionTexture(TableauPoints[i + 1, j + 1], TableauTexture[1, positionDansTexture+1]);
               TableauSommets[cptSommet++] = new VertexPositionTexture(TableauPoints[i + 1, j], TableauTexture[1, positionDansTexture]);
               TableauSommets[cptSommet++] = new VertexPositionTexture(TableauPoints[i, j + 1], TableauTexture[0, positionDansTexture + 1]);
            }
         }
      }

      int TrouverHeightMoyen(float height0, float height1, float height2)
      {
         float moyenne = (height0 + height1 + height2) / NB_SOMMETS_PAR_TRIANGLE;
         moyenne /= Étendue.Y;
         moyenne *= NbNiveauTexture;
         return Math.Max((int)Math.Ceiling(moyenne) - 1, 0);
      }
      public override void Update(GameTime gameTime)
      { }

      public override void Draw(GameTime gameTime)
      {
         EffetDeBase.World = GetMonde();
         EffetDeBase.View = CaméraJeu.View;
         EffetDeBase.Projection = CaméraJeu.Projection;
         foreach (EffectPass current in EffetDeBase.CurrentTechnique.Passes)
         {
            current.Apply();
            Game.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, TableauSommets, 0, TableauSommets.Length/NB_SOMMETS_PAR_TRIANGLE);
         }
      }
   }
}
