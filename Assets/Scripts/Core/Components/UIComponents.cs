using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MudLike.Core.Components
{
    /// <summary>
    /// Компонент для UI элементов
    /// </summary>
    public struct UIElement : IComponentData
    {
        public int ElementId;
        public bool IsVisible;
        public bool IsInteractive;
    }

    /// <summary>
    /// Компонент для кнопок UI
    /// </summary>
    public struct UIButton : IComponentData
    {
        public int ButtonId;
        public bool IsPressed;
        public bool IsHovered;
        public bool IsClicked;
    }

    /// <summary>
    /// Компонент для панелей UI
    /// </summary>
    public struct UIPanel : IComponentData
    {
        public int PanelId;
        public bool IsVisible;
        public float Alpha;
    }

    /// <summary>
    /// Компонент для диалогов UI
    /// </summary>
    public struct UIDialog : IComponentData
    {
        public int DialogId;
        public bool IsOpen;
        public bool IsModal;
        public float Timeout;
    }

    /// <summary>
    /// Компонент для загрузки UI
    /// </summary>
    public struct UILoading : IComponentData
    {
        public bool IsLoading;
        public float Progress;
        public float Duration;
        public int LoadingId;
    }

    /// <summary>
    /// Компонент для навигации UI
    /// </summary>
    public struct UINavigation : IComponentData
    {
        public int CurrentScreen;
        public int PreviousScreen;
        public bool CanGoBack;
        public float TransitionTime;
    }

    /// <summary>
    /// Компонент для конфигурации UI
    /// </summary>
    public struct UIConfig : IComponentData
    {
        public float AnimationSpeed;
        public float FadeTime;
        public bool EnableAnimations;
        public bool EnableSounds;
    }
}
