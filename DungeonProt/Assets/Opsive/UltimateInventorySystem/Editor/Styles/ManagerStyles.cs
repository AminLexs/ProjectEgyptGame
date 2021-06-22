/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Editor.Styles
{
    using Opsive.Shared.Editor.Utility;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>
    /// Specifies the names for the manager style sheet.
    /// </summary>
    public static class ManagerStyles
    {
        public static StyleSheet StyleSheet => InspectorUtility.LoadAsset<StyleSheet>("dde60a90d31dc9d44be74c339427d85f");

        public static Texture2D UncategorizedIcon => InspectorUtility.LoadAsset<Texture2D>("cae5ceed97260c84aa54154a126e1c78");
        public static Texture2D MissingCurrencyIcon => InspectorUtility.LoadAsset<Texture2D>("70014ee6687184847a67dc36978f3001");

        public static string MenuBackground => EditorGUIUtility.isProSkin ? "dark-menu-background" : "light-menu-background";
        public static string MenuButton => "menu-button";
        public static string SelectedMenuButtonBackground => EditorGUIUtility.isProSkin ? "selected-dark-menu-button-background" : "selected-light-menu-button-background";
        public static string BoxBackground => EditorGUIUtility.isProSkin ? "dark-box-background" : "light-box-background";

        public static string SelectedTabButton => EditorGUIUtility.isProSkin ? "selected-dark-tab-button" : "selected-light-tab-button";

        public static string LinkCursor => "link-cursor";

        public static readonly string s_ColoredBox = "colored-box";
        public static readonly string s_ColoredBox_Error = "colored-box__error";
        public static readonly string s_ColoredBox_Preview = "colored-box__preview";

        public static readonly string s_AttributeView_Margin = "attribute-view__margin";
        public static readonly string s_AttributeViewNameAndValue_Value = "attribute-view-name-and-value__value";
        public static readonly string s_AttributeViewName = "attribute-view-name";
        public static readonly string s_AttributeViewName_Label = "attribute-view-name__label";
        public static readonly string s_AttributeViewName_LabelSmall = "attribute-view-name__label-small";

        public static readonly string s_AttributeValueField = "attribute-value-field";
        public static readonly string s_AttributeOverrideValueField = "attribute-override-value-field";

        public static readonly string s_AttributeBinding = "attribute-binding";
        public static readonly string s_AttributeBindingObject = "attribute-binding__object";
        public static readonly string s_AttributeBindingPopup = "attribute-binding__popup";

        public static readonly string s_InventoryObjectField = "inventory-object-field";

        public static readonly string s_WarningPopupWindow = "warning-popup-window";
        public static readonly string s_WarningPopupWindow_Message = "warning-popup-window__message";
        public static readonly string s_WarningPopupWindow_ButtonContainer = "warning-popup-window__button-container";

        public static readonly string s_CurrencyFamilyItemView = "currency-family-item-view";
        public static readonly string s_CurrencyFamilyItemView_LeftSide = "currency-family-item-view__left-side";
        public static readonly string s_CurrencyFamilyItemView_RightSide = "currency-family-item-view__right-side";

        public static readonly string s_CraftingRecipeView = "crafting-recipe-view";

        public static readonly string SubMenu = "sub-menu";
        public static readonly string SubMenuTitle = "sub-menu-title";
        public static readonly string SubMenuButton = "sub-menu-button";
        public static readonly string SubMenuTop = "sub-menu-top";
        public static readonly string SubMenuIconOptions = "sub-menu-icon-options";
        public static readonly string SubMenuIconOption = "sub-menu-icon-option";
        public static readonly string SubMenuIconDescription = "sub-menu-description";
        public static readonly string CreateDeleteSelectContainer = "create-delete-select-container";
    }
}
