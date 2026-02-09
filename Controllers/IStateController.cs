using RealismCommonLib.ModifierHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StanceOverhaul.Controllers
{
    public interface IStateController
    {
        public void RunOnAwake();

        public void RunOnDestroy();

        public void RunOnUpdate();
    }
}
