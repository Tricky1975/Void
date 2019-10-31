// Lic:
// Void
// ...
// 
// 
// 
// (c) Jeroen P. Broks, 
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
// Please note that some references to data like pictures or audio, do not automatically
// fall under this licenses. Mostly this is noted in the respective files.
// 
// Version: 19.10.31
// EndLic
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using UseJCR6;
using TrickyUnits;
using Void.Stages;

namespace Void {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Void : Game {
        bool StopIt = false;
        internal readonly TJCRDIR JCR;
        TQMGImage MousePointer;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        static public KeyboardState kb { get; private set; }
        static public MouseState ms { get; private set; }


        static public void FatalError(string msg) {
            Confirm.Annoy(msg, "Void - FATAL ERROR", System.Windows.Forms.MessageBoxIcon.Error);
        }

        static public void Assert(bool c,string msg) {
            if (!c) FatalError(msg);
        }
        static public void Assert(int c, string msg) => Assert(c != 0, msg);
        static public void Assert(string c, string msg) => Assert(c.Length, msg);
        static public void Assert(object c, string msg) => Assert(c != null, msg);

        static internal TQMGFont Font { get; private set; } = null;
        static internal TQMGImage Back { get; private set; } = null;

        public Void() {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width-5;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height-100;

            try {
                new JCR6_lzma();
                JCR = JCR6.Dir($"{qstr.StripExt(MKL.MyExe)}.jcr"); Assert(JCR, "Void.jcr has not been properly loaded!");
            } catch (Exception QuelleCatastrophe) {
#if DEBUG
                FatalError($"Exception Thrown:\n{QuelleCatastrophe.Message}\n\n{QuelleCatastrophe.StackTrace}");
#else
                FatalError($"Exception Thrown:\n{QuelleCatastrophe.Message}");
#endif
            }

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            try {
                spriteBatch = new SpriteBatch(GraphicsDevice);
                TQMG.Init(graphics, GraphicsDevice, spriteBatch, JCR);
                MousePointer = TQMG.GetImage("Mouse.png"); Assert(MousePointer, JCR6.JERROR);
                Font = TQMG.GetFont("DosFont.JFBF"); Assert(Font, JCR6.JERROR);
                Back = TQMG.GetImage("Back.png");
                Stage.GoTo(new Editor());
            } catch (Exception QuelleCatastrophe) {
#if DEBUG
                FatalError($"Exception Thrown:\n{QuelleCatastrophe.Message}\n\n{QuelleCatastrophe.StackTrace}");
#else
                FatalError($"Exception Thrown:\n{QuelleCatastrophe.Message}");
#endif
            }

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            ms = Mouse.GetState();
            kb = Keyboard.GetState();
            if (StopIt || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || kb.IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap, null, null);
            // Stage

            // Mouse
            TQMG.Color(255, 255, 255); if (ms.X>0 && ms.Y>0) MousePointer.Draw(ms.X, ms.Y);
            
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}



