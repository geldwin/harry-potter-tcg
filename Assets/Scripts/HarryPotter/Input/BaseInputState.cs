using HarryPotter.Input.Controllers;
using HarryPotter.StateManagement;

namespace HarryPotter.Input
{
    public abstract class BaseInputState : BaseState
    {
        public InputSystem InputSystem { get; set; }
    }
}