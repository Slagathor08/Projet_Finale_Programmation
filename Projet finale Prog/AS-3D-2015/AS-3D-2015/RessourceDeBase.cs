using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
    class RessourceDeBase<T> : IEquatable<RessourceDeBase<T>>
    {
      public string Nom { get; private set; }
      public T Ressource { get; private set; }
      ContentManager Content { get; set; }
      string Répertoire { get; set; }

      public RessourceDeBase(string nom, T ressource)
      {
         Nom = nom;
         Content = null;
         Répertoire = "";
         Ressource = ressource;
      }

      public RessourceDeBase(ContentManager content, string répertoire, string nom)
      {
         Nom = nom;
         Content = content;
         Répertoire = répertoire;
         Ressource = default(T);
      }

      public void Load()
      {
         if (Ressource == null)
         {
            string NomComplet = Répertoire + "/" + Nom;
            Ressource = Content.Load<T>(NomComplet);
         }
      }
      public bool Equals(RessourceDeBase<T> other)
      {
         return Nom == other.Nom;
      }
   }
}

