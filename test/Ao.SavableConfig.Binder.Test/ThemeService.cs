using Ao.SavableConfig.Binder.Annotations;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Ao.SavableConfig.Binder.Test
{
    public enum Styles
    {
        None,
        SingleButton
    }
    public class ObjectStyle: ObservableObject
    {
        private int order;

        public virtual int Order
        {
            get => order;
            set => Set(ref order, value);
        }
    }
    public class ButtonStyle : ObservableObject
    {
        private string background;

        public virtual string Background
        {
            get => background;
            set => Set(ref background, value);
        }
        [ConfigStepIn]
        public ObjectStyle ObjectStyle { get; set; }
    }
    public class ThemeService : ObservableObject
    {
        private string title;
        private Styles windowStyle;
        private bool buttonEnable;
        private int age;

        public virtual int Age
        {
            get => age;
            set => Set(ref age, value);
        }

        public virtual bool ButtonEnable
        {
            get { return buttonEnable; }
            set => Set(ref buttonEnable, value);
        }

        public virtual Styles WindowStyle
        {
            get { return windowStyle; }
            set => Set(ref windowStyle, value);
        }


        public virtual string Title
        {
            get { return title; }
            set => Set(ref title, value);
        }
        [ConfigStepIn]
        public ButtonStyle ButtonStyle { get; set; }
    }
}
