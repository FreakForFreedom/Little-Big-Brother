# Little-Big-Brother
Initial version of the Big Brother A.I. I developed for my master's thesis.

This is not the complete project from my thesis, just the A.I. part. It works by quantifying certain aspects of the player's playstyle which are read from an external xml file and then getting the current game world's state (with all "watchable" objects) and comparing it against preset 'laws' or 'story elements'. These have specific criteria which must be met so they can be applied to the game world.

It is heavily inspired by Valve's director A.I. for Left4Dead.

And of course... there is a lot of room for improvement. But it works and I am proud I managed to get it working this far.
If my little game Proles (for which I have developped this A.I.) should ever be in a playable state, then I will update this repo to the newest version.

BigBrother.cs -> Container class which adds the Consciousness.css, Personality.cs and Memory.cs as components to itself.

Consciousness.cs -> Contains event listener for telling the player what's what.

Personality.cs -> Still only a dummy, but in later versions Big Brother will have traits and criteria itself which define how it will react on different events.

Memory.cs -> Takes whatever object is thrown in it's general direction and puts it into an appropriate list so it can be watched with some black magic generic method voodoo.

LawManager.cs -> Responsible for checking Law.cs with the state of the game world and than calling the respective function (defined in the LawResponses.lua)

StoryElement.cs -> Quite similar to the law manager.

HPlayerModel.cs -> Class for 'traits' that should be monitored.

This is the update process:

![screenshot](http://notenoughsleep.eu/files/screenshots/ma/Überprüfungsprozess.png)

And some screens (which won't really tell you anything other then it works for me ;) )
![screenshot](http://notenoughsleep.eu/files/screenshots/ma/server1.png)
![screenshot](http://notenoughsleep.eu/files/screenshots/ma/server2.png)

## License

Copyright (C) 2016 Mischa J. P. Wasmuth

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
