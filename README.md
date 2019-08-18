# What's this?
An Audio framework for handling audio in Unity3D.

![Sample](https://user-images.githubusercontent.com/9117538/63221563-28e00980-c1c5-11e9-8af8-77a304190f5d.png)

## The idea

There are common audio user stories when we working in a game project such as:

- Play click sound when the user press on any button
- Play an audio file with repeating duration/time.
- Play a BGM music when user enters a level. Notes: the BGM must have a fadeIn duration, and also fade out other BGM such as Menu music
- When an NPC die, play random scream sounds
- etc ...

And we often have to do all those things via scripting, in some cases, to achieve the fadeIn/out and random sounds, it cost us a lot of effort, a lot of scripting. And the worst thing is we might have to re-do the same things on other projects, cause the script is "coupling" to the scene, the project structure ... So

- What if we convert those audio command things into ScriptableObject files?
- What if all the complicated request (fade in/out, stop other audio, repeating ...) can be serialized to a single scriptableObject?
- To play audio, we simply call some method command.Execute()

# Workflow

## Create AudioCommand asset files

AudioCommand file, they're commands, their name already defines their usage. You create a Command file, declare common things for an audio command in-game: what clip to play, mixer, loop .. you can even declare a command to mute/unmute audio globally. Play around with the sample to understand all the current tweak-able config in the command files.

![AudioCommand](https://user-images.githubusercontent.com/9117538/63221659-6db87000-c1c6-11e9-9d3a-587346b4d190.png)

## Drag AudioManager prefab to the scene

Every set-up things were done in the prefab, drag it o the scene and you're ready to go

![AudioManager prefab](https://user-images.githubusercontent.com/9117538/63221564-2a113680-c1c5-11e9-8100-8a3d81282584.png)

## Call the command.Execute()

Here's the sample code:

```csharp
[SerializeField] AudioCommand screamAudioCmd;
void OnUserDead() {
    screamAudioCmd.Execute();
}
```

# Conclusion

I wrote this custom library long ago and keep updating it, it solves almost common Audio2D things in my projects (company/personal), not sure it'll fit your project, just give it a try if you are interested on my idea about using ScriptableObject. Hope it'll help.
