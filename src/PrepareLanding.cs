﻿using System;
using HugsLib;
using PrepareLanding.Core.Gui.World;
using PrepareLanding.GameData;
using PrepareLanding.Patches;
using Verse;

//TODO: general TODO -> translate all GUI strings.

namespace PrepareLanding
{
    /// <summary>
    ///     Main Mod class.
    /// </summary>
    public class PrepareLanding : ModBase
    {
        /// <summary>
        ///     Filter Options (from the GUI window 'options' tab).
        /// </summary>
        private FilterOptions _filterOptions;

        /// <summary>
        ///     Main mod class constructor. Sets up the static instance.
        /// </summary>
        public PrepareLanding()
        {
            Logger.Message("In constructor.");
            if (Instance == null)
                Instance = this;
        }

        /// <summary>
        ///     Main static instance, holding references to useful class instances.
        /// </summary>
        public static PrepareLanding Instance { get; private set; }

        /// <summary>
        ///     User choices on the GUI are kept in this instance.
        /// </summary>
        public UserData UserData { get; private set; }

        /// <summary>
        ///     The filtering class instance used to filter tiles on the world map.
        /// </summary>
        public WorldTileFilter TileFilter { get; private set; }

        /// <summary>
        ///     Allow highlighting filtered tiles on the world map.
        /// </summary>
        public TileHighlighter TileHighlighter { get; private set; }

        public GameData.GameData GameData { get; private set; }

        /// <summary>
        ///     The main GUI window instance.
        /// </summary>
        public MainWindow MainWindow { get; set; }

        //TODO see if this can be set to a "private set" rather than a public one

        /// <summary>
        ///     A unique identifier for your mod.
        ///     Valid characters are A-z, 0-9, -, no spaces.
        /// </summary>
        public override string ModIdentifier => "PrepareLanding";

        /// <summary>
        ///     The full path of the mod folder.
        /// </summary>
        public string ModFolder => ModContentPack.RootDir;

        /// <summary>
        ///     Methods can register to this event to be called when definitions (Defs) have been loaded.
        /// </summary>
        public event Action OnDefsLoaded = delegate { };

        /// <summary>
        ///     Methods can register to this event to be called when the world has been generated. See also
        ///     <seealso cref="WorldGenerated" />.
        /// </summary>
        /// <remarks>This is not a RimWorld event, it is generated by this mod from an Harmony patch.</remarks>
        public event Action OnWorldGenerated = delegate { };

        /// <summary>
        ///     Methods can register to this event to be called when the OnGUI() method (while on the world map) is called.
        ///     See also <seealso cref="WorldInterfaceOnGui" />.
        /// </summary>
        public event Action OnWorldInterfaceOnGui = delegate { };

        /// <summary>
        ///     Methods can register to this event to be called when the Update() method (while on the world map) is called.
        ///     See also <seealso cref="WorldInterfaceUpdate" />.
        /// </summary>
        public event Action OnWorldInterfaceUpdate = delegate { };

        /// <summary>
        ///     Set the main instance to null.
        /// </summary>
        public static void RemoveInstance()
        {
            Instance = null;
        }

        /// <summary>
        ///     Called during mod initialization.
        /// </summary>
        public override void Initialize()
        {
            Logger.Message("Initializing.");

            // initialize events
            PatchWorldInterfaceOnGui.OnWorldInterfaceOnGui += WorldInterfaceOnGui;
            PatchWorldInterfaceUpdate.OnWorldInterfaceUpdate += WorldInterfaceUpdate;

            // Holds various mod options (shown on the 'option' tab on the GUI).
            _filterOptions = new FilterOptions();

            GameData = new GameData.GameData(_filterOptions);

            // main instance to keep user filter choices on the GUI.
            UserData = new UserData(_filterOptions);

            TileFilter = new WorldTileFilter(UserData);

            // instantiate the main window now
            MainWindow = new MainWindow(UserData);

            // instantiate the tile highlighter
            TileHighlighter = new TileHighlighter();
            Instance.OnWorldInterfaceOnGui += TileHighlighter.HighlightedTileDrawerOnGui;
            Instance.OnWorldInterfaceUpdate += TileHighlighter.HighlightedTileDrawerUpdate;
        }

        /// <summary>
        ///     Called when the world map is generated.
        /// </summary>
        /// <remarks>This is not a RimWorld event. It is generated by an Harmony patch.</remarks>
        public void WorldGenerated()
        {
            // disable all tiles that are currently highlighted
            TileHighlighter.RemoveAllTiles();

            // call onto subscribers to tell them that the world has been generated.
            OnWorldGenerated.Invoke();
        }

        /// <summary>
        ///     Called after Initialize and when defs have been reloaded. This is a good place to inject defs.
        ///     Get your settings handles here, so that the labels will properly update on language change.
        ///     If the mod is disabled after being loaded, this method will STILL execute. Use ModIsActive to check.
        /// </summary>
        public override void DefsLoaded()
        {
            // do not go further if this mod is not active. This will mostly prevent all other classes from running.
            if (!ModIsActive)
            {
                Log.Message("[PrepareLanding] DefsLoaded: Mod is not active, bailing out.");
                return;
            }

            Log.Message("[PrepareLanding] DefsLoaded");

            // alert subscribers that definitions have been loaded.
            OnDefsLoaded.Invoke();
        }

        /// <summary>
        ///     Called on each <see cref="RimWorld.WorldInterface" /> Gui event.
        /// </summary>
        public void WorldInterfaceOnGui()
        {
            OnWorldInterfaceOnGui.Invoke();
        }

        /// <summary>
        ///     Called on each <see cref="RimWorld.WorldInterface" /> update event.
        /// </summary>
        public void WorldInterfaceUpdate()
        {
            OnWorldInterfaceUpdate.Invoke();
        }
    }
}