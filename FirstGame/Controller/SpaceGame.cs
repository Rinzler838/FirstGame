using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using FirstGame.Model;
using FirstGame.View;

namespace FirstGame.Controller
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class SpaceGame : Game
	{
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;

		private Player player;

		// Keyboard states used to determine key presses
		private KeyboardState currentKeyboardState;
		private KeyboardState previousKeyboardState;

		// Gamepad states used to determine button presses
		private GamePadState currentGamePadState;
		private GamePadState previousGamePadState; 

		// A movement speed for the player
		private float playerMoveSpeed;

		// Image used to display the static background
		private Texture2D mainBackground;

		// Parallaxing Layers
		private ParallaxingBackground bgLayer1;
		private ParallaxingBackground bgLayer2;

		// Enemies
		private Texture2D enemyTexture;
		private List<Enemy> enemies;

		// The rate at which the enemies appear
		private TimeSpan enemySpawnTime;
		private TimeSpan previousSpawnTime;

		// A random number generator
		private Random random;

		private Texture2D projectileTexture;
		private List<Projectile> projectiles;

		// The rate of fire of the player laser
		private TimeSpan fireTime;
		private TimeSpan previousFireTime;

		private Texture2D explosionTexture;
		private List<Animation> explosions;

		// The sound that is played when a laser is fired
		private SoundEffect laserSound;

		// The sound used when the player or an enemy dies
		private SoundEffect explosionSound;

		// The music played during gameplay
		private Song gameplayMusic;

		//Number that holds the player score
		private long score;
		// The font used to display UI elements
		private SpriteFont font;

		private Texture2D freezeTexture;
		private TimeSpan freezeTime;
		private TimeSpan previousFreezeTime;
		private List<FreezeBeam> freezeProjectiles;

		private Texture2D rayTexture;
		private TimeSpan rayTime;
		private TimeSpan previousRayTime;
		private List<DeathRay> rayProjectiles;


		public SpaceGame ()
		{
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			// TODO: Add your initialization logic here
			// Initialize the player class
			player = new Player ();

			// Set a constant player move speed
			playerMoveSpeed = 5f; //4.5

			//Initialize background layers
			bgLayer1 = new ParallaxingBackground();
			bgLayer2 = new ParallaxingBackground();

			// Initialize the enemies list
			enemies = new List<Enemy> ();

			// Set the time keepers to zero
			previousSpawnTime = TimeSpan.Zero;

			// Used to determine how fast enemy respawns
			enemySpawnTime = TimeSpan.FromSeconds(0.5f); //1

			// Initialize our random number generator
			random = new Random();

			projectiles = new List<Projectile>();

			freezeProjectiles = new List<FreezeBeam>();
			freezeTime = TimeSpan.FromSeconds (0.5f);

			rayProjectiles = new List<DeathRay>();
			rayTime = TimeSpan.FromSeconds (0.05f);

			// Set the laser to fire every quarter second
			fireTime = TimeSpan.FromSeconds(0.15f); //0.15

			explosions = new List<Animation>();

			//Set player's score to zero
			score = 0;
            
			base.Initialize ();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);

			//TODO: use this.Content to load your game content here

			// Load the player resources
			Animation playerAnimation = new Animation();
			Texture2D playerTexture = Content.Load<Texture2D>("Animation/ImportedMetroid");
			playerAnimation.Initialize(playerTexture, Vector2.Zero, 78, 86, 20, 60, Color.White, 1f, true);

			Vector2 playerPosition = new Vector2 (GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y
				+ GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
			player.Initialize(playerAnimation, playerPosition);

			// Load the parallaxing background
			bgLayer1.Initialize(Content, "Texture/ImportedBGTwo", GraphicsDevice.Viewport.Width, -15); //-1
			bgLayer2.Initialize(Content, "Texture/ImportedBGOne", GraphicsDevice.Viewport.Width, -25); //-2

			mainBackground = Content.Load<Texture2D>("Texture/ImportedMain");

			enemyTexture = Content.Load<Texture2D>("Animation/ImportedParasite");

			projectileTexture = Content.Load<Texture2D>("Texture/ImportedLazer");

			freezeTexture = Content.Load<Texture2D>("Texture/ImportedFreeze");

			rayTexture = Content.Load<Texture2D> ("Texture/ImportedLaser");

			explosionTexture = Content.Load<Texture2D>("Animation/ImportedExplode");

			// Load the music
			gameplayMusic = Content.Load<Song>("Sound/ridley");

			// Load the laser and explosion sound effect
			laserSound = Content.Load<SoundEffect>("Sound/laserFire");
			explosionSound = Content.Load<SoundEffect>("Sound/explosion");

			// Start the music right away
			PlayMusic(gameplayMusic);

			// Load the score font
			font = Content.Load<SpriteFont>("Font/gameFont");
		}

		private void PlayMusic(Song song)
		{
			// Due to the way the MediaPlayer plays music,
			// we have to catch the exception. Music will play when the game is not tethered
			try
			{
				// Play the music
				MediaPlayer.Play(song);

				// Loop the currently playing song
				MediaPlayer.IsRepeating = true;
			}
			catch { }
		}

		private void UpdatePlayer(GameTime gameTime)
		{
			player.Update(gameTime);

			// Get Thumbstick Controls
			player.Position.X += currentGamePadState.ThumbSticks.Left.X *playerMoveSpeed;
			player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y *playerMoveSpeed;

			// Use the Keyboard / Dpad
			if (currentKeyboardState.IsKeyDown(Keys.Left) || currentGamePadState.DPad.Left == ButtonState.Pressed)
			{
				player.Position.X -= playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Right) || currentGamePadState.DPad.Right == ButtonState.Pressed)
			{
				player.Position.X += playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Up) ||
				currentGamePadState.DPad.Up == ButtonState.Pressed)
			{
				player.Position.Y -= playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Down) ||
				currentGamePadState.DPad.Down == ButtonState.Pressed)
			{
				player.Position.Y += playerMoveSpeed;
			}

			// Make sure that the player does not go out of bounds
			player.Position.X = MathHelper.Clamp(player.Position.X, player.Width/2,GraphicsDevice.Viewport.Width - player.Width/2);
			player.Position.Y = MathHelper.Clamp(player.Position.Y, player.Height/2,GraphicsDevice.Viewport.Height - player.Height);

			// reset score if player health goes to zero
			if (player.Health <= 0) 
			{
				player.Health = 100;
				score = 0;
			}

			if (gameTime.TotalGameTime - previousFireTime > fireTime && currentKeyboardState.IsKeyDown ((Keys.G)))
			{
				previousFireTime = gameTime.TotalGameTime;
				AddProjectile(player.Position + new Vector2(player.Width-50, 0));
				laserSound.Play();
			}

			if (gameTime.TotalGameTime - previousFreezeTime > freezeTime && currentKeyboardState.IsKeyDown ((Keys.F)))
			{
				previousFreezeTime = gameTime.TotalGameTime;
				AddFreezeBeam(player.Position + new Vector2(player.Width-50, 0));
				laserSound.Play();
			}

			if (gameTime.TotalGameTime - previousRayTime > rayTime && currentKeyboardState.IsKeyDown ((Keys.D)))
			{
				previousRayTime = gameTime.TotalGameTime;
				AddDeathRay(player.Position + new Vector2(player.Width-50, 0));
				laserSound.Play();
			}
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
			#if !__IOS__ &&  !__TVOS__
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState ().IsKeyDown (Keys.Escape))
				Exit ();
			#endif
            
			// TODO: Add your update logic here

			// Save the previous state of the keyboard and game pad so we can determinesingle key/button presses
			previousGamePadState = currentGamePadState;
			previousKeyboardState = currentKeyboardState;

			// Read the current state of the keyboard and gamepad and store it
			currentKeyboardState = Keyboard.GetState();
			currentGamePadState = GamePad.GetState(PlayerIndex.One);

			//Update the player
			UpdatePlayer (gameTime);

			// Update the parallaxing background
			bgLayer1.Update();
			bgLayer2.Update();

			// Update the enemies
			UpdateEnemies(gameTime);

			// Update the collision
			UpdateCollision();

			// Update the projectiles
			UpdateProjectiles();

			// Update the explosions
			UpdateExplosions(gameTime);

			UpdateFreezeBeams();

			UpdateDeathRays();
            
			base.Update (gameTime);
		}

		private void UpdateCollision()
		{
			// Use the Rectangle's built-in intersect function to 
			// determine if two objects are overlapping
			Rectangle rectangle1;
			Rectangle rectangle2;

			// Only create the rectangle once for the player
			rectangle1 = new Rectangle((int)player.Position.X,
				(int)player.Position.Y,
				player.Width-10,
				player.Height-10);

			// Do the collision between the player and the enemies
			for (int i = 0; i <enemies.Count; i++)
			{
				rectangle2 = new Rectangle((int)enemies[i].Position.X,
					(int)enemies[i].Position.Y,
					enemies[i].Width-10,
					enemies[i].Height-10);

				// Determine if the two objects collided with each
				// other
				if(rectangle1.Intersects(rectangle2))
				{
					// Subtract the health from the player based on
					// the enemy damage
					player.Health -= enemies[i].Damage;

					// Since the enemy collided with the player
					// destroy it
					enemies[i].Health = 0;

					// If the player health is less than zero we died
					if (player.Health <= 0)
						player.Active = false; 
				}

			}

			// Projectile vs Enemy Collision
			for (int i = 0; i < projectiles.Count; i++)
			{
				for (int j = 0; j < enemies.Count; j++)
				{
					// Create the rectangles we need to determine if we collided with each other
					rectangle1 = new Rectangle((int)projectiles[i].Position.X - 
						projectiles[i].Width / 2,(int)projectiles[i].Position.Y - 
						projectiles[i].Height / 2,projectiles[i].Width, projectiles[i].Height);

					rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2,
						(int)enemies[j].Position.Y - enemies[j].Height / 2,
						enemies[j].Width, enemies[j].Height);

					// Determine if the two objects collided with each other
					if (rectangle1.Intersects(rectangle2))
					{
						enemies[j].Health -= projectiles[i].Damage;
						projectiles[i].Active = false;
					}
				}
			}

			for (int i = 0; i < freezeProjectiles.Count; i++)
			{
				for (int j = 0; j < enemies.Count; j++)
				{
					// Create the rectangles we need to determine if we collided with each other
					rectangle1 = new Rectangle((int)freezeProjectiles[i].Position.X - 
						freezeProjectiles[i].Width / 2,(int)freezeProjectiles[i].Position.Y - 
						freezeProjectiles[i].Height / 2,freezeProjectiles[i].Width, freezeProjectiles[i].Height);

					rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2,
						(int)enemies[j].Position.Y - enemies[j].Height / 2,
						enemies[j].Width, enemies[j].Height);

					// Determine if the two objects collided with each other
					if (rectangle1.Intersects(rectangle2))
					{
						enemies[j].Health -= freezeProjectiles[i].Damage;
						enemies[j].EnemyMoveSpeed = 0;
						freezeProjectiles[i].active = false;
					}
				}
			}

			for (int i = 0; i < rayProjectiles.Count; i++)
			{
				for (int j = 0; j < enemies.Count; j++)
				{
					// Create the rectangles we need to determine if we collided with each other
					rectangle1 = new Rectangle((int)rayProjectiles[i].Position.X - 
						rayProjectiles[i].Width / 2,(int)rayProjectiles[i].Position.Y - 
						rayProjectiles[i].Height / 2,rayProjectiles[i].Width, rayProjectiles[i].Height);

					rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2,
						(int)enemies[j].Position.Y - enemies[j].Height / 2,
						enemies[j].Width, enemies[j].Height);

					// Determine if the two objects collided with each other
					if (rectangle1.Intersects(rectangle2))
					{
						enemies[j].Health -= rayProjectiles[i].Damage;
						enemies[j].Damage = 0;
						rayProjectiles[i].active = false;
					}
				}
			}
		}

		private void AddEnemy()
		{ 
			// Create the animation object
			Animation enemyAnimation = new Animation();

			// Initialize the animation with the correct animation information
			enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 80, 80, 8, 60,Color.White, 1f, true);

			// Randomly generate the position of the enemy
			Vector2 position = new Vector2(GraphicsDevice.Viewport.Width +enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height -100));

			// Create an enemy
			Enemy enemy = new Enemy();

			// Initialize the enemy
			enemy.Initialize(enemyAnimation, position); 

			// Add the enemy to the active enemies list
			enemies.Add(enemy);
		}

		private void UpdateEnemies(GameTime gameTime)
		{
			// Spawn a new enemy enemy every 1.5 seconds
			if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime) 
			{
				previousSpawnTime = gameTime.TotalGameTime;

				// Add an Enemy
				AddEnemy();
			}

			// Update the Enemies
			for (int i = enemies.Count - 1; i >= 0; i--) 
			{
				enemies[i].Update(gameTime);

				if (enemies[i].Active == false)
				{
					// If not active and health <= 0
					if (enemies[i].Health <= 0)
					{
						// Add an explosion
						AddExplosion(enemies[i].Position);
						// Play the explosion sound
						explosionSound.Play();
						//Add to the player's score
						score += enemies[i].Value;
					}
					enemies.RemoveAt(i);
				} 
			}
		}

		private void AddProjectile(Vector2 position)
		{
			Projectile projectile = new Projectile(); 
			projectile.Initialize(GraphicsDevice.Viewport, projectileTexture,position); 
			projectiles.Add(projectile);
		}

		private void UpdateProjectiles()
		{
			// Update the Projectiles
			for (int i = projectiles.Count - 1; i >= 0; i--) 
			{
				projectiles[i].Update();

				if (projectiles[i].Active == false)
				{
					projectiles.RemoveAt(i);
				} 
			}
		}

		private void AddExplosion(Vector2 position)
		{
			Animation explosion = new Animation();
			explosion.Initialize(explosionTexture,position, 160, 160, 5, 45, Color.White, 1f,false);
			explosions.Add(explosion);
		}

		private void UpdateExplosions(GameTime gameTime)
		{
			for (int i = explosions.Count - 1; i >= 0; i--)
			{
				explosions[i].Update(gameTime);
				if (explosions[i].Active == false)
				{
					explosions.RemoveAt(i);
				}
			}
		}

		private void AddFreezeBeam(Vector2 position)
		{
			FreezeBeam freezeBeam = new FreezeBeam();
			freezeBeam.Initialize(GraphicsDevice.Viewport, freezeTexture, position);
			freezeProjectiles.Add(freezeBeam);
		}

		private void UpdateFreezeBeams()
		{
			// Update the Projectiles
			for (int i = freezeProjectiles.Count - 1; i >= 0; i--) 
			{
				freezeProjectiles[i].Update();

				if (freezeProjectiles[i].active == false)
				{
					freezeProjectiles.RemoveAt(i);
				} 
			}
		}

		private void AddDeathRay(Vector2 position)
		{
			DeathRay deathRay = new DeathRay();
			deathRay.Initialize(GraphicsDevice.Viewport, rayTexture, position);
			rayProjectiles.Add(deathRay);
		}

		private void UpdateDeathRays()
		{
			// Update the Projectiles
			for (int i = rayProjectiles.Count - 1; i >= 0; i--) 
			{
				rayProjectiles[i].Update();

				if (rayProjectiles[i].active == false)
				{
					rayProjectiles.RemoveAt(i);
				} 
			}
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.White);
			//TODO: Add your drawing code here
			// Start drawing
			spriteBatch.Begin();

			spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);

			// Draw the moving background
			bgLayer1.Draw(spriteBatch);
			bgLayer2.Draw(spriteBatch);

			// Draw the Enemies
			for (int i = 0; i < enemies.Count; i++)
			{
				enemies[i].Draw(spriteBatch);
			}

			// Draw the Projectiles
			for (int i = 0; i < projectiles.Count; i++)
			{
				projectiles[i].Draw(spriteBatch);
			}

			// Draw the explosions
			for (int i = 0; i < explosions.Count; i++)
			{
				explosions[i].Draw(spriteBatch);
			}

			for (int index = 0; index < freezeProjectiles.Count; index++)
			{
				freezeProjectiles [index].Draw (spriteBatch);		
			}

			for (int index = 0; index < rayProjectiles.Count; index++)
			{
				rayProjectiles [index].Draw (spriteBatch);		
			}
				
			// Draw the score
			spriteBatch.DrawString(font, "SCORE: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
			// Draw the player health
			spriteBatch.DrawString(font, "HP: " + player.Health, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);

			spriteBatch.DrawString(font, "G: Regular", new Vector2(650, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
			spriteBatch.DrawString(font, "F: Freeze", new Vector2(650, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);
			spriteBatch.DrawString(font, "D: Death", new Vector2(650, GraphicsDevice.Viewport.TitleSafeArea.Y + 60), Color.White);

			// Draw the Player
			player.Draw(spriteBatch);

			// Stop drawing
			spriteBatch.End();

			base.Draw (gameTime);
		}
	}
}

