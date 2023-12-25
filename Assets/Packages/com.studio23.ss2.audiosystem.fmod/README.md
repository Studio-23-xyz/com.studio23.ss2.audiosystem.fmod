<h1 align="center">Audio System FMOD</h1>
<p align="center">
<a href="https://openupm.com/packages/com.studio23.ss2.audiosystem.fmod/"><img src="https://img.shields.io/npm/v/com.studio23.ss2.audiosystem.fmod?label=openupm&amp;registry_uri=https://package.openupm.com" /></a>
</p>

Audio System FMOD is a tool designed for managing and utilizing the FMOD engine in a simpler and unified manner within Unity. It makes it easier to manage all game audio using and allows the user to focus more on audio and sound design.

## Table of Contents

1. [Installation](#installation)
2. [Setup](#setup)
    - [Important](#important)
    - [FMOD Settings in Unity](#fmod-settings-in-unity)
        - [Bank Import](#bank-import)
        - [Initialization](#initialization)
	- [Renaming FMOD Project Locales](#renaming-fmod-project-settings) 
    - [Generating all FMOD data](#generating-all-fmod-data)
3. [Usage](#usage)
    - [FMOD Manager](#fmod-manager)
    - [CallBack Handlers](#callback-handlers)  


## Installation

### Install via Git URL

You can also use the "Install from Git URL" option from Unity Package Manager to install the package.
```
https://github.com/Studio-23-xyz/com.studio23.ss2.audiosystem.fmod.git#upm
```

## Setup

### Important

This package cannot function on its own and is intended to be used with FMOD. Make sure you have the FMOD for Unity plugin installed in your project before installing Audio System FMOD. You can find the plugin on the Unity Asset Store or on the [FMOD website](https://www.fmod.com/download). This package assumes familiarity with FMOD for its usage so please read up of FMOD's documentation.


### FMOD Settings in Unity

#### Bank Import

Follow FMOD's documentation on where to set up your FMOD Project. After the FMOD project is set up, make sure to build your FMOD project for all platforms. In Unity, go to FMOD -> Edit Settings. Under Bank Import, set Source Type to Single Platform Build and specify your desired platform's build directory. This package does not support Source Type of FMOD Studio Project or Multiple Platform Build. To build the Unity project for multiple platforms, select the desired platform's build directory before building for that platform.

#### Initialization

In Unity, go to FMOD -> Edit Settings. Under Initialization, set Load Banks to None or Specified. If set to None, make sure to manually load the Master bank and Master.strings bank before trying to playback any audio. If you have more than one Master bank, then load those and their corresponding .strings banks too. Ideally, it may be wise to set Load Banks to Specified so that the master bank(s), .strings bank(s) and special banks (such as the base dialogue bank) can be loaded on game start.


### Renaming FMOD Project Locales

When setting up localized audio tables in FMOD Studio, the project locales must be renamed or else the data generation will be incorrect because it will not be able to find the correct banks. In FMOD Studio, go to Edit -> Preferences -> Assets. Add the keyword "LOCALE_" before the language code. For example, EN should become LOCALE_EN. Rename the locales before performing the next step.


### Generating all FMOD data

To use FMOD and its features, we need references to events, parameters, banks, buses and VCAs. Go to Studio-23 -> Generate All FMOD Data. This will generate data containing the references from FMOD Studio. Make sure to build the FMOD project and select the correct platform build directory before generating the FMOD Data. You will need to generate the data again if you rebuild your FMOD project or if you want to build the Unity project for a different platform. Frequent regeneration will probably be needed.

In the data generated you will find:
- ```FMODBank_[BankName]``` containing references to all the events under the ```[BankName]``` Bank.
- ```FMODBankList``` containing references to all the banks.
- ```FMODBusList``` containing references to all the buses.
- ```FMODVCAList``` containing references to all the VCAs.
- ```FMODParameterList``` containing references to all the parameter names.
- ```FMODLocaleList``` containing references to all languages supported by the current FMOD build and their corresponding bank.


## Usage

### FMOD Manager

1. Attach the FMODManager script to a GameObject in your scene. You can access the EventsManager, BanksManager and MixerManager through the FMODManager.
2. The EventsManager can be used to play, pause, unpause, stop events and change parameters. All events, game wide can be paused and unpaused. Programmer sounds can also be played. 
    - Every sound is played through a Custom Studio Event Emitter. The only difference between our custom emitter and FMOD's emitter is how  the event instance is initialized.
    - Sounds are not played through separate event intances. Sounds are not played through ```PlayOneShot()```. This is to ensure we have full control over what sounds are being played and their individual states.
    - Emitters can be created at runtime or set beforehand.
    - Make sure to load the correct banks before creating an emitter at runtime or else the sound won't play and will give an error.
3. The BanksManager can be used to load/unload banks, load/unload sample data and switch localization. Make sure to load the correct banks beofre trying to play a sound.
4. The MixerManager can be used to control the Buses and VCAs within FMOD.

### CallBack Handlers

We have provided two callback handlers.
1. The default callback handler manages the internal states of each event. It can also handle looping audio. This callback is fired with every event that is played.
2. The programmer callback handler manages the playback of externally loaded sounds and is required for dialogues and localized audio tables.
