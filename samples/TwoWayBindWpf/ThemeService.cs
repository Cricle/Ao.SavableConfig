using Ao.SavableConfig.Binder.Annotations;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace TwoWayBindWpf
{
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
        private ObjectStyle objectStyle;

        public virtual string Background
        {
            get => background;
            set => Set(ref background, value);
        }
        [ConfigStepIn]
        public ObjectStyle ObjectStyle
        {
            get => objectStyle;
            set => Set(ref objectStyle, value);
        }
    }
    public class ThemeService : ObservableObject
    {
        private string title;
        private WindowStyle windowStyle;
        private bool buttonEnable;
        private int age;
        private ButtonStyle buttonStyle;

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

        public virtual WindowStyle WindowStyle
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
        public ButtonStyle ButtonStyle
        {
            get => buttonStyle;
            set => Set(ref buttonStyle, value);
        }
    }
}
