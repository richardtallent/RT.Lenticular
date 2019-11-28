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
			var widthInches = getIntArg(args, 0, 8);
			var heightInches = getIntArg(args, 1, 8);
			var dpi = getIntArg(args, 2, 600);
			var lpi = getIntArg(args, 3, 60);
			var stepLPTI = getIntArg(args, 4, 100);     // default 1/10th of a line per inch per row
			var numSteps = getIntArg(args, 5, 10);      // number of variations on each side of LPI
			var frameCount = getIntArg(args, 6, 2);     // number of frames
			var rowHeightPx = heightInches * dpi / (numSteps * 2 + 1);
			var fontSizePx = rowHeightPx / 2;
			var textWidthInches = 1;                    // Bit of a hack, should be based on font size
			var usedWidthInches = widthInches - textWidthInches;
			var font = SixLabors.Fonts.SystemFonts.CreateFont("Arial", rowHeightPx / 2);
			var colors = new[] {
				Rgba32.Black, Rgba32.White,
				Rgba32.Red, Rgba32.Green, Rgba32.Blue,
				Rgba32.Yellow, Rgba32.Teal, Rgba32.Purple,
				Rgba32.Orange, Rgba32.Turquoise, Rgba32.Violet
			};
			Console.WriteLine($"{widthInches}x{heightInches}, {dpi} DPI, {lpi} LPI, {stepLPTI} LPTI/step, {numSteps} steps, {frameCount} frames");
			// Starting position for the row
			var y = 0;
			// Lines per thousand inches for this step
			var lpti = lpi * 1000 + (-numSteps * stepLPTI);
			// Height of the actual stripe, leaving a gutter between rows
			var lineHeight = (int)((float)rowHeightPx * .8);
			Console.WriteLine($"{lpti}, {lineHeight}, {fontSizePx}, {textWidthInches}");
			using (var img = new Image<Rgba32>(null, widthInches * dpi, heightInches * dpi, Rgba32.White)) {
				for (var i = -numSteps; i <= numSteps; i++) {
					// Label the row, centering the text vertically
					img.Mutate(ctx => ctx.DrawText(
						$"  {(float)lpti / 1000f}",
						font,
						Rgba32.Black,
						new PointF(0, y + (rowHeightPx - fontSizePx) / 2)
					));
					// Draw the stripes
					foreach (var l in LenticuleMaker.Lenticules(usedWidthInches, dpi, lpti, frameCount)) {
						img.Mutate(ctx => ctx.Fill(
							new GraphicsOptions(false),
							colors[l.Frame - 1],
							new SixLabors.Primitives.Rectangle(l.Left + textWidthInches * dpi, y, l.Right - l.Left, lineHeight)
						));
					}
					// Increment for the next step
					y += rowHeightPx;
					lpti += stepLPTI;
				}
				img.Save("test.png");
			}
		}

		public static int getIntArg(string[] args, int place, int def) =>
			(place > args.GetUpperBound(0)) ? def : Int32.Parse(args[place]);

	}

	// Stripe
	struct Lenticule {
		public int Left { get; set; }
		public int Right { get; set; }
		public int Frame { get; set; }
	}

	// Given a width, height, DPI, MLPI, and frame count, emits a series of points containing the
	// rectangles to be drawn (in pixels) and the frame number to draw them from. Math is done in
	// 100,000ths of an inch and returned in whole pixels. LPTI = lines per *thousand* inches
	// (avoids the need for floating point math).
	static class LenticuleMaker {

		public static IEnumerable<Lenticule> Lenticules(
			int WidthInches,
			int DPI,
			int LPTI,
			int FrameCount
		) {
			const int hundredk = 100000;
			var width = WidthInches * DPI * hundredk;
			var left = 0;
			var right = 0;
			var frameIndex = 0;
			var milliPixelsPerLine = DPI * hundredk / FrameCount / LPTI * 1000;
			while (right < width) {
				right += milliPixelsPerLine;
				frameIndex++;
				yield return new Lenticule()
				{
					Left = left / hundredk,
					Right = (right % width) / hundredk,
					Frame = frameIndex % FrameCount + 1
				};
				left = right;
			}
		}

	}

}
