using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace AtelierXNA
{
   class ObjetDePatrouille : ObjetDeBase
   {
      Vector3 PositionAxe { get; set; }
      int NbPtsPatrouille { get; set; }
      float Vitesse { get; set; }
      float IntervalleMAJ { get; set; }
      float TempsDepuisMAJ { get; set; }
      Vector3[] TableauPositionPatrouille { get; set; }
      Vector3 Déplacement { get; set; }
      int NombreDéplacementsRequis { get; set; }
      int NombreDéplacementFait { get; set; }
      int Objectif { get; set; }
      float Yaw { get; set; }
      float YawInitiale { get; set; }
      float IncrémentYaw { get; set; }
      float YawPourUnSegmentDeLaPatrouille { get; set; }

      public ObjetDePatrouille(Game jeu, String nomModèle, float échelleInitiale,
         Vector3 rotationInitiale, Vector3 positionInitiale, Vector3 positionAxe, int nbPtsPatrouille, float vitesse, float intervalleMAJ)
         : base(jeu, nomModèle, échelleInitiale, rotationInitiale, positionInitiale)
      {
         PositionAxe = positionAxe;
         NbPtsPatrouille = nbPtsPatrouille;
         Vitesse = vitesse;
         IntervalleMAJ = intervalleMAJ;
      }
      public override void Initialize()
      {
         
         base.Initialize();
         YawInitiale = Rotation.Y;
         InitialiserTableauPositionPatrouille();
         TempsDepuisMAJ = 0;
         Position = TableauPositionPatrouille[0];
         Objectif = 1;
         InitialiserNombreDéplacementsRequis();
         TrouverDéplacement(0);         
         IncrémentYaw = MathHelper.TwoPi / NbPtsPatrouille / NombreDéplacementsRequis;
         YawPourUnSegmentDeLaPatrouille = MathHelper.TwoPi / NbPtsPatrouille;
         Yaw = YawInitiale - (Objectif+1) * YawPourUnSegmentDeLaPatrouille;
      }

      void InitialiserTableauPositionPatrouille()
      {
         TableauPositionPatrouille = new Vector3[NbPtsPatrouille];
         float bond = MathHelper.TwoPi / NbPtsPatrouille;
         float angle = TrouverAngleInitial(Position);
         float rayon = CalculerRayon(Position);
         for (int i = 0; i < TableauPositionPatrouille.Length; ++i)
         {
            TableauPositionPatrouille[i] = new Vector3(PositionAxe.X + (float)Math.Cos(angle) * rayon, Position.Y, PositionAxe.Z + (float)Math.Sin(angle) * rayon);
            angle += bond;
         }
      }

      float TrouverAngleInitial(Vector3 position)
      {
         float angle = (float)Math.Atan(position.Z / position.X);
         angle *= position.X >= 0 ? 1 : -1;
         return angle;
      }

      float CalculerRayon(Vector3 position)
      {
         return (float)Math.Sqrt(position.X * position.X + position.Z * position.Z);
      }

      public override void Update(GameTime gameTime)
      {
         TempsDepuisMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;
         if (TempsDepuisMAJ >= IntervalleMAJ)
         {
            EffectuerMiseÀJour();
            TempsDepuisMAJ = 0;
         }
         base.Update(gameTime);
      }

      protected virtual void EffectuerMiseÀJour()
      {
         Position += Déplacement;
         ++NombreDéplacementFait;
         Yaw -= IncrémentYaw;
         if (NombreDéplacementFait >= NombreDéplacementsRequis)
         {
            ChangerDObjectif();
         }
         Rotation = new Vector3(Rotation.X, Yaw, Rotation.Z);
         FaireMonde();
      }

      void ChangerDObjectif()
      {
         Position = TableauPositionPatrouille[Objectif];
         int ancienObjectif = Objectif;
         Objectif = (Objectif + 1) % NbPtsPatrouille;
         TrouverDéplacement(ancienObjectif);
         NombreDéplacementFait = 0;
         Yaw = YawInitiale - (Objectif + 1) * YawPourUnSegmentDeLaPatrouille;
      }

      void TrouverDéplacement(int pointActuel)
      {
         Déplacement = (TableauPositionPatrouille[Objectif] - TableauPositionPatrouille[pointActuel])/NombreDéplacementsRequis;
      }

      void InitialiserNombreDéplacementsRequis()
      {
         NombreDéplacementsRequis = (int)(1 / (Vitesse * IntervalleMAJ));
      }

      

      void FaireMonde()
      {
         Monde = Matrix.Identity;
         Monde *= Matrix.CreateScale(Échelle);
         Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
         Monde *= Matrix.CreateTranslation(Position);
      }
   }
 }
