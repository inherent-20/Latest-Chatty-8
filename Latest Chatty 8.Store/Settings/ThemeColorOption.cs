﻿using System;
using Windows.UI;

namespace Latest_Chatty_8.Settings
{
	public class ThemeColorOption
	{
		public string Name { get; private set; }

		public Color AccentBackgroundColor { get; private set; }

		public Color AccentForegroundColor { get; private set; }

		public Color WindowTitleBackgroundColor { get; private set; }

		public Color WindowTitleForegroundColor { get; private set; }

		public Color WindowTitleForegroundColorInactive { get; private set; }

		public ThemeColorOption(string name, Color accentBackground, Color accentForeground, Color windowTitleBackground, Color windowTitleForeground)
		{
			this.Name = name;
			this.AccentBackgroundColor = accentBackground;
			this.AccentForegroundColor = accentForeground;
			this.WindowTitleBackgroundColor = windowTitleBackground;
			this.WindowTitleForegroundColor = windowTitleForeground;
		}

		public ThemeColorOption(string name, Color accentBackground, Color accentForeground)
		{
			this.Name = name;
			this.AccentBackgroundColor = accentBackground;
			this.AccentForegroundColor = accentForeground;
			this.WindowTitleBackgroundColor = Color.FromArgb(accentBackground.A , (byte)Math.Max(accentBackground.R - 20, 0), (byte)Math.Max(accentBackground.G - 20, 0), (byte)Math.Max(accentBackground.B - 20, 0));
			this.WindowTitleForegroundColor = accentForeground;
			this.WindowTitleForegroundColorInactive = Color.FromArgb(accentForeground.A, (byte)Math.Max(accentForeground.R - 120, 0), (byte)Math.Max(accentForeground.G - 120, 0), (byte)Math.Max(accentForeground.B - 120, 0));
		}
	}
}