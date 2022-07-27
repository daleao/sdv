namespace DaLion.Stardew.Alchemy.Framework.UI;

#region using directives

using Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using Textures;

#endregion using directives

public class AlchemyMenu : ItemGrabMenu
{
    #region constants

    // layout
    private const int SCALE_I = 4;
    private const int SCALE_SMALL_I = 3;
    private const int LIST_ROWS_I = 5;
    private const int LIST_HEIGHT_I = 16 * SCALE_I;
    private const int GRID_ROWS_I = 4;
    private const int GRID_COLUMNS_I = 4;
    private const int GRID_HEIGHT_I = 18 * SCALE_I;
    private const int INVENTORY_SELECT_BUTTONS_WIDE_I = 2;
    private const int INGREDIENT_SLOTS_WIDE_I = 3;
    private const int MARGIN_LEFT_I = 16 * SCALE_I;
    private const int MARGIN_RIGHT_I = 8 * SCALE_I;
    private const int TEXT_DIVIDER_GAP_I = 1 * SCALE_I;
    private const int TEXT_SPACING_FROM_ICONS_I = 20 * SCALE_I;
    private const int TEXT_MUFFIN_TOP_OVER_DIVIDER_I = (int)(1.5f * SCALE_I);
    private const int ANIM_FRAME_TIME_I = 100;
    private const int ANIM_FRAMES_I = 8;
    private const int ANIM_TIMER_LIMIT_I = ANIM_FRAME_TIME_I * ANIM_FRAMES_I;
    private const float DIVIDER_OPACITY_F = 0.5f;

    // sound cues
    private const string CLICK_CUE_S = "coin";
    private const string CANCEL_CUE_S = "cancel";
    private const string SUCCESS_CUE_S = "reward";
    private const string HOVER_IN_CUE_S = "breathin";
    private const string HOVER_OUT_CUE_S = "breathout";
    private const string PAGE_CHANGE_CUE_S = "shwip";
    private const string MENU_CHANGE_CUE_S = "bigSelect";
    private const string MENU_CLOSE_CUE_S = "bigDeSelect";

    // inventory containers
    private const int BACKPACK_INVENTORY_ID_I = 0;
    private const int MAX_EXTRA_INVENTORIES_I = 24;

    // other
    private const string QUANTITY_TEXT_BOX_DEFAULT_I = "1";

    #endregion constants

    #region static fields

    // custom spritesheet source rectangles
    private static readonly Rectangle BookSource = new(0, 32, 238, 123);
    private static readonly Rectangle IngredientSlotSource = new(112, 0, 20, 20);
    private static readonly Rectangle CookButtonSource = new(110, 155, 16, 22);
    private static readonly Rectangle SearchTabButtonSource = new(32, 0, 16, 16);
    private static readonly Rectangle PotionsTabButtonSource = new(48, 0, 16, 16);
    private static readonly Rectangle BombsTabButtonSource = new(64, 0, 16, 16);
    private static readonly Rectangle OilsTabButtonSource = new(80, 0, 16, 16);
    private static readonly Rectangle MiscTabButtonSource = new(96, 0, 16, 16);
    private static readonly Rectangle FilterButtonSource = new(0, 16, 16, 16);
    private static readonly Rectangle ViewStyleButtonSource = new(16, 16, 16, 16);
    private static readonly Rectangle SortButtonSource = new(48, 16, 16, 16);
    private static readonly Rectangle SearchButtonSource = new(64, 16, 16, 16);
    private static readonly Rectangle AutofillButtonSource = new(80, 16, 16, 16);
    private static readonly Rectangle InventoryTabButtonSource = new(132, 0, 16, 21);
    private static readonly Rectangle InventoryChestIconSource = new(229, 19, 9, 13);
    private static readonly Rectangle UpSmallButtonSource = new(148, 0, 8, 8);
    private static readonly Rectangle DownSmallButtonSource = new(148, 8, 8, 8);
    private static readonly Rectangle AetherSubstanceSource = new(112, 23, 9, 9);
    private static readonly Rectangle HydragenumSubstanceSource = new(121, 23, 9, 9);
    private static readonly Rectangle QuebrithSubstanceSource = new(130, 23, 9, 9);
    private static readonly Rectangle RebisSubstanceSource = new(139, 23, 9, 9);
    private static readonly Rectangle VermilionSubstanceSource = new(148, 23, 9, 9);
    private static readonly Rectangle VitriolSubstanceSource = new(157, 23, 9, 9);
    private static readonly Rectangle AlcoholIconSource = new(193, 20, 12, 12);
    private static readonly Rectangle PowderIconSource = new(205, 20, 12, 12);
    private static readonly Rectangle GreaseIconSource = new(217, 20, 12, 12);

    // vanilla cursor source rectangles
    private static readonly Rectangle DownButtonSource = new(0, 64, 64, 64);
    private static readonly Rectangle UpButtonSource = new(64, 64, 64, 64);
    private static readonly Rectangle RightButtonSource = new(0, 192, 64, 64);
    private static readonly Rectangle LeftButtonSource = new(0, 256, 64, 64);
    private static readonly Rectangle MinusButtonSource = new(177, 345, 7, 8);
    private static readonly Rectangle PlusButtonSource = new(184, 345, 7, 8);
    private static readonly Rectangle OkButtonSource = new(128, 256, 64, 64);
    private static readonly Rectangle NoButtonSource = new(192, 256, 64, 64);
    private static readonly Rectangle RedCrossSource = new(268, 470, 16, 16);
    private static readonly Rectangle InventoryBackpackIconSource = new(257, 1436, 9, 13);
    private static readonly Rectangle IridiumStarSource = new(346, 392, 8, 8);

    // colors
    private static readonly Color SubtextColor = Game1.textColor * 0.75f;
    private static readonly Color BlockedColor = Game1.textColor * 0.325f;
    private static Color DividerColor;

    // localization
    private static readonly Dictionary<string, Rectangle> MixTextSourceByLocale = new()
    {
        { "de", new(47, 179, 43, 12) },
        { "en", new(0, 155, 21, 12) },
        { "es", new(0, 167, 43, 12) },
        { "fr", new(0, 179, 45, 12) },
        { "it", new(45, 167, 57, 12) },
        { "pt", new(23, 154, 48, 12) },
        { "po", new(0, 191, 42, 12) }
    };

    #endregion static fields

    #region instance fields

    public readonly List<ClickableComponent> ChildComponents = new();
    private readonly List<ClickableTextureComponent> _ingredientClickables = new();

    // navigation buttons
    private readonly ClickableTextureComponent _navDownButton;
    private readonly ClickableTextureComponent _navUpButton;
    private readonly ClickableTextureComponent _navRightButton;
    private readonly ClickableTextureComponent _navLeftButton;

    // toggle buttons
    private readonly ClickableTextureComponent _toggleFilterButton;
    private readonly ClickableTextureComponent _toggleViewButton;
    private readonly ClickableTextureComponent _toggleSortOrderButton;
    private readonly ClickableTextureComponent _toggleAutofillButton;
    private readonly List<ClickableTextureComponent> _toggleButtonClickables = new();

    // search box
    private readonly ClickableComponent _searchBarClickable;
    private readonly ClickableTextureComponent _searchButton;
    private readonly TextBox _searchBarTextBox;
    private Rectangle _searchBarTextBoxBounds;
    private int _searchBarTextBoxMaxWidth;
    private int _searchBarTextBoxMinWidth;

    // tab selection
    private readonly ClickableTextureComponent _searchTabButton;
    private readonly ClickableTextureComponent _potionsTabButton;
    private readonly ClickableTextureComponent _bombsTabButton;
    private readonly ClickableTextureComponent _oilsTabButton;
    private readonly ClickableTextureComponent _miscTabButton;
    private readonly Stack<Tab> _tabHistory = new();

    // mix button, quantity selection and confirmation
    private readonly ClickableComponent _mixButton;
    private Rectangle _mixTextSource;
    private int _mixTextMidpoint;
    private Rectangle _mixIconBounds;
    private readonly TextBox _quantityTextBox;
    private Rectangle _quantityTextBoxBounds;
    private readonly ClickableTextureComponent _quantityUpButton;
    private readonly ClickableTextureComponent _quantityDownButton;
    private Rectangle _quantityScrollableArea;
    private readonly ClickableTextureComponent _confirmButton;
    private readonly ClickableTextureComponent _cancelButton;
    private bool _showingConfirmationDialog;

    // book content
    private Rectangle _bookLeftRect = new(-1, -1, BookSource.Width * SCALE_I / 2, BookSource.Height * SCALE_I);
    private Rectangle _bookRightRect = new(-1, -1, BookSource.Width * SCALE_I / 2, BookSource.Height * SCALE_I);
    private Point _leftContent;
    private Point _rightContent;
    private int _lineWidth;
    private int _textWidth;
    private readonly List<ClickableComponent> _listViewClickables = new();
    private readonly List<ClickableComponent> _gridViewClickables = new();
    private Rectangle _formulaeDisplayArea;

    // inventory selection
    private readonly List<Chest> _adjacentContainers = new();
    private readonly List<KeyValuePair<Color, bool>> _containerColours = new();
    private readonly ClickableTextureComponent _inventoryTabButton;
    private readonly ClickableTextureComponent _inventoryUpButton;
    private readonly ClickableTextureComponent _inventoryDownButton;
    private readonly List<ClickableTextureComponent> _inventorySelectButtons = new();
    private Rectangle _inventoryCardArea; ////
    private Rectangle _inventoriesScrollableArea;
    private Rectangle _inventoriesPopoutArea;
    private int _currentInventoryId = 0;
    private bool _showingInventoriesPopout;

    // animation
    private int _animTimer;
    private int _animFrame;

    // formulae
    private List<Formula> _displayedFormulae;
    private int _selectedFormulaIndex;
    private readonly ClickableTextureComponent _selectedFormulaIcon;

    // ingredients
    private readonly List<Item> _availableIngredients;

    // other
    private int _mouseHeldTicks = 0;
    private string _locale = "";
    private readonly Dictionary<int, double> _iconShakeTimerField = new();
    internal static readonly int SpriteId = (int)Game1.player.UniqueMultiplayerID + 7070707;

    #endregion instance fields

    #region properties

    private IEnumerable<Formula> _AllFormulae => ModEntry.PlayerState.KnownFormulae;
    private IEnumerable<Formula> _AbleToMix => _AllFormulae.Where(formula => formula.CanMix(_availableIngredients));
    private Formula? _SelectedFormula => _selectedFormulaIndex < _displayedFormulae.Count ? _displayedFormulae[_selectedFormulaIndex] : null;
    private bool _ReadyToMix => _SelectedFormula?.CanMix() == true;

    private static bool _UsingGridView
    {
        get => ModEntry.PlayerState.UsingGridView;
        set => ModEntry.PlayerState.UsingGridView = value;
    }

    private static bool _AppliedFiltering
    {
        get => ModEntry.PlayerState.AppliedFiltering;
        set => ModEntry.PlayerState.AppliedFiltering = value;
    }

    private static bool _ReversedSortOrder
    {
        get => ModEntry.PlayerState.ReversedSortOrder;
        set => ModEntry.PlayerState.ReversedSortOrder = value;
    }

    private bool _UsingAutofill => ModEntry.PlayerState.Autofill != Autofill.Off;
    private Autofill _Autofill
    {
        get => ModEntry.PlayerState.Autofill;
        set => ModEntry.PlayerState.Autofill = value;
    }

    private bool _UseHorizontalInventorySelection => _inventorySelectButtons.Count > 0 && Context.IsSplitScreen;
    private bool _ShouldShowInventoryElements => _adjacentContainers.Count > 1;

    #endregion properties

    #region enums

    public enum Autofill
    {
        Off,
        Cheapest,
        Purest
    }

    public enum Tab
    {
        Search,
        Potions,
        Bombs,
        Oils,
        Misc
    }

    #endregion enums

    public AlchemyMenu(IEnumerable<Chest>? containers = null)
        : base(null)
    {
        /// set dimensions and localization
        width = BookSource.Width * SCALE_I;
        height = 720;
        initializeUpperRightCloseButton();

        _locale = LocalizedContentManager.CurrentLanguageCode.ToString();
        if (!MixTextSourceByLocale.TryGetValue(_locale, out _mixTextSource))
            _mixTextSource = MixTextSourceByLocale["en"];

        /// initialize clickable components

        // navigation buttons
        _navDownButton = new("navDown", new(-1, -1, DownButtonSource.Width, DownButtonSource.Height), null, null,
            Game1.mouseCursors, DownButtonSource, 1f, true);
        _navUpButton = new("navUp", new(-1, -1, UpButtonSource.Width, UpButtonSource.Height), null, null,
            Game1.mouseCursors, UpButtonSource, 1f, true);
        _navRightButton = new("navRight", new(-1, -1, RightButtonSource.Width, RightButtonSource.Height), null, null,
            Game1.mouseCursors, RightButtonSource, 1f, true);
        _navLeftButton = new("navLeft", new(-1, -1, LeftButtonSource.Width, LeftButtonSource.Height), null, null,
            Game1.mouseCursors, LeftButtonSource, 1f, true);

        // toggle buttons
        _toggleFilterButton = new("toggleFilter",
            new(-1, -1, FilterButtonSource.Width * SCALE_SMALL_I, FilterButtonSource.Height * SCALE_SMALL_I), null,
            ModEntry.i18n.Get("menu.mixing_search.filter_label"), Textures.InterfaceTx, FilterButtonSource,
            SCALE_SMALL_I, true);
        _toggleSortOrderButton = new("toggleOrder",
            new(-1, -1, SortButtonSource.Width * SCALE_SMALL_I, SortButtonSource.Height * SCALE_SMALL_I), null,
            ModEntry.i18n.Get("menu.mixing_search.order_label"), Textures.InterfaceTx, SortButtonSource, SCALE_SMALL_I,
            true);
        _toggleViewButton = new("toggleView",
            new(-1, -1, ViewStyleButtonSource.Width * SCALE_SMALL_I, ViewStyleButtonSource.Height * SCALE_SMALL_I),
            null, ModEntry.i18n.Get("menu.mixing_search.view." + (_UsingGridView ? "grid" : "list")),
            Textures.InterfaceTx, ViewStyleButtonSource, SCALE_SMALL_I, true);
        _toggleAutofillButton = new("autofill",
            new(-1, -1, AutofillButtonSource.Width * SCALE_SMALL_I, AutofillButtonSource.Height * SCALE_SMALL_I), null,
            ModEntry.i18n.Get("menu.mixing_recipe.autofill_label"), Textures.InterfaceTx, AutofillButtonSource,
            SCALE_SMALL_I, true);

        _toggleButtonClickables.AddRange(new[]
        {
            _toggleFilterButton,
            _toggleSortOrderButton,
            _toggleViewButton,
            _toggleAutofillButton
        });
        _toggleViewButton.sourceRect.X = ViewStyleButtonSource.X + (_UsingGridView
            ? ViewStyleButtonSource.Width
            : 0);
        _toggleAutofillButton.sourceRect.X = AutofillButtonSource.X;
        if (_Autofill >= Autofill.Cheapest) _toggleAutofillButton.sourceRect.X += AutofillButtonSource.X;
        if (_Autofill >= Autofill.Purest) _toggleAutofillButton.sourceRect.X += AutofillButtonSource.X;

        // search box
        _searchBarClickable = new(Rectangle.Empty, "searchbox");
        _searchButton = new("search",
            new(-1, -1, SearchButtonSource.Width * SCALE_SMALL_I, SearchButtonSource.Height * SCALE_SMALL_I), null,
            ModEntry.i18n.Get("menu.mixing_recipe.search_label"), Textures.InterfaceTx, SearchButtonSource,
            SCALE_SMALL_I, true);
        _searchBarTextBox = new(
            Game1.content.Load<Texture2D>("LooseSprites\\textBox"),
            null, Game1.smallFont, Game1.textColor)
        {
            textLimit = 32,
            Selected = false,
            Text = "Search",
        };

        // tab selection
        _searchTabButton = new(
            "searchTab", new(-1, -1, SearchTabButtonSource.Width * SCALE_I, SearchTabButtonSource.Height * SCALE_I),
            null, null, Textures.InterfaceTx, SearchTabButtonSource, SCALE_I, true);
        _potionsTabButton = new(
            "potionsTab", new(-1, -1, PotionsTabButtonSource.Width * SCALE_I, PotionsTabButtonSource.Height * SCALE_I),
            null, null, Textures.InterfaceTx, PotionsTabButtonSource, SCALE_I, true);
        _bombsTabButton = new(
            "bombsTab", new(-1, -1, BombsTabButtonSource.Width * SCALE_I, BombsTabButtonSource.Height * SCALE_I),
            null, null, Textures.InterfaceTx, BombsTabButtonSource, SCALE_I, true);
        _oilsTabButton = new(
            "oilsTab", new(-1, -1, OilsTabButtonSource.Width * SCALE_I, OilsTabButtonSource.Height * SCALE_I),
            null, null, Textures.InterfaceTx, OilsTabButtonSource, SCALE_I, true);
        _miscTabButton = new(
            "miscTab", new(-1, -1, MiscTabButtonSource.Width * SCALE_I, MiscTabButtonSource.Height * SCALE_I),
            null, null, Textures.InterfaceTx, MiscTabButtonSource, SCALE_I, true);

        // mix button, quantity selection and confirmation
        _mixButton = new(Rectangle.Empty, "mix");
        _quantityTextBox = new(
            Game1.content.Load<Texture2D>("LooseSprites\\textBox"),
            null, Game1.smallFont, Game1.textColor)
        {
            numbersOnly = true,
            textLimit = 2,
            Selected = false,
            Text = QUANTITY_TEXT_BOX_DEFAULT_I,
        };

        _quantityUpButton = new("quantityUp", new(-1, -1, PlusButtonSource.Width * 4, PlusButtonSource.Height * 4),
            null, null, Game1.mouseCursors, PlusButtonSource, 4f, true);
        _quantityDownButton = new("quantityDown",
            new(-1, -1, MinusButtonSource.Width * 4, MinusButtonSource.Height * 4), null, null, Game1.mouseCursors,
            MinusButtonSource, 4f, true);
        _confirmButton = new("confirm", new(-1, -1, OkButtonSource.Width, OkButtonSource.Height), null, null,
            Game1.mouseCursors, OkButtonSource, 1f, true);
        _cancelButton = new("cancel", new(-1, -1, NoButtonSource.Width, NoButtonSource.Height), null, null,
            Game1.mouseCursors, NoButtonSource, 1f, true);

        // book content
        for (var i = 0; i < LIST_ROWS_I; ++i)
            _listViewClickables.Add(new(new(-1, -1, -1, -1), "listView" + i));

        for (var i = 0; i < GRID_ROWS_I * GRID_COLUMNS_I; ++i)
            _gridViewClickables.Add(new(new(-1, -1, -1, -1), "gridView" + i));

        // inventory selection
        _inventoryTabButton = new("inventoryTab",
            new(-1, -1, InventoryTabButtonSource.Width * SCALE_I, InventoryTabButtonSource.Height * SCALE_I),
            null, null, Textures.InterfaceTx, InventoryTabButtonSource, SCALE_I);
        _inventoryUpButton = new("inventoryUp",
            new(-1, -1, UpSmallButtonSource.Width * SCALE_I, UpSmallButtonSource.Height * SCALE_I),
            null, null, Textures.CursorsTx, UpSmallButtonSource, SCALE_I);
        _inventoryDownButton = new("inventoryDown",
            new(-1, -1, DownSmallButtonSource.Width * SCALE_I, DownSmallButtonSource.Height * SCALE_I),
            null, null, Textures.CursorsTx, DownSmallButtonSource, SCALE_I);

        /// populate available ingredients

        containers ??= Enumerable.Empty<Chest>();
        _adjacentContainers.AddRange(containers.Take(MAX_EXTRA_INVENTORIES_I));
        if (_adjacentContainers.Count > 0)
        {
            var destRect = new Rectangle(-1, -1, 16 * SCALE_I, 16 * SCALE_I);
            _inventorySelectButtons.Add(new("backpack", destRect, null, null, Textures.InterfaceTx,
                InventoryBackpackIconSource, SCALE_I));
            for (var i = 0; i < _adjacentContainers.Count; ++i)
                _inventorySelectButtons.Add(new($"chest{i}", destRect, null, null, Textures.InterfaceTx,
                    InventoryChestIconSource, SCALE_I));
        }

        _availableIngredients = Game1.player.Items
            .Concat(_adjacentContainers.SelectMany(container => container.items))
            .Where(item => item is not null && (item.IsValidIngredient() || item.IsAlchemicalBase())).ToList();

        /// align and set some base fields

        AlignElements();

        okButton = null;
        trashCan = null;
        canExitOnKey = true;
    }

    private void AlignElements()
    {
        // book
        var (cx, cy) = Utility.PointToVector2(Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea().Center);
        if (Context.IsSplitScreen) cx /= 2;

        int xOffset = 0, yOffset = 54 * SCALE_I;
        if (_UseHorizontalInventorySelection) yOffset = yOffset / 3 * 2;

        xPositionOnScreen = (int)(cx - BookSource.Center.X * SCALE_I * Game1.options.uiScale + xOffset);
        yPositionOnScreen = (int)(cy - BookSource.Center.Y * SCALE_I * Game1.options.uiScale + yOffset);

        _bookLeftRect.X = xPositionOnScreen;
        _bookRightRect.X = _bookLeftRect.X + _bookLeftRect.Width;
        _bookLeftRect.Y = _bookRightRect.Y = yPositionOnScreen;

        _leftContent = new(_bookLeftRect.X + MARGIN_LEFT_I, _bookLeftRect.Y);
        _rightContent = new(_bookRightRect.X + MARGIN_RIGHT_I, _bookRightRect.Y);

        _lineWidth = _bookLeftRect.Width - MARGIN_LEFT_I * 3 / 2;
        _textWidth = _lineWidth + TEXT_MUFFIN_TOP_OVER_DIVIDER_I * 2;

        upperRightCloseButton.bounds.Y = yPositionOnScreen + 9 * SCALE_I;
        upperRightCloseButton.bounds.X = xPositionOnScreen + (BookSource.Width - 11) * SCALE_I;

        if (Context.IsSplitScreen)
        {
            var pos = upperRightCloseButton.bounds.X + upperRightCloseButton.bounds.Width;
            var bound = Game1.viewport.Width / 2;
            var scale = Game1.options.uiScale;
            var diff = (pos - bound) * scale;
            upperRightCloseButton.bounds.X -= (int)Math.Max(0, diff / 2);
        }

        // navigation
        _navUpButton.bounds.X = _navDownButton.bounds.X = _searchButton.bounds.X + SCALE_I;
        _navUpButton.bounds.Y = _searchButton.bounds.Y + _searchButton.bounds.Height + 4 * SCALE_I;
        _navDownButton.bounds.Y = _bookLeftRect.Bottom - 32 * SCALE_I;
        _navLeftButton.bounds.X = _leftContent.X - 6 * SCALE_I;
        _navRightButton.bounds.X = _navLeftButton.bounds.X + _lineWidth - 3 * SCALE_I;
        _navRightButton.bounds.Y = _navLeftButton.bounds.Y = _leftContent.Y + 6 * SCALE_I;

        // tabs
        _searchTabButton.bounds.Y = _bookLeftRect.Y + 18 * SCALE_I;
        _potionsTabButton.bounds.Y = _searchTabButton.bounds.Y + _searchTabButton.bounds.Height + 4 * SCALE_I;
        _bombsTabButton.bounds.Y = _potionsTabButton.bounds.Y + _potionsTabButton.bounds.Height + 4 * SCALE_I;
        _oilsTabButton.bounds.Y = _bombsTabButton.bounds.Y + _bombsTabButton.bounds.Height + 4 * SCALE_I;
        _miscTabButton.bounds.Y = _oilsTabButton.bounds.Y + _oilsTabButton.bounds.Height + 4 * SCALE_I;
        _searchTabButton.bounds.X = _potionsTabButton.bounds.X = _bombsTabButton.bounds.X =
            _oilsTabButton.bounds.X = _miscTabButton.bounds.X = _bookLeftRect.X - 10 * SCALE_I;

        // toggles
        yOffset = 8 * SCALE_I;
        var padding = 2 * SCALE_I;
        _toggleButtonClickables[0].bounds.X = _bookRightRect.X - _toggleButtonClickables.Sum(c => c.bounds.Width) -
                                                   padding * _toggleButtonClickables.Count - 6 * SCALE_I;
        for (var i = 0; i < _toggleButtonClickables.Count; ++i)
        {
            _toggleButtonClickables[i].bounds.Y = _leftContent.Y + yOffset;
            if (i > 0)
                _toggleButtonClickables[i].bounds.X = _toggleButtonClickables[i - 1].bounds.X +
                                                      _toggleButtonClickables[i - 1].bounds.Width + padding;
        }

        _searchButton.bounds = _toggleButtonClickables[^1].bounds;
        _searchBarTextBoxMaxWidth = _searchButton.bounds.X - _searchBarTextBox.X - 6 * SCALE_I;

        // search
        const int minWidth = 33 * SCALE_I;
        _searchBarTextBoxMinWidth = Math.Min(_toggleButtonClickables[0].bounds.X - _searchBarTextBoxBounds.X,
            Math.Max(minWidth,
                6 * SCALE_I + (int)Math.Ceiling(Game1.smallFont.MeasureString(_searchBarTextBox.Text).X)));
        _searchBarTextBox.Width = _searchBarTextBoxMinWidth;
        _searchBarClickable.bounds = _searchBarTextBoxBounds;
        _searchBarTextBoxBounds.Width = _searchBarTextBox.Width;
        _searchBarTextBox.X = _leftContent.X;
        _searchBarTextBox.Y = _leftContent.Y + yOffset + SCALE_I;
        _searchBarTextBox.Selected = false;
        _searchBarTextBox.Update();
        _searchBarTextBoxBounds = new(_searchBarTextBox.X, _searchBarTextBox.Y, -1, _searchBarTextBox.Height);


        // content
        int x, y;
        xOffset = 0;
        yOffset = (_formulaeDisplayArea.Height - GRID_ROWS_I * GRID_HEIGHT_I) / 2;
        padding = _formulaeDisplayArea.Width - GRID_COLUMNS_I * GRID_HEIGHT_I;

        // grid view
        for (var i = 0; i < _gridViewClickables.Count; ++i)
        {
            x = _formulaeDisplayArea.X + xOffset + padding + i % GRID_COLUMNS_I * GRID_HEIGHT_I;
            y = _formulaeDisplayArea.Y + yOffset + i / GRID_COLUMNS_I * GRID_HEIGHT_I + (GRID_HEIGHT_I - StardewValley.Object.spriteSheetTileSize * SCALE_I) / 2;
            _gridViewClickables[i].bounds = new(x, y, StardewValley.Object.spriteSheetTileSize * SCALE_I,
                StardewValley.Object.spriteSheetTileSize * SCALE_I);
        }

        // list view
        x = _formulaeDisplayArea.X;
        yOffset = _formulaeDisplayArea.Height % LIST_HEIGHT_I / 2;
        for (var i = 0; i < LIST_ROWS_I; ++i)
        {
            y = _formulaeDisplayArea.Y + yOffset + i * LIST_HEIGHT_I + (LIST_HEIGHT_I - StardewValley.Object.spriteSheetTileSize * SCALE_I) / 2;
            _listViewClickables[i].bounds = new(x, y, _formulaeDisplayArea.Width, -1);
        }

        foreach (var clickable in _listViewClickables)
            clickable.bounds.Height = _listViewClickables[^1].bounds.Y - _listViewClickables[^2].bounds.Y;

        // ingredient slots
        if (_ingredientClickables.Count > 0)
        {
            var w = _ingredientClickables[0].bounds.Width;
            var h = _ingredientClickables[0].bounds.Height;
            xOffset = 0;
            yOffset = 6 * SCALE_I;
            padding = 0;
            for (var i = 0; i < _ingredientClickables.Count; ++i)
            {
                xOffset += w;
                if (i % INGREDIENT_SLOTS_WIDE_I == 0)
                {
                    if (i != 0) yOffset += h;
                    xOffset = 0;
                }

                if (i == _ingredientClickables.Count - _ingredientClickables.Count % INGREDIENT_SLOTS_WIDE_I)
                    padding = (int)(w / 2f * (_ingredientClickables.Count % INGREDIENT_SLOTS_WIDE_I) / 2f); ;

                _ingredientClickables[i].bounds.X = _rightContent.X + xOffset + padding + 4 * SCALE_I;
                _ingredientClickables[i].bounds.Y = _rightContent.Y + yOffset;
            }

        }

        // mixing confirmation
        xOffset = _rightContent.X + _bookRightRect.Width / 2 - MARGIN_RIGHT_I;
        yOffset = _rightContent.Y + 86 * SCALE_I;
        var mixTextMidpoint = Math.Max(9 * SCALE_I, MixTextSourceByLocale[_locale].Width);
        _mixButton.bounds = new(xOffset, yOffset, MixTextSourceByLocale[_locale].Width * SCALE_I,
            MixTextSourceByLocale[_locale].Height * SCALE_I);

        xOffset -= 40 * SCALE_I;
        yOffset -= 9 * SCALE_I;
        _mixIconBounds = new(xOffset, yOffset + 6, 90, 90);

        xOffset += 12 * SCALE_I + _mixIconBounds.Width;
        _quantityUpButton.bounds.X = _quantityDownButton.bounds.X = xOffset;
        _quantityUpButton.bounds.Y = yOffset - 12;

        var textSize = _quantityTextBox.Font.MeasureString(
            Game1.parseText("999", _quantityTextBox.Font, 96));
        _quantityTextBox.Text = QUANTITY_TEXT_BOX_DEFAULT_I;
        _quantityTextBox.limitWidth = false;
        _quantityTextBox.Width = (int)textSize.X + 6 * SCALE_I;

        padding = (_quantityTextBox.Width - _quantityUpButton.bounds.Width) / 2;
        _quantityTextBox.X = _quantityUpButton.bounds.X - padding;
        _quantityTextBox.Y = _quantityUpButton.bounds.Y + _quantityUpButton.bounds.Height + 2 * SCALE_I;
        _quantityTextBox.Update();
        _quantityTextBoxBounds = new(_quantityTextBox.X, _quantityTextBox.Y, _quantityTextBox.Width,
            _quantityTextBox.Height);

        _quantityDownButton.bounds.Y = _quantityTextBox.Y + _quantityTextBox.Height + 5;

        _confirmButton.bounds.X = _cancelButton.bounds.X =
            _quantityUpButton.bounds.X + _quantityUpButton.bounds.Width + padding + 4 * SCALE_I;
        _confirmButton.bounds.Y = yOffset - 4 * SCALE_I;
        _cancelButton.bounds.Y = _confirmButton.bounds.Y + _confirmButton.bounds.Height + SCALE_I;

        padding = 4 * SCALE_I;
        _quantityScrollableArea = new(
            _mixIconBounds.X - padding,
            _mixIconBounds.Y - padding,
            _confirmButton.bounds.X + _confirmButton.bounds.Width - _mixIconBounds.X + padding * 2,
            _cancelButton.bounds.Y + _cancelButton.bounds.Height - _confirmButton.bounds.Y + padding * 2);

        // inventory
        const int itemsPerRow = 12;
        const int itemRows = 3;
        inventory.rows = _currentInventoryId == BACKPACK_INVENTORY_ID_I && ModEntry.LoadedBackpackMod
            ? itemRows + 1
            : itemRows;
        inventory.capacity = inventory.rows * itemsPerRow;
        var isHorizontal = _UseHorizontalInventorySelection;
        yOffset = yPositionOnScreen + BookSource.Height * SCALE_I - 3 * SCALE_I;
        padding = 4 * SCALE_I + StardewValley.Object.spriteSheetTileSize * SCALE_I / 2;
        inventory.xPositionOnScreen = xPositionOnScreen + BookSource.Width / 2 * SCALE_I - inventory.width / 2 +
                                      (isHorizontal ? 4 * SCALE_I : 0);
        inventory.yPositionOnScreen = yOffset +
                                      (_currentInventoryId == BACKPACK_INVENTORY_ID_I && ModEntry.LoadedBackpackMod
                                          ? -Math.Max(0, padding - (Game1.uiViewport.Height - 720))
                                          : 0);
        padding = 2 * SCALE_I;
        inventory.width = StardewValley.Object.spriteSheetTileSize * SCALE_I * itemsPerRow;
        inventory.height = StardewValley.Object.spriteSheetTileSize * SCALE_I * inventory.rows;
        _inventoryCardArea = new(
            inventory.xPositionOnScreen - spaceToClearSideBorder - padding,
            inventory.yPositionOnScreen - spaceToClearSideBorder - padding / 2,
            inventory.width + spaceToClearSideBorder * 2 + padding * 2,
            inventory.height + spaceToClearSideBorder * 2 + padding / 2);

        yOffset = -1 * SCALE_I;
        for (var i = 0; i < inventory.capacity; ++i)
        {
            if (i % itemsPerRow == 0 && i != 0)
                yOffset += inventory.inventory[i].bounds.Height + 1 * SCALE_I;
            inventory.inventory[i].bounds.X = inventory.xPositionOnScreen + i % itemsPerRow * inventory.inventory[i].bounds.Width;
            inventory.inventory[i].bounds.Y = inventory.yPositionOnScreen + yOffset;
        }

        // inventory selection
        xOffset = 4 * SCALE_I;
        yOffset = 1 * SCALE_I;
        _inventoryTabButton.bounds.X = _inventoryCardArea.X - _inventoryTabButton.bounds.Width + 1 * SCALE_I;
        _inventoryTabButton.bounds.Y = _inventoryCardArea.Y + (_inventoryCardArea.Height - InventoryTabButtonSource.Height * SCALE_I) / 2;
        _inventoryUpButton.bounds.X = _inventoryDownButton.bounds.X = _inventoryTabButton.bounds.X + xOffset;
        _inventoryUpButton.bounds.Y = _inventoryTabButton.bounds.Y - _inventoryUpButton.bounds.Height - yOffset;
        _inventoryDownButton.bounds.Y = _inventoryTabButton.bounds.Y + _inventoryTabButton.bounds.Height + yOffset;

        if (!_ShouldShowInventoryElements) return;

        const int areaPadding = 3 * SCALE_I;
        const int longSideSpacing = 4 * SCALE_I;
        const int wideSideLength = 2;
        const int addedSpacing = 2 * SCALE_I;

        var longSideLength = 2 * ((_inventorySelectButtons.Count + 1) / 2) / 2;
        var xLength = isHorizontal ? longSideLength : wideSideLength;
        var yLength = isHorizontal ? wideSideLength : longSideLength;
        var cardHeight = (int)(_inventorySelectButtons[0].bounds.Height * (yLength + 0.5f)) + areaPadding;
    }

    private void InitialiseControllerFlow()
    {
        if (Constants.TargetPlatform == GamePlatform.Android)
            return;

        var id = ModEntry.Manifest.UniqueID.GetHashCode();
        ChildComponents.AddRange(new List<ClickableComponent>
        {
            _navDownButton,
            _navUpButton,
            _navRightButton,
            _navLeftButton,
            _searchTabButton,
            _potionsTabButton,
            _bombsTabButton,
            _oilsTabButton,
            _miscTabButton,
            _inventoryTabButton,
            _inventoryUpButton,
            _inventoryDownButton,
            _searchButton,
            _searchBarClickable
        });
        ChildComponents.AddRange(_toggleButtonClickables);
        ChildComponents.AddRange(_ingredientClickables);
        ChildComponents.AddRange(_inventorySelectButtons);
        ChildComponents.AddRange(_listViewClickables);
        ChildComponents.AddRange(_gridViewClickables);

        foreach (var component in ChildComponents)
            component.myID = ++id;

        _searchBarClickable.rightNeighborID = _toggleButtonClickables[0].myID;
        for (var i = 0; i < _toggleButtonClickables.Count; ++i)
        {
            _toggleButtonClickables[i].leftNeighborID = i > 0
                ? _toggleButtonClickables[i - 1].myID
                : _searchBarClickable.myID;

            _toggleButtonClickables[i].rightNeighborID = i < _toggleButtonClickables.Count - 1
                ? _toggleButtonClickables[i + 1].myID
                : _ingredientClickables[0].myID;
        }

        _ingredientClickables[^1].rightNeighborID = upperRightCloseButton.myID;
        upperRightCloseButton.leftNeighborID = _ingredientClickables[^1].myID;
        upperRightCloseButton.downNeighborID = _ingredientClickables[^1].myID;
        _navLeftButton.leftNeighborID = _searchTabButton.myID;
        _navRightButton.rightNeighborID = _ingredientClickables[0].myID;

        _navUpButton.upNeighborID = _toggleButtonClickables[^1].myID;
        _navDownButton.downNeighborID = 0;

        _mixButton.upNeighborID = _ingredientClickables[0].myID;
        _mixButton.downNeighborID = 0;

        _confirmButton.leftNeighborID = _quantityUpButton.myID;
        _cancelButton.leftNeighborID = _quantityDownButton.myID;
        _quantityUpButton.rightNeighborID = _quantityDownButton.rightNeighborID = _confirmButton.myID;
        _quantityUpButton.downNeighborID = _quantityDownButton.myID;
        _quantityDownButton.upNeighborID = _quantityUpButton.myID;
        _confirmButton.upNeighborID = _quantityUpButton.upNeighborID = _ingredientClickables[0].myID;
        _cancelButton.upNeighborID = _confirmButton.myID;
        _confirmButton.downNeighborID = _cancelButton.myID;
        _cancelButton.downNeighborID = _quantityDownButton.downNeighborID = 0;

        for (var i = 0; i < _ingredientClickables.Count; ++i)
        {
            _ingredientClickables[i].leftNeighborID =
                i - 1 >= 0 ? _ingredientClickables[i - 1].myID : _ingredientClickables[^1].myID;

            _ingredientClickables[i].rightNeighborID = i + 1 < _ingredientClickables.Count
                ? _ingredientClickables[i + 1].myID
                : _ingredientClickables[0].myID;

            _ingredientClickables[i].downNeighborID =
                i - 1 >= 0 ? _ingredientClickables[i - 1].myID : _ingredientClickables[^1].myID;

            _ingredientClickables[i].upNeighborID = i + 1 < _ingredientClickables.Count
                ? _ingredientClickables[i + 1].myID
                : _ingredientClickables[0].myID;
        }

        for (var i = 0; i < _inventorySelectButtons.Count; ++i)
            if (_UseHorizontalInventorySelection)
            {
                _ingredientClickables[i].leftNeighborID =
                    i - 1 >= 0 ? _ingredientClickables[i - 1].myID : _ingredientClickables[^1].myID;

                _ingredientClickables[i].rightNeighborID = i + 1 < _ingredientClickables.Count
                    ? _ingredientClickables[i + 1].myID
                    : _ingredientClickables[0].myID;
            }
            else
            {
                _ingredientClickables[i].downNeighborID =
                    i - 1 >= 0 ? _ingredientClickables[i - 1].myID : _ingredientClickables[^1].myID;

                _ingredientClickables[i].upNeighborID = i + 1 < _ingredientClickables.Count
                    ? _ingredientClickables[i + 1].myID
                    : _ingredientClickables[0].myID;
            }

        for (var i = 0; i < _listViewClickables.Count; ++i)
        {
            _listViewClickables[i].downNeighborID = i + 1 < _listViewClickables.Count
                ? _listViewClickables[i + 1].myID
                : _listViewClickables[0].myID;

            _listViewClickables[i].upNeighborID =
                i - 1 >= 0 ? _listViewClickables[i - 1].myID : _listViewClickables[^1].myID;
        }
        _listViewClickables[0].upNeighborID = _toggleFilterButton.myID;
        _listViewClickables[^1].downNeighborID = 0;

        for (var i = 0; i < _gridViewClickables.Count; ++i)
        {
            if (i > 0 && i % GRID_COLUMNS_I != 0)
                _gridViewClickables[i].leftNeighborID = _gridViewClickables[i - 1].myID;
            if (i < _gridViewClickables.Count - 1)
                _gridViewClickables[i].rightNeighborID = _gridViewClickables[i + 1].myID;

            _gridViewClickables[i].upNeighborID = i < GRID_COLUMNS_I
                ? _toggleFilterButton.myID
                : _gridViewClickables[i - GRID_COLUMNS_I].myID;
            _gridViewClickables[i].downNeighborID = i > _gridViewClickables.Count - 1 - GRID_COLUMNS_I
                ? 0
                : _gridViewClickables[i + GRID_COLUMNS_I].myID;
        }

        if (_adjacentContainers.Count > 0)
        {
            _inventorySelectButtons[0].leftNeighborID = _UseHorizontalInventorySelection
                ? -1
                : GetColumnCount() - 1; // last element in the first row of the inventory
            _inventorySelectButtons[0].upNeighborID = _UseHorizontalInventorySelection
                ? GetColumnCount() * 2 // first element in the last row of the inventory
                : _inventorySelectButtons[1].upNeighborID = _ingredientClickables[^1].myID; // last ingredient slot
        }

        populateClickableComponentList();
    }

    public override void emergencyShutDown()
    {
        exitFunction();
        base.emergencyShutDown();
    }

    protected override void cleanupBeforeExit()
    {
        Game1.displayHUD = true;
        base.cleanupBeforeExit();
    }

    public override void setUpForGamePadMode()
    {
        base.setUpForGamePadMode();
        snapToDefaultClickableComponent();
    }

    public override void setCurrentlySnappedComponentTo(int id)
    {
        if (id == -1)
            return;

        currentlySnappedComponent = getComponentWithID(id);
        snapCursorToCurrentSnappedComponent();
    }

    public override void snapToDefaultClickableComponent()
    {
        if (_tabHistory.Count <= 0) return;

        switch (_tabHistory.Peek())
        {
            case Tab.Potions:
                setCurrentlySnappedComponentTo(_potionsTabButton.myID);
                break;
            case Tab.Bombs:
                setCurrentlySnappedComponentTo(_bombsTabButton.myID);
                break;
            case Tab.Oils:
                setCurrentlySnappedComponentTo(_oilsTabButton.myID);
                break;
            case Tab.Misc:
                setCurrentlySnappedComponentTo(_miscTabButton.myID);
                break;

            default:
                setCurrentlySnappedComponentTo(_searchBarClickable.myID);
                break;
        }
    }

    public override void automaticSnapBehavior(int direction, int oldRegion, int oldID)
    {
        customSnapBehavior(direction, oldRegion, oldID);
        //base.automaticSnapBehavior(direction, oldRegion, oldID);
    }

    protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
    {
        base.customSnapBehavior(direction, oldRegion, oldID);
    }

    public override void performHoverAction(int x, int y)
    {
        base.performHoverAction(x, y);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        base.receiveLeftClick(x, y, playSound);
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
        base.receiveRightClick(x, y, playSound);
    }

    public override void leftClickHeld(int x, int y)
    {
        base.leftClickHeld(x, y);
    }

    public override void releaseLeftClick(int x, int y)
    {
        base.releaseLeftClick(x, y);
    }

    public override void receiveGamePadButton(Buttons b)
    {
        base.receiveGamePadButton(b);
    }

    public override void gamePadButtonHeld(Buttons b)
    {
        base.gamePadButtonHeld(b);
    }

    public override void receiveScrollWheelAction(int direction)
    {
        base.receiveScrollWheelAction(direction);
    }

    public override void receiveKeyPress(Keys key)
    {
        base.receiveKeyPress(key);
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        base.gameWindowSizeChanged(oldBounds, newBounds);
        AlignElements();
    }

    public override void update(GameTime time)
    {
        _animTimer += time.ElapsedGameTime.Milliseconds;
        if (_animTimer >= ANIM_TIMER_LIMIT_I) _animTimer = 0;
        _animFrame = (int)((float)_animTimer / ANIM_TIMER_LIMIT_I * ANIM_FRAMES_I);

        // expand search bar on selected, contract on deselected
        var delta = 256f / time.ElapsedGameTime.Milliseconds;
        if (_searchBarTextBox.Selected && _searchBarTextBox.Width < _searchBarTextBoxMaxWidth)
            _searchBarTextBox.Width = (int)Math.Min(_searchBarTextBoxMaxWidth, _searchBarTextBox.Width + delta);
        else if (!_searchBarTextBox.Selected && _searchBarTextBox.Width > _searchBarTextBoxMinWidth)
            _searchBarTextBox.Width = (int)Math.Max(_searchBarTextBoxMinWidth, _searchBarTextBox.Width - delta);

        _searchBarTextBoxBounds.Width = _searchBarTextBox.Width;
        _searchBarClickable.bounds.Width = _searchBarTextBoxBounds.Width;

        base.update(time);
    }

    public override void draw(SpriteBatch b)
    {
        base.draw(b);

        drawMouse(b);
    }

    private void DrawSearchPage(SpriteBatch b)
    {

    }

    private void DrawListView(SpriteBatch b)
    {

    }

    private void DrawGridView(SpriteBatch b)
    {

    }

    private void DrawMixingPage(SpriteBatch b)
    {

    }

    private void DrawDetailsPage(SpriteBatch b)
    {

    }

    /// <summary>Replicates <see cref="InventoryMenu.draw"/>, disabling non-ingredient items.</summary>
    /// <param name="b">A <see cref="SpriteBatch"/> to draw to.</param>
    private void DrawEffectiveInventory(SpriteBatch b)
    {

    }

    private void DrawExtraStuff(SpriteBatch b)
    {
        Game1.mouseCursorTransparency = 1f;
        drawMouse(b);
    }

    private void DrawText(SpriteBatch b, string text, float scale, float x, float y, float? w, bool isLeftSide, bool isRightJustified = false, Color? colour = null)
    {

    }

    private void DrawHorizontalDivider(SpriteBatch b, float x, float y, int w, bool isLeftSide)
    {

    }
}