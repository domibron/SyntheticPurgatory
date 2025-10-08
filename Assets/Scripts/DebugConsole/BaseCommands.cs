using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//by    _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
// © 2025 domibron

public class BaseCommands
{
    private DebugConsole console;

    public static Command test;
    public static Command help;
    public static Command reloadLevel;
    public static Command<string[]> testMessage;
    public static Command<int> loadLevel;
    public static Command<float, float, float> tp;
    public static Command destroyObjectCommand;
    public static Command<float> setSprintSpeed;

    public static Command<float> damagePlayer;
    public static Command<float> healPlayer;
    public static Command noClip;
    public static Command unlockAllAbilities;

    public static Command removeDialog;
    public static Command removeHud;

    public static Command<float> setAttackDamage;


    public BaseCommands(DebugConsole console)
    {
        test = new Command("test", "test the debug console", "test", () =>
        {
            console.TextToConsole($"Test + {System.DateTime.Now}");
        });

        testMessage = new Command<string[]>("send", "sends the message to the debug console", "send <string>", (message) =>
        {
            string finalMessage = "";

            foreach (var str in message)
            {
                finalMessage += str + " ";
            }

            console.TextToConsole(finalMessage);
        });

        help = new Command("help", "generates help message", "help", () =>
        {
            for (int i = 0; i < console.commands.Count; i++)
            {
                console.TextToConsole((console.commands[i] as CommandBase).CommandHelp + " - " + (console.commands[i] as CommandBase).CommandDescription);
            }
        });

        loadLevel = new Command<int>("loadlevel", "Loads the desired scene with that build index", "loadlevel <int>", (index) =>
        {
            try
            {

                if (index >= SceneManager.sceneCountInBuildSettings)
                {
                    console.TextToConsole("Does not exists");
                    throw new NullReferenceException();
                }

                console.TextToConsole($"Loading scene {index}");
                if (LevelLoading.instance == null)
                {
                    SceneManager.LoadScene(index);
                    return;
                }

                if (LevelLoading.instance.loading) return;
                LevelLoading.instance.LoadScene(index);
            }
            catch (Exception e)
            {
                console.TextToConsole("I have failed to load that scene \n" + e.Message);
            }
        });

        tp = new Command<float, float, float>("tp", "teleports the player in that direction (direction is relative to player z forward)", "tp <float> <float> <float>", (x, y, z) =>
        {
#nullable enable
            GameObject? go = GameObject.FindGameObjectWithTag("Player");
#nullable restore
            if (go != null && go.transform.name == "Player")
            {
                go.transform.GetComponent<CharacterController>().enabled = false;
                go.transform.position += go.transform.forward * z + go.transform.right * x + go.transform.up * y;
                go.transform.GetComponent<CharacterController>().enabled = true;
                console.TextToConsole("Moved the player");
            }
            else
            {
                console.TextToConsole("Cannot find the player");
                return;
            }
        });

        destroyObjectCommand = new Command("obliterate", "Deletes the game object 50m in front of the camera", "obliterate", () =>
        {
            try
            {
                RaycastHit hit;
                Physics.Raycast(Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f)), out hit, 50);
                console.DestroyUnityObject(hit.transform.gameObject);
                console.TextToConsole("Gone!");
            }
            catch
            {
                console.TextToConsole("Failed");
            }
        });

        //         setSprintSpeed = new Command<float>("walkspeed", "set the sprint speed of the player", "walkspeed <float>", (newSpeed) =>
        //         {
        // #nullable enable
        //             GameObject? go = GameObject.FindGameObjectWithTag("Player");
        // #nullable restore
        //             if (go != null && go.transform.name == "Player")
        //             {
        //                 go.transform.GetComponent<PlayerMovementHandler>().walkSpeed = newSpeed;
        //                 console.TextToConsole("Set sprint speed of the player to " + newSpeed);
        //             }
        //             else
        //             {
        //                 console.TextToConsole("Cannot find the player");
        //                 return;
        //             }
        //         });


        reloadLevel = new Command("reload", "reloads the level", "reload", () =>
        {
            try
            {
                console.TextToConsole($"Reloading...");
                if (LevelLoading.instance == null)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    return;
                }

                if (LevelLoading.instance.loading) return;
                LevelLoading.instance.Reload();
            }
            catch (Exception e)
            {
                console.TextToConsole("I have failed to load that scene \n" + e.Message);
            }
        });

        //         damagePlayer = new Command<float>("damage", "damages the player", "damage <float>", (damage) =>
        //         {


        // #nullable enable
        //             GameObject? go = GameObject.FindGameObjectWithTag("Player");
        // #nullable restore
        //             if (go != null && go.transform.name == "Player")
        //             {
        //                 go.transform.GetComponent<IDamageable>().TakeDamage(damage);
        //                 console.TextToConsole($"player hp is now at {go.transform.GetComponent<Health>().ReturnHealthValue()}");

        //             }
        //             else
        //             {
        //                 console.TextToConsole("Cannot find the player");
        //                 return;
        //             }

        //         });


        damagePlayer = new Command<float>("heal", "damages the player", "damage <float>", (health) =>
        {


#nullable enable
            GameObject? go = GameObject.FindGameObjectWithTag("Player");
#nullable restore
            if (go != null && go.transform.name == "Player")
            {
                go.transform.GetComponent<Health>().AddToHealth(health);
                console.TextToConsole($"player hp is now at {go.transform.GetComponent<Health>().ReturnHealthValue()}");

            }
            else
            {
                console.TextToConsole("Cannot find the player");
                return;
            }

        });

        removeDialog = new Command("rmrfdialog", "damages the player", "rmrfdialog", () =>
        {

#nullable enable
            GameObject[] dialogueObjects = GameObject.FindGameObjectsWithTag("DialogueObject");
#nullable restore
            if (dialogueObjects.Length > 0)
            {
                foreach (var dobj in dialogueObjects)
                {
                    GameObject.Destroy(dobj);
                }

                console.TextToConsole($"removed {dialogueObjects.Length} from the scene");
            }
            else
            {
                console.TextToConsole("Cannot find any dialogueObjects");
                return;
            }

        });


        noClip = new Command("noclip", "gives you that ability to walk through walls", "noclip", () =>
        {
#nullable enable
            GameObject? go = GameObject.FindGameObjectWithTag("Player");
#nullable restore
            if (go != null && go.transform.name == "Player")
            {
                if (go.transform.GetComponent<NoClipPlayerController>() != null)
                {
                    console.DestroyUnityObject(go.GetComponent<NoClipPlayerController>());
                    go.transform.GetComponent<CharacterController>().enabled = true;

                    console.TextToConsole($"No clip mode deactivated");
                }
                else
                {

                    go.transform.GetComponent<CharacterController>().enabled = false;
                    go.AddComponent<NoClipPlayerController>();

                    console.TextToConsole($"No clip mode activated");
                }


            }
            else
            {
                console.TextToConsole("Cannot find the player");
                return;
            }
        });


        //         unlockAllAbilities = new Command("unlockall", "gives you all the abilities", "unlockall", () =>
        //         {
        // #nullable enable
        //             GameObject? go = GameObject.FindGameObjectWithTag("Player");
        // #nullable restore
        //             if (go != null && go.transform.name == "Player")
        //             {

        //                 go.GetComponent<ShieldAbility>().unlockedShield = true;
        //                 go.GetComponent<PlayerAttackHandler>().heavyAttackUnlocked = true;

        //                 console.TextToConsole($"Unlocked all abilities");

        //             }
        //             else
        //             {
        //                 console.TextToConsole("Cannot find the player");
        //                 return;
        //             }
        //         });


        // removeHud = new Command("togglehud", "removes the hud", "togglehud", () =>
        // {
        //     if (PlayerCanvasReference.instance == null)
        //     {
        //         console.TextToConsole("Cannot find the canvas!");
        //         return;
        //     }

        //     GameObject playerCanvas = PlayerCanvasReference.instance.GetPlayerCanvasReference();

        //     GameObject hudCanvas = playerCanvas.transform.Find("Player HUD").gameObject;

        //     hudCanvas.SetActive(!hudCanvas.activeSelf);

        //     console.TextToConsole("Toggled");
        // });

        //         setAttackDamage = new Command<float>("setdmg", "sets the player light attack damage", "setdmg", (x) =>
        //         {
        // #nullable enable
        //             GameObject? go = GameObject.FindGameObjectWithTag("Player");
        // #nullable restore
        //             if (go != null && go.transform.name == "Player")
        //             {

        //                 go.GetComponent<PlayerAttackHandler>().lightAttackDamage = x;

        //                 console.TextToConsole($"Setted the damage");

        //             }
        //             else
        //             {
        //                 console.TextToConsole("Cannot find the player");
        //                 return;
        //             }
        //         });

        // foreach


        List<object> commandsToAdd = new List<object>()
        {
            test,
            testMessage,
            help,
            loadLevel,
            tp,
            destroyObjectCommand,
            // setSprintSpeed,
            reloadLevel,
            // damagePlayer,
            removeDialog,
            noClip,
            // unlockAllAbilities,
            // removeHud,
            // setAttackDamage,
        };

        foreach (var command in commandsToAdd)
        {
            console.commands.Add(command);
        }
    }
}