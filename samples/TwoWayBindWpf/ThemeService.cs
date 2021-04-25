using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace TwoWayBindWpf
{
    public class ThemeService:ObservableObject
    {
        private string title;
        private WindowStyle windowStyle;
        private bool buttonEnable;

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

    }
}
