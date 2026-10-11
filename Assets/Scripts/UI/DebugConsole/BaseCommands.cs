using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class BaseCommands
{
    private DebugConsole console;

    // could throw all the conmmands in the list creations as short hand.

    public Command test;
    public Command help;
    public Command reloadLevel;
    public Command<string[]> testMessage;
    public Command<int> loadLevel;
    public Command<float, float, float> tp;
    public Command destroyObjectCommand;
    public Command<float> setSprintSpeed;

    public Command<float> damagePlayer;
    public Command<float> healPlayer;
    public Command noClip;

    public Command<float> setAttackDamage;

    public Command toggleDoors;
    public Command<int> giveScrap;
    public Command<int, int> giveUpgradeCard;

    public Command refreshUpgradeMenu;
    public Command<bool> errorsOnly;

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
                if (LevelLoading.Instance == null)
                {
                    SceneManager.LoadScene(index);
                    return;
                }

                if (LevelLoading.Instance.IsLoading) return;
                LevelLoading.Instance.LoadScene(index);
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

        reloadLevel = new Command("reload", "reloads the level", "reload", () =>
        {
            try
            {
                console.TextToConsole($"Reloading...");
                if (LevelLoading.Instance == null)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    return;
                }

                if (LevelLoading.Instance.IsLoading) return;
                LevelLoading.Instance.Reload();
            }
            catch (Exception e)
            {
                console.TextToConsole("I have failed to load that scene \n" + e.Message);
            }
        });

        damagePlayer = new Command<float>("damage", "damages the player", "damage <float>", (health) =>
        {


#nullable enable
            GameObject? go = GameObject.FindGameObjectWithTag("Player");
#nullable restore
            if (go != null && go.transform.name == "Player")
            {
                go.transform.GetComponent<Health>().AddToHealth(health);
                console.TextToConsole($"player hp is now at {go.transform.GetComponent<Health>().GetHealthValue()}");

            }
            else
            {
                console.TextToConsole("Cannot find the player");
                return;
            }

        });



        noClip = new Command("noclip", "gives you that ability to walk through walls", "noclip", () =>
        {
#nullable enable
            GameObject? go = GameObject.FindGameObjectWithTag("Player");
#nullable restore
            if (go && go.transform.name.ToLower().Contains("player"))
            {
                if (go.transform.GetComponent<NoClipPlayerController>())
                {
                    console.DestroyUnityObject(go.GetComponent<NoClipPlayerController>());
                    if (go.transform.GetComponent<CharacterController>())
                        go.transform.GetComponent<CharacterController>().enabled = true;
                    if (go.transform.GetComponent<Rigidbody>())
                        go.transform.GetComponent<Rigidbody>().isKinematic = false;


                    console.TextToConsole($"No clip mode deactivated");
                }
                else
                {

                    if (go.transform.GetComponent<CharacterController>())
                        go.transform.GetComponent<CharacterController>().enabled = false;
                    if (go.transform.GetComponent<Rigidbody>())
                        go.transform.GetComponent<Rigidbody>().isKinematic = true;

                    go.AddComponent<NoClipPlayerController>();

                    console.TextToConsole($"No clip mode activated");
                }


            }
            else
            {
                throw new NullReferenceException("Cannot locate the player object. No object with Player tag and name containing player");
                //console.TextToConsole("Cannot find the player");
                // return;
            }
        });


        toggleDoors = new Command("toggledoors", "Toggles all the door's open states in the current room of the player.", "toggledoors", () =>
        {
            if (LevelGenObjectRefGetter.Instance == null)
            {
                console.TextToConsole("<color=red>Cannot find the level generator object</color>");
                return;
            }

            GameObject levelGen = LevelGenObjectRefGetter.Instance.GetReference();

            if (PlayerRefFetcher.Instance == null)
            {
                console.TextToConsole("<color=red>Cannot find the player</color>");
                return;
            }

            GameObject player = PlayerRefFetcher.Instance.GetPlayerRef();

            levelGen.GetComponent<DoorGenerator>().ToggleDoors(levelGen.GetComponent<LevelGenerator>().GetRoomIDFromCoordinates(levelGen.GetComponent<LevelGenerator>().GetGridCoordinates(player.transform.position)));

            console.TextToConsole("Toggled the doors");
        });

        giveScrap = new Command<int>("givescrap", "Gives the desired amount of scrap into inventory.", "givescrap <int>", (scrap) =>
        {
            if (RunManager.Instance == null)
            {
                console.TextToConsole("<color=red>Cannot find the game manager</color>");
                return;
            }


            RunManager.Instance.AddToDepositedScrap(scrap);

            console.TextToConsole($"Gave {scrap}. Current amount is {RunManager.Instance.GetCurrentScrapCount()}");

        });

        giveUpgradeCard = new Command<int, int>("giveupcard", "Gives the desired amount of the upgrade card.", "giveupcard <int card type> <int amount>", (type, amount) =>
        {
            if (RunManager.Instance == null)
            {
                console.TextToConsole("<color=red>Cannot find the game manager</color>");
                return;
            }

            ModuleTier? cardTeir = null;



            cardTeir = (ModuleTier)type;

            if (!Enum.IsDefined(typeof(ModuleTier), (ModuleTier)type))
            {
                console.TextToConsole("<color=red>Not a valid card type</color>");
                return;
            }


            RunManager.Instance.AddToStoredModules(cardTeir.Value, amount);

            console.TextToConsole($"Gave {amount} of {cardTeir.Value.ToString()}. Current amount is {RunManager.Instance.GetModuleCount(cardTeir.Value)}");
        });

        refreshUpgradeMenu = new Command("refupmenu", "Refreshes the upgrade menu.", "refupmenu", () =>
        {
            if (UpgradeMenuManager.Instance == null)
            {
                throw new NullReferenceException($"Cannot find the {nameof(UpgradeMenuManager)}!");
            }

            UpgradeMenuManager.Instance.UpdateStatUI();
        });

        errorsOnly = new Command<bool>("errorsOnly", "Only log errors in console", "errorsOnly <bool>", (b) =>
        {
            console.ChangeWatchMessage(b);
        });

        List<object> commandsToAdd = new List<object>()
        {
            test,
            testMessage,
            help,
            loadLevel,
            tp,
            destroyObjectCommand,
            reloadLevel,
            damagePlayer,
            healPlayer,
            noClip,
            toggleDoors,
            giveScrap,
            giveUpgradeCard,
            refreshUpgradeMenu,
            errorsOnly,
        };

        foreach (var command in commandsToAdd)
        {
            console.commands.Add(command);
        }
    }
}