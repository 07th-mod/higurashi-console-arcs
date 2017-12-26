# Higurashi PS3-exclusive Arcs Patch for PC version

This patch aims to eventually port all the arcs from the PS3 version of Higurashi to Mangagamer's Steam release of Higurashi.  This is a large task and it will take a long time to port them all, so each arc will be rolled out one at a time.

## Current Status

| Chapter     | PC Port  | Translation | Editing |
| ----------- | -------- | ----------- | ------- |
| Someutsushi | 100%     | 100%        | 100%    | 
| Kageboushi  | 100%     | 100%        | 100%    | 

Someutsushi and Kageboushi are fully ported to PC and English translated, including the tips.  The latest release can be played in both ADV and NVL mode, so play however you like!  I'll spend some time working through issues in this release before beginning work on the next arc.

The old poll results can be found [here](https://strawpoll.com/ee8gge6).

## Screenshots

![](https://i.imgur.com/mBxKC8p.png)
![](https://i.imgur.com/A5Iym0R.png)


## Requirements
Installation requires a copy of Ch 4 Himatsubushi, either the Steam version or directly from Mangagamer's website.  The patch converts a Himatsubushi installation into one that can play all of the currently released console arcs.

### Manual Installation Instructions

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
10. You can play the colsole arcs by runnung ``HigurashiEp04.exe``, which is in the ``Higurashi 04 - Himatsubushi`` directory.
11. The first time you launch the application, it will spend 10-15 seconds processing the new script files before it loads.  I won't do this on subsequent launches.

## Recruiting

Looking for people interested in helping with the rest of the arcs!  I could use a more experienced translator or two to handle the other arcs, or fix any translation issues in the existing translation (This is the first time I've translated anything, so it's not the best). And it would be great to have an editor or two to fix any spelling/grammar errors.  Any help would be appreciated.
