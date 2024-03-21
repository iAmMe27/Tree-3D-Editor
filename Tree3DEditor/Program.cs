using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using Mutagen.Bethesda.Plugins;
using Noggog;

namespace Tree3DEditor
{
    public class Settings
    {
        [SettingName("Tree EDID")]
        [Tooltip("Editor ID of the Tree you want to edit.")]
        public FormLink<ITreeGetter> TreeToEdit = null!;

        [SettingName("Edit X Position Value")]
        public bool ToggleXPosEdit = false;

        [SettingName("X Position Modifier")]
        [Tooltip("Value to modify the X position by.")]
        public float XPosModifier = 0.0f;

        [SettingName("Edit Y Position Value")]
        public bool ToggleYPosEdit = false;

        [SettingName("Y Position Modifier")]
        [Tooltip("Value to modify the Y position by.")]
        public float YPosModifier = 0.0f;

        [SettingName("Edit Z Position Value")]
        public bool ToggleZPosEdit = false;

        [SettingName("Z Position Modifier")]
        [Tooltip("Value to modify the Z position by.")]
        public float ZPosModifier = 0.0f;
    }

    public class Program
    {
        private static Lazy<Settings> _settings = null!;
        private static Settings Settings => _settings.Value;

        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetAutogeneratedSettings("settings", "settings.json", out _settings)
                .SetTypicalOpen(GameRelease.SkyrimSE, "Tree3DEditor.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            // This code was adapted from https://github.com/Super-Shadow/TreeScaler
            foreach (var placedObjectGetter in state.LoadOrder.PriorityOrder.PlacedObject().WinningContextOverrides(state.LinkCache))
            {
                var placedObject = placedObjectGetter.Record;

                placedObject.Base.TryResolve<ITreeGetter>(state.LinkCache, out var placedObjectBase);

                if (placedObjectBase?.FormKey == null)
                {
                    continue;
                }

                if (Settings.TreeToEdit.FormKey == placedObjectBase.FormKey)
                {
                    var modifiedObject = placedObjectGetter.GetOrAddAsOverride(state.PatchMod);
                    
                    // X Position
                    if (Settings.ToggleXPosEdit)
                    {
                        if (modifiedObject.Placement != null)
                        {
                            if (Settings.XPosModifier < 0)
                            {
                                modifiedObject.Placement.Position = new P3Float(modifiedObject.Placement.Position.X - Settings.XPosModifier, modifiedObject.Placement.Position.Y, modifiedObject.Placement.Position.Z);
                            }
                            else if (Settings.XPosModifier > 0)
                            {
                                modifiedObject.Placement.Position = new P3Float(modifiedObject.Placement.Position.X + Settings.XPosModifier, modifiedObject.Placement.Position.Y, modifiedObject.Placement.Position.Z);
                            }
                            else if (Settings.XPosModifier == 0)
                            { 
                                continue; 
                            }
                        }
                    }

                    // Y Position
                    if (Settings.ToggleYPosEdit)
                    {
                        if (modifiedObject.Placement != null)
                        {
                            if (Settings.YPosModifier < 0)
                            {
                                modifiedObject.Placement.Position = new P3Float(modifiedObject.Placement.Position.X, modifiedObject.Placement.Position.Y - Settings.YPosModifier, modifiedObject.Placement.Position.Z);
                            }
                            else if (Settings.YPosModifier > 0)
                            {
                                modifiedObject.Placement.Position = new P3Float(modifiedObject.Placement.Position.X, modifiedObject.Placement.Position.Y + Settings.YPosModifier, modifiedObject.Placement.Position.Z);
                            }
                            else if (Settings.YPosModifier == 0)
                            {  
                                continue; 
                            }
                        }
                    }

                    // Z Position
                    if (Settings.ToggleZPosEdit)
                    {
                        if (modifiedObject.Placement != null)
                        {
                            if (Settings.ZPosModifier < 0)
                            {
                                modifiedObject.Placement.Position = new P3Float(modifiedObject.Placement.Position.X, modifiedObject.Placement.Position.Y, modifiedObject.Placement.Position.Z - Settings.ZPosModifier);
                            }
                            else if (Settings.ZPosModifier > 0)
                            {
                                modifiedObject.Placement.Position = new P3Float(modifiedObject.Placement.Position.X, modifiedObject.Placement.Position.Y, modifiedObject.Placement.Position.Z + Settings.ZPosModifier);
                            }
                            else if (Settings.ZPosModifier == 0)
                            {
                                continue;
                            }
                        }
                    }
                }
            }
        }
    }
}
