using Foundation;

namespace SpriteKitGame
{
	partial class GameViewController
	{
		[Outlet]
		SpriteKit.SKView GameView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (GameView != null) {
				GameView.Dispose ();
				GameView = null;
			}
		}
	}
}


