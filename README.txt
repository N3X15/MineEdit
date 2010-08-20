MineEdit v1.0
	by N3X15 and copyboy

== Compiling ==

=== Windows Compile ===

Make sure you have .NET 3.5 installed.

Then, just doubleclick on one of the runprebuild batchfiles, and then run the compile batchfile that appears.

=== Linux/Mac Compile ===

Ensure you have a recent mono with GDI+, winforms.  You'll also need to install nant or monodevelop.  XCode may work, as well, but it's not supported.

Then do the following in the terminal:

cd /path/to/code
./prebuild.sh && make