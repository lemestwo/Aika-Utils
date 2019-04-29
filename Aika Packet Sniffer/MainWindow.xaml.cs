using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Aika_Packet_Sniffer.Logger;
using Aika_Packet_Sniffer.Logger.Xml;
using Aika_Packet_Sniffer.Model;
using Aika_Packet_Sniffer.Network;
using Aika_Packet_Sniffer.PacketDump;

namespace Aika_Packet_Sniffer
{
    public partial class MainWindow : Window
    {
        private List<ListViewModel> _packets = new List<ListViewModel>();
        private List<ListViewModel> _backupPackets = new List<ListViewModel>();
        private PacketListView _backupIndex = null;
        private Proxy _snifferProxy;
        private bool _isProxyRunning;

        public MainWindow()
        {
            InitializeComponent();

            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
            RulesParser.Init();
            Dumper.Init();
            _isProxyRunning = false;

            AddHandler(Keyboard.KeyDownEvent, (KeyEventHandler) HandleKeys);
        }

        private void HandleKeys(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                SearchBox.Text = string.Empty;
                SearchBox.Focus();
            }
        }

        private void PacketsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0) return;

            var packet = e.AddedItems[0];
            if (!(packet is PacketListView data)) return;

            HexView.Stream = new MemoryStream(data.Data);
            HexView.ReadOnlyMode = true;

            foreach (var pack in _packets)
            {
                if (pack.PacketListView.Index != data.Index) continue;

                PacketParseListView.Items.Clear();
                foreach (var listView in pack.PacketParseListView)
                {
                    PacketParseListView.Items.Add(listView);
                }
            }
        }

        private void PacketParseListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0) return;

            var packet = e.AddedItems[0];
            if (!(packet is PacketParseListView data)) return;

            HexView.SelectionStart = data.Start;
            HexView.SelectionStop = data.End - 1;
        }

        private void UpdateWithParsedPacket(List<ListViewModel> data)
        {
            foreach (var model in data)
            {
                _packets.Add(model);
                _packets[_packets.Count - 1].PacketListView.Index = (uint) _packets.Count - 1;
                PacketsListView.Dispatcher.Invoke(DispatcherPriority.Normal, (Action) delegate
                {
                    PacketsListView.Items.Add(model.PacketListView);
                    PacketsListView.ScrollIntoView(PacketsListView.Items[PacketsListView.Items.Count - 1]);
                });
            }
        }

        private void LogReadFinished(List<ListViewModel> packets)
        {
            _packets.Clear();
            _packets = packets;
            PacketsListView.Dispatcher.Invoke(DispatcherPriority.Normal, (Action) delegate
            {
                PacketsListView.Items.Clear();
                PacketsListView.SelectedIndex = -1;
            });
            foreach (var packet in packets)
            {
                PacketsListView.Dispatcher.Invoke(DispatcherPriority.Normal, (Action) delegate { PacketsListView.Items.Add(packet.PacketListView); });
            }
        }

        private void PlayBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_isProxyRunning)
            {
                PlayBtn.IsEnabled = false;
                PacketsListView.Items.Clear();
                PacketsListView.SelectedIndex = -1;
                PacketParseListView.Items.Clear();
                PacketParseListView.SelectedIndex = -1;
                _packets.Clear();
                StatusRunning.IsIndeterminate = true;
                var action = new Action<List<ListViewModel>>(UpdateWithParsedPacket);
                _snifferProxy = new Proxy(action);
                _snifferProxy.Start();
            }
            else
            {
                PlayBtn.IsEnabled = true;
                StatusRunning.IsIndeterminate = false;
                _snifferProxy.Stop();
                _isProxyRunning = false;
            }
        }

        private void StopBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (_isProxyRunning) return;

            PlayBtn.IsEnabled = true;
            StatusRunning.IsIndeterminate = false;
            _snifferProxy?.Stop();
            _isProxyRunning = false;
        }

        private void SaveBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog
            {
                DefaultExt = ".packet",
                Filter = "Packet log (*.packet)|*.packet"
            };
            saveDialog.ShowDialog();
            if (saveDialog.FileName == "") return;

            var logSave = new LogSave(_packets);
            logSave.Save(saveDialog.FileName);
        }

        private void OpenBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (_isProxyRunning) return;

            var fileDialog = new OpenFileDialog
            {
                DefaultExt = ".packet",
                Filter = "Packet log (*.packet)|*.packet"
            };
            if (fileDialog.ShowDialog() != true) return;

            Dumper.Init();
            var action = new Action<List<ListViewModel>>(LogReadFinished);
            var logReader = new LogReader(fileDialog.FileName, action);
            logReader.Load();
        }

        private void Clear_OnClick(object sender, RoutedEventArgs e)
        {
            if (_isProxyRunning)
            {
                MessageBox.Show("Proxy is still running.");
                return;
            }

            _packets.Clear();
            PacketsListView.Items.Clear();
            PacketParseListView.Items.Clear();
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            _isProxyRunning = false;
            _snifferProxy?.Stop();
            Environment.Exit(0);
        }

        private void HexView_OnSelectionLengthChanged(object sender, EventArgs e)
        {
            SelectionHexLenght.Text = $"Selected: {HexView.SelectionLength}";
            if (HexView.SelectionLength <= 0 && HexView.SelectionLength > 20) return;
            try
            {
                var hex = HexView.SelectionByteArray;
                var value = "";
                switch (hex.Length)
                {
                    case 1:
                        value = $"int8: {hex[0]} / {unchecked((sbyte) hex[0])} ";
                        break;
                    case 2:
                        value = $"int16: {BitConverter.ToInt16(hex, 0)} / {BitConverter.ToUInt16(hex, 0)} ";
                        break;
                    case 4:
                        value = $"int32: {BitConverter.ToInt32(hex, 0)} / {BitConverter.ToUInt32(hex, 0)} ";
                        value += $"float: {BitConverter.ToSingle(hex, 0)}";
                        break;
                    case 8:
                        value = $"int64: {BitConverter.ToInt64(hex, 0)} / {BitConverter.ToUInt64(hex, 0)} ";
                        value += $"double: {BitConverter.ToDouble(hex, 0)}";
                        break;
                    default:
                        var tmpByte = hex;
                        for (var i = 0; i < hex.Length; i++)
                            if (tmpByte[i].Equals(0xCC))
                                tmpByte[i] = 0x00;
                        value = $"string: {Encoding.UTF8.GetString(tmpByte)}";
                        break;
                }

                SelectionHex.Text = value.Replace(Environment.NewLine, " ");
            }
            catch
            {
                // ignored
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var opcode = SearchBox.Text.Trim();
            if (string.IsNullOrEmpty(opcode) || opcode.Length <= 0 || opcode == "0")
            {
                _backupIndex = (PacketListView) PacketsListView.SelectedItem;
                LogReadFinished(_backupPackets);
                _packets = _backupPackets.ToList();
                _backupPackets.Clear();
                if (_backupIndex != null)
                {
                    PacketsListView.ScrollIntoView(_backupIndex);
                    PacketsListView.SelectedIndex = (int) _backupIndex.Index;
                    _backupIndex = null;
                }

                return;
            }

            if (opcode.Contains("0x")) opcode = Parser.ParseLong(opcode).ToString();

            if (_backupPackets.Count <= 0)
                _backupPackets = _packets.ToList();

            _packets.Clear();
            PacketsListView.Items.Clear();

            var list = new List<ListViewModel>();
            foreach (var packet in _backupPackets)
            {
                if (packet.PacketListView.Opcode == opcode)
                {
                    list.Add(packet);
                }
            }

            LogReadFinished(list);
        }

        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
            SearchButton_Click(sender, e);
        }

        private void DumperButton_OnClick(object sender, RoutedEventArgs e)
        {
            Dumper.IsEnabled = !Dumper.IsEnabled;
            DumperLabel.Content = "Status: Dumper " + (Dumper.IsEnabled ? "ON" : "OFF") + ".";
        }
    }
}