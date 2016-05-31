using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace FirstGame
{
	public class DeathRay
	{
		// Image representing the Projectile
		public Texture2D texture;

		// Position of the Projectile relative to the upper left side of the screen
		public Vector2 position;

		public Vector2 Position
		{
			get { return position; }
			set { position = value; }
		}

		// The amount of damage the projectile can inflict to an enemy
		public int damage;

		public int Damage
		{
			get { return damage; }
			set { damage = value; }
		}

		public int Width
		{
			get { return texture.Width; }
		}

		public int Height
		{
			get { return texture.Height; }
		}

		// Determines how fast the projectile moves
		public float projectileMoveSpeed;

		public float ProjectileMoveSpeed
		{
			get { return projectileMoveSpeed; }
			set { projectileMoveSpeed = value; }
		}

		private Viewport viewport;

		public Viewport Viewport
		{
			get { return viewport; }
			set { viewport = value; }
		}

		public bool active;

		public bool Active
		{
			get { return active; }
			set { active = value; }
		}

		public void Initialize(Viewport viewport, Texture2D texture, Vector2 position)
		{
			this.texture = texture;
			this.position = position;
			this.damage = 500;
			this.projectileMoveSpeed = 20f; 
			this.viewport = viewport;
			this.active = true;
		}

		public void Update()
		{
			// Projectiles always move to the right
			position.X += projectileMoveSpeed;

			// Deactivate the bullet if it goes out of screen
			if (Position.X + texture.Width / 2 > viewport.Width) 
			{
				active = false;
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, position, null, Color.White, 0f, new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 0f);
		}
	}
}

