# HeatMargin

Heat Margin shows you where you've been editing your code!

Download it here: https://visualstudiogallery.msdn.microsoft.com/66a4194a-4271-493f-8d87-91052d7db09d

Visual studio has a feature built in to that shows you where you've edited your file (green for saved, yellow for unsaved). But over time, your entire document will show the green bar!

With heat Margin as you edit lines, indicators are added in the scroll bar and next to the line numbers (much like the default green and yellow makers you're used to).

The difference with Heat Margin is as you edit more lines, previously edited line indicators start to fade. You'll quickly start to build up an idea of where you are working!

Works with VS15RC and 2013.  

With special thanks to https://github.com/laurentkempe/GitDiffMargin for getting me quickly up to speed with VS extensions!

![Heat Margin Screeny](https://raw.githubusercontent.com/jakkaj/HeatMargin/master/Screenshots/VS2013_Screeny.jpg)


If for some reason you cannot get it to launch with the debugger attached, try going to Project Settings -> Debug and setting Start external program to "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\\devenv.exe" and command line arguments to "/rootSuffix Exp".
