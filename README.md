<h1 align="center">Audio System for FMOD</h1>
<p align="center">
<a href="https://openupm.com/packages/com.studio23.ss2.audiosystem.fmod/"><img src="https://img.shields.io/npm/v/com.studio23.ss2.audiosystem.fmod?label=openupm&amp;registry_uri=https://package.openupm.com" /></a>
</p>

Audio System for FMOD is a tool designed for managing and utilizing the FMOD engine in a simpler and unified manner within Unity. It makes it easier to manage all game audio using FMOD and allows the user to focus more on audio and sound design.

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
    - [Events Manager](#events-manager)
    - [Banks Manager](#banks-manager)
    - [Mixer Manager](#mixer-manager)
    - [CallBack Handlers](#callback-handlers)  
4. [Using the sample provided](#using-the-sample-provided)


## Installation

### Install via Git URL

You can also use the "Install from Git URL" option from Unity Package Manager to install the package.
```
https://github.com/Studio-23-xyz/com.studio23.ss2.audiosystem.fmod.git#upm
```

## Setup

### Important

This package cannot function on its own and is intended to be used with FMOD. Make sure you have the FMOD for Unity plugin installed in your project before installing Audio System for FMOD. You can find the plugin on the Unity Asset Store or on the [FMOD website](https://www.fmod.com/download). This package assumes familiarity with FMOD for its usage so please read up on FMOD's documentation.


### FMOD Settings in Unity

#### Bank Import

1. Follow FMOD's documentation on where to set up your FMOD Project. 
2. After the FMOD project is set up, build your FMOD project for all platforms. 
3. In Unity, go to FMOD -> Edit Settings. 
4. Under Bank Import, set Source Type to Single Platform Build and specify your desired platform's build directory. 
    - This package does not support Source Type of FMOD Studio Project or Multiple Platform Build. 
5. To build the Unity project for other platforms, select the desired platform's build directory from the FMOD settings before building for that platform.

#### Initialization

1. In Unity, go to FMOD -> Edit Settings. 
2. Under Initialization, Load Banks can be set to All, None or Specified. 
    - If set to All, then you don't have to worry about bank loading/unloading other than the banks for dialogue audio tables. This may not be the best option for larger games.
    - Ideally, it is better to set Load Banks to Specified so that you can specify important banks to be loaded on game start. For example, you can specify the master bank(s), .strings bank(s) and banks that are always required (such as a UI bank, base dialogue bank). 
        - Specified banks cannot be unloaded. Don't specifiy a bank you may need to unload such as a localization bank. 
    - If set to None, make sure to manually load the Master bank and Master.strings bank before trying to playback any audio. If you have more than one Master bank, then load those and their corresponding .strings banks too. 


### Renaming FMOD Project Locales

When setting up localized audio tables in FMOD Studio, the project locales must be renamed or else the data generation will be incorrect as it will not be able to find the correct banks. 
1. In FMOD Studio, go to Edit -> Preferences -> Assets. Add the keyword "LOCALE_" before the language code.
2. For example, EN should become LOCALE_EN. Rename all the locales before performing the next step.


### Generating all FMOD data

To use FMOD and its features, we need references to events, parameters, banks, buses and VCAs. 
1. Make sure to build the FMOD project and select the correct platform build directory before generating the FMOD Data. 
2. Go to Studio-23 -> Generate All FMOD Data. This will generate data containing the references from FMOD Studio. 
3. You will need to generate the data again if you rebuild your FMOD project, if you want to build the Unity project for a different platform. Frequent regeneration will be needed as your project grows.

In the data generated you will find:
- ```FMODBank_[BankName]``` containing references to all the events under the ```[BankName]``` Bank.
- ```FMODBankList``` containing references to all the banks.
- ```FMODBusList``` containing references to all the buses.
- ```FMODVCAList``` containing references to all the VCAs.
- ```FMODParameterList``` containing references to all the parameter names.
- ```FMODLocaleList``` containing references to all languages supported by the current FMOD build and their corresponding bank.


## Usage

### FMOD Manager

Attach the FMODManager script to a GameObject in your scene. You can access the EventsManager, BanksManager and MixerManager through the FMODManager.


### Events Manager

1. The EventsManager can be used to play, pause, unpause, stop events. All events, game wide can be paused and unpaused. Programmer sounds can be played. Local and global parameters can be changed.
2. Every sound is played through a Custom Studio Event Emitter. The only difference between our custom emitter and FMOD's emitter is in how the event instance is initialized.
    - Sounds are not played through separate event intances.
    - Sounds are not played through ```PlayOneShot()```. This is to ensure the event instance isn't released automatically and it allows us to hold its reference so we can reuse it or pause/unpause it.
3. Make sure the correct banks are loaded before playing a sound or creating an emitter, or else the sound won't play and will give you an error.
4. Use ```Play()``` to play an event.
    - It will take the event we want to play and the gameobject from which it will play. 
    - By default, it will create an emitter on the GameObject and will set the event instance's ```STOP_MODE``` to ```ALLOWFADEOUT```.
    - If the GameObject already has an emitter component, we can pass that emitter instead to initialize it.
    - If an emitter of type "event and GameObject" already exists, it will just play the event.
5. ```Pause()```,  ```UnPause()```, ```Stop()```, ```Release()```, ```LoadEventSampleData()```, work similarly, but they need an emitter of type "event and GameObject" to exist or else they will do nothing.
6. ```PlayAllOfType()``` Plays all existing emitters of the same type of event. This does not create any emitters on its own.
6. ```StopAllOfType()``` and ```ReleaseAllOfType``` does not take in any GameObjects. They stop or release all emitters of the same event type.
7. ```StopAll()``` and ```ReleaseAll``` stop or release all existing emitters.
8. Sometimes an emitter may need to be created or initialized at runtime without playing it.
    - Call ```CreateEmitter()``` to create an emitter on the GameObject.
    - It will take the event we want to play and the gameobject from which it will play. 
    - By default, it will create an emitter on the GameObject and will set the event instance's ```STOP_MODE``` to ```ALLOWFADEOUT```.
    - If the GameObject already has an emitter component, we can pass that emitter instead to initialize it.
    - Once an emitter of type "event and GameObject" is created, we no longer have to create this emitter again to use playback methods.
    - We can call playback methods on this emitter by passing in the event and the GameObject.
9. ```PlayProgrammerSound()``` creates an emitter similar to ```Play()``` and plays the sound immediately. 
    - It will take the key of the audio file we want to play, the event we want to play and the gameobject from which it will play.
    - The key can be a key from a audio table from FMOD.
    - The key can also be the file name of an audio file in the streaming assets folder.
10. ```LoadEventSampleData()``` Loads the sample data of an event. It may be beneficial to load the sample data of an event that is frequently used, instead of loading/unloading every time the event is called.
11. Make sure to call ```Release()``` for events that are no longer needed. It will release the event instance and destroy the emitter.
12. ```TogglePauseAll()``` will pause/unpause all the events in the game.
13. ```SetLocalParameterByName()``` is used to set a local parameter using its name for an event. 
    - It will take the event, the gameobject from which it is playing, the parameter name and the parameter value. 
14. ```SetLocalParameterAllOfTypeByName()``` is used to set a local parameter using its name for all active instances of that event.
14. ```SetGlobalParameter()``` is used to set a global parameter value by name.
15. ```GetLocalParameterValueByName()``` and ```GetLocalParameterFinalValueByName()``` is used to get the local parameter's current value and current final value respectively using its name.
16. ```SetGlobalParameterByName()``` and ```GetGlobalParameterValueByName()``` is used to get the global parameter's current value and current final value respectively using its name.

### FMOD Emitter Utility

1. It can be attached to game objects as a component to play and manage events without the need for code.
2. You need to assign the event you want to play and optionally one parameter that you would like to control.
3. It also allows you to automate and ramp parameter changes for any event.
4. ```FadeIn()```, ```FadeInAllOfType()```, ```FadeOut()```, ```FadeOutAllOfType()``` are used for fading in/out audio within a certain duration. 
    - The event must have some parameter that controls the master volume of the event.
    - Should not be used for ramping other parameters. May not work as expected.
    - For the intended behavior, ```startValue``` should be set to ```0``` and ```endValue``` should be set to ```1```. 
    - The bools ```StopOnFadeOut``` and ```ReleaseOnFadeOut``` can be marked as true to stop or release the event respectively after fading out. 
    - Ideally these should not be true when ramping any other parameters other than volume. May not work as expected.
5. ```RampUpLocalParameter()```, ```RampUpLocalParameterAllOfType()```, ```RampDownLocalParameter()```, ```RampDownLocalParameterAllOfType()``` are used to ramp up/down local parameters within a certain  duration.
    - Should not be used for volume parameters or fading in/out. May not work as expected.
6. ```RampUpGlobalParameter()```, ```RampDownGlobalParameter()```, are used to ramp up/down global parameters within a certain  duration.
7. The rest of the methods are similar to the methods in Events Manager.

### Banks Manager

1. The BanksManager can be used to load/unload banks, load/unload sample data and switch localization. Make sure to load the correct banks before trying to play a sound.
2. ```LoadBank()``` is used to load a bank. By default, ```LOAD_BANK_FLAGS``` is set to ```NORMAL```.
3. ```UnloadBank()``` is used to unload bank. It cannot remove banks that have been specified FMOD in the FMOD Settings.
4. ```UnloadAllBanks()``` is used to unload all banks. It does not remove banks that have been specified FMOD in the FMOD Settings.
5. ```LoadBankSampleData()``` is used to load all the non-streaming sample data of a bank. Can also be used if banks are built with separate metadata and assets.
6. ```SwitchLocalization()``` is used to switch localization. It will unload the current localization bank and load the target localization bank.


### Mixer Manager

1. The MixerManager can be used to control the Buses and VCAs within FMOD.
2. ```SetBusVolume()```, ```PauseBus()```, ```MuteBus()```, all perform their respective functionality on the specified bus.
3. ```StopAllBusEvents()``` stops all events under a specific bus.
4. ```SetVCAVolume()``` is used to change the volume of a VCA.


### CallBack Handlers

This system uses two callback handlers.
1. The default callback handler ```FMODCallBackHandler``` manages the internal playback states of each event (the internal states here refers to the internal states used by this package to keep track of which event is playing, suspended, paused or stopped or whether the event is looping; not FMOD's own playback states). This callback is set whenever an emitter is created and is fired with every event that is played.
2. The programmer callback handler ```FMODProgrammerSoundCallBackHandler``` manages the playback of externally loaded sounds and is required for dialogues and localized audio tables. This callback is set whenever an ```PlayProgrammerSound()``` is called and is fired with every subsequent call.
3. It is possible to create and set your own callback handler. You can fetch or create an emitter using ```CreateEmitter()``` and pass this to the initialize method of your callback handler. Look at the initialize methods of our callback handle to get an idea on how to make your initialize method. Not using the the default callback handler ```FMODCallBackHandler``` will result in that events internal states not being tracked/handled.

## Using the sample provided

1. Open the FMOD Project file in the samples folder. Go to Files -> Build All Platforms.
2. In Unity, go to FMOD -> Edit Settings.
3. Under Bank Import, set Source Type to Single Platform Build and specify the Desktop build directory. 
4. Under Initialization, set Load Banks Specified. 
5. Under Specified Banks add ```Master```, ```Master.strings```, ```Sample```, and ```Dialogue```
6. Play the SampleScene.
