using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PathfinderMG.Core.Source
{
    /// <summary>
    /// Manages the mouse input.
    /// </summary>
    public class MouseManager
    {
        private MouseState previousMouse, currentMouse;

        #region Properties

        public MouseState PreviousMouse
        {
            get
            {
                return previousMouse;
            }
        }
        public MouseState CurrentMouse
        {
            get
            {
                return currentMouse;
            }
        }
        public Vector2 PreviousPosition
        {
            get
            {
                return new Vector2(previousMouse.X, previousMouse.Y);
            }
        }
        public Vector2 CurrentPosition
        {
            get
            {
                return new Vector2(currentMouse.X, currentMouse.Y);
            }
        }
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle(currentMouse.X, currentMouse.Y, 1, 1);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if mouse is hovering over provided area
        /// </summary>
        public bool Hovers(Rectangle rectangle)
        {
            return Rectangle.Intersects(rectangle);
        }

        /// <summary>
        /// Get distance between last and present mouse position.
        /// </summary>
        public Vector2 GetDragMovement()
        {
            return new Vector2(currentMouse.X - previousMouse.X, currentMouse.Y - previousMouse.Y);
        }

        public bool LeftButtonPressed()
        {
            return currentMouse.LeftButton == ButtonState.Pressed;
        }

        public bool LeftButtonClicked()
        {
            return (currentMouse.LeftButton == ButtonState.Released && previousMouse.LeftButton == ButtonState.Pressed);
        }

        public bool LeftButtonReleased()
        {
            return (previousMouse.LeftButton == ButtonState.Released && currentMouse.LeftButton == ButtonState.Pressed);
        }

        public bool LeftButtonHeld()
        {
            return (previousMouse.LeftButton == ButtonState.Pressed && currentMouse.LeftButton == ButtonState.Pressed);
        }

        public bool RightButtonPressed()
        {
            return currentMouse.RightButton == ButtonState.Pressed;
        }

        public bool RightButtonClicked()
        {
            return (currentMouse.RightButton == ButtonState.Released && previousMouse.RightButton == ButtonState.Pressed);
        }

        public bool RightButtonReleased()
        {
            return (previousMouse.RightButton == ButtonState.Released && currentMouse.RightButton == ButtonState.Pressed);
        }

        public bool RightButtonHeld()
        {
            return (previousMouse.RightButton == ButtonState.Pressed && currentMouse.RightButton == ButtonState.Pressed);
        }

        #endregion

        public void Update()
        {
            previousMouse = currentMouse;
            currentMouse = Mouse.GetState();
        }
    }
}
