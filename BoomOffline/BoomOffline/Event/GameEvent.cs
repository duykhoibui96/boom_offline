using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoomOffline.Event
{
    class GameEvent
    {
        public enum Type
        {
            SwitchView,
            Exit,
            Resume,
            ApplySetting,
            CancelSetting,
            ResumeView,
            Save,
            Continue,
            OpenDialog,
            DismissDialog,
            NewGame,
            WantToExit
        }

        private Type type;
        private int param;
        private string param_2;

        public Type EventType
        {
            get { return type; }
            set { type = value; }
        }

        public int Param
        {
            get { return param; }
            set { param = value; }
        }

        public string Param2
        {
            get { return param_2; }
            set { param_2 = value; }
        }

        public GameEvent(Type type, int param = -1)
        {
            this.type = type;
            this.param = param;
        }

        public GameEvent(Type type, string param)
        {
            this.type = type;
            this.param_2 = param;
        }

        public GameEvent(Type type, int param_1, string param_2)
        {
            this.type = type;
            this.param = param_1;
            this.param_2 = param_2;
        }

    }
}
