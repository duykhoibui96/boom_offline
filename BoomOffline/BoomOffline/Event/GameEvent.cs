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
            ResumeView
        }

        private Type type;
        private int param;

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

        public GameEvent(Type type, int param = -1)
        {
            this.type = type;
            this.param = param;
        }

    }
}
