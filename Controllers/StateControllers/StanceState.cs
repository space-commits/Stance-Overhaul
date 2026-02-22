using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StanceOverhaul.Stances;
using StanceOverhaul.Enums;

namespace StanceOverhaul.Controllers.StateControllers
{
    public class StanceState : MonoBehaviour
    {
        private StanceController _stanceController;

        private List<IStance> Stances = new List<IStance>();

        public StanceState(StanceController stanceController)
        {
            _stanceController = stanceController;

            InitStances();
        }

        void InitStances() 
        {
            Stances.Add(new PatrolStance(_stanceController));
        }

        //TODO change to use events or actions instead of update loop
        void RunOnUpdate()
        {
            
        }
    }
}