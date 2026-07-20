using RealismCommonLib.ModifierHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StanceOverhaul.Handlers
{
    public interface IControllerHelper
    {
        public void RunOnAwake();

        public void RunOnDestroy();

        public void RunOnUpdate(float deltaTime);
    }
}
