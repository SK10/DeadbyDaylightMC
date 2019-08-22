using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace DeadbyDaylightMC
{
    public partial class Form1 : Form
    {
        Boolean drag;
        Boolean ahkFound = false;
        Boolean addToStartup = true;
        string winStartupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        int mouseX;
        int mouseY;
        string pressedKey;
        List<string> ahkScript;
        List<string> convHotKeys = new List<string>(new string[] {
            "Capital", "Return", "Back", "Scroll", "PageUp" ,"Next" ,"LWin" ,"RWin" ,"ControlKey" ,
            "Menu", "ShiftKey" ,"Apps" ,"Decimal", "Divide" ,"Multiply" ,"Add", "Subtract",
            "Oemtilde", "Oemcomma","OemPeriod", "OemQuestion",
            "Oem1", "Oem7","OemOpenBrackets","Oem6" });

        List<string> unsupportedKeys = new List<string>(new string[] {
            "Escape", "LWin", "Apps", "Scroll", "NumLock"}); 

        List<string> otherKeys = new List<string>(new string[] {
            "Oemtilde", "Oemcomma","OemPeriod", "OemQuestion",
            "Oem1", "Oem7","OemOpenBrackets","Oem6" }); // They are all the same in dbd

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            radioButton2.Checked = true;
            // checking if ahk is installed on the user machine
            foreach (var item in Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall").GetSubKeyNames())
            {
                if (item == "AutoHotkey")
                {
                    ahkFound = true;
                }
            }
            if (ahkFound != true)
            {
                DialogResult result = MessageBox.Show(this, "AutoHotkey is not installed on this computer" 
                    + Environment.NewLine + "Click ok to download it.", "Warning", MessageBoxButtons.OKCancel);

                if (result == DialogResult.OK) { System.Diagnostics.Process.Start("https://www.autohotkey.com/"); }
                Application.Exit();
            }
        }

        private void checkBoxes_CheckedChanged(object sender, EventArgs e)
        {           
            if (checkBox2.Text != "Confirm" && checkBox3.Text != "Confirm" && checkBox1.Checked == true)
            {
                checkBox1.Text = "Confirm";
            }
            else
            {
                checkBox1.Checked = false;
                checkBox1.Text = "Struggle Button";
            }
            // =============================================================================================
            // =============================================================================================
            if (checkBox1.Text != "Confirm" && checkBox3.Text != "Confirm" &&  checkBox2.Checked == true)
            {
                checkBox2.Text = "Confirm";
            }
            else
            {
                checkBox2.Checked = false;
                checkBox2.Text = "Struggle Hotkey";
            }
            // =============================================================================================
            // =============================================================================================
            if (checkBox2.Text != "Confirm" && checkBox1.Text != "Confirm" && checkBox3.Checked == true)
            {
                checkBox3.Text = "Confirm";
            }
            else
            {
                checkBox3.Checked = false;
                checkBox3.Text = "Wiggle Hotkey";
            }
        }

        private void checkBoxes_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // catching the tab key, without this tab would just move the focus to another checkbox
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
            }
        }

        private void checkBoxes_KeyDown(object sender, KeyEventArgs e)
        {
            pressedKey = e.KeyCode.ToString();
            // without this pressing a number would give something like this D1,D2,D3...
            // so i'm just removing the D and keeping the number
            if (e.KeyValue > 47 && e.KeyValue < 58)
            {
                pressedKey = pressedKey.Remove(0, 1);
            }

            // converting the keys in convHotKeys list to their equivalent in ahk 
            foreach (string i in convHotKeys)
            {
                if (i == e.KeyCode.ToString())
                {
                    switch (i)
                    {
                        case "Capital":
                            pressedKey = "CapsLock";                        
                            break;
                        case "Return":
                            pressedKey = "Enter";
                            break;
                        case "Back":
                            pressedKey = "Backspace";
                            break;
                        case "Scroll":
                            pressedKey = "ScrollLock";
                            break;
                        case "PageUp":
                            pressedKey = "PGUP";
                            break;
                        case "Next":
                            pressedKey = "PGDN";
                            break;
                        case "LWin":
                            pressedKey = "<#";
                            break;
                        case "RWin":
                            pressedKey = ">#";
                            break;
                        case "ControlKey":
                            pressedKey = "^";
                            break;
                        case "Menu":
                            pressedKey = "!";
                            break;
                        case "ShiftKey":
                            pressedKey = "+";
                            break;
                        case "Apps":
                            pressedKey = "Menu";
                            break;
                        case "Decimal":
                            pressedKey = "NumpadDot";
                            break;
                        case "Divide":
                            pressedKey = "NumpadDiv";
                            break;
                        case "Multiply":
                            pressedKey = "NumpadMult";
                            break;
                        case "Add":
                            pressedKey = "NumpadAdd";
                            break;
                        case "Subtract":
                            pressedKey = "NumpadSub";
                            break;
                        case "Oemtilde":
                            pressedKey = "029";
                            break;
                        case "Oemcomma":
                            pressedKey = "033";
                            break;
                        case "OemPeriod":
                            pressedKey = "034";
                            break;
                        case "OemQuestion":
                            pressedKey = "035";
                            break;
                        case "Oem1":
                            pressedKey = "027";
                            break;
                        case "Oem7":
                            pressedKey = "028";
                            break;
                        case "OemOpenBrackets":
                            pressedKey = "01A";
                            break;
                        case "Oem6":
                            pressedKey = "01B";
                            break;
                    }                                   
                }
            }

            // =============================================================================================
            // =============================================================================================
            if (checkBox1.Checked == true)
            {
                // excluding some keys that aren't supported in dbd
                foreach (string i in unsupportedKeys)
                {
                    if (pressedKey == i)
                    {
                        textBox1.Text = "--------";
                        label2.Text = "The " + i + " key is not supported in dbd";
                        return;
                    }
                }

                // all the keys in otherKeys list is set to ' in dbd
                foreach (string i in otherKeys)
                {
                    if (e.KeyCode.ToString() == i)
                    {
                        pressedKey = "'";
                    }
                }
                textBox1.Text = pressedKey;
            }

            // =============================================================================================
            // =============================================================================================
            if (checkBox2.Checked == true) {textBox2.Text = pressedKey;}
            if (checkBox3.Checked == true){textBox3.Text = pressedKey;}   
        }

        private void button1_Click(object sender, EventArgs e)
        {
            addToStartup = true;
            // making sure that user confirmed all the keys.
            if (checkBox1.Text == "Confirm" || checkBox2.Text == "Confirm" || checkBox3.Text == "Confirm")
            {
                        label2.Text = "Confirm to proceed";
            }
            else
            {
                if (textBox1.Text == "--------" || textBox2.Text == "--------" || textBox3.Text == "--------")
                {
                    label2.Text = "Assign all keys to proceed";
                }
                else
                {
                    if (textBox2.Text == textBox3.Text)
                    {
                        label2.Text = "Struggle and Wiggle hotkey cannot be the same";
                    }
                    else
                    {
                        if (checkBox4.Checked == false) 
                        {
                            // confirming that the user doesn't want to start the script with windows.
                            DialogResult result = MessageBox.Show("if you don't start the script with windows" +
                                " you would have to start it yourself every time you play the game." + Environment.NewLine +
                                "Are your sure you want to continue?", "Warning", MessageBoxButtons.OKCancel);

                            if (result == DialogResult.OK) { addToStartup = false; } else { return; }
                        }

                        label2.Text = "";
                        ModifyScript(textBox1.Text, textBox2.Text, textBox3.Text);
                        CreateScript();
                    }
                }
            }      
        }

        private void ModifyScript(string sButton, string sHotkey, string wHotkey)
        {
            if (radioButton2.Checked == true) // checking if the user wants toggle or hold mode.
            {
                ahkScript = new List<string>(new string[] {
                "#SingleInstance force",
                "#ifWinActive, ahk_exe DeadByDaylight-Win64-Shipping.exe",
                "*" + wHotkey + "::",
                "			Send, {a Down}",
                "			Sleep, 30",
                "			Send, {a Up}",
                "			Sleep, 150",
                "			Send, {d Down}",
                "			Sleep, 30",
                "			Send, {d Up}",
                "return",
                "*" + sHotkey + "::",
                "			Send, {" + sButton + " Down}",
                "			Sleep, 30",
                "			Send, {" + sButton + " Up}",
                "			Sleep, 100",
                "return"});
            }
            else
            {
                ahkScript = new List<string>(new string[] {
                "#SingleInstance force",
                "#MaxThreadsPerHotkey 2",
                "#ifWinActive, ahk_exe DeadByDaylight-Win64-Shipping.exe",
                "*" + wHotkey + "::",
                "           Toggle := !Toggle",
                "           while Toggle",
                "           {",
                "               Send, {a Down}",
                "               Sleep, 30",
                "               Send, {a Up}",
                "               Sleep, 150",
                "               Send, {d Down}",
                "               Sleep, 30",
                "               Send, {d Up}",
                "           }",
                "return",
                "*" + sHotkey + "::",
                "           Toggle := !Toggle",
                "           while Toggle",
                "           {",
                "			    Send, {" + sButton + " Down}",
                "			    Sleep, 30",
                "			    Send, {" + sButton + " Up}",
                "			    Sleep, 100",
                "           }",
                "return"});
            }
        }

        private void CreateScript()
        {
            using (StreamWriter writer = new StreamWriter(desktopPath + "\\dbd-spammer.ahk", append: false))
            {               
                foreach (string i in ahkScript)
                {
                    writer.WriteLine(i);
                }
            }

            if (File.Exists(winStartupPath + "\\dbd-spammer.ahk"))
            {
                File.Delete(winStartupPath + "\\dbd-spammer.ahk");
            }              
            if (addToStartup == true)
            {
                CopyScript();
            }   
            else
            {
                MessageBox.Show("Done! the script should be on your desktop now. Double click it to run it.");
            }    
        }

        private void CopyScript()
        {
            if (File.Exists(desktopPath + "\\dbd-spammer.ahk"))
            {
                File.Move(desktopPath + "\\dbd-spammer.ahk", winStartupPath + "\\dbd-spammer.ahk");
                System.Diagnostics.Process.Start(winStartupPath + "\\dbd-spammer.ahk");
                MessageBox.Show("Done! the script will run automatically with windows.");
            }
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            if (File.Exists(desktopPath + "\\dbd-spammer.ahk")) { File.Delete(desktopPath + "\\dbd-spammer.ahk"); }
            if (File.Exists(winStartupPath + "\\dbd-spammer.ahk")) { File.Delete(winStartupPath + "\\dbd-spammer.ahk"); }
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            drag = true;
            mouseX = Cursor.Position.X - Left;
            mouseY = Cursor.Position.Y - Top;
        }

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            drag = false;
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (drag)
            {
                Top = Cursor.Position.Y - mouseY;
                Left = Cursor.Position.X - mouseX;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
