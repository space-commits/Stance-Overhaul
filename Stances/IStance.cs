using System;
using System.Collections.Generic;
using System.Text;
using EFT;
using Unity;
using StanceOverhaul.Enums;
using StanceOverhaul.Controllers;

namespace StanceOverhaul.Stances
{
    public interface IStance
    {
        public EStance StanceType { get; }
        public bool ReadyToTransitionState { get; }
        public bool HasReset { get; }
        public bool HasCompleted { get; }
        public bool DoStance { get; set; }

        public void StanceUpdate();

    }
}
