MineEdit Minecraft Editor
	Insomnia Edition

http://github.com/N3X15/MineEdit

By Rob "N3X15/Harbinger" Nelson

*** THIS SOFTWARE IS A WORK IN PROGRESS, KEEP UP TO DATE BY CHECKING GITHUB OR THE MINECRAFT FORUMS OFTEN. ***
*** THIS ALSO MEANS STUFF WILL BE BROKEN.  BE PATIENT, I'LL GET TO YOUR BUG/FEATURE REQUESTS EVENTUALLY. ***

I. LICENSE

  First and foremost, licensing.

  MineEdit has an MIT license, although I honestly really don't care what you do to it, as long as my name is somewhere on the end result of your tinkering.

  LibNBT, which is included with MineEdit, has a GPL license.
  
  I have no idea what lightgen is, Moose just kinda went "here" and was never heard from again.  

  We worked hard on this stuff, and don't want to see our creations twisted into some horrible monster that doesn't credit us and our work. So credit us and follow the appropriate licensing.

  See LICENSE.txt and LICENSE.LibNBT.txt for the full legalese.

II. INSTALLATION

  First, depending on your OS, you need either .NET 3.5 (Windows) or a recent Mono install (Linux/Mac).

  Then, just unzip and open MineEdit.exe by doubleclicking on it or, if you're on Linux/Mac, by using the included shell script (chmod +x if you have problems using it).

III. EDITING INVENTORY

	1. Open an existing Infdev map using the Open menu (CTRL+O)
	2. Go to the inventory tab (provided stuff hasn't crashed)
	3. Click on a slot you wish to edit. It will have an orange outline to show it as selected.
	4. Tinker with the settings to the left to set the settings for all the selected items.  
		NOTE: ALL PARTS OF THESE SETTINGS WILL BE APPLIED TO ALL SELECTED INVENTORY SLOTS.
		NOTE 2: Setting a part to AIR (0x00) or a count of 0 will delete it from inventory.
	5. Click Apply to Selected to apply.

