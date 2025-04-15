using Iguina.Defs;
using Iguina.Entities;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.Resources;
using MonoGo.Engine.SceneSystem;
using MonoGo.Iguina;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Color = Iguina.Defs.Color;
using Entity = Iguina.Entities.Entity;
using Point = Iguina.Defs.Point;
using Rectangle = Iguina.Defs.Rectangle;

namespace MonoGo.Samples.Demos
{
    public class UIDemo : GUIEntity
    {
        public static void ResetCurrentExample()
        {
            _currentExample = 0;
            UpdateAfterExampleChange();
        } 
        private static int _currentExample = 0;

        public static bool HasTextInput = false;

        private static List<Panel> _panels = new();
        private static Button _nextExampleButton;
        private static Button _previousExampleButton;

        public UIDemo(Layer layer) : base(layer) { }

        public override void CreateUI()
        {
            base.CreateUI();

            _panels.Clear();

            var system = GUIMgr.System;

            // load some alt stylesheets that are not loaded by default from the system stylesheet
            var hProgressBarAltStyle = StyleSheet.LoadFromJsonFile(Path.Combine(GUIMgr.ThemeActiveFolder, "Styles", "progress_bar_horizontal_alt.json"));
            var hProgressBarAltFillStyle = StyleSheet.LoadFromJsonFile(Path.Combine(GUIMgr.ThemeActiveFolder, "Styles", "progress_bar_horizontal_alt_fill.json"));
            var panelTitleStyle = StyleSheet.LoadFromJsonFile(Path.Combine(GUIMgr.ThemeActiveFolder, "Styles", "panel_title.json"));
            var listPanelCentered = StyleSheet.LoadFromJsonFile(Path.Combine(GUIMgr.ThemeActiveFolder, "Styles", "list_panel_centered.json"));
            var listItemCentered = StyleSheet.LoadFromJsonFile(Path.Combine(GUIMgr.ThemeActiveFolder, "Styles", "list_item_centered.json"));

            // icons
            var blizzCrafterIcon = $"${{ICO:Textures/Icons|0|32|32|32|2}}    ";
            var smileyIcon = $"${{ICO:Textures/Icons|32|32|16|16|3}}  ";

            // create top panel
            int topPanelHeight = 65;
            Panel topPanel = new(system, null!)
            {
                Identifier = "Top Panel",
                Anchor = Anchor.TopCenter
            };
            topPanel.Size.Y.SetPixels(topPanelHeight + 2);
            AddGUIEntity(topPanel);

            // add previous example button
            _previousExampleButton = new(system, "<- GUI.Back")
            {
                Anchor = Anchor.TopCenter
            };
            _previousExampleButton.Size.SetPixels(280, topPanelHeight);
            _previousExampleButton.Offset.X.SetPixels(-500);
            _previousExampleButton.Events.OnClick = (Entity control) => PreviousExample();
            topPanel.AddChild(_previousExampleButton);

            // add button to enable debug mode
            {
                Button button = new(system, "Debug Mode")
                {
                    Anchor = Anchor.TopCenter,
                    ToggleCheckOnClick = true
                };
                button.Size.SetPixels(240, topPanelHeight);
                button.Offset.X.SetPixels(-240);
                button.Events.OnClick = (Entity control) => GUIMgr.DebugDraw = !GUIMgr.DebugDraw;
                topPanel.AddChild(button);
            }

            // add button to the GitHub repo
            {
                Button button = new(system, "GitHub")
                {
                    Anchor = Anchor.TopCenter
                };
                button.Size.SetPixels(240, topPanelHeight);
                button.Offset.X.SetPixels(240);
                button.Events.OnClick = (Entity control) =>
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://github.com/MonoGo-Engine/MonoGo",
                        UseShellExecute = true
                    });
                };
                topPanel.AddChild(button);
            }

            // theme switcher drop down.
            DropDown themeDropDown = new(system, listPanelCentered, listItemCentered)
            {
                Identifier = "Theme Switcher",
                Anchor = Anchor.TopCenter,
                AllowDeselect = false
            };
            themeDropDown.Size.X.SetPixels(240);
            foreach (string theme in GUIMgr.ThemeFolders)
            {
                themeDropDown.AddItem(theme);
            }
            themeDropDown.SelectedValue = GUIMgr.ThemeActiveName;
            themeDropDown.Events.OnValueChanged = (Entity control) =>
            {
                GUIMgr.LoadTheme(themeDropDown.SelectedValue);
            };
            topPanel.AddChild(themeDropDown);

            // add next example button
            _nextExampleButton = new(system, "GUI.Next ->")
            {
                Anchor = Anchor.TopCenter
            };
            _nextExampleButton.Size.SetPixels(280, topPanelHeight);
            _nextExampleButton.Offset.X.SetPixels(500);
            _nextExampleButton.Identifier = "next_btn";
            _nextExampleButton.Events.OnClick = (Entity control) => NextExample();
            topPanel.AddChild(_nextExampleButton);

            // init all examples
            if (true)
            {
                // example: welcome message
                {
                    var logo = new Panel(GUIMgr.System, null!)
                    {
                        Identifier = "LOGO",
                        Anchor = Anchor.AutoCenter
                    };
                    var logoTexture = ResourceHub.GetResource<Sprite>("DemoSprites", "Logo")[0].Texture;
                    GUIMgr.RegisterTexture(logoTexture, "Logo");

                    logo.OverrideStyles.Icon = new IconTexture
                    {
                        TextureId = "Logo",
                        CenterHorizontally = true,
                        CenterVertically = true,
                        TextureScale = 0.5f,
                        SourceRect = new Rectangle(0, 0, logoTexture.Width, logoTexture.Height)
                    };
                    logo.OverrideStyles.MarginAfter = new Point(0, 25);
                    logo.Size.SetPixels(logoTexture.Width, logoTexture.Height);

                    // add title and text
                    var panel = CreateDemoContainer(null, new Point(1200, -1));
                    panel.StyleSheet = new StyleSheet(); // Empty StyleSheet to hide the panel.
                    var welcomeText = new Paragraph(system, @$"Welcome to ${{FC:FFDB5F}}Mono${{FC:e60000}}Go${{RESET}}!

A Cross-Platform .NET 8 C# 2D game engine build ontop of ${{FC:e60000}}MonoGame${{RESET}}.

Please click the ${{FC:df00e6}}GUI.Next${{RESET}} button at the top to see more GUI-DEMOS or the ${{FC:FFDB5F}}Next${{RESET}} button below to see more SAMPLE-DEMOS of the engine.

Stay tuned for more things to come! (probably ${{FC:f8c102}}{smileyIcon}${{RESET}} )
") { TextOverflowMode = TextOverflowMode.WrapWords };
                    welcomeText.OverrideStyles.FontSize = 28;
                    panel.AddChild(logo);
                    panel.AddChild(welcomeText);
                    var version = new Paragraph(system, "${FC:FFDB5F}v" + Assembly.GetAssembly(typeof(Engine.EC.Entity)).GetName().Version + "${RESET}")
                    {
                        Anchor = Anchor.AutoRTL
                    };
                    panel.AddChild(version);
                }

                // create container for a demo example
                Panel CreateDemoContainer(string demoTitle, Point size)
                {
                    // create panel
                    var panel = new Panel(system);
                    panel.Size.SetPixels(size.X, size.Y);
                    panel.Anchor = Anchor.Center;
                    panel.AutoHeight = true;
                    panel.OverflowMode = OverflowMode.HideOverflow;
                    AddGUIEntity(panel);
                    _panels.Add(panel);

                    if (demoTitle != null)
                    {
                        // add title and underline
                        panel.AddChild(new Title(system, demoTitle) { Anchor = Anchor.AutoCenter });
                        panel.AddChild(new HorizontalLine(system));
                    }

                    // return panel
                    return panel;
                }

                // anchors
                {
                    var panel = CreateDemoContainer("Anchors", new Point(780, 1));
                    panel.AddChild(new Paragraph(system, @"Controls are positioned using Anchors. An Anchor can be a pre-defined position on the parent control, like Top-Left, or Center."));

                    var anchorsPanel = new Panel(system);
                    anchorsPanel.Size.X.SetPercents(100f);
                    anchorsPanel.Size.Y.SetPixels(400);
                    anchorsPanel.Anchor = Anchor.AutoCenter;
                    panel.AddChild(anchorsPanel);

                    anchorsPanel.AddChild(new Paragraph(system, "TopLeft") { Anchor = Anchor.TopLeft });
                    anchorsPanel.AddChild(new Paragraph(system, "TopRight") { Anchor = Anchor.TopRight });
                    anchorsPanel.AddChild(new Paragraph(system, "TopCenter") { Anchor = Anchor.TopCenter });
                    anchorsPanel.AddChild(new Paragraph(system, "BottomLeft") { Anchor = Anchor.BottomLeft });
                    anchorsPanel.AddChild(new Paragraph(system, "BottomRight") { Anchor = Anchor.BottomRight });
                    anchorsPanel.AddChild(new Paragraph(system, "BottomCenter") { Anchor = Anchor.BottomCenter });
                    anchorsPanel.AddChild(new Paragraph(system, "CenterLeft") { Anchor = Anchor.CenterLeft });
                    anchorsPanel.AddChild(new Paragraph(system, "CenterRight") { Anchor = Anchor.CenterRight });
                    anchorsPanel.AddChild(new Paragraph(system, "Center") { Anchor = Anchor.Center });
                }

                // auto anchors
                {
                    var panel = CreateDemoContainer("Auto Anchors", new Point(750, 1));
                    panel.AddChild(new Paragraph(system,
                        @"Previously we saw regular Anchors. Now its time to explore the Automatic anchors."));

                    panel.AddChild(new RowsSpacer(system));
                    var anchorsPanel = new Panel(system);
                    anchorsPanel.Size.X.SetPercents(100f);
                    anchorsPanel.AutoHeight = true;
                    anchorsPanel.Anchor = Anchor.AutoCenter;
                    panel.AddChild(anchorsPanel);

                    {
                        anchorsPanel.AddChild(new Paragraph(system, "AutoLTR first item.") { Anchor = Anchor.AutoLTR });
                        anchorsPanel.AddChild(new Paragraph(system, "AutoLTR second item. Will be in a different row.") { Anchor = Anchor.AutoLTR });
                        var btn = anchorsPanel.AddChild(new Button(system, "Button set to AutoLTR too.") { Anchor = Anchor.AutoLTR });
                        btn.Size.X.SetPixels(400);
                    }
                    anchorsPanel.AddChild(new HorizontalLine(system));
                    {
                        anchorsPanel.AddChild(new Paragraph(system, "This item is AutoRTL.") { Anchor = Anchor.AutoRTL });
                        anchorsPanel.AddChild(new Paragraph(system, "AutoRTL second item. Will be in a different row.") { Anchor = Anchor.AutoRTL });
                        var btn = anchorsPanel.AddChild(new Button(system, "Button set to AutoRTL too.") { Anchor = Anchor.AutoRTL });
                        btn.Size.X.SetPixels(400);
                    }
                    anchorsPanel.AddChild(new HorizontalLine(system));
                    {
                        {
                            anchorsPanel.AddChild(new Paragraph(system, "We also have inline anchors that arrange entities next to each other, and only break line when need to. For example, AutoInlineLTR buttons:") { Anchor = Anchor.AutoLTR });
                            for (int i = 0; i < 5; ++i)
                            {
                                var btn = anchorsPanel.AddChild(new Button(system, "AutoInlineLTR") { Anchor = Anchor.AutoInlineLTR });
                                btn.Size.X.SetPixels(200);
                            }
                        }
                    }
                }

                // panels
                {
                    var panel = CreateDemoContainer("Panels", new Point(650, 1));
                    panel.AddChild(new Paragraph(system,
                        @"Panels are simple containers for entities. They can have graphics, like the panel this text is in, or be transparent and used only for grouping."));
                    panel.AddChild(new HorizontalLine(system));
                    panel.AddChild(new RowsSpacer(system));
                    {
                        var panelLeft = new Panel(system, null!);
                        panelLeft.IgnoreInteractions = true;
                        panelLeft.AutoHeight = true;
                        panelLeft.Size.X.SetPercents(50f);
                        panelLeft.Anchor = Anchor.AutoInlineLTR;
                        panel.AddChild(panelLeft);

                        panelLeft.AddChild(new Paragraph(system, "Left Panel"));
                        panelLeft.AddChild(new Button(system));
                    }
                    {
                        var panelRight = new Panel(system, null!);
                        panelRight.IgnoreInteractions = true;
                        panelRight.AutoHeight = true;
                        panelRight.Size.X.SetPercents(50f);
                        panelRight.Anchor = Anchor.AutoInlineLTR;
                        panel.AddChild(panelRight);

                        panelRight.AddChild(new Paragraph(system, "Right Panel"));
                        panelRight.AddChild(new Button(system));
                    }

                    panel.AddChild(new RowsSpacer(system));
                    panel.AddChild(new Paragraph(system, @"You can add a small title to panels when you create them:"));
                    panel.AddChild(new RowsSpacer(system, 2));
                    {
                        var titledPanel = new Panel(system);
                        titledPanel.IgnoreInteractions = true;
                        titledPanel.AutoHeight = true;
                        titledPanel.Size.X.SetPercents(100f);
                        titledPanel.Anchor = Anchor.AutoLTR;
                        panel.AddChild(titledPanel);

                        var title = new Paragraph(system, panelTitleStyle, "Panel Title");
                        titledPanel.AddChild(title);
                        title.Anchor = Anchor.TopCenter;
                        title.Offset.Y.SetPixels(-26);

                        titledPanel.AddChild(new Paragraph(system, "Looks nice, isn't it? Check out the source code to see how we did it."));
                    }

                    panel.AddChild(new RowsSpacer(system));
                    panel.AddChild(new Paragraph(system,
                        @"This panel can be dragged by the way, lets try it out!
The small box in the corner is draggable too:"));
                    panel.DraggableMode = DraggableMode.DraggableConfinedToScreen;

                    // create draggable small box
                    var draggableBox = panel.AddChild(new Panel(system)
                    {
                        DraggableMode = DraggableMode.DraggableConfinedToParent,
                        Anchor = Anchor.AutoRTL,
                    });
                    draggableBox.Size.SetPixels(20, 20);
                }

                // buttons
                {
                    var panel = CreateDemoContainer("Buttons", new Point(650, 1));
                    panel.AddChild(new Paragraph(system, "Easily place buttons and register click events:"));
                    {
                        int clicksCount = 0;
                        var btn = panel.AddChild(new Button(system, "Click Me!"));
                        btn.Events.OnClick += (Entity control) =>
                        {
                            clicksCount++;
                            btn.Paragraph.Text = "Thanks x " + clicksCount;
                        };
                    }

                    panel.AddChild(new RowsSpacer(system));
                    panel.AddChild(new Paragraph(system, @"Buttons can also function as checkboxes, allowing you to click on them to toggle their state (checked/unchecked):"));
                    {
                        var btn = panel.AddChild(new Button(system, "Toggle Me!"));
                        btn.ToggleCheckOnClick = true;
                    }

                    panel.AddChild(new RowsSpacer(system));
                    panel.AddChild(new Paragraph(system, @"And they can even function as a radio button, meaning only one button can be checked at any given time:"));
                    {
                        var btn = panel.AddChild(new Button(system, "First Option"));
                        btn.ToggleCheckOnClick = true;
                        btn.CanClickToUncheck = false;
                        btn.ExclusiveSelection = true;
                    }
                    {
                        var btn = panel.AddChild(new Button(system, "Second Option"));
                        btn.ToggleCheckOnClick = true;
                        btn.CanClickToUncheck = false;
                        btn.ExclusiveSelection = true;
                    }
                    {
                        var btn = panel.AddChild(new Button(system, "Third Option"));
                        btn.ToggleCheckOnClick = true;
                        btn.CanClickToUncheck = false;
                        btn.ExclusiveSelection = true;
                    }
                }

                // paragraphs
                {
                    var panel = CreateDemoContainer("Paragraphs", new Point(650, 1));
                    panel.AddChild(new Paragraph(system,
                        @$"${{FC:00FF00}}Paragraphs${{RESET}} are entities that draw text.
They can be used as labels for buttons, titles, or long texts like the one you read now.

${{FC:00FF00}}Paragraphs${{RESET}} support special ${{OC:FF0000}}style changing commands${{RESET}}, so you can easily ${{OC:00FFFF,FC:000000,OW:2}}highlight specific words${{RESET}} within the paragraph.

You can change ${{FC:00FF00}}Fill Color${{RESET}}, ${{OC:AA0000}}Outline Color${{RESET}}, and ${{OW:0}}Outline Width${{RESET}}. 

And you can even embed icons {smileyIcon} inside text paragraphs!"));

                }

                // checkbox and radio
                {
                    var panel = CreateDemoContainer("Checkbox / Radio", new Point(680, 1));
                    
                    panel.AddChild(new Paragraph(system, @"Basic Checkbox control:"));
                    panel.AddChild(new Checkbox(system, "Checkbox Option 1"));
                    panel.AddChild(new Checkbox(system, "Checkbox Option 2"));
                    panel.AddChild(new Checkbox(system, "Checkbox Option 3"));

                    panel.AddChild(new HorizontalLine(system));

                    panel.AddChild(new Paragraph(system, @"Radio button controls:"));
                    panel.AddChild(new RadioButton(system, "Radio Option 1")).Checked = true;
                    panel.AddChild(new RadioButton(system, "Radio Option 2"));
                    panel.AddChild(new RadioButton(system, "Radio Option 3"));
                }

                // sliders
                {
                    var panel = CreateDemoContainer("Sliders", new Point(680, 1));

                    panel.AddChild(new Paragraph(system, @"Sliders are useful to select numeric values:"));
                    {
                        var slider = panel.AddChild(new Slider(system));
                        var label = panel.AddChild(new Label(system, @$"Slider Value: {slider.Value}"));
                        panel.AddChild(new RowsSpacer(system, 2));
                        slider.Events.OnValueChanged = (Entity control) => { label.Text = $"Slider Value: {slider.Value}"; };
                    }

                    panel.AddChild(new Paragraph(system, @"Sliders can also be vertical:"));
                    {
                        var slider = panel.AddChild(new Slider(system, Orientation.Vertical));

                        slider.Size.Y.SetPixels(280);
                        slider.Offset.X.SetPixels(40);
                        var label = panel.AddChild(new Label(system, @$"Slider Value: {slider.Value}"));
                        slider.Events.OnValueChanged = (Entity control) => { label.Text = $"Slider Value: {slider.Value}"; };
                    }
                }

                // progress bars
                {
                    var panel = CreateDemoContainer("Progress Bars", new Point(680, 1));

                    panel.AddChild(new Paragraph(system, @"Progress Bars are similar to sliders, but are designed to show progress or things like health bars:"));
                    {
                        var progressBar = panel.AddChild(new ProgressBar(system));
                        var label = panel.AddChild(new Label(system, @$"Progress Bar Value: {progressBar.Value}"));
                        panel.AddChild(new RowsSpacer(system));
                        float _timeForNextValueChange = 3f;
                        progressBar.Events.AfterUpdate = (Entity control) =>
                        {
                            _timeForNextValueChange -= GUIMgr.System.LastDeltaTime;
                            if (_timeForNextValueChange <= 0f)
                            {
                                progressBar.Value = Random.Shared.Next(progressBar.MaxValue);
                                _timeForNextValueChange = 3f;
                            }
                        };
                        progressBar.Events.OnValueChanged = (Entity control) => { label.Text = $"Progress Bar Value: {progressBar.Value}"; };
                    }

                    panel.AddChild(new Paragraph(system, @"By default Progress Bars are not interactable, but you can make them behave like sliders by settings 'IgnoreInteractions' to false:"));
                    {
                        var progressBar = panel.AddChild(new ProgressBar(system));
                        var label = panel.AddChild(new Label(system, @$"Progress Bar Value: {progressBar.Value}"));
                        panel.AddChild(new RowsSpacer(system));
                        progressBar.Handle.OverrideStyles.TintColor = new Color(255, 0, 0, 255);
                        progressBar.IgnoreInteractions = false;
                        progressBar.Events.OnValueChanged = (Entity control) => { label.Text = $"Progress Bar Value: {progressBar.Value}"; };
                    }

                    panel.AddChild(new Paragraph(system, @"And finally, here's an alternative progress bar design, without animation:"));
                    {
                        var progressBar = panel.AddChild(new ProgressBar(system, hProgressBarAltStyle, hProgressBarAltFillStyle));
                        progressBar.Size.X.SetPixels(420 + 36);
                        progressBar.MaxValue = 11;
                        progressBar.Value = 6;
                        progressBar.IgnoreInteractions = false;
                        progressBar.Anchor = Anchor.AutoCenter;
                    }
                }

                // for lists and dropdowns
                List<string> dndClasses = new List<string> { "Barbarian", "Bard", "Cleric", "Druid", "Fighter", "Monk", "Paladin", "Ranger", "Rogue", "Sorcerer", "Warlock", "Wizard", "Artificer", "Blood Hunter", "Mystic", "Psion", "Alchemist", "Cavalier", "Hexblade", "Arcane Archer", "Samurai", "Zzz" };
                dndClasses.Sort(StringComparer.OrdinalIgnoreCase);

                // list box
                {
                    var panel = CreateDemoContainer("List Box", new Point(680, 1));

                    panel.AddChild(new Paragraph(system, @"List Boxes allow you to add items and select them from a list:"));
                    panel.AddChild(new RowsSpacer(system));
                    {
                        panel.AddChild(new Label(system, @"Select Race:"));
                        var listbox = panel.AddChild(new ListBox(system));
                        listbox.AddItem("Human");
                        listbox.SetItemLabel("Human", new IconTexture() { TextureId = "Textures/Icons.png", SourceRect = new Rectangle(0, 0, 32, 32) }, true);
                        listbox.AddItem("Elf");
                        listbox.SetItemLabel("Elf", new IconTexture() { TextureId = "Textures/Icons.png", SourceRect = new Rectangle(32, 0, 32, 32) }, true);
                        listbox.AddItem("Orc");
                        listbox.SetItemLabel("Orc", new IconTexture() { TextureId = "Textures/Icons.png", SourceRect = new Rectangle(64, 0, 32, 32) }, true);
                        listbox.AddItem("Dwarf");
                        listbox.SetItemLabel("Dwarf", new IconTexture() { TextureId = "Textures/Icons.png", SourceRect = new Rectangle(96, 0, 32, 32) }, true);
                        listbox.AutoHeight = true;
                        listbox.AllowDeselect = false;
                    }
                    {
                        panel.AddChild(new Paragraph(system, @"Clear the selection by clicking the selected item again."));
                        panel.AddChild(new RowsSpacer(system));
                        panel.AddChild(new Label(system, @"Select Class:"));
                        var listbox = panel.AddChild(new ListBox(system));
                        listbox.AutoHeight = false;
                        listbox.Size.Y.SetPixels(250);
                        foreach (var val in dndClasses)
                        {
                            listbox.AddItem(val);
                        }
                        var selectedParagraph = panel.AddChild(new Paragraph(system));
                        selectedParagraph.Text = "Selected Class: None";
                        listbox.Events.OnValueChanged = (Entity control) =>
                        {
                            selectedParagraph.Text = "Selected Class: " + (listbox.SelectedValue ?? "None");
                        };
                    }
                }

                // drop down
                {
                    var panel = CreateDemoContainer("Drop Down", new Point(680, 1));

                    panel.AddChild(new Paragraph(system, @"Drop Down entities are basically list boxes, but the list is hidden while not interacted with. For example:"));
                    panel.AddChild(new RowsSpacer(system));
                    {
                        panel.AddChild(new Label(system, @"Select Race:"));
                        var dropdown = panel.AddChild(new DropDown(system));
                        dropdown.DefaultSelectedText = "< Select Race >";
                        dropdown.AddItem("Human");
                        dropdown.SetItemLabel("Human", new IconTexture() { TextureId = "Textures/Icons.png", SourceRect = new Rectangle(0, 0, 32, 32) }, true);
                        dropdown.AddItem("Elf");
                        dropdown.SetItemLabel("Elf", new IconTexture() { TextureId = "Textures/Icons.png", SourceRect = new Rectangle(32, 0, 32, 32) }, true);
                        dropdown.AddItem("Orc");
                        dropdown.SetItemLabel("Orc", new IconTexture() { TextureId = "Textures/Icons.png", SourceRect = new Rectangle(64, 0, 32, 32) }, true);
                        dropdown.AddItem("Dwarf");
                        dropdown.SetItemLabel("Dwarf", new IconTexture() { TextureId = "Textures/Icons.png", SourceRect = new Rectangle(96, 0, 32, 32) }, true);
                        dropdown.AddItem("Gnome");
                        dropdown.SetItemLabel("Gnome", new IconTexture() { TextureId = "Textures/Icons.png", SourceRect = new Rectangle(128, 0, 32, 32) }, true);
                        dropdown.AddItem("Tiefling");
                        dropdown.SetItemLabel("Tiefling", new IconTexture() { TextureId = "Textures/Icons.png", SourceRect = new Rectangle(160, 0, 32, 32) }, true);
                        dropdown.AllowDeselect = false;
                        dropdown.AutoHeight = true;
                    }
                    {
                        panel.AddChild(new Paragraph(system, @"In the dropdown below, you can clear selection by clicking the selected item again."));
                        panel.AddChild(new RowsSpacer(system));
                        panel.AddChild(new Label(system, @"Select Class:"));
                        var dropdown = panel.AddChild(new DropDown(system));
                        dropdown.SetVisibleItemsCount(7);
                        dropdown.DefaultSelectedText = "< Select Class >";
                        foreach (var val in dndClasses)
                        {
                            dropdown.AddItem(val);
                        }
                        var selectedParagraph = panel.AddChild(new Paragraph(system));
                        selectedParagraph.Text = "Selected Class: None";
                        dropdown.Events.OnValueChanged = (Entity control) =>
                        {
                            selectedParagraph.Text = "Selected Class: " + (dropdown.SelectedValue ?? "None");
                        };
                    }
                }

                // color inputs
                {
                    var panel = CreateDemoContainer("Color Pickers", new Point(650, 350));
                    panel.AutoHeight = true;

                    // color slider
                    {
                        panel.AddChild(new Paragraph(system, @"Color Slider entities can be used to get a color value from a range using a slider and a source texture:"));
                        var slider = panel.AddChild(new ColorSlider(system));
                        var value = panel.AddChild(new Label(system));
                        slider.Events.OnValueChanged = (Entity control) =>
                        {
                            var color = slider.ColorValue;
                            value.Text = $"Color value: {color.R}, {color.G}, {color.B}, {color.A}";
                            value.OverrideStyles.TextFillColor = color;
                        };
                        slider.Value = 1;

                        {
                            var colorBtn = panel.AddChild(new Button(system, "Red"));
                            colorBtn.Anchor = Anchor.AutoLTR;
                            colorBtn.Size.X.SetPercents(24f);
                            colorBtn.Events.OnClick = (Entity control) =>
                            {
                                slider.SetColorValueApproximate(new Color(255, 0, 0, 255));
                            };
                        }
                        {
                            var colorBtn = panel.AddChild(new Button(system, "Green"));
                            colorBtn.Anchor = Anchor.AutoInlineLTR;
                            colorBtn.Size.X.SetPercents(24f);
                            colorBtn.Events.OnClick = (Entity control) =>
                            {
                                slider.SetColorValueApproximate(new Color(0, 255, 0, 255));
                            };
                        }
                        {
                            var colorBtn = panel.AddChild(new Button(system, "Blue"));
                            colorBtn.Anchor = Anchor.AutoInlineLTR;
                            colorBtn.Size.X.SetPercents(24f);
                            colorBtn.Events.OnClick = (Entity control) =>
                            {
                                slider.SetColorValueApproximate(new Color(0, 0, 255, 255));
                            };
                        }
                        {
                            var colorBtn = panel.AddChild(new Button(system, "Purple"));
                            colorBtn.Anchor = Anchor.AutoInlineLTR;
                            colorBtn.Size.X.SetPercents(24f);
                            colorBtn.Events.OnClick = (Entity control) =>
                            {
                                slider.SetColorValueApproximate(new Color(255, 0, 255, 255));
                            };
                        }
                    }

                    panel.AddChild(new RowsSpacer(system));
                    panel.AddChild(new HorizontalLine(system));

                    // color picker
                    {
                        panel.AddChild(new Paragraph(system, @"Color Picker entities can be used to get a color value from a rectangle region by picking pixels off a source texture:"));
                        var picker = panel.AddChild(new ColorPicker(system));
                        var value = panel.AddChild(new Label(system));
                        picker.Events.OnValueChanged = (Entity control) =>
                        {
                            var color = picker.ColorValue;
                            value.Text = $"Color value: {color.R}, {color.G}, {color.B}, {color.A}";
                            value.OverrideStyles.TextFillColor = color;
                        };

                        {
                            var colorBtn = panel.AddChild(new Button(system, "Red"));
                            colorBtn.Anchor = Anchor.AutoLTR;
                            colorBtn.Size.X.SetPercents(24f);
                            colorBtn.Events.OnClick = (Entity control) =>
                            {
                                picker.SetColorValueApproximate(new Color(255, 0, 0, 255));
                            };
                        }
                        {
                            var colorBtn = panel.AddChild(new Button(system, "Green"));
                            colorBtn.Anchor = Anchor.AutoInlineLTR;
                            colorBtn.Size.X.SetPercents(24f);
                            colorBtn.Events.OnClick = (Entity control) =>
                            {
                                picker.SetColorValueApproximate(new Color(0, 255, 0, 255));
                            };
                        }
                        {
                            var colorBtn = panel.AddChild(new Button(system, "Blue"));
                            colorBtn.Anchor = Anchor.AutoInlineLTR;
                            colorBtn.Size.X.SetPercents(24f);
                            colorBtn.Events.OnClick = (Entity control) =>
                            {
                                picker.SetColorValueApproximate(new Color(0, 0, 255, 255));
                            };
                        }
                        {
                            var colorBtn = panel.AddChild(new Button(system, "Purple"));
                            colorBtn.Anchor = Anchor.AutoInlineLTR;
                            colorBtn.Size.X.SetPercents(24f);
                            colorBtn.Events.OnClick = (Entity control) =>
                            {
                                picker.SetColorValueApproximate(new Color(255, 0, 255, 255));
                            };
                        }
                    }
                }

                // scrollbars
                {
                    var panel = CreateDemoContainer("Scrollbars", new Point(780, 450));
                    panel.AutoHeight = false;
                    panel.CreateVerticalScrollbar(true);
                    panel.AddChild(new Paragraph(system,
                        @"Sometimes panels content is too long, and we need scrollbars to show everything.
This panel has some random controls below that go wayyyy down.

Use the scrollbar on the right to see more of it.
"));
                    panel.AddChild(new Button(system, "Some Button"));
                    panel.AddChild(new Paragraph(system,
                        @"
Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
"));
                    panel.AddChild(new Button(system, "Another Button"));
                    panel.AddChild(new Slider(system));
                    panel.AddChild(new Checkbox(system, "A Checkbox"));
                    panel.AddChild(new RadioButton(system, "A Radio Button"));
                    var listbox = panel.AddChild(new ListBox(system));
                    listbox.AddItem("Human");
                    listbox.AddItem("Elf");
                    listbox.AddItem("Orc");
                    listbox.AddItem("Dwarf");
                    listbox.Size.Y.SetPixels(170);
                }

                // text input
                {
                    var panel = CreateDemoContainer("Text Input", new Point(680, 1));

                    panel.AddChild(new Paragraph(system, @"Text Input entity is useful to get free text input from users. This is a single-line text input:"));
                    {
                        var textInput = panel.AddChild(new TextInput(system));
                        textInput.PlaceholderText = "Click to edit text input.";
                    }

                    panel.AddChild(new RowsSpacer(system));
                    panel.AddChild(new Paragraph(system, @"And here's a multiline text input:"));
                    {
                        var textInput = panel.AddChild(new TextInput(system));
                        textInput.PlaceholderText = "A multiline text input..\nClick to edit.";
                        textInput.Size.Y.SetPixels(300);
                        textInput.Multiline = true;
                        //textInput.MaxLines = 8;
                        textInput.CreateVerticalScrollbar();
                    }

                    panel.AddChild(new RowsSpacer(system));
                    panel.AddChild(new Paragraph(system, @"You can also mask the text, for password input:"));
                    {
                        var textInput = panel.AddChild(new TextInput(system));
                        textInput.PlaceholderText = "Password";
                        textInput.MaskingCharacter = '*';
                        var showPassword = panel.AddChild(new Checkbox(system, "Show Password"));
                        showPassword.Events.OnValueChanged = (Entity control) =>
                        {
                            textInput.MaskingCharacter = showPassword.Checked ? null : '*';
                        };
                    }
                }

                // numeric text input
                {
                    var panel = CreateDemoContainer("Numeric Input", new Point(680, 1));

                    panel.AddChild(new Paragraph(system, @"Numeric text input get float or integer value from the user in a form similar to a text input. For example, with decimal point:"));
                    {
                        var textInput = panel.AddChild(new NumericInput(system));
                    }

                    panel.AddChild(new RowsSpacer(system));
                    panel.AddChild(new Paragraph(system, @"This Numeric Input don't accept a decimal point:"));
                    {
                        var textInput = panel.AddChild(new NumericInput(system));
                        textInput.AcceptsDecimal = false;
                    }

                    panel.AddChild(new RowsSpacer(system));
                    panel.AddChild(new Paragraph(system, @"This Numeric Input has min and max limits (-10, 10):"));
                    {
                        var textInput = panel.AddChild(new NumericInput(system));
                        textInput.MinValue = -10;
                        textInput.MaxValue = 10;
                    }

                    panel.AddChild(new RowsSpacer(system));
                    panel.AddChild(new Paragraph(system, @"You can also create a Numeric Input entity without the buttons:"));
                    {
                        var textInput = panel.AddChild(new NumericInput(system, false, false));
                    }
                }

                // message boxes
                {
                    var panel = CreateDemoContainer("Message Boxes", new Point(780, 1));
                    panel.AddChild(new Paragraph(system, @"Message boxes are useful to get quick input from the user. 
Click below to see an example."));

                    panel.AddChild(new Button(system, "Show Message Box")).Events.OnClick = (Entity control) =>
                    {
                        system.MessageBoxes.ShowConfirmMessageBox("Hi There!",
                            @"This is a simple message box with just confirm / cancel options.

Note that message boxes can have their own stylesheets, and you can set their defaults per-system.

This specific message box won't do much.
You can just close it.");
                    };

                    panel.AddChild(new RowsSpacer(system));
                }

                // locked / disabled
                {
                    var panel = CreateDemoContainer("Locked / Disabled", new Point(900, 600));
                    panel.AutoHeight = false;
                    panel.CreateVerticalScrollbar();
                    panel.AddChild(new Paragraph(system, @"You can disable controls to make them ignore user interactions and render them with 'disabled' effect (you can create your own effects for this):"));
                    panel.AddChild(new Button(system, "Disabled Button") { Enabled = false });
                    panel.AddChild(new RowsSpacer(system));
                    panel.AddChild(new Paragraph(system, @"When you disable a panel, all controls under it will be disabled too.

If you want to just lock items without rendering them with 'disabled' style, you can also set the Locked property. For example the following button is locked, but will render normally:"));
                    panel.AddChild(new RowsSpacer(system));
                    panel.AddChild(new Button(system, "Locked Button") { Locked = true });
                    panel.AddChild(new RowsSpacer(system));
                    panel.AddChild(new Paragraph(system, @"Any type of control can be locked and disabled:"));
                    panel.AddChild(new Slider(system) { Enabled = false });
                    panel.AddChild(new Checkbox(system, "Disabled Checkbox") { Enabled = false });
                    panel.AddChild(new RadioButton(system, "Disabled Radio Button") { Enabled = false });
                    var listbox = panel.AddChild(new ListBox(system));
                    listbox.AddItem("Human");
                    listbox.AddItem("Elf");
                    listbox.AddItem("Orc");
                    listbox.AddItem("Dwarf");
                    listbox.Size.Y.SetPixels(140);
                    listbox.Enabled = false;
                }

                // example: epilogue
                {
                    var panel = CreateDemoContainer("End Of GUI-DEMO", new Point(700, 400));
                    panel.StyleSheet = new StyleSheet();

                    // add title and text
                    panel.AddChild(new Paragraph(system, $@"That was only the GUI-DEMO! There is still much to learn about ${{FC:FFDB5F}}Mono${{FC:e60000}}Go${{RESET}}.

Try more samples by clicking the ${{FC:FFDB5F}}Next${{RESET}} button below.

If you like this engine then don't forget to star the repo on ${{FC:96FF5F}}GitHub${{RESET}} with the button above.

Have a nice day!

{blizzCrafterIcon} ${{FC:df00e6}}BlizzCrafter${{RESET}}"));
                }

                // init panels and buttons
                UpdateAfterExampleChange();

            }
        }

        public override void Update()
        {
            base.Update();

            _panels[_currentExample].IterateChildren(
                x =>
                {
                    if (x is TextInput)
                    {
                        if (x.IsTargeted)
                        {
                            HasTextInput = true;
                            x.UserData = "!Target!";
                            return false;
                        }
                        else if (x.UserData != null && x.UserData.Equals("!Target!"))
                        {
                            HasTextInput = false;
                            x.UserData = null;
                            return false;
                        }
                    }
                    return true;
                });
        }

        /// <summary>
        /// Show next UI example.
        /// </summary>
        public void NextExample()
        {
            _currentExample++;
            UpdateAfterExampleChange();
        }

        /// <summary>
        /// Show previous UI example.
        /// </summary>
        public void PreviousExample()
        {
            _currentExample--;
            UpdateAfterExampleChange();
        }

        /// <summary>
        /// Called after we change current example index, to hide all examples
        /// except for the currently active example + disable prev / next buttons if
        /// needed (if first or last example).
        /// </summary>
        private static void UpdateAfterExampleChange()
        {
            // hide all panels and show current example panel
            foreach (Panel panel in _panels)
            {
                panel.Visible = false;
            }
            _panels[_currentExample].Visible = true;

            // disable / enable next and previous buttons
            _nextExampleButton.Enabled = _currentExample != _panels.Count - 1;
            _previousExampleButton.Enabled = _currentExample != 0;
        }
    }
}
