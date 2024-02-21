﻿using System;
using System.Text.Json;
using System.Net.Sockets;
using System.Threading.Tasks;
using annulus.Gamepad.XInputDotNetPure;

namespace annulus.Network
{
    internal class Socket
    {
        public static void StartGamepadProcess()
        {
            string host = "169.254.49.103"; // Change to your Raspberry Pi's IP
            int port = 12345;
            Task.Run(() =>
            {
                using (var client = new TcpClient(host, port))
                using (var stream = client.GetStream())
                {
                    while (true)
                    {
                        GamePadState state = GamePad.GetState(PlayerIndex.One);
                        if (!state.IsConnected) continue;

                        var controllerValues = new
                        {
                            LeftThumb = new List<float> { state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y },
                            RightThumb = new List<float> { state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y },
                            Triggers = new List<float> { state.Triggers.Left, state.Triggers.Right },
                            Buttons = new List<int>
                            {
                                state.Buttons.A == ButtonState.Pressed ? 1 : 0,
                                state.Buttons.B == ButtonState.Pressed ? 1 : 0,
                                state.Buttons.X == ButtonState.Pressed ? 1 : 0,
                                state.Buttons.Y == ButtonState.Pressed ? 1 : 0
                            },
                            Shoulders = new List<int>
                            {
                                state.Buttons.LeftShoulder == ButtonState.Pressed ? 1 : 0,
                                state.Buttons.RightShoulder == ButtonState.Pressed ? 1 : 0
                            },
                            DPad = new List<int>
                            {
                                state.DPad.Up == ButtonState.Pressed ? 1 : 0,
                                state.DPad.Down == ButtonState.Pressed ? 1 : 0,
                                state.DPad.Left == ButtonState.Pressed ? 1 : 0,
                                state.DPad.Right == ButtonState.Pressed ? 1 : 0
                            },
                            Other = new List<int>
                            {
                                state.Buttons.Start == ButtonState.Pressed ? 1 : 0,
                                state.Buttons.Back == ButtonState.Pressed ? 1 : 0
                            }
                        };

                        string jsonData = JsonSerializer.Serialize(controllerValues);
                        byte[] byteData = System.Text.Encoding.ASCII.GetBytes(jsonData);
                        stream.Write(byteData, 0, byteData.Length);
                        System.Threading.Thread.Sleep(100); // Control the frequency of updates
                    }
                }

            });
        }
    }
}