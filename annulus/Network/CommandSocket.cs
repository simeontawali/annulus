using System;
using System.Text.Json;
using System.Net.Sockets;
using System.Threading.Tasks;
using annulus.Gamepad.XInputDotNetPure;
using System.Windows;

namespace annulus.Network
{
    internal class CommandSocket
    {
        //private string host { get; set; }
        //private int port { get; set; }


        public async void StartGamepadProcess()
        {
            string host = "bpmi.local"; // Change to your Raspberry Pi's IP
            int port = 12345;
            try
            {
                await Task.Run(() =>
                {
                    try
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
                                Thread.Sleep(100); // Control the frequency of updates
                            }
                        }
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine($"Failed to connect or send data: {ex2.Message}.\nRetrying in 10 seconds...\n");
                        Task.Delay(10000).Wait(); // Wait for 10 seconds before retrying
                    }

                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
