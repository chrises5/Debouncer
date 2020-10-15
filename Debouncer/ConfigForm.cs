using Debouncer.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Debouncer
{
    public partial class ConfigForm : Form
    {
        private Control[] Editors;
        private readonly Dictionary<int, int> CodeToItem = new Dictionary<int, int>();
        private readonly Dictionary<int, int> ItemtoCode = new Dictionary<int, int>();
        private int itemCounter = 0;
        private readonly Config config;
        public static ConfigForm Instance { get; private set; } = null;

        private ConfigForm(Config mouseDebouncer)
        {
            config = mouseDebouncer;
            InitializeComponent();
        }

        public static ConfigForm CreateInstance(Config config)
        {
            if (Instance == null)
                Instance = new ConfigForm(config);

            return Instance;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        public void Init()
        {
            FormBorderStyle = FormBorderStyle.FixedSingle;
            listViewEx1.Columns.Add("Active", 50, HorizontalAlignment.Left);
            listViewEx1.Columns.Add("Name", 120, HorizontalAlignment.Left);
            listViewEx1.Columns.Add("Code", 80, HorizontalAlignment.Left);
            listViewEx1.Columns.Add("Delay", 60, HorizontalAlignment.Left);
            listViewEx1.Columns.Add("State", 60, HorizontalAlignment.Center);
            listViewEx1.OwnerDraw = true;
            listViewEx1.SubItemClicked += ListViewEx1_SubItemClicked;
            listViewEx1.DrawColumnHeader += ListView1_DrawColumnHeader;
            listViewEx1.DrawItem += ListView1_DrawItem;
            listViewEx1.SubItemEndEditing += ListViewEx1_SubItemEndEditing;

            foreach (var input in config)
            {
                var lvi = new ListViewItem("")
                {
                    Checked = input.Value.Debounce
                };
                lvi.SubItems.Add(Enum.GetName(typeof(MouseInput.WindowsCode), input.Key));
                lvi.SubItems.Add($"0x{input.Key:X4}");
                lvi.SubItems.Add($"{input.Value.Delay}");
                lvi.SubItems.Add(".");
                listViewEx1.Items.Add(lvi);
                CodeToItem.Add(input.Key, itemCounter);
                ItemtoCode.Add(itemCounter, input.Key);
                itemCounter++;
            }

            listViewEx1.ItemCheck += ListViewEx1_ItemCheck;

            checkBox1.Hide();
            label3.Hide();
            label4.Hide();
            textBox1.Hide();
            label5.Hide();

            Editors = new Control[] {
                                        checkBox1,
                                        label3,
                                        label4,
                                        textBox1,
                                        label5
                                    };

            notifyIcon1.ContextMenuStrip = new ContextMenuStrip
            {
                ImageScalingSize = new Size(15, 15)
            };
            notifyIcon1.ContextMenuStrip.Items.Add("Settings");
            notifyIcon1.ContextMenuStrip.Items.Add("Exit");
            notifyIcon1.ContextMenuStrip.Items[0].Image = Resources.settings1.ToBitmap();
            notifyIcon1.ContextMenuStrip.Items[1].Image = Resources.exit.ToBitmap();
            notifyIcon1.ContextMenuStrip.Items[0].Click += Form1_SettingsClick;
            notifyIcon1.ContextMenuStrip.Items[1].Click += Form1_ExitClick;
            notifyIcon1.BalloonTipClicked += NotifyIcon1_BalloonTipClicked;
            notifyIcon1.ShowBalloonTip(1000);
        }

        private void NotifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            Show();
        }

        private void Form1_SettingsClick(object sender, EventArgs e)
        {
            Show();
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
        }

        private void Form1_ExitClick(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            MouseHook.Instance.RemoveHook();
            Environment.Exit(0);
        }

        private void ListViewEx1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.CurrentValue == CheckState.Unchecked)
                config[ItemtoCode[e.Index]].Debounce = true;
            else if (e.CurrentValue == CheckState.Checked)
                config[ItemtoCode[e.Index]].Debounce = false;

            config.Save();
        }

        private void ListViewEx1_SubItemEndEditing(object sender, ListViewEx.SubItemEndEditingEventArgs e)
        {
            config[ItemtoCode[e.Item.Index]].Delay = int.Parse(e.DisplayText);
            config.Save();
        }

        private void ListViewEx1_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e)
        {
            if (e.SubItem == 3)
                listViewEx1.StartEditing(Editors[e.SubItem], e.Item, e.SubItem);
        }

        private void ListView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            using (var brush = new SolidBrush(Color.FromArgb(0x1C1C1C)))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            var bounds = e.Bounds;
            bounds.Width -= 1;
            bounds.Height -= 1;
            if (e.ColumnIndex > 0)
                e.Graphics.DrawLine(SystemPens.ControlLightLight, bounds.X, bounds.Y, bounds.X, bounds.Bottom); //LEFT
            e.Graphics.DrawLine(SystemPens.ControlDark, bounds.X, bounds.Bottom, bounds.Right, bounds.Bottom); //BOTTOM
            TextFormatFlags flags = TextFormatFlags.GlyphOverhangPadding;
            bounds = Rectangle.Inflate(e.Bounds, -3, -3);
            TextRenderer.DrawText(e.Graphics, e.Header.Text, e.Font, bounds, Color.White, flags);
        }

        private void ListView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        public void OnInputRegistered(MouseInput e)
        {
            if (Instance.Visible)
            {
                Instance.Invoke((MethodInvoker)(() =>
                {
                    var item = listViewEx1.Items[CodeToItem[e.WParam.ToInt32()]];
                    item.SubItems[4].Text = "+";
                }));

                Task.Delay(100).ContinueWith(t =>
                {
                    try
                    {
                        Instance.Invoke((MethodInvoker)(() =>
                        {
                            try
                            {
                                var item = listViewEx1.Items[CodeToItem[e.WParam.ToInt32()]];
                                item.SubItems[4].Text = ".";
                            }
                            catch (Exception l)
                            {
                                Logger.LogException(l, System.Reflection.MethodBase.GetCurrentMethod().Name);
                            }
                        }));
                    }
                    catch (Exception l)
                    {
                        Logger.LogException(l, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    }
                });
            }

        }
    }
}
