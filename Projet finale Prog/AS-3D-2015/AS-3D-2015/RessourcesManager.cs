using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    class RessourcesManager<T>
    {
      Game Jeu { get; set; }
      string RépertoireDesRessources { get; set; }
      List<RessourceDeBase<T>> ListeRessources { get; set; }

      public RessourcesManager(Game jeu, string répertoireDesRessources)
      {
         Jeu = jeu;
         RépertoireDesRessources = répertoireDesRessources;
         ListeRessources = new List<RessourceDeBase<T>>();
      }

      public void Add(string nom, T ressourceÀAjouter)
      {
         RessourceDeBase<T> ressourcePrimeÀAjouter = new RessourceDeBase<T>(nom, ressourceÀAjouter);
         if (!ListeRessources.Contains(ressourcePrimeÀAjouter))
         {
            ListeRessources.Add(ressourcePrimeÀAjouter);
         }
      }

      void Add(RessourceDeBase<T> ressourceÀAjouter)
      {
         ressourceÀAjouter.Load();
         ListeRessources.Add(ressourceÀAjouter);
      }

      public T Find(string nomRessource)
      {
         const int RESSOURCES_PAS_TROUVÉE = -1;
         RessourceDeBase<T> RessourceÀRechercher = new RessourceDeBase<T>(Jeu.Content, RépertoireDesRessources, nomRessource);
         int indexRessource = ListeRessources.IndexOf(RessourceÀRechercher);
         if (indexRessource == RESSOURCES_PAS_TROUVÉE)
         {
            Add(RessourceÀRechercher);
            indexRessource = ListeRessources.Count - 1;
         }
         return ListeRessources[indexRessource].Ressource;
      }
    }
}
