Lenticular printing can create beautiful 3D or animated images, but it requires careful calibration to ensure that the interlacing of the frames of the image, as printed, match the measurements of the lenticular lens.

I've just started experimenting with making these images, and this library comes out of my frustration at not being able to find good, \*_free_ software to calibrate my printer for the lenticular sheets I bought online (which are _theoretically_ 60lpi).

After fiddling around in PhotoShop and deciding that was too time-consuming, I decided to make an app to generate a calibration image, which I could then print in PhotoShop and test.

This is a command-line application, written in .NET Core. It should compiled and work on Windows, Linux, and MacOS (I use a Mac). The only non-framework dependency is ImageSharp, which seems to be fairly new, but it's far better than the old System.Drawing library, which is old and crusty and doesn't seem to work well outside of Windows.

Use `dotnet run` to execute the application. Optional parameters can be used to tweak the output. To pass arguments, just pass each of these numbers (there are no "--parameter" or "-parameter" style prefixes):

- Width, in inches (default: 8)
- Height, in inches (default: 8),
- DPI of your printer (default: 600)
- LPI of your lenticular sheets (default: 60)
- Step rate of the text, in 100ths of a a line, default 100 (i.e, jumps 1 LPI per test)
- Number of test bars to generate on either side of the nominal LPI (default: 5, resulting in 11 bars)

This will generate a PNG file with a bar of lines for each test, where the lines are spaced to create the test pattern. There are sites online explaining how to use these test sheets, so I won't belabor that, other than to mention that it's important that you _do not scale_ the graphic when you print it -- it must be printed at exactly the dimensions you selected. PhotoShop makes this easy, just select "Center," and before you print, use the Canvas Size to match the DPI there to the DPI used to generate the file.

So far, this is only designed to create test patterns for stereographs (two images per lenticule). It could be expanded for other purposes, but I wanted to get this part right before refactoring it for more frames.

Assuming I can get these prints actually working, I'll probably also enhance this application to do my interlacing for me, using high-resolution source files. If you're looking for a free app for this, please feel free to jump in and help make that part happen.
