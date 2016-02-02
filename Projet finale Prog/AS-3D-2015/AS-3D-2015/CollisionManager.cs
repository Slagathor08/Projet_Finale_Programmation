using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Linq;

namespace AtelierXNA
{
    public class CollisionManager : Microsoft.Xna.Framework.GameComponent
    {
        public CollisionManager(Game jeu)
            : base(jeu)
        { }

        #region CollisionsDuJoueur
        public bool CollisionJoueurs(Caméra joueurA, Caméra joueurB)
        {
            Vector3 distance = joueurA.ZoneCollision.Center - joueurB.ZoneCollision.Center;
            float normeDist = distance.Length();
            float somme = joueurA.ZoneCollision.Radius + joueurB.ZoneCollision.Radius;

            return somme > normeDist;
        }

        public bool CollisionJoueurMur(Caméra joueur, CubeColoré mur)
        {
            bool estEnCollision = false;
            Vector3 distance = joueur.ZoneCollision.Center - mur.ZoneVerifCollision.Center;
            float normeDist = distance.Length();
            float somme = joueur.ZoneCollision.Radius + mur.ZoneVerifCollision.Radius;

            if (somme > normeDist)
            {
                estEnCollision = joueur.ZoneCollision.Intersects(mur.ZoneCollision);
            }

            return estEnCollision;
        }

        //public bool Collision JoueurMonstre(Caméra joueur, Monstre zombie)
        //{
        //    Vector3 distance = joueur.ZoneCollision.Center - zombie.ZoneCollision.Center;
        //    float normeDist = distance.Length();
        //    float somme = joueur.ZoneCollision.Radius + zombie.ZoneCollision.Radius;

        //    return somme > normeDist;
        //}

        //public bool Collision JoueurObjet(Caméra joueur, Item objet)
        //{
        //    Vector3 distance = joueur.ZoneCollision.Center - objet.ZoneCollision.Center;
        //    float normeDist = distance.Length();
        //    float somme = joueur.ZoneCollision.Radius + objet.ZoneCollision.Radius;

        //    return somme > normeDist;
        //}

        #endregion

        #region CollisionsDuMonstre
        //public bool CollisionMonstreMur(FuturZombie zombie, CubeColoré mur)
        //{
        //    Vector3 distance = zombie.ZoneCollision.Center - mur.ZoneCollisionZombie.Center;
        //    float normeDist = distance.Length();
        //    float somme = zombie.ZoneCollision.Radius + mur.ZoneCollisionZombie.Radius;
        //    return somme > normeDist;
        //}
        #endregion



    }
}
