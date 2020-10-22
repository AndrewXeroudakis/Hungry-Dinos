// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Input/MouseControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @MouseControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @MouseControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""MouseControls"",
    ""maps"": [
        {
            ""name"": ""PlayerMouse"",
            ""id"": ""775ec04e-733f-4a52-8c98-584aed21e29b"",
            ""actions"": [
                {
                    ""name"": ""SelectCell"",
                    ""type"": ""Button"",
                    ""id"": ""095cc67b-b7ba-43d4-a34f-4f820971eb47"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""NextWave"",
                    ""type"": ""Button"",
                    ""id"": ""728750dc-e690-40d4-8188-9503edb97df0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Position"",
                    ""type"": ""Value"",
                    ""id"": ""9cf74b30-38d4-4e2f-86ea-fd4ff2a5ab56"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""386152a8-e146-4c5c-b1e6-388bbf1f238e"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SelectCell"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""db066e79-8ff8-493c-997f-d53d150673fc"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""689c8f8c-81d4-436a-901d-a6edf9a93afa"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NextWave"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // PlayerMouse
        m_PlayerMouse = asset.FindActionMap("PlayerMouse", throwIfNotFound: true);
        m_PlayerMouse_SelectCell = m_PlayerMouse.FindAction("SelectCell", throwIfNotFound: true);
        m_PlayerMouse_NextWave = m_PlayerMouse.FindAction("NextWave", throwIfNotFound: true);
        m_PlayerMouse_Position = m_PlayerMouse.FindAction("Position", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // PlayerMouse
    private readonly InputActionMap m_PlayerMouse;
    private IPlayerMouseActions m_PlayerMouseActionsCallbackInterface;
    private readonly InputAction m_PlayerMouse_SelectCell;
    private readonly InputAction m_PlayerMouse_NextWave;
    private readonly InputAction m_PlayerMouse_Position;
    public struct PlayerMouseActions
    {
        private @MouseControls m_Wrapper;
        public PlayerMouseActions(@MouseControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @SelectCell => m_Wrapper.m_PlayerMouse_SelectCell;
        public InputAction @NextWave => m_Wrapper.m_PlayerMouse_NextWave;
        public InputAction @Position => m_Wrapper.m_PlayerMouse_Position;
        public InputActionMap Get() { return m_Wrapper.m_PlayerMouse; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerMouseActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerMouseActions instance)
        {
            if (m_Wrapper.m_PlayerMouseActionsCallbackInterface != null)
            {
                @SelectCell.started -= m_Wrapper.m_PlayerMouseActionsCallbackInterface.OnSelectCell;
                @SelectCell.performed -= m_Wrapper.m_PlayerMouseActionsCallbackInterface.OnSelectCell;
                @SelectCell.canceled -= m_Wrapper.m_PlayerMouseActionsCallbackInterface.OnSelectCell;
                @NextWave.started -= m_Wrapper.m_PlayerMouseActionsCallbackInterface.OnNextWave;
                @NextWave.performed -= m_Wrapper.m_PlayerMouseActionsCallbackInterface.OnNextWave;
                @NextWave.canceled -= m_Wrapper.m_PlayerMouseActionsCallbackInterface.OnNextWave;
                @Position.started -= m_Wrapper.m_PlayerMouseActionsCallbackInterface.OnPosition;
                @Position.performed -= m_Wrapper.m_PlayerMouseActionsCallbackInterface.OnPosition;
                @Position.canceled -= m_Wrapper.m_PlayerMouseActionsCallbackInterface.OnPosition;
            }
            m_Wrapper.m_PlayerMouseActionsCallbackInterface = instance;
            if (instance != null)
            {
                @SelectCell.started += instance.OnSelectCell;
                @SelectCell.performed += instance.OnSelectCell;
                @SelectCell.canceled += instance.OnSelectCell;
                @NextWave.started += instance.OnNextWave;
                @NextWave.performed += instance.OnNextWave;
                @NextWave.canceled += instance.OnNextWave;
                @Position.started += instance.OnPosition;
                @Position.performed += instance.OnPosition;
                @Position.canceled += instance.OnPosition;
            }
        }
    }
    public PlayerMouseActions @PlayerMouse => new PlayerMouseActions(this);
    public interface IPlayerMouseActions
    {
        void OnSelectCell(InputAction.CallbackContext context);
        void OnNextWave(InputAction.CallbackContext context);
        void OnPosition(InputAction.CallbackContext context);
    }
}
