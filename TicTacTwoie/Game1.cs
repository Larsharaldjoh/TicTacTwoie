using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace TicTacTwoie
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>

	enum PlayerState { POne, PTwo };
	enum EndGame { Blank, POne, PTwo, Draw };
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		SpriteFont font;


		List<Square> squareList;
		Texture2D boardText;
		Color boardColor;
		Vector2 boardSize;

		int turnCount;

		PlayerState playerState;
		EndGame endGame;
		MouseState mouseState;

		private const float delay = 150; // milliseconds
		private float remainingDelay = delay;


		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			graphics.IsFullScreen = false;
			Window.IsBorderless = false;
			graphics.PreferredBackBufferWidth = 800;
			graphics.PreferredBackBufferHeight = 600;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			IsMouseVisible = true;

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);


			font = Content.Load<SpriteFont>("Roboto");
			boardSize = new Vector2(4, 4);
			boardText = new Texture2D(GraphicsDevice, 1, 1);
			boardText.SetData(new Color[] { Color.White });
			boardColor = Color.Black;

			squareList = generateBoard();

			turnCount = 0;

		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
				this.Exit();


			mouseState = Mouse.GetState();

			Point mousePosition = Mouse.GetState().Position;

			float timer = (float)gameTime.ElapsedGameTime.Milliseconds;
			remainingDelay -= timer;

			if (turnCount <= (int)boardSize.X * boardSize.Y)
			{
				if (remainingDelay <= 0)
				{
					if (endGame.Equals(EndGame.Blank))
					{
						foreach (Square s in squareList)
						{
							if (intersects(s.rectangle, mousePosition) && mouseState.LeftButton == ButtonState.Pressed && s.getState() == State.Blank)
							{
								if (playerState.Equals(PlayerState.POne))
								{
									s.setState(State.X);
									playerState = PlayerState.PTwo;
								}
								else if (playerState.Equals(PlayerState.PTwo))
								{
									s.setState(State.O);
									playerState = PlayerState.POne;
								}

								turnCount++;
								remainingDelay = delay;
							}
						}
						if (turnCount > (boardSize.X - 1)*2 || turnCount > (boardSize.Y - 1)*2)
						{
							endGame = checkWin();
						}
						if (turnCount >= boardSize.X * boardSize.Y)
						{
							endGame = EndGame.Draw;
						}
					}
				}
			}
			else
				endGame = EndGame.Draw;
			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			drawGrid();

			cursorCoordinates();

			spriteBatch.Begin();
			spriteBatch.DrawString(font," THE WINNER IS " + endGame.ToString(), new Vector2(30, 30), Color.LawnGreen);
			spriteBatch.End();


			base.Draw(gameTime);
		}

		List<Square> generateBoard()
		{
			List<Square> gb = new List<Square>();
			for (int x = 1; x <= (int)boardSize.X; x++)
			{
				for (int y = 1; y <= (int)boardSize.Y; y++)
				{
					Square square = new Square(new Rectangle(
						GraphicsDevice.Viewport.Width / (int)boardSize.X * x 
							- GraphicsDevice.Viewport.Width / (int)boardSize.X,
						GraphicsDevice.Viewport.Height / (int)boardSize.Y * y 
							- GraphicsDevice.Viewport.Height / (int)boardSize.Y,

						GraphicsDevice.Viewport.Width / (int)boardSize.X,
						GraphicsDevice.Viewport.Height / (int)boardSize.Y), 
						y, 
						x);

					gb.Add(square);
				}
			}
			return gb;
		}

		public bool intersects(Rectangle rectangle, Point point)
		{
			return ((point.X >= rectangle.X && point.X <= rectangle.X + rectangle.Width) && (point.Y >= rectangle.Y && point.Y <= rectangle.Y + rectangle.Height));
		}


		private void cursorCoordinates()
		{
			Point mousePosition = Mouse.GetState().Position;
			spriteBatch.Begin();
			spriteBatch.DrawString(font, "X:" + mousePosition.X.ToString(), new Vector2(20, 45), Color.White);
			spriteBatch.DrawString(font, "Y:" + mousePosition.Y.ToString(), new Vector2(20, 60), Color.White);
			spriteBatch.End();
		}

		private void drawGrid()
		{
			spriteBatch.Begin();
			foreach (Square square in squareList)
			{   
				if (square.stateEquals(State.X))
					boardColor = Color.Red;
				else if (square.stateEquals(State.O))
					boardColor = Color.Blue;
				else
					boardColor = Color.Black;

				spriteBatch.Draw(boardText, square.rectangle, boardColor);
				DrawBorder(square.rectangle, 2, Color.Yellow);
			}
			spriteBatch.End();
		}

		private void DrawBorder(Rectangle rectangleToDraw, int thicknessOfBorder, Color borderColor)
		{
			// Draw top line
			spriteBatch.Draw(boardText, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Width, thicknessOfBorder), borderColor);

			// Draw left line
			spriteBatch.Draw(boardText, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, thicknessOfBorder, rectangleToDraw.Height), borderColor);

			// Draw right line
			spriteBatch.Draw(boardText, new Rectangle((rectangleToDraw.X + rectangleToDraw.Width - thicknessOfBorder),
											rectangleToDraw.Y,
											thicknessOfBorder,
											rectangleToDraw.Height), borderColor);
			// Draw bottom line
			spriteBatch.Draw(boardText, new Rectangle(rectangleToDraw.X,
											rectangleToDraw.Y + rectangleToDraw.Height - thicknessOfBorder,
											rectangleToDraw.Width,
											thicknessOfBorder), borderColor);
		}


		EndGame checkWin()     // I've commented the last (and the hardest, for me anyway) (char gameBoard[ROWS][COLS], int player)
		{                                                           // check, which checks for backwards diagonal runs below >>>
			int row = 0, col = 0, x = 0, o = 0; 

			for ( row = 1; row <= boardSize.X; row++)                       // This first for loop checks every row
			{
				for ( col = 1; col <= boardSize.Y; col++)           // And all columns until N away from the end
				{
					foreach (Square square in squareList)
					{
						if (square.column == col && square.row == row && (square.getState() == State.O || square.getState() == State.X))
						{
							if (square.getState() == State.O)
								o++;
							if (square.getState() == State.X)
								x++;
						}

						if (o == boardSize.X)
						{
								return EndGame.PTwo;
						}
						if (x == boardSize.X)
						{
								return EndGame.POne;
						}
					}
				}

				x = 0;
				o = 0;
			}
			for ( col = 1; col <= boardSize.Y; col++)                       // This one checks for columns of consecutive marks
			{
				for ( row = 1; row <= boardSize.X; row++)
				{
					foreach (Square square in squareList)
					{
						if (square.column == col && square.row == row && (square.getState() == State.O || square.getState() == State.X))
						{
							if (square.getState() == State.O)
								o++;
							if (square.getState() == State.X)
								x++;
						}

						if (o == boardSize.Y)
						{
							return EndGame.PTwo;
						}
						if (x == boardSize.Y)
						{
							return EndGame.POne;
						}
					}
				}

				x = 0;
				o = 0;
			}

			row = 1;
			for (col = 1; col <= boardSize.Y; col++)             // This one checks for "forwards" diagonal runs
			{
				foreach (Square square in squareList)
				{
					if (square.column == col && square.row == row && (square.getState() == State.O || square.getState() == State.X))
					{
						if (square.getState() == State.O)
							o++;
						if (square.getState() == State.X)
							x++;
					}

					if (o == boardSize.Y)
					{
						return EndGame.PTwo;
					}
					if (x == boardSize.Y)
					{
						return EndGame.POne;
					}
				}
				row++;
			}
			x = 0;
			o = 0;

			col = (int)boardSize.Y;
			for (row = 1; row <= boardSize.X; row++)             // This one checks for "forwards" diagonal runs
			{
				foreach (Square square in squareList)
				{
					if (square.column == col && square.row == row && (square.getState() == State.O || square.getState() == State.X))
					{
						if (square.getState() == State.O)
							o++;
						if (square.getState() == State.X)
							x++;
					}

					if (o == boardSize.Y)
					{
						return EndGame.PTwo;
					}
					if (x == boardSize.Y)
					{
						return EndGame.POne;
					}
				}
				col--;
			}
			x = 0;
			o = 0;

			return EndGame.Blank;                                           // If we got to here, no winner has been detected,
		}          
	}
}
