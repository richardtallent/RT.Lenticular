using System;
using System.Collections.Generic;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.Shapes;

namespace RT.Lenticular {
	class Program {
		static void Main(string[] args) {
			var c = new Calibration()
			{
				WidthInches = getIntArg(args, 0, 8),
				HeightInches = getIntArg(args, 1, 8),
				DPI = getIntArg(args, 2, 600),
				LPI = getIntArg(args, 3, 60),
				StepCentiLPI = getIntArg(args, 4, 100),
				NumSteps = getIntArg(args, 5, 10)
			};
			Console.WriteLine($"{c.WidthInches} {c.HeightInches} {c.DPI} {c.LPI} {c.StepCentiLPI} {c.NumSteps}");
			Console.WriteLine("Creating image");
			using (var img = new Image<Rgba32>(null, c.WidthInches * c.DPI, c.HeightInches * c.DPI, Rgba32.White)) {
				// Height in pixels of each block of lines
				var lines = new List<int[]>();
				var h = c.HeightInches * c.DPI / (c.NumSteps * 2 + 1);
				var marginInches = 0.5f;
				var font = SixLabors.Fonts.SystemFonts.CreateFont("Arial", h / 2, FontStyle.Regular);
				//var fontBrush = new SolidBrush(Color.Black);
				for (var i = -c.NumSteps; i <= c.NumSteps; i++) {
					float thisCentiLPI = c.LPI * 100f + (i * c.StepCentiLPI);
					var pixelsPerLine = (int)Math.Round(c.DPI / (thisCentiLPI / 100));
					var p = new Pen(Color.Black, (float)pixelsPerLine);
					var numLines = (c.WidthInches - marginInches) * c.DPI / 2 / pixelsPerLine;
					var y = (i + c.NumSteps) * h;
					Console.WriteLine($"Step: {pixelsPerLine} {numLines} {y}");
					img.Mutate(ctx => ctx.DrawText(
						$"{thisCentiLPI / 100}",
						font,
						Rgba32.Black,
						new PointF(marginInches * c.DPI / 10, y + h / 5)
					));
					//img.DrawString($" Fooooo", font, fontBrush, new Point(c.DPI / 15, y + h / 10));
					for (var j = 0; j < numLines; j++) {
						var x = j * pixelsPerLine * 2 + (int)(marginInches * c.DPI);
						var lh = (int)((float)h * .8);
						//Console.WriteLine($"Line: {i} {j} {h} {x} {y} {y + lh}");
						img.Mutate(ctx => ctx.DrawLines(
							new GraphicsOptions(false),
							Rgba32.Black,
							pixelsPerLine,
							new PointF(x, y),
							new PointF(x, y + lh)
						));
					}
				}
				img.Save("test.png");
			}
		}

		public static int getIntArg(string[] args, int place, int def) =>
			(place > args.GetUpperBound(0)) ? def : Int32.Parse(args[place]);

	}

	class Calibration {
		public int WidthInches { get; set; } = 8;
		public int HeightInches { get; set; } = 8;
		public int DPI { get; set; } = 600;
		public int LPI { get; set; } = 60;
		public int StepCentiLPI { get; set; } = 10;    // 100ths of line per inch to increment test
		public int NumSteps { get; set; } = 4;          // On either side of the nominal side
	}

}
