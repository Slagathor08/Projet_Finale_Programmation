using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Linq;


namespace AtelierXNA
{
    public class InputManager : Microsoft.Xna.Framework.GameComponent
    {
        Keys[] AnciennesTouches { get; set; }
        Keys[] NouvellesTouches { get; set; }
        List<Buttons> NewButton { get; set; }
        List<Buttons> OldButton { get; set; }

        KeyboardState ÉtatClavier { get; set; }
        GamePadState ÉtatManette { get; set; }
        MouseState AncienÉtatSouris { get; set; }
        MouseState NouveauÉtatSouris { get; set; }

        public InputManager(Game game)
            : base(game)
        { }

        public override void Initialize()
        {
            NewButton = new List<Buttons>();
            OldButton = new List<Buttons>();
            AnciennesTouches = new Keys[0];
            NouvellesTouches = new Keys[0];
            AncienÉtatSouris = Mouse.GetState();
            NouveauÉtatSouris = Mouse.GetState();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            OldButton = NewButton;
            AnciennesTouches = NouvellesTouches;
            AncienÉtatSouris = NouveauÉtatSouris;
            ÉtatManette = GamePad.GetState(PlayerIndex.One);
            ÉtatClavier = Keyboard.GetState();
            NouveauÉtatSouris = Mouse.GetState();
            NewButton = AvoirBoutonsPesés();
            NouvellesTouches = ÉtatClavier.GetPressedKeys();
            base.Update(gameTime);
        }


        public List<Buttons> AvoirBoutonsPesés()
        {
            return Enum.GetValues(typeof(Buttons))
                .Cast<Buttons>()
                .Where(b => ÉtatManette.IsButtonDown(b))
                .ToList();
        }
        public bool EstSourisActive
        {
            get { return Abougé() || EstNouveauClicGauche() ||EstNouveauClicDroit(); }
        }
        public bool EstClavierActivé
        {
            get { return NouvellesTouches.Length > 0; }
        }
        public bool EstManetteActivé
        {
            get { return NewButton.Count > 0; }
        }

        public bool EstEnfoncée(Keys touche)
        {
            return ÉtatClavier.IsKeyDown(touche);
        }
        public bool EstBoutonEnfoncée(Buttons bouton)
        {
            return ÉtatManette.IsButtonDown(bouton);
        }
        public bool EstNouveauBouton(Buttons bouton)
        {
            int NbTouches = OldButton.Count;
            bool estNouveauBouton = ÉtatManette.IsButtonDown(bouton);
            int i = 0;
            while (i < NbTouches && estNouveauBouton)
            {
                estNouveauBouton = OldButton[i] != bouton;
                ++i;
            }
            return estNouveauBouton;
        }
        public bool EstNouvelleTouche(Keys touche)
        {
            int NbTouches = AnciennesTouches.Length;
            bool EstNouvelleTouche = ÉtatClavier.IsKeyDown(touche);
            int i = 0;
            while (i < NbTouches && EstNouvelleTouche)
            {
                EstNouvelleTouche = AnciennesTouches[i] != touche;
                ++i;
            }
            return EstNouvelleTouche;
        }

        public bool EstAncienClicDroit()
        {
            return MouseState.Equals(AncienÉtatSouris.RightButton, NouveauÉtatSouris.RightButton);
        }
        public bool EstAncienClicGauche()
        {
            return MouseState.Equals(AncienÉtatSouris.LeftButton, NouveauÉtatSouris.LeftButton);
        }
        public bool EstNouveauClicDroit()
        {
            return NouveauÉtatSouris.RightButton.Equals(ButtonState.Pressed);
        }
        public bool EstNouveauClicGauche()
        {
            return NouveauÉtatSouris.LeftButton.Equals(ButtonState.Pressed);
        }

        public Point GetPositionSouris()
        {
            return new Point(NouveauÉtatSouris.X, NouveauÉtatSouris.Y);
        }

        public bool Abougé()
        {
            return NouveauÉtatSouris.Y != AncienÉtatSouris.Y || NouveauÉtatSouris.X != AncienÉtatSouris.X;
        }
    }
}