# Tour the ISS with IBM Watson [needs changing]

A demonstration of how developers can add new layers of immersion to their Virtual Reality environments through the use of the [Watson Unity SDK](https://github.com/watson-developer-cloud/unity-sdk) and [IBM Watson Cloud Services](https://www.ibm.com/watson/products-services/). In this Code Pattern we showcase IBM Watson's functionality through a virtual reality experience for the International Space Station.

When the reader has completed this Code Pattern, they will understand how to:

- Integrate [Watson Speech-To-Text](https://www.ibm.com/watson/services/speech-to-text/), [Assistant](https://www.ibm.com/watson/ai-assistant/) and [Text-to-Speech](https://www.ibm.com/watson/services/text-to-speech/) within a Unity environment.
- Utilise [Watson Assistant's context functionality](https://console.bluemix.net/docs/services/conversation/dialog-runtime.html#dialog-runtime) with data from Unity (e.g. virtual location and viewport).
- Use [Watson Assistant](https://www.ibm.com/watson/ai-assistant/) to command & control a virtual environment in Unity.

## Demo

Video of demo

## Video

Video of tutorial/tech run through?

## Flow

1. User flies around the International Space Station in virtual reality. The user can ask questions about their surroundings, such as _"Where am I?"_, _"What am I looking at?"_ and can follow these with _"Tell me more information"_. They can also gives commands such as _"Teleport me outside"_, _"Take me to the window"_, _"Rotate the solar arrays"_ and _"Open the window"_.
2. The Virtual Reality Hardware microphone picks up the voice command and the running application sends it to Watson Speech-to-Text.
3. Watson Speech-to-Text converts the audio to text and returns it to the running Application that powers the VR Hardware. The application sends the text, along with the context of the user's location and viewport, to Watson Assistant.
4. Watson Assistant returns the recognized intent _(e.g. "#faq--size" or "#nav--teleport")_, any recognized entities _(e.g. "@ISS_Module:Node 1")_ and the appropriate dialog response to the Application.
5. Any dialog response is sent to Watson Text-to-Speech and read aloud to the user.
6. Any intents that contain an action that needs to be taken within the Application (e.g. "#action--openclose" or "#nav--path"), the Application will trigger the relevant code (e.g. Open the windows in the Cupola module or Show the User the request path).

## Included Components

- [Watson Speech-To-Text](https://www.ibm.com/watson/services/speech-to-text/): A service that converts human voice into written text.
- [Watson Assistant](https://www.ibm.com/watson/ai-assistant/): Create a chatbot with a program that conducts a conversation via auditory or textual methods.
- [Watson Text-to-Speech](https://www.ibm.com/watson/services/text-to-speech/): Converts written text into natural sounding audio in a variety of languages and voices.
- [Watson Unity SDK](https://github.com/watson-developer-cloud/unity-sdk):

## Featured Technologies

- [Artificial Intelligence](https://medium.com/ibm-watson): Artificial intelligence can be applied to disparate solution spaces to deliver disruptive technologies.
- Gaming: Gaming is driving innovation in computing.
- Virtual Reality: Virtual reality has found many applications in various industries that it is poised to revolutionize.
- [Unity](https://unity3d.com/): A cross-platform game engine used to develop video games for PC, consoles, mobile devices and websites.

# Steps

1. [Clone the repo](#1-clone-the-repo)
2. [Install third-party dependencies](#2-install-third-party-dependencies)
3. [Setup the skybox](#3-setup-the-skybox)
4. [Create Watson services with IBM Cloud](#4-create-watson-services-with-IBM-cloud)
5. [Import the Watson Assistant workspace](#5-import-the-watson-assistant-workspace)
6. [Get the IBM Watson service credentials](#6-get-the-IBM-watson-service-credentials)
7. [Complete a Lighting Build](#7-lighting-build)

### 1. Clone the repo

Clone the `watsoninspace-unity-core` repo locally. In a terminal, run:

```
git@github.com:<USER>/<repo>.git
```

### 2. Install third-party dependencies
To work with this Code Pattern, you will need to import some free-to-use third-party packages, all of which are available through the Unity Asset Store.

When importing these assets, place them in the `Assets/Third Party/Packages` folder.

- __Earth & Planet Skyboxes__ - [Open in Asset Store](https://assetstore.unity.com/packages/2d/textures-materials/sky/earth-planets-skyboxes-53752) - Earth Skyboxes
- __Watson Unity SDK__ - [Open in Asset Store](https://assetstore.unity.com/packages/tools/ai/ibm-watson-sdk-for-unity-108831) - IBM Cloud Services
- __SteamVR__ - [Open in Asset Store](https://assetstore.unity.com/packages/templates/systems/steamvr-plugin-32647) - Standard VR Library

Once you have downloaded and imported each of these modules you can proceed to the next step.

### 3. Set up the Skybox

Once you've imported the `Earth & Planet Skyboxes` you can use this asset to setup the scene's skybox.

1. Open the `Lighting` tab in Unity.
2. Select the `Sun` game object found in the scene's `Environment` object.
3. Find the `Sun1` flare inside `Third Party > Packages > Plugins > SkyboxEarthPlanets > flares` and drag this onto the `Flare` option of the `Sun` object. The `Flare` value can be found in the `Light` component of the `Sun` object.
3. Select the Earth skybox of your choice from `Third Party > Packages > Plugins > SkyboxEarthPlanets > Skyboxes` and drag the skybox onto the `Skybox Material` value in Unity's `Lighting > Scene` tab.

### 4. Create Watson services with IBM Cloud

For this Code Pattern you need three IBM Cloud Services, each of which can be found in the [IBM Cloud Catalog](https://console.bluemix.net/catalog/):

- [Watson Assistant](https://console.bluemix.net/catalog/services/watson-assistant-formerly-conversation) - Define an appropriate `Service name` _(e.g. "issvr-assistant")_ and click `Create`.
- [Watson Speech-to-Text](https://console.bluemix.net/catalog/services/speech-to-text) - Define an appropriate `Service name` _(e.g. "issvr-stt")_ and click `Create`.
- [Watson Text-to-Speech](https://console.bluemix.net/catalog/services/text-to-speech) - Define an appropriate `Service name` _(e.g. "issvr-tts")_ and click `Create`.

For each service created, you'll need either the credentials for username and password or an IAM apikey, either of which you should copy down to be used later. (Click `Show` to expose them).

> Image here

### 5. Import the Watson Assistant workspace

For your created instance of Watson Assistant, you will have the option to `Launch Tool`. Do this and then click the `Workspaces` tab. Import the workspace by clicking the upload icon:

> Image Here

Click `Choose a file` and navigate to `Data/Watson Assistant Workspaces/iss-vr-assistant-workspace.json` in this repository. Click `Import`. You will also need the `Workspace ID`, get this by clicking the 3 vertical dots for the created workspace and click `View Details`. Save this for later.

> Link to Detailed Wiki here?

### 6. Add the IBM Watson service credentials to Unity

In order for Unity to talk to the IBM Cloud services, we need to provide Unity the relevant credentials that are associated with each of the created Watson services. If you did not make a note of your credentials for each service in [Step 4](#create-watson-services-with-IBM-cloud), open each service on the IBM Cloud.

You'll see either the credentials for username and password or an IAM apikey. (Click `Show` to expose them).

Each of the Watson services in the Unity Application are configured within the `Watson` game object. You can see this in Unity's `Hierachy` tab. Within this, the `Speech-to-Text Manager`, `Assistant Manager` and `Text-to-Speech Manager` each have public variables within which you can paste your credentials. You only need to use the `CF Authentication` __or__ the `IAM Authentication` options, not both.

For Watson Assistant, you will also need the `Workspace Id` which you would have got in [Step 5](#import-the-watson-assistant-workspace) and the `Version Date` which you can attain from [here](https://console.bluemix.net/docs/services/conversation/release-notes.html#release-notes).

__Important Note from the [Watson Unity SDK Documentation](https://github.com/watson-developer-cloud/unity-sdk#configuring-unity):__
> - Change the build settings in Unity (File > Build Settings) to any platform except for web player/Web GL. The IBM Watson SDK for Unity does not support Unity > Web Player.
> - If using Unity 2018.2 or later you'll need to set Scripting Runtime Version in Build Settings to .NET 4.x equivalent. We need to access security options to > enable TLS 1.2.

### 7. Lighting build
The inside of the I.S.S. is lit through the use of emissive materials. This lighting is calculated through a lighting build. As the lighting build file is very large, this cannot be uploaded to GitHub, you  will need to run the build yourself once you've setup the rest of the environment following the instructions above.

To trigger off a lighting build (which can take some time), you should open Unity's `Lighting` tab, scroll down to the bottom of this window and select `Generate Lighting`.

## Attributions

- ISS 3d Model (Internal): https://nasa3d.arc.nasa.gov/detail/iss-internal
- ISS 3d Model (External): https://nasa3d.arc.nasa.gov/detail/iss-hi-res
- Astronaut Glove: https://nasa3d.arc.nasa.gov/detail/astronaut-glove
