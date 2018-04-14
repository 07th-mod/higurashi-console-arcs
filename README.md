# Higurashi PS3-exclusive Arcs Patch for PC version

This patch aims to eventually port all the arcs from the PS3 version of Higurashi to Mangagamer's Steam release of Higurashi.  This is a large task and it will take a long time to port them all, so each arc will be rolled out one at a time.

Here's a [flowchart](https://ibb.co/dTRmmb) that explains where all these arc fit in with the main series and the recommended reading order.

## Current Status

| Chapter     | PC Port  | Translation | Editing |
| ----------- | -------- | ----------- | ------- |
| Someutsushi | 100%     | 100%        | 100%    | 
| Kageboushi  | 100%     | 100%        | 100%    | 
| Tsukiotoshi | 100%     |  99%        |   0%    | 

Tsukiotoshi has been ported to the PC engine.  Script translation is complete, but a few images still require translation.  Beginning editing now.

## Screenshots

![](https://i.imgur.com/mBxKC8p.png)
![](https://i.imgur.com/A5Iym0R.png)


## Requirements
Installation requires a copy of Ch 4 Himatsubushi, either the Steam version or the DRM-free version from Mangagamer's website.  The patch converts a Himatsubushi installation into one that can play both of the currently released console arcs.

## Installation Instructions
### Automated installer (Windows only)

The automated installer will automatically drop all the necessary files in the appropriate locations.  You just need to download the console arcs installer: [Here](https://github.com/07th-mod/resources/releases/download/installer/Console.Arcs.PS3.Voice.and.Graphics.Installer.exe)

After downloading the console arcs installer, run it and set the installation directory to your Himatsubushi Steam game directory (also works for MangaGamer's DRM free release). If you are unsure **where** the Steam game is located, find it in your Steam library, right-click on it and choose ``Properties``. In the new window that just opened, click on the ``LOCAL FILES`` tab and then on the ``Browse local files...`` button. Now you should be able to see where your game is located in Windows Explorer.

> **Protip**: copy the address bar in Windows Explorer and paste it in the installer!

After properly finding and directing the installation to the game's directory, hit the install button and wait. The first step of the installation should be pretty fast, and a CMD window (Windows command line) should open and do the rest of the installation automatically. When the process is finished, it'll display a message and close the window automatically. The game should be already patched and ready to play.  You can launch it by running ``HigurashiEp04.exe``, which is in the ``Higurashi 04 - Himatsubushi`` directory.

> **PSA**: as reported, some antivirus software might warn you about the installer. This is a false positive, and it happens because the installer runs a batch file (that results in the CMD window opening to download and install the patch). The installer code can be freely accessed in our [Resource](https://github.com/07th-mod/resources) repository and is clean of viruses.

### Manual Installation (Windows, Mac, linux)

1. Download all four of the following archives:
[BGM](https://github.com/07th-mod/resources/releases/download/Nipah/ConsoleArcs-BGM.zip), [CG](https://github.com/07th-mod/resources/releases/download/Nipah/ConsoleArcs-CG.zip), [SE](https://github.com/07th-mod/resources/releases/download/Nipah/ConsoleArcs-SE.zip), [voice](https://github.com/07th-mod/resources/releases/download/Nipah/ConsoleArcs-voice.zip)
2. Download the [latest version of the patch](https://github.com/07th-mod/higurashi-console-arcs/releases/latest)
3. Navigate to the ``Higurashi 04 - Himatsubushi\HigurashiEp04_Data\StreamingAssets`` directory on your local system
4. Delete the existing BGM, CG, and SE folders
5. Extract the Voice patch ``voice`` folder **inside** the ``StreamingAssets`` folder.
6. Extract the graphics patch ``CG`` folder **inside** the ``StreamingAssets`` folder.
7. Extract the music patch ``BGM`` folder **inside** the ``StreamingAssets`` folder.
8. Extract the sound effects patch ``SE`` folder **inside** the ``StreamingAssets`` folder.
9. Navigate back to ``\HigurashiEp04_Data`` and extract the patch ``StreamingAssets`` and ``Managed`` folders inside this folder.
10. You can play the console arcs by running ``HigurashiEp04.exe``, which is in the ``Higurashi 04 - Himatsubushi`` directory.
11. The first time you launch the application, it will spend 10-15 seconds processing the new script files before it loads.  It won't do this on subsequent launches.

## Recruiting

Looking for people interested in helping with the rest of the arcs!  I could use a more experienced translator or two to handle the other arcs, or fix any translation issues in the existing translation (This is the first time I've translated anything, so it's not the best). And it would be great to have an editor or two to fix any spelling/grammar errors.  Any help would be appreciated.
