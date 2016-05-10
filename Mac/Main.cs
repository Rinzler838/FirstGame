using System;

using AppKit;

namespace FirstGame.Mac
{
	static class MainClass
	{
		static void Main (string[] args)
		{
			NSApplication.Init ();
			NSApplication.Main (args);
		}
	}
}

