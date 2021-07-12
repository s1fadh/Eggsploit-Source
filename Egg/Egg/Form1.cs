﻿using ScintillaNET;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Egg
{
    public partial class Egg : Form
    {
        #region Variables
        EasyExploits.Module Eggs = new EasyExploits.Module();
        #endregion

        public Egg()
        {
            InitializeComponent();

            foreach (var dir in new[] { "workspace", "scripts" })
            {
                if (Directory.Exists(dir)) continue;
                Directory.CreateDirectory(dir);
            }

            RefreshList();
        }

        #region Events
        private void Egg_Load(object sender, EventArgs e)
        {
            #region Syntax
            // Extracted from the Lua Scintilla lexer and SciTE .properties file

            var alphaChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var numericChars = "0123456789";
            var accentedChars = "ŠšŒœŸÿÀàÁáÂâÃãÄäÅåÆæÇçÈèÉéÊêËëÌìÍíÎîÏïÐðÑñÒòÓóÔôÕõÖØøÙùÚúÛûÜüÝýÞþßö";

            // Configuring the default style with properties
            // we have common to every lexer style saves time.
            scintilla1.StyleResetDefault();
            scintilla1.Styles[Style.Default].Font = "Consolas";
            scintilla1.Styles[Style.Default].Size = 10;
            scintilla1.StyleClearAll();

            // Configure the Lua lexer styles
            scintilla1.Styles[Style.Lua.Default].ForeColor = Color.Silver;
            scintilla1.Styles[Style.Lua.Comment].ForeColor = Color.Green;
            scintilla1.Styles[Style.Lua.CommentLine].ForeColor = Color.Green;
            scintilla1.Styles[Style.Lua.Number].ForeColor = Color.Olive;
            scintilla1.Styles[Style.Lua.Word].ForeColor = Color.Blue;
            scintilla1.Styles[Style.Lua.Word2].ForeColor = Color.BlueViolet;
            scintilla1.Styles[Style.Lua.Word3].ForeColor = Color.DarkSlateBlue;
            scintilla1.Styles[Style.Lua.Word4].ForeColor = Color.DarkSlateBlue;
            scintilla1.Styles[Style.Lua.String].ForeColor = Color.Red;
            scintilla1.Styles[Style.Lua.Character].ForeColor = Color.Red;
            scintilla1.Styles[Style.Lua.LiteralString].ForeColor = Color.Red;
            scintilla1.Styles[Style.Lua.StringEol].BackColor = Color.Pink;
            scintilla1.Styles[Style.Lua.Operator].ForeColor = Color.Purple;
            scintilla1.Styles[Style.Lua.Preprocessor].ForeColor = Color.Maroon;
            scintilla1.Lexer = Lexer.Lua;
            scintilla1.WordChars = alphaChars + numericChars + accentedChars;

            // Console.WriteLine(scintilla.DescribeKeywordSets());

            // Keywords
            scintilla1.SetKeywords(0, "and break do else elseif end for function if in local nil not or repeat return then until while" + " false true" + " goto");
            // Basic Functions
            scintilla1.SetKeywords(1, "assert collectgarbage dofile error _G getmetatable ipairs loadfile next pairs pcall print rawequal rawget rawset setmetatable tonumber tostring type _VERSION xpcall string table math coroutine io os debug" + " getfenv gcinfo load loadlib loadstring require select setfenv unpack _LOADED LUA_PATH _REQUIREDNAME package rawlen package bit32 utf8 _ENV");
            // String Manipulation & Mathematical
            scintilla1.SetKeywords(2, "string.byte string.char string.dump string.find string.format string.gsub string.len string.lower string.rep string.sub string.upper table.concat table.insert table.remove table.sort math.abs math.acos math.asin math.atan math.atan2 math.ceil math.cos math.deg math.exp math.floor math.frexp math.ldexp math.log math.max math.min math.pi math.pow math.rad math.random math.randomseed math.sin math.sqrt math.tan" + " string.gfind string.gmatch string.match string.reverse string.pack string.packsize string.unpack table.foreach table.foreachi table.getn table.setn table.maxn table.pack table.unpack table.move math.cosh math.fmod math.huge math.log10 math.modf math.mod math.sinh math.tanh math.maxinteger math.mininteger math.tointeger math.type math.ult" + " bit32.arshift bit32.band bit32.bnot bit32.bor bit32.btest bit32.bxor bit32.extract bit32.replace bit32.lrotate bit32.lshift bit32.rrotate bit32.rshift" + " utf8.char utf8.charpattern utf8.codes utf8.codepoint utf8.len utf8.offset");
            // Input and Output Facilities and System Facilities
            scintilla1.SetKeywords(3, "coroutine.create coroutine.resume coroutine.status coroutine.wrap coroutine.yield io.close io.flush io.input io.lines io.open io.output io.read io.tmpfile io.type io.write io.stdin io.stdout io.stderr os.clock os.date os.difftime os.execute os.exit os.getenv os.remove os.rename os.setlocale os.time os.tmpname" + " coroutine.isyieldable coroutine.running io.popen module package.loaders package.seeall package.config package.searchers package.searchpath" + " require package.cpath package.loaded package.loadlib package.path package.preload");

            // Instruct the lexer to calculate folding
            scintilla1.SetProperty("fold", "1");
            scintilla1.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            scintilla1.Margins[2].Type = MarginType.Symbol;
            scintilla1.Margins[2].Mask = Marker.MaskFolders;
            scintilla1.Margins[2].Sensitive = true;
            scintilla1.Margins[2].Width = 2;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++)
            {
                scintilla1.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                scintilla1.Markers[i].SetBackColor(SystemColors.ControlDark);
            }

            // Configure folding markers with respective symbols
            scintilla1.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            scintilla1.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            scintilla1.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            scintilla1.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            scintilla1.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            scintilla1.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            scintilla1.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            scintilla1.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
            #endregion
        }

        private void RefreshList()
        {
            listBox1.Items.Clear();

            foreach (var script in Directory.GetFiles("./scripts", "*.lua"))
                listBox1.Items.Add(Path.GetFileName(script));

            foreach (var script in Directory.GetFiles("./scripts", "*.txt"))
                listBox1.Items.Add(Path.GetFileName(script));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Eggs.ExecuteScript(scintilla1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var input = MessageBox.Show("Are you sure want to clear Text? All changes will be lost!",
                    "Egg", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (input == DialogResult.Yes) scintilla1.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenDialog = new OpenFileDialog()
            {
                Title = "Open File",
                Filter = "Txt Files (*.txt)|*.txt|Lua Files (*.lua)|*.lua",
                FilterIndex = 2,
            };

            if (OpenDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                scintilla1.Text = File.ReadAllText(OpenDialog.FileName);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveDialog = new SaveFileDialog()
            {
                Title = "Save File",
                Filter = "Txt Files (*.txt)|*.txt|Lua Files (*.lua)|*.lua",
                FilterIndex = 2,
            };

            if (SaveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllText(SaveDialog.FileName, scintilla1.Text);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Eggs.LaunchExploit();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            TopMost ^= true;
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scintilla1.Text = File.ReadAllText("./scripts/" + listBox1.SelectedItem.ToString());
        }

        private void executeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Eggs.ExecuteScript(scintilla1.Text);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshList();
        }
        #endregion
    }
}
