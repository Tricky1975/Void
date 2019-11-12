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
// Version: 19.11.05
// EndLic
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using UseJCR6;
using TrickyUnits;
using Void.Stages;
using Void.Parts;
using Void.Lex;

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

        
        static internal TQMGImage VoidBack;

        static public KeyboardState kb { get; private set; }
        static public KeyboardState oldkb { get; private set; }
        static public MouseState ms { get; private set; }
        static internal TMap<Keys, int> KeyHold = new TMap<Keys, int>();


        static Keys DoReadKey {
            get {
                bool Has(Keys[]a, Keys k) { foreach (Keys ck in a) if (ck == k) return true; return false; }
                var pressed = kb.GetPressedKeys();
                var ret = Keys.None;
                foreach(Keys k in (Keys[])Enum.GetValues(typeof(Keys))) {
                    if (!Has(pressed, k))
                        KeyHold[k] = 0;
                    else {
                        KeyHold[k]++;
                        if (((!oldkb.IsKeyDown(k)) || KeyHold[k] > 40) && k!=Keys.RightShift && k!=Keys.LeftShift && k!=Keys.LeftControl && k!=Keys.RightControl && k!=Keys.LeftAlt && k!=Keys.RightAlt)
                            ret = k;
                    }
                }
                return ret;
            }
        }
        static public Keys ReadKey { get; private set; } = Keys.None;
        static public bool Shift => kb.IsKeyDown(Keys.LeftShift) || kb.IsKeyDown(Keys.RightShift);

        static public char ReadChar { get {
                if (kb.IsKeyDown(Keys.LeftControl) || kb.IsKeyDown(Keys.RightControl)) return '\0';
                switch (ReadKey) {
                    case Keys.NumPad0: return '0';
                    case Keys.NumPad1: return '1';
                    case Keys.NumPad2: return '2';
                    case Keys.NumPad3: return '3';
                    case Keys.NumPad4: return '4';
                    case Keys.NumPad5: return '5';
                    case Keys.NumPad6: return '6';
                    case Keys.NumPad7: return '7';
                    case Keys.NumPad8: return '8';
                    case Keys.NumPad9: return '9';
                    case Keys.A:
                    case Keys.B:
                    case Keys.C:
                    case Keys.D:
                    case Keys.E:
                    case Keys.F:
                    case Keys.G:
                    case Keys.H:
                    case Keys.I:
                    case Keys.J:
                    case Keys.K:
                    case Keys.L:
                    case Keys.M:
                    case Keys.N:
                    case Keys.O:
                    case Keys.P:
                    case Keys.Q:
                    case Keys.R:
                    case Keys.S:
                    case Keys.T:
                    case Keys.U:
                    case Keys.V:
                    case Keys.W:
                    case Keys.X:
                    case Keys.Y:
                    case Keys.Z: {
                            var k = ReadKey.ToString();
                            if (kb.IsKeyDown(Keys.LeftShift) || kb.IsKeyDown(Keys.RightShift) || kb.CapsLock)
                                return k[0];
                            else
                                return k.ToLower()[0];
                        }
                    case Keys.D0:
                        if (Shift) return ')'; else return '0';
                    case Keys.D1:
                        if (Shift) return '!'; else return '1';
                    case Keys.D2:
                        if (Shift) return '@'; else return '2';
                    case Keys.D3:
                        if (Shift) return '#'; else return '3';
                    case Keys.D4:
                        if (Shift) return '$'; else return '4';
                    case Keys.D5:
                        if (Shift) return '%'; else return '5';
                    case Keys.D6:
                        if (Shift) return '^'; else return '6';
                    case Keys.D7:
                        if (Shift) return '&'; else return '7';
                    case Keys.D8:
                        if (Shift) return '*'; else return '0';
                    case Keys.D9:
                        if (Shift) return '('; else return '0';
                    case Keys.OemMinus:
                        if (!Shift) return '-'; else return '_';
                    case Keys.OemPlus:
                        if (!Shift) return '='; else return '+';
                    case Keys.OemPipe:
                        if (Shift) return '\\'; else return '|';
                    case Keys.Multiply: return '*';
                    case Keys.Tab: return '\t';
                    case Keys.OemOpenBrackets:
                        if (Shift) return '{'; else return '[';
                    case Keys.OemCloseBrackets:
                        if (Shift) return '}'; else return ']';
                    case Keys.OemSemicolon:
                        if (!Shift) return ';'; else return ':';
                    case Keys.OemQuotes:
                        if (!Shift) return '\''; else return '"';
                    case Keys.OemComma:
                        if (!Shift) return ','; else return '<';
                    case Keys.OemPeriod:
                        if (!Shift) return '.'; else return '>';
                    case Keys.OemQuestion:
                        if (!Shift) return '/'; else return '?';
                    case Keys.Space:
                        return ' ';
                    default:
                        //System.Diagnostics.Debug.WriteLine($"Pressed unknown key {ReadKey}");
                        break;
                }
                return '\0';
            }
        }

        static public void FatalError(string msg) {
            Confirm.Annoy(msg, "Void - FATAL ERROR", System.Windows.Forms.MessageBoxIcon.Error);
        }

        static public bool Assert(bool c,string msg) {
            if (!c) FatalError(msg);
            return c;
        }
        static public bool Assert(int c, string msg) => Assert(c != 0, msg);
        static public bool Assert(string c, string msg) => Assert(c.Length, msg);
        static public bool Assert(object c, string msg) => Assert(c != null, msg);

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
                VoidBack = TQMG.GetImage("Void.png");
                Stage.GoTo(new Editor());
                foreach (string prj in Config.GetL("Projects")) {
                    if (System.IO.Directory.Exists(prj)) {
                        Project.ProjMap[prj] = new Project(prj);
                    } else {
                        Confirm.Annoy($"Project directory \"{prj}\" has not been found!");
                    }
                }
                new LexNIL();
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
            oldkb = kb;
            ms = Mouse.GetState();
            kb = Keyboard.GetState();
            ReadKey = DoReadKey;
            if (StopIt || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || kb.IsKeyDown(Keys.Escape))
                Exit();

            Stage.UpdateStage();

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

            Stage.DrawStage();

            // Mouse
            TQMG.Color(255, 255, 255); if (ms.X>0 && ms.Y>0) MousePointer.Draw(ms.X, ms.Y);
            
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}





