using Newtonsoft.Json;
using SS3D.Core;
using SS3D.Systems.Inputs;
using SS3D.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tests
{
    /// <summary>
    /// Logs all input events during gameplay for later analysis.
    /// </summary>
    public sealed class InputEventLogger : MonoBehaviour
    {
        /// <summary>
        /// Represents a logged input event.
        /// </summary>
        private sealed class InputEvent
        {
            public readonly string ActionName;
            public readonly string Phase;
            public readonly double Time;
            public readonly string Value;

            public InputEvent(InputAction.CallbackContext context)
            {
                ActionName = context.action.name;
                Phase = context.phase.ToString();
                Time = context.time;
                Value = ReadValueAsString(context);
            }

            /// <summary>
            /// Reads the input value from the context and converts it to a string.
            /// </summary>
            /// <param name="context">the callback context object for input event.</param>
            /// <returns>the value of input event as string.</returns>
            private static string ReadValueAsString(InputAction.CallbackContext context)
            {
                Type valueType = context.valueType;

                // Handle composites like Move (Vector2) first
                if (valueType == typeof(Vector2))
                {
                    return context.ReadValue<Vector2>().ToString();
                }

                if (valueType == typeof(Vector3))
                {
                    return context.ReadValue<Vector3>().ToString();
                }

                if (valueType == typeof(Vector4))
                {
                    return context.ReadValue<Vector4>().ToString();
                }

                if (valueType == typeof(Quaternion))
                {
                    return context.ReadValue<Quaternion>().ToString();
                }

                // Button actions (Jump, Fire etc.)
                if (context.action is { type: InputActionType.Button })
                {
                    return context.ReadValueAsButton().ToString();
                }

                // Other common scalar types
                if (valueType == typeof(float))
                {
                    return context.ReadValue<float>().ToString();
                }

                if (valueType == typeof(int))
                {
                    return context.ReadValue<int>().ToString();
                }

                // Fallback
                object obj = context.ReadValueAsObject();

                return obj != null ? obj.ToString() : "null";
            }
        }

        private readonly List<InputEvent> _events = new();

        private InputSubSystem _inputSubSystem;

        private void Awake()
        {
            // Get the InputSubSystem from the SubSystems manager
            _inputSubSystem = SubSystems.Get<InputSubSystem>();
        }

        private void OnEnable()
        {
            // Subscribe to all input actions' events
            foreach (InputAction action in _inputSubSystem.Inputs)
            {
                action.started += OnInputEvent;
                action.performed += OnInputEvent;
                action.canceled += OnInputEvent;
            }
        }

        private void OnDisable()
        {
            // Unsubscribe from all input actions' events
            foreach (InputAction action in _inputSubSystem.Inputs)
            {
                action.started -= OnInputEvent;
                action.performed -= OnInputEvent;
                action.canceled -= OnInputEvent;
            }
        }

        /// <summary>
        /// Handles input action events and logs them.
        /// </summary>
        /// <param name="context">the callback context object for input event.</param>
        private void OnInputEvent(InputAction.CallbackContext context)
        {
            // Log the input event
            _events.Add(new(context));
        }

        private void OnApplicationQuit()
        {
            // Save the logged events to a file for analysis
            // Create log file path
            string logFilePath = Path.GetDirectoryName(Application.dataPath);
            logFilePath = Path.Combine(logFilePath, "Logs", "Input", $"InputEventLog_{DateTime.Now:yyyy-MM-dd_HH:mm:ss}.json");

            // Serialize events to JSON
            string logContent;

            try
            {
                logContent = JsonConvert.SerializeObject(_events, Formatting.Indented);
            }
            catch (Exception e)
            {
                Debug.LogException(e);

                return;
            }

            // Save to file
            FileUtil.SaveFile(logFilePath, logContent);
        }
    }
}
