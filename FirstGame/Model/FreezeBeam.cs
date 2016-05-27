using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstGame
{
	public class FreezeBeam
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

		// Determines how fast the projectile moves
		public float projectileMoveSpeed;

		public float ProjectileMoveSpeed
		{
			get { return projectileMoveSpeed; }
			set { projectileMoveSpeed = value; }
		}

		public ViewPort viewport;

		public bool Active;

		public bool Actile
		{
			get { return active; }
			set { active = value; }
		}
			
		public void Initialize(Viewport viewposrt, Texture2D texture, Vector2 position)
		{
			this.texture = texture;
			this.position = position;
			this.damage = 0; //2
			this.projectileMoveSpeed = 5f; //20
			this.viewport = viewport;
			this.active = true;
		}

		public void Update()
		{
			// Projectiles always move to the right
			position.X += projectileMoveSpeed;

			position.Y += 0.1;

			// Deactivate the bullet if it goes out of screen
			if (Position.X + Texture.Width / 2 > viewport.Width)
				Active = false;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, position, null, Color.White, position.Y*3f,Vector2.Zero, 2f, SpriteEffects.None, 0f);
		}
	}
}

