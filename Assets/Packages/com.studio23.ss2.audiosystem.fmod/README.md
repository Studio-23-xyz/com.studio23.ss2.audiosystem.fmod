<h1 align="center">Audio System FMOD</h1>
<p align="center">
<a href="https://openupm.com/packages/com.studio23.ss2.audiosystem.fmod/"><img src="https://img.shields.io/npm/v/com.studio23.ss2.audiosystem.fmod?label=openupm&amp;registry_uri=https://package.openupm.com" /></a>
</p>

Audio System FMOD is a tool designed for managing and utilizing the FMOD engine in a simpler and unified manner within Unity. It makes it easier to manage all game audio using and allows the user to focus more on audio and sound design.

## Table of Contents

1. [Installation](#installation)
2. [Usage](#usage)
    - [Important](#important)
    - [Setting up Credit Sections](#setting-up-credit-sections)
    - [Setting up Credits Settings](#setting-up-credits-settings)
	- [Usage of Credits Controller](#usage-of-credits-controller)
	- [Usage of Scroll View Controller](#usage-of-scroll-view-controller)
3. [License](#license)

## Installation

### Install via Git URL

You can also use the "Install from Git URL" option from Unity Package Manager to install the package.
```
https://github.com/Studio-23-xyz/com.studio23.ss2.audiosystem.fmod.git#upm
```

## Usage

### Important

This package cannot function on its own and is intended to be used with FMOD. Make sure you have the FMOD for Unity plugin installed in your project before installing Audio System FMOD. You can find the plugin on the Unity Asset Store or on the [FMOD website](https://www.fmod.com/download).


### FMOD Settings

Follow FMOD's documentation on where to set up your FMOD Project. After the FMOD project is set up, make sure to build your FMOD project for all platforms. Now go to FMOD -> Edit Settings. Under Bank Import, set Source Type to Single Platform Build and specify your desired platform's build directory. This package does not support Source Type of FMOD Studio Project or Multiple Platform Build. To build the Unity project for multiple platforms, select the desired platform's build directory before building for that platform.


### Generating all FMOD data

To use FMOD and its features, we need references to events, parameters, banks, buses and VCAs. Go to Studio 23 -> Generate All FMOD Data. This will generate all the references from FMOD Studio. Make sure to build the FMOD project and select the correct platform build directory before generating the FMOD Data. You will need to generate the data again if you rebuild your FMOD project.


### Setting up Credit Settings

You can change your own font asset and style in credit settings. To change just go to Studio 23 -> Credit System -> Create Credit Settings.
![CreditSettings](Screenshots/CreditSettings.png)


### Usage of Credits Controller

Credit Controller will generate credit data which you've created from credit section and also generate credit utlities from credit settings asset.

1. **Retreving The Credit Data**:

   Credit Data will be generated from the creditsection asset which you've create and it will generate assets in the hirerchy panel. You just need to press "Generate Credits Data". In CreditController script
   `GenerateSections()` will be responsible for the things. 

2. **Credits Layout Change**:
	
	`GenerateVerticalSelections()`  and `GenerateHorizontalSelections()` will change the credits roll layout elements.
   
 
### Usage of Scroll View Controller

Scrollviewcontroller will change the scrolling speed and positions for the scroll view controller. There is a `scrollDampValue` where you can adjust to control the scroll reset position. 
`OnEndScroll()` will be used whenever the scrolling will be finished.