// *************************************************************************** 
// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org>
// ***************************************************************************

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PathfinderMG.Core;
using PathfinderMG.Core.Source.GUI;
using System;

namespace MonoGame_Textbox
{
    class TextBox : Control
    {
        public enum InputType
        {
            All = 0,
            NumbersOnly = 1,
            Alphanumeric = 2
        }

        public GraphicsDevice GraphicsDevice { get; set; }

        public new Rectangle Rectangle
        {
            get
            {
                Renderer.Rectangle = new Rectangle((int)Position.X, (int)Position.Y, Renderer.Rectangle.Width, Renderer.Rectangle.Height);
                return Renderer.Rectangle;
            }
            set 
            { 
                Dimensions = new Vector2(value.Width, value.Height);
                Position = new Vector2(value.X, value.Y);
                Renderer.Rectangle = value; 
            }
        }
        public readonly new Text Text;
        public readonly TextRenderer Renderer;
        public readonly Cursor Cursor;
        public bool Active { get; set; }
        public InputType InputAllowed  { get; set; }

        public event EventHandler<KeyboardInput.KeyEventArgs> EnterDown;
        public event EventHandler<string> InputChanged;

        private string clipboard;
        private Texture2D backgroundTex;
        private Color backgroundColor;

        public TextBox(Rectangle area, int maxCharacters, string text, GraphicsDevice graphicsDevice,
            SpriteFont spriteFont, InputType inputType,
            Color cursorColor, Color selectionColor, Color backgroundColor, int ticksPerToggle)
        {
            GraphicsDevice = graphicsDevice;
            InputAllowed = inputType;

            Text = new Text(maxCharacters)
            {
                String = text
            };

            Renderer = new TextRenderer(this)
            {
                Rectangle = area,
                Font = spriteFont,
                Color = Color.Black
            };

            Cursor = new Cursor(this, cursorColor, selectionColor, new Rectangle(0, 0, 1, 1), ticksPerToggle);
            Cursor.TextCursor = Text.Length;
            backgroundTex = GameRoot.ContentMgr.Load<Texture2D>("Controls/PanelBackground");
            this.backgroundColor = backgroundColor;

            KeyboardInput.CharPressed += CharacterTyped;
            KeyboardInput.KeyPressed += KeyPressed;
        }

        public void Dispose()
        {
            KeyboardInput.Dispose();
        }

        public void Clear()
        {
            Text.RemoveCharacters(0, Text.Length);
            Cursor.TextCursor = 0;
            Cursor.SelectedChar = null;
        }

        private void KeyPressed(object sender, KeyboardInput.KeyEventArgs e, KeyboardState ks)
        {
            if (Active)
            {
                int oldPos = Cursor.TextCursor;
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        EnterDown?.Invoke(this, e);
                        break;
                    case Keys.Left:
                        if (KeyboardInput.CtrlDown)
                        {
                            Cursor.TextCursor = IndexOfLastCharBeforeWhitespace(Cursor.TextCursor, Text.Characters);
                        }
                        else
                        {
                            Cursor.TextCursor--;
                        }
                        ShiftMod(oldPos);
                        break;
                    case Keys.Right:
                        if (KeyboardInput.CtrlDown)
                        {
                            Cursor.TextCursor = IndexOfNextCharAfterWhitespace(Cursor.TextCursor, Text.Characters);
                        }
                        else
                        {
                            Cursor.TextCursor++;
                        }
                        ShiftMod(oldPos);
                        break;
                    case Keys.Home:
                        Cursor.TextCursor = 0;
                        ShiftMod(oldPos);
                        break;
                    case Keys.End:
                        Cursor.TextCursor = Text.Length;
                        ShiftMod(oldPos);
                        break;
                    case Keys.Delete:
                        if (DelSelection() == null && Cursor.TextCursor < Text.Length)
                        {
                            Text.RemoveCharacters(Cursor.TextCursor, Cursor.TextCursor + 1);
                        }
                        break;
                    case Keys.Back:
                        if (DelSelection() == null && Cursor.TextCursor > 0)
                        {
                            Text.RemoveCharacters(Cursor.TextCursor - 1, Cursor.TextCursor);
                            Cursor.TextCursor--;
                        }
                        break;
                    case Keys.A:
                        if (KeyboardInput.CtrlDown)
                        {
                            if (Text.Length > 0)
                            {
                                Cursor.SelectedChar = 0;
                                Cursor.TextCursor = Text.Length;
                            }
                        }
                        break;
                    case Keys.C:
                        if (KeyboardInput.CtrlDown)
                        {
                            clipboard = DelSelection(true);
                        }
                        break;
                    case Keys.X:
                        if (KeyboardInput.CtrlDown)
                        {
                            if (Cursor.SelectedChar.HasValue)
                            {
                                clipboard = DelSelection();
                            }
                        }
                        break;
                    case Keys.V:
                        if (KeyboardInput.CtrlDown)
                        {
                            if (clipboard != null)
                            {
                                DelSelection();
                                foreach (char c in clipboard)
                                {
                                    if (Text.Length < Text.MaxLength)
                                    {
                                        Text.InsertCharacter(Cursor.TextCursor, c);
                                        Cursor.TextCursor++;
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }

        private void ShiftMod(int oldPos)
        {
            if (KeyboardInput.ShiftDown)
            {
                if (Cursor.SelectedChar == null)
                {
                    Cursor.SelectedChar = oldPos;
                }
            }
            else
            {
                Cursor.SelectedChar = null;
            }
        }

        private void CharacterTyped(object sender, KeyboardInput.CharacterEventArgs e, KeyboardState ks)
        {
            if (Active && !KeyboardInput.CtrlDown)
            {
                if (IsLegalCharacter(Renderer.Font, e.Character) && !e.Character.Equals('\r') &&
                    !e.Character.Equals('\n'))
                {
                    DelSelection();
                    if (Text.Length < Text.MaxLength)
                    {
                        if (InputAllowed == InputType.NumbersOnly && (e.Character > 57 || e.Character < 48))
                            return;
                        
                        Text.InsertCharacter(Cursor.TextCursor, e.Character);
                        Cursor.TextCursor++;
                        InputChanged?.Invoke(this, Text.String);
                    }
                }
            }
        }

        private string DelSelection(bool fakeForCopy = false)
        {
            if (!Cursor.SelectedChar.HasValue)
            {
                return null;
            }
            int tc = Cursor.TextCursor;
            int sc = Cursor.SelectedChar.Value;
            int min = Math.Min(sc, tc);
            int max = Math.Max(sc, tc);
            string result = Text.String.Substring(min, max - min);

            if (!fakeForCopy)
            {
                Text.Replace(Math.Min(sc, tc), Math.Max(sc, tc), string.Empty);
                if (Cursor.SelectedChar.Value < Cursor.TextCursor)
                {
                    Cursor.TextCursor -= tc - sc;
                }
                Cursor.SelectedChar = null;
            }
            return result;
        }

        public static bool IsLegalCharacter(SpriteFont font, char c)
        {
            return font.Characters.Contains(c) || c == '\r' || c == '\n';
        }

        public static int IndexOfNextCharAfterWhitespace(int pos, char[] characters)
        {
            char[] chars = characters;
            char c = chars[pos];
            bool whiteSpaceFound = false;
            while (true)
            {
                if (c.Equals(' '))
                {
                    whiteSpaceFound = true;
                }
                else if (whiteSpaceFound)
                {
                    return pos;
                }

                ++pos;
                if (pos >= chars.Length)
                {
                    return chars.Length;
                }
                c = chars[pos];
            }
        }

        public static int IndexOfLastCharBeforeWhitespace(int pos, char[] characters)
        {
            char[] chars = characters;

            bool charFound = false;
            while (true)
            {
                --pos;
                if (pos <= 0)
                {
                    return 0;
                }
                var c = chars[pos];

                if (c.Equals(' '))
                {
                    if (charFound)
                    {
                        return ++pos;
                    }
                }
                else
                {
                    charFound = true;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            // sskorka: Reenable textbox on click
            MouseState currentMouse = Mouse.GetState();
            Rectangle mouseRectangle = new Rectangle(currentMouse.X, currentMouse.Y, 1, 1);

            if (mouseRectangle.Intersects(Rectangle) && currentMouse.LeftButton == ButtonState.Pressed)
                Active = true;

            Renderer.Update();
            Cursor.Update();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // sskorka: Draw textbox rectangle first
            spriteBatch.Draw(backgroundTex, Rectangle, null, backgroundColor);

            Renderer.Draw(spriteBatch);
            if (Active)
            {
                Cursor.Draw(spriteBatch);
            }
        }
    }
}